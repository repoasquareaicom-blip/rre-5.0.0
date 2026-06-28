<?php

namespace App\Services;

use App\Models\ActivityEvent;
use App\Support\BranchDateTime;
use Carbon\Carbon;
use Illuminate\Database\Eloquent\Builder;

/**
 * Build live monitor feed from activity_events (one row per document, updated on each sync).
 */
class ActivityFeedService
{
    public function __construct(
        private MirrorActivityScope $mirrorScope,
        private FeedItemEnrichmentService $enrichment,
    ) {}

    /**
     * @param  list<int>  $branchIds
     * @param  list<string>  $types  Feed entity types (quotation, estimation, sale, …). Empty = all types.
     * @return array{items: list<array<string, mixed>>, latest_id: int}
     */
    public function feed(
        Carbon $date,
        array $branchIds,
        array $types,
        int $sinceId,
        int $limit,
    ): array {
        $dateString = $date->toDateString();
        $query = $this->baseQuery($dateString, $branchIds, $types);

        if ($sinceId > 0) {
            $events = (clone $query)
                ->where('id', '>', $sinceId)
                ->orderBy('id')
                ->limit($limit)
                ->get();
        } else {
            $events = $query
                ->orderByDesc('occurred_at')
                ->orderByDesc('id')
                ->limit($limit)
                ->get();
        }

        $items = $this->enrichment->enrichMany($events->map(fn (ActivityEvent $event) => $this->mapEvent($event)));

        $latestId = (int) ($events->max('id') ?? 0);

        if ($sinceId === 0 && $events->count() < $limit) {
            $latestId = $this->maxIdForDate($dateString, $branchIds, $types);
        }

        return [
            'items' => $items->values()->all(),
            'latest_id' => $latestId,
        ];
    }

    /**
     * @param  list<int>  $branchIds
     * @param  list<string>  $types
     * @return Builder<ActivityEvent>
     */
    private function baseQuery(string $dateString, array $branchIds, array $types): Builder
    {
        $query = ActivityEvent::query()
            ->with('branch:id,code,short_code,name')
            ->where(function (Builder $q) use ($dateString) {
                $q->whereDate('activity_date', $dateString)
                    ->orWhereDate('occurred_at', $dateString);
            })
            ->when($branchIds !== [], fn (Builder $q) => $q->whereIn('branch_id', $branchIds))
            ->when($types !== [], fn (Builder $q) => $q->whereIn('entity_type', $types));

        $this->mirrorScope->applyExistsOnMirror($query);

        return $query;
    }

    /**
     * @return array<string, mixed>
     */
    private function mapEvent(ActivityEvent $event): array
    {
        $branch = $event->branch;
        $occurredRaw = $event->getRawOriginal('occurred_at');
        $syncon = BranchDateTime::toDisplay($occurredRaw);

        return [
            'id' => (int) $event->id,
            'branch_id' => (int) $event->branch_id,
            'entity_type' => $event->entity_type,
            'external_id' => $event->external_id,
            'event_kind' => $event->event_kind,
            'title' => $event->title,
            'subtitle' => $event->subtitle,
            'amount' => $event->amount > 0 ? (float) $event->amount : null,
            'activity_date' => $event->activity_date?->toDateString(),
            'syncon' => $syncon,
            'syncon_ts' => $this->synconToMs($occurredRaw),
            'occurred_at' => $syncon,
            'branch' => $branch ? [
                'id' => $branch->id,
                'code' => $branch->code,
                'short_code' => $branch->short_code ?? null,
                'name' => $branch->name,
            ] : null,
        ];
    }

    private function synconToMs(mixed $value): int
    {
        $display = BranchDateTime::toDisplay($value);
        if (! $display) {
            return 0;
        }

        return (int) Carbon::parse($display)->format('Uv');
    }

    /**
     * @param  list<int>  $branchIds
     * @param  list<string>  $types
     */
    private function maxIdForDate(string $dateString, array $branchIds, array $types): int
    {
        return (int) ($this->baseQuery($dateString, $branchIds, $types)->max('id') ?? 0);
    }
}
