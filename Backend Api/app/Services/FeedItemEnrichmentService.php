<?php

namespace App\Services;

use App\Models\QuotationEstimation;
use App\Models\QuotationEstimationDetails;
use App\Models\QuotationHeader;
use App\Models\Sales;
use App\Models\SalesDetails;
use App\Models\SalesPipes;
use App\Models\SalesPipesDetails;
use App\Models\SalesTraders;
use App\Models\SalesTradersDetails;
use App\Services\Monitor\MonitorDocumentPresenter;
use App\Support\BranchDateTime;
use Carbon\Carbon;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Support\Collection;

/**
 * Attach mirror document summaries to feed item rows (quotation, estimation, sales, etc.).
 */
class FeedItemEnrichmentService
{
    /** @var array<string, QuotationHeader> */
    private array $quotationHeaderCache = [];

    public function __construct(
        private QuotationSummaryService $quotationSummaries,
    ) {}

    /**
     * @param  Collection<int, array<string, mixed>>  $items
     * @return Collection<int, array<string, mixed>>
     */
    public function enrichMany(Collection $items): Collection
    {
        $this->quotationHeaderCache = [];
        $this->prefetchQuotationHeaders($items);

        return $items->map(fn (array $row) => $this->enrich($row));
    }

    /**
     * @param  array<string, mixed>  $row
     * @return array<string, mixed>
     */
    public function enrich(array $row): array
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

    /**
     * @param  array<string, mixed>  $row
     * @return array<string, mixed>
     */
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
            $this->quotationSummaries->attachLineStats(collect([$header]));
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

        $freshSyncon = $header->syncon ? Carbon::parse($header->getRawOriginal('syncon') ?? $header->syncon) : null;
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

        $freshSyncon = $header->syncon ? Carbon::parse($header->getRawOriginal('syncon') ?? $header->syncon) : null;
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

        $freshSyncon = $header->syncon ? Carbon::parse($header->getRawOriginal('syncon') ?? $header->syncon) : null;
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
            'document_datetime' => BranchDateTime::toDisplay($documentDate),
            'line_count' => $lineCount,
            'total_amount' => round($totalAmount, 2),
            'quotation_id' => $quotationId,
            'payment_mode' => $paymentMode,
        ];
    }

    private function synconToMs(?Carbon $syncon): int
    {
        if (! $syncon) {
            return 0;
        }

        return (int) $syncon->format('Uv');
    }
}
