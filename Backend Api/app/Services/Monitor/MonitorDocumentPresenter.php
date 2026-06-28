<?php

namespace App\Services\Monitor;

use App\Support\BranchDateTime;
use App\Models\QuotationEstimation;
use App\Models\QuotationHeader;
use App\Models\Sales;
use Illuminate\Database\Eloquent\Model;

class MonitorDocumentPresenter
{
    public static function parseAmount(mixed $value): float
    {
        if ($value === null || $value === '') {
            return 0.0;
        }

        return (float) preg_replace('/[^0-9.\-]/', '', (string) $value);
    }

    /**
     * @return array<string, mixed>
     */
    public static function quotation(QuotationHeader $row): array
    {
        return [
            'id' => (int) ($row->sino ?? 0),
            'external_id' => (string) $row->Quotationid,
            'customer_name' => $row->customename ?? null,
            'document_date' => self::formatDate($row->date),
            'status' => $row->Status ?? null,
            'city' => $row->City ?? null,
            'reference_id' => $row->Referenceid ?? null,
            'line_count' => (int) ($row->line_count ?? 0),
            'total_amount' => (float) ($row->total_amount ?? 0),
            'amount' => (float) ($row->total_amount ?? 0),
            'synced_at' => self::formatDateTime($row->syncon),
            'branch' => $row->branch?->only(['id', 'code', 'name']),
        ];
    }

    /**
     * @return array<string, mixed>
     */
    public static function estimation(QuotationEstimation $row): array
    {
        $amount = self::parseAmount($row->GrandTotal ?? $row->Total);

        return [
            'id' => (int) ($row->SlNo ?? 0),
            'external_id' => (string) $row->Estimationid,
            'quotation_external_id' => $row->Quotationid ?? null,
            'customer_name' => $row->customername ?? null,
            'document_date' => self::formatDate($row->date),
            'status' => $row->Status ?? null,
            'city' => $row->City ?? null,
            'reference_id' => $row->Referenceid ?? null,
            'payment_mode' => $row->Paymentmode ?? null,
            'is_billed' => (bool) $row->IsBilled,
            'line_count' => (int) ($row->line_count ?? 0),
            'amount' => $amount,
            'total_amount' => $amount,
            'synced_at' => self::formatDateTime($row->syncon),
            'branch' => $row->branch?->only(['id', 'code', 'name']),
        ];
    }

    /**
     * @param  'sales'|'pipes'|'traders'  $channel
     * @return array<string, mixed>
     */
    public static function sale(Model $row, string $channel): array
    {
        $amount = self::parseAmount($row->GrandTotal ?? $row->TotalAmount);

        return [
            'id' => (int) ($row->slno ?? 0),
            'external_id' => (string) $row->Salesid,
            'customer_name' => $row->customername ?? null,
            'document_date' => self::formatDate($row->EnteredOn ?? $row->Updatedon),
            'status' => null,
            'city' => $row->City ?? null,
            'reference_id' => $row->Referenceid ?? null,
            'payment_mode' => $row->Paymentmode ?? null,
            'amount' => $amount,
            'total_amount' => $amount,
            'sales_channel' => $channel,
            'synced_at' => self::formatDateTime($row->syncon),
            'branch' => $row->branch?->only(['id', 'code', 'name']),
        ];
    }

    private static function formatDate(mixed $value): ?string
    {
        return BranchDateTime::toDateString($value);
    }

    private static function formatDateTime(mixed $value): ?string
    {
        return BranchDateTime::toDisplay($value);
    }
}
