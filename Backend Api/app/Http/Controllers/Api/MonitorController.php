<?php

namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use App\Models\Branch;
use App\Models\QuotationDetails;
use App\Models\QuotationEstimation;
use App\Models\QuotationEstimationDetails;
use App\Models\QuotationHeader;
use App\Models\Sales;
use App\Models\SalesPipes;
use App\Models\SalesTraders;
use App\Models\Settlement;
use App\Models\Voucher;
use App\Services\FeedDocumentDetailService;
use App\Services\Monitor\MonitorDocumentPresenter;
use App\Services\QuotationSummaryService;
use Illuminate\Database\Eloquent\Builder;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Http\JsonResponse;
use Illuminate\Http\Request;
use Illuminate\Pagination\LengthAwarePaginator;
use Illuminate\Support\Facades\DB;

class MonitorController extends Controller
{
    public function __construct(
        private QuotationSummaryService $quotationSummaries,
        private FeedDocumentDetailService $feedDocumentDetails,
    ) {}

    public function feedDetail(Request $request): JsonResponse
    {
        $entityType = $request->string('entity_type')->toString();
        $branchId = (int) $request->input('branch_id', 0);
        $externalId = trim($request->string('external_id')->toString());

        if ($entityType === '' || $branchId <= 0 || $externalId === '') {
            return response()->json(['message' => 'entity_type, branch_id and external_id are required.'], 422);
        }

        $detail = $this->feedDocumentDetails->detail($entityType, $branchId, $externalId);

        if ($detail === null) {
            return response()->json(['message' => 'Document not found.'], 404);
        }

        return response()->json($detail);
    }

    public function dashboard(Request $request): JsonResponse
    {
        [$from, $to, $branchId] = $this->filters($request);

        $branchFilter = fn ($q) => $branchId ? $q->where('branch_id', $branchId) : $q;

        $quotationHeaders = $branchFilter(QuotationHeader::query()->whereBetween('date', [$from, $to]));
        $quotationAmount = (float) QuotationDetails::query()
            ->when($branchId, fn ($q) => $q->where('branch_id', $branchId))
            ->whereHas('header', fn ($q) => $q->whereBetween('date', [$from, $to])
                ->when($branchId, fn ($inner) => $inner->where('branch_id', $branchId)))
            ->get()
            ->sum(fn (QuotationDetails $line) => $line->amount_decimal);

        $estimations = $branchFilter(QuotationEstimation::query()->whereBetween('date', [$from, $to]));
        $sales = $branchFilter(Sales::query()->whereBetween('EnteredOn', [$from, $to]));
        $salesPipes = $branchFilter(SalesPipes::query()->whereBetween('EnteredOn', [$from, $to]));
        $salesTraders = $branchFilter(SalesTraders::query()->whereBetween('EnteredOn', [$from, $to]));
        $vouchers = $branchFilter(Voucher::query()->whereBetween('document_date', [$from, $to]));

        return response()->json([
            'period' => ['from' => $from, 'to' => $to],
            'summary' => [
                'quotations' => [
                    'count' => (clone $quotationHeaders)->count(),
                    'amount' => $quotationAmount,
                ],
                'estimations' => $this->mirrorAmountSummary($estimations, 'GrandTotal', 'Total'),
                'sales' => $this->mirrorAmountSummary($sales, 'GrandTotal', 'TotalAmount'),
                'sales_pipes' => $this->mirrorAmountSummary($salesPipes, 'GrandTotal', 'TotalAmount'),
                'sales_traders' => $this->mirrorAmountSummary($salesTraders, 'GrandTotal', 'TotalAmount'),
                'vouchers' => [
                    'count' => (clone $vouchers)->count(),
                    'amount' => (float) (clone $vouchers)->sum('amount'),
                ],
            ],
            'sales_by_branch' => $this->salesByBranch($from, $to, $branchId),
            'branches' => Branch::query()->where('is_active', true)->orderBy('name')->get(['id', 'code', 'name']),
        ]);
    }

    public function quotations(Request $request): JsonResponse
    {
        [$from, $to, $branchId] = $this->filters($request);
        $search = $request->string('search')->toString();
        $perPage = min(100, max(10, (int) $request->input('per_page', 25)));

        $query = QuotationHeader::query()
            ->with('branch:id,code,name')
            ->whereBetween('date', [$from, $to])
            ->when($branchId, fn ($q) => $q->where('branch_id', $branchId))
            ->when($search !== '', function ($q) use ($search) {
                $q->where(function ($inner) use ($search) {
                    $inner->orWhere('customename', 'like', "%{$search}%")
                        ->orWhere('Status', 'like', "%{$search}%")
                        ->orWhere('Quotationid', 'like', "%{$search}%")
                        ->orWhere('City', 'like', "%{$search}%")
                        ->orWhere('Referenceid', 'like', "%{$search}%");
                });
            })
            ->orderByDesc('date');

        /** @var LengthAwarePaginator $page */
        $page = $query->paginate($perPage);
        $this->quotationSummaries->attachLineStats(collect($page->items()));

        $page->setCollection(
            collect($page->items())->map(fn (QuotationHeader $row) => MonitorDocumentPresenter::quotation($row))
        );

        return response()->json($page);
    }

