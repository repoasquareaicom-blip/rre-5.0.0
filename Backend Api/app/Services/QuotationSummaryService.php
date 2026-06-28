<?php

namespace App\Services;

use App\Support\BranchDateTime;
use App\Models\ActivityEvent;
use App\Models\QuotationDetails;
use App\Models\QuotationHeader;
use Illuminate\Support\Collection;

class QuotationSummaryService
{
    /**
     * @param  Collection<int, ActivityEvent>  $events
     * @return array<string, array<string, mixed>>
     */
    public function summariesForEvents(Collection $events): array
    {
        $quotationEvents = $events->filter(fn (ActivityEvent $e) => $e->entity_type === 'quotation');

        if ($quotationEvents->isEmpty()) {
            return [];
        }

        $byBranch = [];
        foreach ($quotationEvents as $event) {
            $id = (string) $event->external_id;
            if ($id === '') {
                continue;
            }
            $byBranch[$event->branch_id][$id] = true;
        }

        $summaries = [];

        foreach ($byBranch as $branchId => $quotationIds) {
            $ids = array_keys($quotationIds);
            $headers = QuotationHeader::query()
                ->where('branch_id', $branchId)
                ->whereIn('Quotationid', $ids)
                ->get()
                ->keyBy('Quotationid');

            $lineStats = $this->lineStatsForBranch((int) $branchId, $ids);

            foreach ($ids as $quotationId) {
                $header = $headers->get($quotationId);
                if (! $header) {
                    continue;
                }

                $stats = $lineStats[$quotationId] ?? ['line_count' => 0, 'total_amount' => 0.0];
                $summaries[$this->cacheKey((int) $branchId, $quotationId)] = $this->formatSummary(
                    $header,
                    (int) $stats['line_count'],
                    (float) $stats['total_amount'],
                );
            }
        }

        return $summaries;
    }

    /**
     * @param  Collection<int, QuotationHeader>  $headers
     */
    public function attachLineStats(Collection $headers): void
    {
        if ($headers->isEmpty()) {
            return;
        }

        $byBranch = $headers->groupBy('branch_id');

        foreach ($byBranch as $branchId => $branchHeaders) {
            $ids = $branchHeaders->pluck('Quotationid')->map(fn ($id) => (string) $id)->all();
            $lineStats = $this->lineStatsForBranch((int) $branchId, $ids);

            foreach ($branchHeaders as $header) {
                $stats = $lineStats[(string) $header->Quotationid] ?? ['line_count' => 0, 'total_amount' => 0.0];
                $header->setAttribute('line_count', (int) $stats['line_count']);
                $header->setAttribute('total_amount', (float) $stats['total_amount']);
            }
        }
    }

    /**
     * @return array<string, array{line_count: int, total_amount: float}>
     */
    private function lineStatsForBranch(int $branchId, array $quotationIds): array
    {
        if ($quotationIds === []) {
            return [];
        }

        $lines = QuotationDetails::query()
            ->where('branch_id', $branchId)
            ->whereIn('Quotationid', $quotationIds)
            ->get(['Quotationid', 'Amount']);

        $stats = [];

        foreach ($lines as $line) {
            $qid = (string) $line->Quotationid;
            if (! isset($stats[$qid])) {
                $stats[$qid] = ['line_count' => 0, 'total_amount' => 0.0];
            }
            $stats[$qid]['line_count']++;
            $stats[$qid]['total_amount'] += $line->amount_decimal;
        }

        return $stats;
    }

    /**
     * @return array<string, mixed>
     */
    public function formatSummary(QuotationHeader $header, int $lineCount, float $totalAmount): array
    {
        return [
            'quotation_id' => (string) $header->Quotationid,
            'customer_name' => $header->customename,
            'customer_id' => $header->Customerid,
            'status' => $header->Status,
            'document_date' => BranchDateTime::toDateString($header->date),
            'document_datetime' => BranchDateTime::toDisplay($header->date),
            'syncon' => BranchDateTime::toDisplay($header->syncon),
            'synced_at' => BranchDateTime::toDisplay($header->syncon),
            'city' => $header->City,
            'reference_id' => $header->Referenceid,
            'assist_name' => $header->AssistName ?? $header->Assist,
            'line_count' => $lineCount,
            'total_amount' => round($totalAmount, 2),
            'is_estimate' => $header->Isestimate,
            'is_cancelled' => (bool) ($header->Iscancel ?? 0),
        ];
    }

    public function cacheKey(int $branchId, string $quotationId): string
    {
        return "{$branchId}:{$quotationId}";
    }
}
