<?php

namespace App\Services;

use App\Support\BranchDateTime;
use App\Services\Monitor\MonitorDocumentPresenter;
use App\Models\QuotationEstimation;
use App\Models\QuotationEstimationDetails;
use App\Models\QuotationHeader;
use App\Models\Sales;
use App\Models\SalesDetails;
use App\Models\SalesPipes;
use App\Models\SalesPipesDetails;
use App\Models\SalesTraders;
use App\Models\SalesTradersDetails;
use Carbon\Carbon;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Support\Collection;

/**
 * @deprecated Superseded by ActivityFeedService (reads activity_events). Kept for reference.
 * Build live monitor feed directly from MSSQL mirror header tables (not activity_events).
 */
class MirrorFeedService
{
    /** @var array<string, QuotationHeader> */
    private array $quotationHeaderCache = [];

    /**
     * Feed day filter matches cloud receive time (syncon) or document business date.
     *
     * @var array<string, array{entity_type: string, model: class-string<Model>, document_date_column: string, id_column: string, line_no: string, label: string}>
     */
    private const SOURCES = [
        'quotation' => [
            'entity_type' => 'quotation',
            'model' => QuotationHeader::class,
            'document_date_column' => 'date',
            'id_column' => 'Quotationid',
            'line_no' => 'sino',
            'label' => 'Quotation',
        ],
        'estimation' => [
            'entity_type' => 'estimation',
            'model' => QuotationEstimation::class,
            'document_date_column' => 'date',
            'id_column' => 'Estimationid',
            'line_no' => 'SlNo',
            'label' => 'Estimation',
        ],
        'sale' => [
            'entity_type' => 'sale',
            'model' => Sales::class,
            'document_date_column' => 'EnteredOn',
            'id_column' => 'Salesid',
            'line_no' => 'slno',
            'label' => 'Sale',
        ],
        'sales-pipe' => [
            'entity_type' => 'sales-pipe',
            'model' => SalesPipes::class,
            'document_date_column' => 'EnteredOn',
            'id_column' => 'Salesid',
            'line_no' => 'slno',
            'label' => 'Sale (Pipes)',
        ],
        'sales-trader' => [
            'entity_type' => 'sales-trader',
            'model' => SalesTraders::class,
            'document_date_column' => 'EnteredOn',
            'id_column' => 'Salesid',
            'line_no' => 'slno',
            'label' => 'Sale (Traders)',
        ],
    ];

    public function __construct(
        private QuotationSummaryService $quotationSummaries,
    ) {}

    /**
     * @param  list<int>  $branchIds
     * @param  list<string>  $types  Feed entity types (quotation, estimation, sale, …). Empty = all mirror types.
     * @return array{items: list<array<string, mixed>>, latest_id: int}
     */
    public function feed(
        Carbon $date,
        array $branchIds,
        array $types,
        int $sinceId,
        int $limit,
    ): array {
        $sources = $this->resolveSources($types);
        $items = collect();

        foreach ($sources as $source) {
            $items = $items->merge($this->rowsForSource($source, $date, $branchIds));
        }

        $this->quotationHeaderCache = [];
        $this->prefetchQuotationHeaders($items);

        $items = $items
            ->map(fn (array $row) => $this->attachDocumentSummary($row))
            ->groupBy(fn (array $row) => "{$row['entity_type']}:{$row['branch_id']}:{$row['external_id']}")
            ->map(fn (Collection $group) => $group->sortByDesc(fn (array $row) => $this->synconSortKey($row))->first())
            ->sortByDesc(fn (array $row) => $this->synconSortKey($row))
            ->values();

        if ($sinceId > 0) {
            $items = $items
                ->filter(fn (array $row) => (int) $row['id'] > $sinceId)
                ->sortBy('id')
                ->take($limit)
                ->values();
        } else {
            $items = $items->take($limit)->values();
        }

        $latestId = (int) ($items->max('id') ?? 0);

        if ($sinceId === 0 && $items->count() < $limit) {
            $latestId = $this->maxFeedIdForDate($date, $branchIds, $sources);
        }

        return [
            'items' => $items->values()->all(),
            'latest_id' => $latestId,
        ];
    }

