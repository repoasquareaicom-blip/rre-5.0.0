<?php

namespace App\Services;

use App\Models\Branch;
use App\Models\Receipt;
use App\Models\Settlement;
use App\Models\SyncCursor;
use App\Models\SyncLog;
use App\Models\Voucher;
use App\Support\BranchDateTime;
use Carbon\Carbon;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Support\Arr;
use Illuminate\Support\Facades\DB;

class SyncUpsertService
{
    /** @var array<string, class-string<Model>> */
    private const LEGACY_ENTITY_MAP = [
        'vouchers' => Voucher::class,
        'receipts' => Receipt::class,
        'settlements' => Settlement::class,
    ];

    public function __construct(
        private SyncCursorService $cursorService,
        private HeaderSyncCursorService $headerCursors,
        private MssqlHeaderDetailSyncService $mirrorSync,
        private MirrorSyncChangeDetector $changeDetector,
        private ActivityFeedLogger $activityFeed,
    ) {}

    public function upsert(Branch $branch, string $entityType, array $records, ?string $explicitLastSyncedAt = null): array
    {
        $isMirror = $this->mirrorSync->entityConfig($entityType) !== null;
        $isLegacy = isset(self::LEGACY_ENTITY_MAP[$entityType]);

        if (! $isMirror && ! $isLegacy) {
            throw new \InvalidArgumentException("Unknown entity type: {$entityType}");
        }

        $log = SyncLog::create([
            'branch_id' => $branch->id,
            'entity_type' => $entityType,
            'records_received' => count($records),
            'status' => 'running',
            'started_at' => BranchDateTime::nowString(),
        ]);

        $upserted = 0;
        $linesUpserted = 0;
        $headersCreated = 0;
        $headersUpdated = 0;
        $headersUnchanged = 0;
        $latestSourceAt = null;

        try {
            if ($isMirror) {
                $chunkSize = (int) config('sync_mirror.chunk_size', 10);

                foreach (array_chunk($records, $chunkSize) as $chunk) {
                    DB::transaction(function () use ($branch, $entityType, $chunk, &$upserted, &$linesUpserted, &$headersCreated, &$headersUpdated, &$headersUnchanged, &$latestSourceAt) {
                        foreach ($chunk as $record) {
                            $this->upsertMirrorRecord(
                                $branch,
                                $entityType,
                                $record,
                                $upserted,
                                $linesUpserted,
                                $headersCreated,
                                $headersUpdated,
                                $headersUnchanged,
                                $latestSourceAt
                            );
                        }
                    });
                }
            } else {
                $modelClass = self::LEGACY_ENTITY_MAP[$entityType];

                DB::transaction(function () use ($branch, $entityType, $records, $modelClass, &$upserted, &$latestSourceAt) {
                    foreach ($records as $record) {
                        $this->upsertLegacyRecord($branch, $entityType, $record, $modelClass, $upserted, $latestSourceAt);
                    }
                });
            }

            if ($latestSourceAt && $this->headerCursors->tableNameForEntity($entityType) === null) {
                SyncCursor::updateOrCreate(
                    [
                        'branch_id' => $branch->id,
                        'entity_type' => $entityType,
                    ],
                    [
                        'last_synced_at' => $latestSourceAt,
                    ]
                );
            }

            $this->cursorService->applyExplicitLastSyncedAt(
                $branch,
                $entityType,
                $explicitLastSyncedAt
            );

            $log->update([
                'records_upserted' => $upserted,
                'status' => 'success',
                'completed_at' => BranchDateTime::nowString(),
            ]);
        } catch (\Throwable $e) {
            $log->update([
                'records_upserted' => $upserted,
                'status' => 'failed',
                'error_message' => $e->getMessage(),
                'completed_at' => BranchDateTime::nowString(),
            ]);

            throw $e;
        }

        $result = [
            'entity' => $entityType,
            'received' => count($records),
            'upserted' => $upserted,
        ];

        if ($isMirror) {
            $result['lines_upserted'] = $linesUpserted;
            $result['headers_created'] = $headersCreated;
            $result['headers_updated'] = $headersUpdated;
            $result['headers_unchanged'] = $headersUnchanged;

            $key = $this->headerCursors->responseKeyForEntity($entityType);
            if ($key) {
                $result[$key] = $this->headerCursors->lastSyncOnForEntity($branch, $entityType);
            }
        }

        return $result;
    }