    public function estimations(Request $request): JsonResponse
    {
        return $this->mirrorPaginatedList(
            $request,
            QuotationEstimation::class,
            'date',
            ['customername', 'Status', 'Estimationid', 'Quotationid', 'City', 'Referenceid', 'Paymentmode'],
            fn (QuotationEstimation $row) => MonitorDocumentPresenter::estimation($row),
            fn (LengthAwarePaginator $page) => $this->attachEstimationLineStats($page),
        );
    }

    public function sales(Request $request): JsonResponse
    {
        return $this->mirrorPaginatedList(
            $request,
            Sales::class,
            'EnteredOn',
            ['customername', 'Salesid', 'City', 'Referenceid', 'Paymentmode', 'State'],
            fn (Sales $row) => MonitorDocumentPresenter::sale($row, 'sales'),
        );
    }

    public function salesPipes(Request $request): JsonResponse
    {
        return $this->mirrorPaginatedList(
            $request,
            SalesPipes::class,
            'EnteredOn',
            ['customername', 'Salesid', 'City', 'Referenceid', 'Paymentmode', 'State'],
            fn (SalesPipes $row) => MonitorDocumentPresenter::sale($row, 'pipes'),
        );
    }

    public function salesTraders(Request $request): JsonResponse
    {
        return $this->mirrorPaginatedList(
            $request,
            SalesTraders::class,
            'EnteredOn',
            ['customername', 'Salesid', 'City', 'Referenceid', 'Paymentmode', 'State'],
            fn (SalesTraders $row) => MonitorDocumentPresenter::sale($row, 'traders'),
        );
    }

    public function vouchers(Request $request): JsonResponse
    {
        return $this->paginatedList($request, Voucher::class, ['party_name', 'status', 'voucher_type']);
    }

    public function settlementsSummary(Request $request): JsonResponse
    {
        [$from, $to, $branchId] = $this->filters($request);

        $rows = Settlement::query()
            ->with('branch:id,code,name')
            ->whereBetween('settlement_date', [$from, $to])
            ->when($branchId, fn ($q) => $q->where('branch_id', $branchId))
            ->orderByDesc('settlement_date')
            ->get();

        $totals = [
            'count' => $rows->count(),
            'beginning_cash' => (float) $rows->sum('beginning_cash'),
            'cash_in' => (float) $rows->sum('cash_in'),
            'cash_out' => (float) $rows->sum('cash_out'),
            'net_cash' => (float) $rows->sum('net_cash'),
            'actual_cash' => (float) $rows->sum('actual_cash'),
            'over_short' => (float) $rows->sum('over_short'),
        ];

        return response()->json([
            'period' => ['from' => $from, 'to' => $to],
            'totals' => $totals,
            'rows' => $rows,
        ]);
    }

    /**
     * @param  class-string<Model>  $modelClass
     * @param  callable(Model): array<string, mixed>  $present
     */
    private function mirrorPaginatedList(
        Request $request,
        string $modelClass,
        string $dateColumn,
        array $searchColumns,
        callable $present,
        ?callable $afterPaginate = null,
    ): JsonResponse {
        [$from, $to, $branchId] = $this->filters($request);
        $search = $request->string('search')->toString();
        $perPage = min(100, max(10, (int) $request->input('per_page', 25)));

        $query = $modelClass::query()
            ->with('branch:id,code,name')
            ->whereBetween($dateColumn, [$from, $to])
            ->when($branchId, fn ($q) => $q->where('branch_id', $branchId))
            ->when($search !== '', function (Builder $q) use ($search, $searchColumns, $modelClass) {
                $idColumn = match ($modelClass) {
                    QuotationEstimation::class => 'Estimationid',
                    default => 'Salesid',
                };

                $q->where(function ($inner) use ($search, $searchColumns, $idColumn) {
                    foreach ($searchColumns as $column) {
                        $inner->orWhere($column, 'like', "%{$search}%");
                    }
                    $inner->orWhere($idColumn, 'like', "%{$search}%");
                });
            })
            ->orderByDesc($dateColumn);

        /** @var LengthAwarePaginator $page */
        $page = $query->paginate($perPage);

        if ($afterPaginate) {
            $afterPaginate($page);
        }

        $page->setCollection(collect($page->items())->map($present));

        return response()->json($page);
    }