    /**
     * @param  list<string>  $types
     * @return list<array<string, mixed>>
     */
    private function resolveSources(array $types): array
    {
        if ($types === []) {
            return array_values(self::SOURCES);
        }

        $out = [];
        foreach ($types as $type) {
            if (isset(self::SOURCES[$type])) {
                $out[] = self::SOURCES[$type];
            }
        }

        return $out;
    }

    /**
     * @param  array<string, mixed>  $source
     * @param  list<int>  $branchIds
     * @return Collection<int, array<string, mixed>>
     */
    private function rowsForSource(array $source, Carbon $date, array $branchIds): Collection
    {
        /** @var class-string<Model> $modelClass */
        $modelClass = $source['model'];
        $documentDateColumn = $source['document_date_column'];
        $dateString = $date->toDateString();

        $rows = $modelClass::query()
            ->with('branch:id,code,short_code,name')
            ->where(function ($q) use ($dateString, $documentDateColumn) {
                $q->whereDate('syncon', $dateString)
                    ->orWhereDate($documentDateColumn, $dateString);
            })
            ->when($branchIds !== [], fn ($q) => $q->whereIn('branch_id', $branchIds))
            ->orderByDesc('syncon')
            ->get();

        return $rows->map(fn (Model $row) => $this->mapRow($source, $row));
    }

    /**
     * @param  array<string, mixed>  $source
     * @return array<string, mixed>
     */
    private function mapRow(array $source, Model $row): array
    {
        $branch = $row->branch;
        $externalId = (string) $row->{$source['id_column']};
        $lineNo = (int) ($row->{$source['line_no']} ?? 0);
        $documentDateColumn = $source['document_date_column'];
        $syncon = $row->syncon ? Carbon::parse($row->syncon) : null;
        $documentDate = $row->{$documentDateColumn} ?? null;
        $occurredAt = BranchDateTime::toDisplay($syncon ?? $documentDate);
        $activity = BranchDateTime::toDateString($documentDate) ?? BranchDateTime::toDateString($syncon);

        $amount = $this->amountFor($source['entity_type'], $row);
        $subtitle = $this->subtitleFor($source['entity_type'], $row);

        return [
            'id' => $this->feedId($source['entity_type'], (int) $row->branch_id, $externalId),
            'branch_id' => (int) $row->branch_id,
            'entity_type' => $source['entity_type'],
            'external_id' => $externalId,
            'event_kind' => 'synced',
            'title' => "{$source['label']} · ".($branch->name ?? 'Branch'),
            'subtitle' => $subtitle,
            'amount' => $amount > 0 ? $amount : null,
            'activity_date' => $activity,
            'syncon' => BranchDateTime::toDisplay($syncon),
            'syncon_ts' => $this->synconToMs($syncon),
            'occurred_at' => $occurredAt,
            'branch' => $branch ? [
                'id' => $branch->id,
                'code' => $branch->code,
                'short_code' => $branch->short_code ?? null,
                'name' => $branch->name,
            ] : null,
        ];
    }

    /**
     * @param  array<string, mixed>  $row
     * @return array<string, mixed>
     */
    private function attachDocumentSummary(array $row): array
    {
        return match ($row['entity_type']) {
            'quotation' => $this->attachQuotationSummary($row),
            'estimation' => $this->attachEstimationSummary($row),
            'sale' => $this->attachSaleSummary($row, Sales::class, SalesDetails::class),
            'sales-pipe' => $this->attachSaleSummary($row, SalesPipes::class, SalesPipesDetails::class),
            'sales-trader' => $this->attachSaleSummary($row, SalesTraders::class, SalesTradersDetails::class),
            default => $row,
        };
    }

    /**
     * @param  array<string, mixed>  $row
     * @return array<string, mixed>
     */
    /**
     * @param  Collection<int, array<string, mixed>>  $items
     */
    private function prefetchQuotationHeaders(Collection $items): void
    {
        $quotationRows = $items->where('entity_type', 'quotation');
        if ($quotationRows->isEmpty()) {
            return;
        }

        foreach ($quotationRows->groupBy('branch_id') as $branchId => $rows) {
            $ids = $rows->pluck('external_id')->map(fn ($id) => (string) $id)->all();
            $headers = QuotationHeader::query()
                ->where('branch_id', $branchId)
                ->whereIn('Quotationid', $ids)
                ->get();

            $this->quotationSummaries->attachLineStats($headers);

            foreach ($headers as $header) {
                $this->quotationHeaderCache["{$branchId}:{$header->Quotationid}"] = $header;
            }
        }
    }

