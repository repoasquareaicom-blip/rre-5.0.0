<?php

namespace App\Services;

use App\Models\ActivityEvent;
use App\Models\Branch;
use App\Models\QuotationHeader;
use App\Support\BranchDateTime;
use Carbon\Carbon;
use Illuminate\Database\Eloquent\Model;

class ActivityFeedLogger
{
    /** @var array<string, string> */
    private const LABELS = [
        'quotations' => 'Quotation',
        'estimations' => 'Estimation',
        'sales' => 'Sale',
        'sales-pipes' => 'Sales (Pipes)',
        'sales-traders' => 'Sales (Traders)',
        'vouchers' => 'Voucher',
        'receipts' => 'Receipt',
        'settlements' => 'Settlement',
    ];

    public function log(Branch $branch, string $entityType, Model $model, bool $wasCreated): void
    {
        $singular = rtrim($entityType, 's');
        $label = self::LABELS[$entityType] ?? ucfirst($singular);
        $kind = $wasCreated ? 'created' : 'updated';
        $verb = $wasCreated ? 'New' : 'Updated';

        ActivityEvent::query()->updateOrCreate(
            [
                'branch_id' => $branch->id,
                'entity_type' => $singular,
                'external_id' => $this->externalIdFor($entityType, $model),
            ],
            [
                'entity_record_id' => $this->recordIdFor($entityType, $model),
                'event_kind' => $kind,
                'title' => "{$verb} {$label} · {$branch->name}",
                'subtitle' => $this->subtitleFor($entityType, $model),
                'amount' => $this->amountFor($entityType, $model),
                'activity_date' => $this->activityDateFor($entityType, $model),
                'occurred_at' => BranchDateTime::toDisplay($this->occurredAtFor($entityType, $model)),
            ]
        );
    }

    private function externalIdFor(string $entityType, Model $model): string
    {
        return match ($entityType) {
            'quotations' => (string) $model->Quotationid,
            'estimations' => (string) $model->Estimationid,
            'sales', 'sales-pipes', 'sales-traders' => (string) $model->Salesid,
            default => (string) $model->external_id,
        };
    }

    private function recordIdFor(string $entityType, Model $model): int
    {
        return match ($entityType) {
            'quotations' => (int) ($model->sino ?? 0),
            'estimations' => (int) ($model->SlNo ?? 0),
            'sales', 'sales-pipes', 'sales-traders' => (int) ($model->slno ?? 0),
            default => (int) $model->getKey(),
        };
    }

    private function activityDateFor(string $entityType, Model $model): string
    {
        return match ($entityType) {
            'quotations' => $this->quotationActivityDate($model),
            'estimations' => $this->mirrorHeaderActivityDate($model, 'date', 'Updatedon'),
            'sales', 'sales-pipes', 'sales-traders' => $this->mirrorHeaderActivityDate($model, 'EnteredOn', 'Updatedon'),
            'settlements' => BranchDateTime::toDateString($model->getRawOriginal('settlement_date') ?? $model->settlement_date)
                ?? BranchDateTime::today(),
            default => BranchDateTime::toDateString($model->getRawOriginal('document_date') ?? $model->document_date)
                ?? BranchDateTime::toDateString($model->getRawOriginal('source_updated_at') ?? $model->source_updated_at)
                ?? BranchDateTime::today(),
        };
    }

    private function occurredAtFor(string $entityType, Model $model): Carbon
    {
        $raw = match ($entityType) {
            'quotations' => $this->quotationSyncedAt($model),
            'estimations', 'sales', 'sales-pipes', 'sales-traders' => $model->getRawOriginal('syncon') ?? $model->syncon,
            default => $model->getRawOriginal('source_updated_at') ?? $model->source_updated_at ?? $model->updated_at,
        };

        $display = BranchDateTime::toDisplay($raw);

        if ($display) {
            return Carbon::parse($display);
        }

        return BranchDateTime::now();
    }

    private function mirrorHeaderActivityDate(Model $model, string $primaryField, string $fallbackField): string
    {
        $value = $model->getRawOriginal($primaryField) ?? $model->{$primaryField}
            ?? $model->getRawOriginal($fallbackField) ?? $model->{$fallbackField};

        return BranchDateTime::toDateString($value) ?? BranchDateTime::today();
    }

    private function quotationActivityDate(Model $model): string
    {
        $synced = $this->quotationSyncedAt($model);

        return BranchDateTime::toDateString($synced)
            ?? BranchDateTime::dateFromModel($model, 'date')
            ?? BranchDateTime::today();
    }

    private function quotationSyncedAt(Model $model): mixed
    {
        return $model->getRawOriginal('syncon') ?? $model->syncon;
    }

    private function subtitleFor(string $entityType, Model $model): ?string
    {
        return match ($entityType) {
            'quotations' => $model->customename ?? $model->customer_name ?? null,
            'estimations' => $model->customername ?? $model->customer_name ?? null,
            'sales', 'sales-pipes', 'sales-traders' => $model->customername ?? $model->customer_name ?? null,
            'vouchers', 'receipts' => $model->party_name,
            'settlements' => 'Day close · '.$model->settlement_date?->format('d M Y'),
            default => $this->externalIdFor($entityType, $model),
        };
    }

    private function amountFor(string $entityType, Model $model): ?float
    {
        return match ($entityType) {
            'settlements' => (float) ($model->net_cash ?? 0),
            'quotations' => $model instanceof QuotationHeader
                ? ($model->total_amount > 0 ? (float) $model->total_amount : null)
                : null,
            'estimations' => $this->parseAmount($model->GrandTotal ?? null),
            'sales', 'sales-pipes', 'sales-traders' => $this->parseAmount($model->GrandTotal ?? $model->TotalAmount ?? null),
            default => isset($model->amount) ? (float) $model->amount : null,
        };
    }

    private function parseAmount(mixed $value): ?float
    {
        if ($value === null || $value === '') {
            return null;
        }

        $n = (float) preg_replace('/[^0-9.\-]/', '', (string) $value);

        return $n > 0 ? $n : null;
    }
}