    private function attachEstimationLineStats(LengthAwarePaginator $page): void
    {
        $items = collect($page->items());
        if ($items->isEmpty()) {
            return;
        }

        $byBranch = $items->groupBy('branch_id');

        foreach ($byBranch as $branchId => $rows) {
            $ids = $rows->pluck('Estimationid')->map(fn ($id) => (string) $id)->all();

            $counts = QuotationEstimationDetails::query()
                ->where('branch_id', $branchId)
                ->whereIn('Estimationid', $ids)
                ->select('Estimationid', DB::raw('COUNT(*) as line_count'))
                ->groupBy('Estimationid')
                ->pluck('line_count', 'Estimationid');

            foreach ($rows as $row) {
                $row->setAttribute('line_count', (int) ($counts[(string) $row->Estimationid] ?? 0));
            }
        }
    }

    /**
     * @param  Builder<Model>  $query
     * @return array{count: int, amount: float}
     */
    private function mirrorAmountSummary(Builder $query, string ...$amountColumns): array
    {
        $rows = (clone $query)->get(['branch_id', ...$amountColumns]);

        $amount = $rows->sum(function (Model $row) use ($amountColumns) {
            foreach ($amountColumns as $column) {
                $parsed = MonitorDocumentPresenter::parseAmount($row->{$column} ?? null);
                if ($parsed > 0) {
                    return $parsed;
                }
            }

            return 0.0;
        });

        return [
            'count' => $rows->count(),
            'amount' => (float) $amount,
        ];
    }

    /**
     * @return list<array{branch_id: int, count: int, total: float, branch: mixed}>
     */
    private function salesByBranch(string $from, string $to, ?int $branchId): array
    {
        $merged = [];

        foreach ([Sales::class, SalesPipes::class, SalesTraders::class] as $modelClass) {
            $rows = $modelClass::query()
                ->select('branch_id', DB::raw('COUNT(*) as count'))
                ->whereBetween('EnteredOn', [$from, $to])
                ->when($branchId, fn ($q) => $q->where('branch_id', $branchId))
                ->groupBy('branch_id')
                ->get();

            $headers = $modelClass::query()
                ->whereBetween('EnteredOn', [$from, $to])
                ->when($branchId, fn ($q) => $q->where('branch_id', $branchId))
                ->get(['branch_id', 'GrandTotal', 'TotalAmount']);

            $amountsByBranch = $headers->groupBy('branch_id')->map(
                fn ($group) => $group->sum(
                    fn ($row) => MonitorDocumentPresenter::parseAmount($row->GrandTotal ?? $row->TotalAmount)
                )
            );

            foreach ($rows as $row) {
                $bid = (int) $row->branch_id;
                if (! isset($merged[$bid])) {
                    $merged[$bid] = ['branch_id' => $bid, 'count' => 0, 'total' => 0.0];
                }
                $merged[$bid]['count'] += (int) $row->count;
                $merged[$bid]['total'] += (float) ($amountsByBranch[$bid] ?? 0);
            }
        }

        $branchIds = array_keys($merged);
        $branches = Branch::query()->whereIn('id', $branchIds)->get(['id', 'code', 'name'])->keyBy('id');

        return collect($merged)
            ->map(function (array $row) use ($branches) {
                $row['total'] = round($row['total'], 2);
                $row['branch'] = $branches->get($row['branch_id']);

                return $row;
            })
            ->sortByDesc('total')
            ->values()
            ->all();
    }

    /**
     * @param  class-string<Model>  $modelClass
     */
    private function paginatedList(Request $request, string $modelClass, array $searchColumns): JsonResponse
    {
        [$from, $to, $branchId] = $this->filters($request);
        $search = $request->string('search')->toString();
        $perPage = min(100, max(10, (int) $request->input('per_page', 25)));

        $query = $modelClass::query()
            ->with('branch:id,code,name')
            ->whereBetween('document_date', [$from, $to])
            ->when($branchId, fn ($q) => $q->where('branch_id', $branchId))
            ->when($search !== '', function ($q) use ($search, $searchColumns) {
                $q->where(function ($inner) use ($search, $searchColumns) {
                    foreach ($searchColumns as $column) {
                        $inner->orWhere($column, 'like', "%{$search}%");
                    }
                    $inner->orWhere('external_id', 'like', "%{$search}%");
                });
            })
            ->orderByDesc('document_date');

        return response()->json($query->paginate($perPage));
    }

    /** @return array{0: string, 1: string, 2: ?int} */
    private function filters(Request $request): array
    {
        $from = $request->input('from', now()->subDays(30)->toDateString());
        $to = $request->input('to', now()->toDateString());
        $branchId = $request->filled('branch_id') ? (int) $request->input('branch_id') : null;

        return [$from, $to, $branchId];
    }
}
