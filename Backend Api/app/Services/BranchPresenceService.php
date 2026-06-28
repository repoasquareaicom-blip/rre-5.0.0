<?php

namespace App\Services;

use App\Models\Branch;
use App\Models\BranchUserPresence;
use Illuminate\Support\Collection;

class BranchPresenceService
{
    /**
     * @param  array<int, array<string, mixed>>  $users
     */
    public function syncActiveUsers(Branch $branch, array $users): int
    {
        $processed = 0;

        foreach ($users as $row) {
            $externalId = (string) ($row['external_id'] ?? $row['externalId'] ?? $row['user_id'] ?? $row['userId'] ?? '');
            if ($externalId === '') {
                continue;
            }

            $loginFlag = $this->resolveLoginFlag($row);
            if ($loginFlag === null) {
                continue;
            }

            $now = now();
            $isLoggedIn = $loginFlag === 1;

            BranchUserPresence::query()->updateOrCreate(
                [
                    'branch_id' => $branch->id,
                    'external_user_id' => $externalId,
                ],
                [
                    'user_name' => $row['user_name'] ?? $row['Name'] ?? $row['userName'] ?? null,
                    'machine_name' => $row['machine_name'] ?? $row['machineName'] ?? null,
                    'is_logged_in' => $isLoggedIn,
                    'last_seen_at' => $now,
                ]
            );

            $processed++;
        }

        return $processed;
    }

    /**
     * @param  array<string, mixed>  $row
     */
    private function resolveLoginFlag(array $row): ?int
    {
        if (array_key_exists('login_flag', $row)) {
            return (int) $row['login_flag'] === 1 ? 1 : 0;
        }

        if (array_key_exists('loginFlag', $row)) {
            return (int) $row['loginFlag'] === 1 ? 1 : 0;
        }

        if (array_key_exists('is_logged_in', $row)) {
            return filter_var($row['is_logged_in'], FILTER_VALIDATE_BOOLEAN) ? 1 : 0;
        }

        // Batch sync of active users (no explicit flag) — treat as login.
        return 1;
    }

    /**
     * @param  Collection<int, int>  $branchIds
     * @return array<int, int>
     */
    public function onlineCountsByBranch(Collection $branchIds): array
    {
        if ($branchIds->isEmpty()) {
            return [];
        }

        return BranchUserPresence::query()
            ->selectRaw('branch_id, COUNT(*) as aggregate')
            ->whereIn('branch_id', $branchIds)
            ->where('is_logged_in', true)
            ->groupBy('branch_id')
            ->pluck('aggregate', 'branch_id')
            ->map(fn ($count) => (int) $count)
            ->all();
    }

    /**
     * Users currently logged in with presence updated today (inventory login_flag = 1).
     *
     * @param  Collection<int, int>  $branchIds
     * @return array<int, int>
     */
    public function loggedInTodayCountsByBranch(Collection $branchIds): array
    {
        if ($branchIds->isEmpty()) {
            return [];
        }

        $startOfDay = now()->startOfDay();

        return BranchUserPresence::query()
            ->selectRaw('branch_id, COUNT(*) as aggregate')
            ->whereIn('branch_id', $branchIds)
            ->where('is_logged_in', true)
            ->where('last_seen_at', '>=', $startOfDay)
            ->groupBy('branch_id')
            ->pluck('aggregate', 'branch_id')
            ->map(fn ($count) => (int) $count)
            ->all();
    }

    public function onlineCountForBranch(int $branchId): int
    {
        return (int) BranchUserPresence::query()
            ->where('branch_id', $branchId)
            ->where('is_logged_in', true)
            ->count();
    }
}