    /**
     * @param  array<string, mixed>  $record
     */
    private function upsertMirrorRecord(
        Branch $branch,
        string $entityType,
        array $record,
        int &$upserted,
        int &$linesUpserted,
        int &$headersCreated,
        int &$headersUpdated,
        int &$headersUnchanged,
        ?Carbon &$latestSourceAt,
    ): void {
        $config = $this->mirrorSync->entityConfig($entityType);
        /** @var class-string<Model> $headerModel */
        $headerModel = $config['header_model'];

        $headerPayload = $this->mirrorSync->normalizeHeaderRow($entityType, $branch->id, $record);
        $idColumn = $config['header_id_column'];

        if (empty($headerPayload[$idColumn])) {
            return;
        }

        $unique = array_combine(
            $config['header_unique'],
            array_map(fn (string $key) => $key === 'branch_id' ? $branch->id : $headerPayload[$key], $config['header_unique'])
        );

        $existing = $headerModel::query()->where($unique)->first();

        if ($existing && $this->isSalesEntity($entityType) && $this->sameEnteredOn($existing, $headerPayload)) {
            $headersUnchanged++;

            return;
        }

        $identityColumn = $config['header_identity_column'] ?? null;
        if ($identityColumn && empty($headerPayload[$identityColumn])) {
            if ($existing) {
                $headerPayload[$identityColumn] = $existing->{$identityColumn};
            } else {
                throw new \InvalidArgumentException(
                    "Missing required identity column [{$identityColumn}] for {$entityType} {$headerPayload[$idColumn]}"
                );
            }
        }

        $lines = $record['details'] ?? $record['lines'] ?? [];
        $detailLines = is_array($lines) && $lines !== [] ? $lines : null;

        if ($this->changeDetector->isUnchanged($existing, $headerPayload, $detailLines, $config, $branch)) {
            $headersUnchanged++;

            return;
        }

        $syncedAt = BranchDateTime::toDisplay(BranchDateTime::now()->addMilliseconds($upserted));
        $headerChanged = ! $existing || ! $this->changeDetector->headerUnchanged($existing, $headerPayload);

        if (! $existing) {
            ['model' => $model, 'created' => $created] = $headerModel::upsertRow(
                $unique,
                [...$headerPayload, 'syncon' => $syncedAt]
            );
            $headersCreated++;
        } elseif ($headerChanged) {
            ['model' => $model, 'created' => $created] = $headerModel::upsertRow(
                $unique,
                [...$headerPayload, 'syncon' => $syncedAt]
            );
            $headersUpdated++;
        } else {
            $existing->forceFill(['syncon' => $syncedAt])->save();
            $model = $existing->fresh();
            $created = false;
            $headersUpdated++;
        }

        if ($detailLines !== null) {
            $linesUpserted += $this->mirrorSync->syncDetails($entityType, $branch, $model, $detailLines);
        }

        $this->activityFeed->log($branch, $entityType, $model, $created);

        $upserted++;

        $watermark = $config['watermark'];
        if (! empty($headerPayload[$watermark])) {
            $ts = Carbon::parse($headerPayload[$watermark]);
            if (! $latestSourceAt || $ts->gt($latestSourceAt)) {
                $latestSourceAt = $ts;
            }
        }
    }

    /**
     * @param  array<string, mixed>  $record
     * @param  class-string<Model>  $modelClass
     */
    private function upsertLegacyRecord(
        Branch $branch,
        string $entityType,
        array $record,
        string $modelClass,
        int &$upserted,
        ?Carbon &$latestSourceAt,
    ): void {
        $payload = $this->normalizeLegacyRecord($entityType, $record);
        $externalId = $payload['external_id'] ?? null;

        if (! $externalId) {
            return;
        }

        $payload['branch_id'] = $branch->id;

        $model = $modelClass::updateOrCreate(
            [
                'branch_id' => $branch->id,
                'external_id' => $externalId,
            ],
            $payload
        );

        $this->activityFeed->log($branch, $entityType, $model, $model->wasRecentlyCreated);

        $upserted++;

        if (! empty($payload['source_updated_at'])) {
            $ts = Carbon::parse($payload['source_updated_at']);
            if (! $latestSourceAt || $ts->gt($latestSourceAt)) {
                $latestSourceAt = $ts;
            }
        }
    }

