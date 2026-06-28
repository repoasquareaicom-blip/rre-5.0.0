<?php

namespace App\Services;

use App\Support\BranchDateTime;
use App\Models\QuotationDetails;
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
use Illuminate\Database\Eloquent\Model;

class FeedDocumentDetailService
{
    public function __construct(
        private QuotationSummaryService $quotationSummaries,
        private ProductMasterLookupService $productNames,
    ) {}

    /**
     * @return array<string, mixed>|null
     */
    public function detail(string $entityType, int $branchId, string $externalId): ?array
    {
        return match ($entityType) {
            'quotation' => $this->quotationDetail($branchId, $externalId),
            'estimation' => $this->estimationDetail($branchId, $externalId),
            'sale' => $this->saleDetail($branchId, $externalId, Sales::class, SalesDetails::class, 'sale'),
            'sales-pipe' => $this->saleDetail($branchId, $externalId, SalesPipes::class, SalesPipesDetails::class, 'pipes'),
            'sales-trader' => $this->saleDetail($branchId, $externalId, SalesTraders::class, SalesTradersDetails::class, 'traders'),
            default => null,
        };
    }

    /**
     * @return array<string, mixed>|null
     */
    private function quotationDetail(int $branchId, string $externalId): ?array
    {
        $header = QuotationHeader::query()
            ->with('branch:id,code,name')
            ->where('branch_id', $branchId)
            ->where('Quotationid', $externalId)
            ->first();

        if (! $header) {
            return null;
        }

        $this->quotationSummaries->attachLineStats(collect([$header]));

        $lines = QuotationDetails::query()
            ->where('branch_id', $branchId)
            ->where('Quotationid', $externalId)
            ->orderBy('sino')
            ->get();

        $lineRows = $this->productNames->attachProductNames(
            (string) $header->branch?->code,
            $lines->map(fn (QuotationDetails $line, int $index) => $this->formatLine(
                (int) ($line->sino ?: $index + 1),
                $line->Productid,
                $line->Rate,
                $line->Quantity,
                $line->amount_decimal,
            ))->values()->all(),
        );

        $totalAmount = (float) ($header->total_amount ?? 0);

        return [
            'entity_type' => 'quotation',
            'document_id' => (string) $header->Quotationid,
            'header' => [
                'customer_name' => $header->customename,
                'document_datetime' => $this->formatDateTime($header->date ?? $header->syncon),
                'status' => $header->Status,
                'city' => $header->City,
                'reference_id' => $header->Referenceid,
                'payment_mode' => null,
                'quotation_id' => null,
                'line_count' => count($lineRows),
                'total_amount' => round($totalAmount, 2),
                'branch' => $header->branch?->only(['id', 'code', 'name']),
            ],
            'lines' => $lineRows,
        ];
    }

    /**
     * @return array<string, mixed>|null
     */
    private function estimationDetail(int $branchId, string $externalId): ?array
    {
        $header = QuotationEstimation::query()
            ->with('branch:id,code,name')
            ->where('branch_id', $branchId)
            ->where('Estimationid', $externalId)
            ->first();

        if (! $header) {
            return null;
        }

        $lines = QuotationEstimationDetails::query()
            ->where('branch_id', $branchId)
            ->where('Estimationid', $externalId)
            ->orderBy('sino')
            ->get();

        $lineRows = $this->productNames->attachProductNames(
            (string) $header->branch?->code,
            $lines->map(fn (QuotationEstimationDetails $line, int $index) => $this->formatLine(
                (int) ($line->sino ?: $index + 1),
                $line->Productid,
                $line->Rate,
                $line->Quantity,
                MonitorDocumentPresenter::parseAmount($line->Amount),
            ))->values()->all(),
        );

        $totalAmount = MonitorDocumentPresenter::parseAmount($header->GrandTotal ?? $header->Total);

        return [
            'entity_type' => 'estimation',
            'document_id' => (string) $header->Estimationid,
            'header' => [
                'customer_name' => $header->customername,
                'document_datetime' => $this->formatDateTime($header->date ?? $header->syncon),
                'status' => $header->Status,
                'city' => $header->City,
                'reference_id' => $header->Referenceid,
                'payment_mode' => $header->Paymentmode,
                'quotation_id' => $header->Quotationid ? (string) $header->Quotationid : null,
                'line_count' => count($lineRows),
                'total_amount' => round($totalAmount, 2),
                'branch' => $header->branch?->only(['id', 'code', 'name']),
            ],
            'lines' => $lineRows,
        ];
    }

    /**
     * @param  class-string<Model>  $headerModel
     * @param  class-string<Model>  $detailsModel
     * @return array<string, mixed>|null
     */
    private function saleDetail(
        int $branchId,
        string $externalId,
        string $headerModel,
        string $detailsModel,
        string $channel,
    ): ?array {
        $header = $headerModel::query()
            ->with('branch:id,code,name')
            ->where('branch_id', $branchId)
            ->where('Salesid', $externalId)
            ->first();

        if (! $header) {
            return null;
        }

        $lines = $detailsModel::query()
            ->where('branch_id', $branchId)
            ->where('Salesid', $externalId)
            ->get();

        $lineRows = $this->productNames->attachProductNames(
            (string) $header->branch?->code,
            $lines->values()->map(fn (Model $line, int $index) => $this->formatLine(
                $index + 1,
                $line->Productid ?? null,
                $line->Rate ?? null,
                $line->Quantity ?? null,
                MonitorDocumentPresenter::parseAmount($line->Amount ?? null),
                isset($line->gst) ? (float) $line->gst : null,
                $line->product_name ?? null,
            ))->all(),
        );

        $totalAmount = MonitorDocumentPresenter::parseAmount($header->GrandTotal ?? $header->TotalAmount);

        return [
            'entity_type' => $channel === 'sale' ? 'sale' : "sales-{$channel}",
            'document_id' => (string) $header->Salesid,
            'header' => [
                'customer_name' => $header->customername,
                'document_datetime' => $this->formatDateTime($header->EnteredOn ?? $header->Updatedon ?? $header->syncon),
                'status' => null,
                'city' => $header->City ?? null,
                'reference_id' => $header->Referenceid ?? null,
                'payment_mode' => $header->Paymentmode ?? null,
                'quotation_id' => null,
                'line_count' => count($lineRows),
                'total_amount' => round($totalAmount, 2),
                'sales_channel' => $channel,
                'branch' => $header->branch?->only(['id', 'code', 'name']),
            ],
            'lines' => $lineRows,
        ];
    }

    /**
     * @return array<string, mixed>
     */
    private function formatLine(
        int $lineNo,
        mixed $productId,
        mixed $rate,
        mixed $quantity,
        float $amount,
        ?float $gst = null,
        mixed $productName = null,
    ): array {
        $row = [
            'line_no' => $lineNo,
            'product_id' => $productId !== null ? (string) $productId : null,
            'rate' => MonitorDocumentPresenter::parseAmount($rate),
            'quantity' => $quantity !== null ? (string) $quantity : null,
            'amount' => round($amount, 2),
        ];

        $name = trim((string) ($productName ?? ''));
        if ($name !== '') {
            $row['product_name'] = $name;
        }

        if ($gst !== null) {
            $row['gst'] = round($gst, 2);
        }

        return $row;
    }

    private function formatDateTime(mixed $value): ?string
    {
        return BranchDateTime::toDisplay($value);
    }
}