    private function attachQuotationSummary(array $row): array
    {
        $cacheKey = "{$row['branch_id']}:{$row['external_id']}";
        $header = $this->quotationHeaderCache[$cacheKey] ?? QuotationHeader::query()
            ->where('branch_id', $row['branch_id'])
            ->where('Quotationid', $row['external_id'])
            ->first();

        if (! $header) {
            return $row;
        }

        if (! isset($this->quotationHeaderCache[$cacheKey])) {
            $headers = collect([$header]);
            $this->quotationSummaries->attachLineStats($headers);
        }

        $lineCount = (int) ($header->line_count ?? 0);
        $totalAmount = (float) ($header->total_amount ?? 0);

        $row['quotation'] = $this->quotationSummaries->formatSummary(
            $header,
            $lineCount,
            $totalAmount,
        );

        $row['document'] = $this->buildDocumentSummary(
            (string) $header->Quotationid,
            $header->customename,
            BranchDateTime::fromModel($header, 'date') ?? BranchDateTime::fromModel($header, 'syncon'),
            $lineCount,
            $totalAmount,
        );

        if (($row['amount'] ?? null) === null && $totalAmount > 0) {
            $row['amount'] = $totalAmount;
        }

        $freshSyncon = $header->syncon ? Carbon::parse($header->syncon) : null;
        $row['syncon'] = BranchDateTime::toDisplay($freshSyncon);
        $row['syncon_ts'] = $this->synconToMs($freshSyncon);

        return $row;
    }

    /**
     * @param  array<string, mixed>  $row
     * @return array<string, mixed>
     */
    private function attachEstimationSummary(array $row): array
    {
        $header = QuotationEstimation::query()
            ->where('branch_id', $row['branch_id'])
            ->where('Estimationid', $row['external_id'])
            ->first();

        if (! $header) {
            return $row;
        }

        $lineCount = (int) QuotationEstimationDetails::query()
            ->where('branch_id', $row['branch_id'])
            ->where('Estimationid', $row['external_id'])
            ->count();

        $totalAmount = MonitorDocumentPresenter::parseAmount($header->GrandTotal ?? $header->Total);

        $row['document'] = $this->buildDocumentSummary(
            (string) $header->Estimationid,
            $header->customername,
            BranchDateTime::fromModel($header, 'date') ?? BranchDateTime::fromModel($header, 'syncon'),
            $lineCount,
            $totalAmount,
            $header->Quotationid ? (string) $header->Quotationid : null,
            $header->Paymentmode,
        );

        if (($row['amount'] ?? null) === null && $totalAmount > 0) {
            $row['amount'] = $totalAmount;
        }

        $freshSyncon = $header->syncon ? Carbon::parse($header->syncon) : null;
        $row['syncon'] = BranchDateTime::toDisplay($freshSyncon);
        $row['syncon_ts'] = $this->synconToMs($freshSyncon);

        return $row;
    }

    /**
     * @param  class-string<Model>  $headerModel
     * @param  class-string<Model>  $detailsModel
     * @param  array<string, mixed>  $row
     * @return array<string, mixed>
     */
    private function attachSaleSummary(array $row, string $headerModel, string $detailsModel): array
    {
        $header = $headerModel::query()
            ->where('branch_id', $row['branch_id'])
            ->where('Salesid', $row['external_id'])
            ->first();

        if (! $header) {
            return $row;
        }

        $lineCount = (int) $detailsModel::query()
            ->where('branch_id', $row['branch_id'])
            ->where('Salesid', $row['external_id'])
            ->count();

        $totalAmount = MonitorDocumentPresenter::parseAmount($header->GrandTotal ?? $header->TotalAmount);
        $documentDate = $header->EnteredOn ?? $header->Updatedon ?? $header->syncon;

        $row['document'] = $this->buildDocumentSummary(
            (string) $header->Salesid,
            $header->customername,
            $documentDate,
            $lineCount,
            $totalAmount,
        );

        if (($row['amount'] ?? null) === null && $totalAmount > 0) {
            $row['amount'] = $totalAmount;
        }

        $freshSyncon = $header->syncon ? Carbon::parse($header->syncon) : null;
        $row['syncon'] = BranchDateTime::toDisplay($freshSyncon);
        $row['syncon_ts'] = $this->synconToMs($freshSyncon);

        return $row;
    }

