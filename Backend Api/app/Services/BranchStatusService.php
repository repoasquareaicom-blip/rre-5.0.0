<?php

namespace App\Services;

use App\Models\Branch;
use App\Models\SyncInboundLog;
use App\Support\BranchDateTime;
use Carbon\Carbon;
use Illuminate\Support\Collection;

class BranchStatusService
{
    /** Minutes since last successful /api/sync call to consider branch active. */
    private const ONLINE_THRESHOLD_MINUTES = 10;

    public function __construct(private BranchPresenceService $presence) {}

    /**
     * @return Collection<int, array<string, mixed>>
     */
    public function branchesWithStatus(): Collection
    {
        $threshold = BranchDateTime::now()->subMinutes(self::ONLINE_THRESHOLD_MINUTES);

        $branches = Branch::query()
            ->where('is_active', true)
            ->orderBy('name')
            ->get(['id', 'code', 'short_code', 'name']);

        $userCounts = $this->presence->loggedInTodayCountsByBranch($branches->pluck('id'));

        /** @var Collection<int, string> $lastSyncByBranch */
        $lastSyncByBranch = SyncInboundLog::query()
            ->selectRaw('branch_id, MAX(completed_at) as last_sync_at')
            ->where('status', 'success')
            ->whereNotNull('branch_id')
            ->groupBy('branch_id')
            ->pluck('last_sync_at', 'branch_id');

        return $branches->map(function (Branch $branch) use ($threshold, $userCounts, $lastSyncByBranch) {
                $lastSyncDisplay = BranchDateTime::toDisplay($lastSyncByBranch->get($branch->id));
                $lastSyncAt = $lastSyncDisplay
                    ? Carbon::parse($lastSyncDisplay, BranchDateTime::TZ)
                    : null;

                $syncActive = $lastSyncAt !== null && $lastSyncAt->gte($threshold);

                return [
                    'id' => $branch->id,
                    'code' => $branch->code,
                    'short_code' => $branch->short_code ?? self::deriveShortCode($branch->code),
                    'name' => $branch->name,
                    'server_online' => $syncActive,
                    'client_online' => $syncActive,
                    'last_sync_at' => $lastSyncDisplay,
                    'users_logged_in' => $userCounts[$branch->id] ?? 0,
                    'users_online' => $userCounts[$branch->id] ?? 0,
                ];
            });
    }

    public static function deriveShortCode(string $code): string
    {
        $segment = strtoupper((string) str($code)->afterLast('-')->replaceMatches('/[^A-Z]/', ''));

        return str_pad(substr($segment, 0, 3), 3, 'X');
    }
}
