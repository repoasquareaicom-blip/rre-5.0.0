<?php

namespace App\Services;

use App\Models\Branch;
use App\Models\SyncCursor;
use Carbon\Carbon;

class SyncCursorService
{
    public function __construct(
        private HeaderSyncCursorService $headerCursors,
    ) {}

    /** Incremental sync by header `Updatedon` (picks up edits; sino does not change). */
    public const UPDATED_ON_ENTITIES = ['quotations', 'estimations'];

    public function usesUpdatedOnCursor(string $entityType): bool
    {
        return in_array($entityType, self::UPDATED_ON_ENTITIES, true);
    }

    public function usesEnteredOnCursor(string $entityType): bool
    {
        return in_array($entityType, ['sales', 'sales-pipes', 'sales-traders'], true);
    }

    public function getCursor(Branch $branch, string $entityType): SyncCursor
    {
        return SyncCursor::query()->firstOrCreate(
            [
                'branch_id' => $branch->id,
                'entity_type' => $entityType,
            ],
            [
                'last_sino' => 0,
            ]
        );
    }

    /**
     * @return array<string, mixed>
     */
    public function cursorPayload(Branch $branch, string $entityType): array
    {
        $responseKey = $this->headerCursors->responseKeyForEntity($entityType);

        if ($responseKey !== null) {
            return [
                'branch_code' => $branch->code,
                $responseKey => $this->headerCursors->lastSyncOnForEntity($branch, $entityType),
            ];
        }

        $cursor = $this->getCursor($branch, $entityType);

        $type = $this->usesUpdatedOnCursor($entityType) ? 'updatedon' : 'timestamp';

        return [
            'branch_code' => $branch->code,
            'branch_id' => $branch->id,
            'entity_type' => $entityType,
            'cursor_type' => $type,
            'last_sync_on' => $cursor->last_synced_at?->toIso8601String(),
            'last_synced_at' => $cursor->last_synced_at?->toIso8601String(),
            'last_external_id' => $cursor->last_external_id,
            'last_sino' => (int) $cursor->last_sino,
        ];
    }

    public function applyExplicitLastSyncedAt(
        Branch $branch,
        string $entityType,
        ?string $explicitLastSyncedAt,
    ): void {
        if (! $explicitLastSyncedAt) {
            return;
        }

        if ($this->headerCursors->tableNameForEntity($entityType) !== null) {
            return;
        }

        if (! $this->usesUpdatedOnCursor($entityType)) {
            return;
        }

        $ts = Carbon::parse($explicitLastSyncedAt);
        $cursor = $this->getCursor($branch, $entityType);

        if (! $cursor->last_synced_at || $ts->gt($cursor->last_synced_at)) {
            $cursor->last_synced_at = $ts;
            $cursor->save();
        }
    }
}