    /**
     * @param  array<string, mixed>  $record
     * @return array<string, mixed>
     */
    private function normalizeLegacyRecord(string $entityType, array $record): array
    {
        $externalId = $record['external_id'] ?? $record['externalId'] ?? null;
        $sourceUpdated = $record['source_updated_at'] ?? $record['sourceUpdatedAt'] ?? $record['updated_at'] ?? null;

        $base = [
            'external_id' => (string) $externalId,
            'source_updated_at' => $sourceUpdated ? Carbon::parse($sourceUpdated) : null,
            'meta' => Arr::except($record, $this->legacyKnownFields($entityType)),
        ];

        return array_merge($base, match ($entityType) {
            'vouchers' => [
                'voucher_type' => $record['voucher_type'] ?? $record['voucherType'] ?? null,
                'document_date' => $this->date($record['document_date'] ?? $record['documentDate'] ?? null),
                'party_name' => $record['party_name'] ?? $record['partyName'] ?? null,
                'status' => $record['status'] ?? null,
                'amount' => (float) ($record['amount'] ?? 0),
            ],
            'receipts' => [
                'receipt_type' => $record['receipt_type'] ?? $record['receiptType'] ?? 'Cash Receipt',
                'document_date' => $this->date($record['document_date'] ?? $record['documentDate'] ?? null),
                'party_name' => $record['party_name'] ?? $record['partyName'] ?? null,
                'status' => $record['status'] ?? null,
                'amount' => (float) ($record['amount'] ?? 0),
            ],
            'settlements' => [
                'settlement_date' => $this->date($record['settlement_date'] ?? $record['settlementDate'] ?? $record['document_date'] ?? null),
                'beginning_cash' => (float) ($record['beginning_cash'] ?? $record['beginningCash'] ?? 0),
                'cash_in' => (float) ($record['cash_in'] ?? $record['cashIn'] ?? 0),
                'cash_out' => (float) ($record['cash_out'] ?? $record['cashOut'] ?? 0),
                'net_cash' => (float) ($record['net_cash'] ?? $record['netCash'] ?? 0),
                'actual_cash' => (float) ($record['actual_cash'] ?? $record['actualCash'] ?? 0),
                'over_short' => (float) ($record['over_short'] ?? $record['overShort'] ?? 0),
                'status' => $record['status'] ?? null,
            ],
            default => [],
        });
    }

    /**
     * @return list<string>
     */
    private function legacyKnownFields(string $entityType): array
    {
        return match ($entityType) {
            'vouchers' => ['external_id', 'externalId', 'voucher_type', 'voucherType', 'document_date', 'documentDate', 'party_name', 'partyName', 'status', 'amount', 'source_updated_at', 'sourceUpdatedAt', 'updated_at'],
            'receipts' => ['external_id', 'externalId', 'receipt_type', 'receiptType', 'document_date', 'documentDate', 'party_name', 'partyName', 'status', 'amount', 'source_updated_at', 'sourceUpdatedAt', 'updated_at'],
            'settlements' => ['external_id', 'externalId', 'settlement_date', 'settlementDate', 'document_date', 'beginning_cash', 'beginningCash', 'cash_in', 'cashIn', 'cash_out', 'cashOut', 'net_cash', 'netCash', 'actual_cash', 'actualCash', 'over_short', 'overShort', 'status', 'source_updated_at', 'sourceUpdatedAt', 'updated_at'],
            default => [],
        };
    }

    private function date(mixed $value): ?string
    {
        if (! $value) {
            return null;
        }

        return Carbon::parse($value)->toDateString();
    }

    private function isSalesEntity(string $entityType): bool
    {
        return in_array($entityType, ['sales', 'sales-pipes', 'sales-traders'], true);
    }

    /**
     * @param  array<string, mixed>  $headerPayload
     */
    private function sameEnteredOn(Model $existing, array $headerPayload): bool
    {
        $incoming = BranchDateTime::toDisplay($headerPayload['EnteredOn'] ?? null);
        $stored = BranchDateTime::toDisplay($existing->getRawOriginal('EnteredOn') ?? $existing->EnteredOn);

        if ($incoming === null || $stored === null) {
            return false;
        }

        return $incoming === $stored;
    }
}
