<?php

namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use App\Models\Branch;
use App\Models\SyncCursor;
use App\Models\SyncInboundLog;
use App\Models\SyncLog;
use App\Services\BranchPresenceService;
use App\Services\HeaderSyncCursorService;
use App\Services\SyncCursorService;
use App\Services\SyncUpsertService;
use Illuminate\Http\JsonResponse;
use Illuminate\Http\Request;

class SyncController extends Controller
{
    public function __construct(
        private SyncUpsertService $syncService,
        private BranchPresenceService $presence,
        private SyncCursorService $cursorService,
        private HeaderSyncCursorService $headerCursors,
    ) {}

    public function lastSyncOn(Request $request): JsonResponse
    {
        /** @var Branch $branch */
        $branch = $request->attributes->get('sync_branch');

        return response()->json(array_merge(
            ['branch_code' => $branch->code],
            $this->headerCursors->flatLastSyncOn($branch),
        ));
    }

    public function cursor(Request $request, string $entity): JsonResponse
    {
        $entityType = strtolower($entity);
        $allowed = array_merge(
            $this->headerCursors->headerEntityTypes(),
            ['vouchers', 'receipts', 'settlements']
        );

        if (! in_array($entityType, $allowed, true)) {
            return response()->json(['message' => 'Unknown entity type.'], 404);
        }

        /** @var Branch $branch */
        $branch = $request->attributes->get('sync_branch');

        return response()->json($this->cursorService->cursorPayload($branch, $entityType));
    }

    public function quotations(Request $request): JsonResponse
    {
        return $this->syncEntity($request, 'quotations');
    }

    public function estimations(Request $request): JsonResponse
    {
        return $this->syncEntity($request, 'estimations');
    }

    public function sales(Request $request): JsonResponse
    {
        return $this->syncEntity($request, 'sales');
    }

    public function salesPipes(Request $request): JsonResponse
    {
        return $this->syncEntity($request, 'sales-pipes');
    }

    public function salesTraders(Request $request): JsonResponse
    {
        return $this->syncEntity($request, 'sales-traders');
    }

    public function vouchers(Request $request): JsonResponse
    {
        return $this->syncEntity($request, 'vouchers');
    }

    public function receipts(Request $request): JsonResponse
    {
        return $this->syncEntity($request, 'receipts');
    }

    public function settlements(Request $request): JsonResponse
    {
        return $this->syncEntity($request, 'settlements');
    }

    public function presence(Request $request): JsonResponse
    {
        $validated = $request->validate([
            'users' => ['sometimes', 'array'],
            'users.*.external_id' => ['required_without:users.*.externalId', 'string', 'max:64'],
            'users.*.login_flag' => ['sometimes', 'integer', 'in:0,1'],
            'users.*.machine_name' => ['sometimes', 'nullable', 'string', 'max:128'],
            'users.*.user_name' => ['sometimes', 'nullable', 'string', 'max:128'],
            'login_flag' => ['required_without:users', 'integer', 'in:0,1'],
            'external_id' => ['required_without:users', 'string', 'max:64'],
            'machine_name' => ['sometimes', 'nullable', 'string', 'max:128'],
            'user_name' => ['sometimes', 'nullable', 'string', 'max:128'],
        ]);

        /** @var Branch $branch */
        $branch = $request->attributes->get('sync_branch');

        $users = $validated['users'] ?? [[
            'external_id' => $validated['external_id'],
            'login_flag' => $validated['login_flag'],
            'machine_name' => $validated['machine_name'] ?? null,
            'user_name' => $validated['user_name'] ?? null,
        ]];

        $reported = $this->presence->syncActiveUsers($branch, $users);
        $online = $this->presence->onlineCountForBranch($branch->id);

        return response()->json([
            'ok' => true,
            'branch_code' => $branch->code,
            'reported' => $reported,
            'users_online' => $online,
        ]);
    }

    public function status(Request $request): JsonResponse
    {
        /** @var Branch $branch */
        $branch = $request->attributes->get('sync_branch');

        $cursors = SyncCursor::query()
            ->where('branch_id', $branch->id)
            ->get()
            ->keyBy('entity_type');

        $recentLogs = SyncLog::query()
            ->where('branch_id', $branch->id)
            ->latest()
            ->limit(20)
            ->get();

        $inboundLogs = SyncInboundLog::query()
            ->where('branch_id', $branch->id)
            ->orderByDesc('started_at')
            ->limit(30)
            ->get([
                'id',
                'endpoint',
                'http_method',
                'http_status',
                'status',
                'records_received',
                'records_upserted',
                'lines_upserted',
                'error_message',
                'duration_ms',
                'started_at',
                'completed_at',
            ]);

        return response()->json([
            'branch' => [
                'code' => $branch->code,
                'name' => $branch->name,
            ],
            'cursors' => $cursors,
            'recent_logs' => $recentLogs,
            'inbound_logs' => $inboundLogs,
        ]);
    }

    private function syncEntity(Request $request, string $entityType): JsonResponse
    {
        set_time_limit((int) config('sync.max_execution_seconds', 120));

        if ($this->isMirrorEntity($entityType)) {
            $validated = $request->validate([
                'records' => ['required', 'array'],
                'records.*' => ['array'],
                'last_synced_at' => ['sometimes', 'date'],
            ]);

            $records = array_map(
                fn (array $record): array => $this->flattenMirrorRecord($record),
                $validated['records'],
            );
        } else {
            $validated = $request->validate([
                'records' => ['required', 'array'],
                'records.*.external_id' => ['required_without:records.*.externalId', 'string', 'max:64'],
                'last_synced_at' => ['sometimes', 'date'],
            ]);

            $records = $validated['records'];
        }

        /** @var Branch $branch */
        $branch = $request->attributes->get('sync_branch');

        $result = $this->syncService->upsert(
            $branch,
            $entityType,
            $records,
            isset($validated['last_synced_at'])
                ? (string) $validated['last_synced_at']
                : null,
        );

        return response()->json([
            'ok' => true,
            'branch_code' => $branch->code,
            ...$result,
        ]);
    }

    private function isMirrorEntity(string $entityType): bool
    {
        return config("sync_mirror.entities.{$entityType}") !== null;
    }

    /**
     * BranchSync sends { "header": { ...cols }, "details": [...] } per record.
     *
     * @param  array<string, mixed>  $record
     * @return array<string, mixed>
     */
    private function flattenMirrorRecord(array $record): array
    {
        if (isset($record['header']) && is_array($record['header'])) {
            return array_merge($record['header'], [
                'details' => $record['details'] ?? $record['lines'] ?? [],
            ]);
        }

        return $record;
    }
}