    /**
     * @return array<string, mixed>
     */
    private function buildDocumentSummary(
        string $documentId,
        ?string $customerName,
        mixed $documentDate,
        int $lineCount,
        float $totalAmount,
        ?string $quotationId = null,
        ?string $paymentMode = null,
    ): array {
        return [
            'document_id' => $documentId,
            'customer_name' => $customerName,
            'document_datetime' => $this->formatDocumentDateTime($documentDate),
            'line_count' => $lineCount,
            'total_amount' => round($totalAmount, 2),
            'quotation_id' => $quotationId,
            'payment_mode' => $paymentMode,
        ];
    }

    private function formatDocumentDateTime(mixed $value): ?string
    {
        return BranchDateTime::toDisplay($value);
    }

    private function amountFor(string $entityType, Model $row): float
    {
        return match ($entityType) {
            'quotation' => 0.0,
            'estimation' => MonitorDocumentPresenter::parseAmount($row->GrandTotal ?? $row->Total ?? null),
            'sale', 'sales-pipe', 'sales-trader' => MonitorDocumentPresenter::parseAmount(
                $row->GrandTotal ?? $row->TotalAmount ?? null
            ),
            default => 0.0,
        };
    }

    private function synconToMs(?Carbon $syncon): int
    {
        if (! $syncon) {
            return 0;
        }

        return (int) $syncon->format('Uv');
    }

    /** @param  array<string, mixed>  $row */
    private function synconSortKey(array $row): int
    {
        if (! empty($row['syncon_ts'])) {
            return (int) $row['syncon_ts'];
        }

        if (empty($row['syncon'])) {
            return 0;
        }

        return $this->synconToMs(Carbon::parse($row['syncon']));
    }

    private function subtitleFor(string $entityType, Model $row): ?string
    {
        return match ($entityType) {
            'quotation' => $row->customename ?? null,
            'estimation', 'sale', 'sales-pipe', 'sales-trader' => $row->customername ?? null,
            default => null,
        };
    }

    /**
     * Stable per document (branch + entity + business id). Re-sync updates the same card instead of duplicating.
     */
    private function feedId(string $entityType, int $branchId, string $externalId): int
    {
        $typeCode = match ($entityType) {
            'quotation' => 1,
            'estimation' => 2,
            'sale' => 3,
            'sales-pipe' => 4,
            'sales-trader' => 5,
            default => 9,
        };

        $hash = (int) sprintf('%u', crc32("{$entityType}|{$branchId}|{$externalId}"));

        return $typeCode * 1_000_000_000 + ($hash % 999_999_999) + 1;
    }

    /**
     * @param  list<array<string, mixed>>  $sources
     */
    private function maxFeedIdForDate(Carbon $date, array $branchIds, array $sources): int
    {
        $max = 0;

        foreach ($sources as $source) {
            /** @var class-string<Model> $modelClass */
            $modelClass = $source['model'];
            $dateString = $date->toDateString();
            $documentDateColumn = $source['document_date_column'];

            $rows = $modelClass::query()
                ->where(function ($q) use ($dateString, $documentDateColumn) {
                    $q->whereDate('syncon', $dateString)
                        ->orWhere(function ($inner) use ($dateString, $documentDateColumn) {
                            $inner->whereNull('syncon')
                                ->whereDate($documentDateColumn, $dateString);
                        });
                })
                ->when($branchIds !== [], fn ($q) => $q->whereIn('branch_id', $branchIds))
                ->get(['branch_id', $source['id_column'], 'syncon', $documentDateColumn]);

            foreach ($rows as $row) {
                $externalId = (string) $row->{$source['id_column']};
                $id = $this->feedId(
                    $source['entity_type'],
                    (int) $row->branch_id,
                    $externalId,
                );
                $max = max($max, $id);
            }
        }

        return $max;
    }
}
