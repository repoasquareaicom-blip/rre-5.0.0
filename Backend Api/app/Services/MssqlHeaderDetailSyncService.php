<?php

namespace App\Services;

use App\Models\Branch;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Support\Arr;

class MssqlHeaderDetailSyncService
{
    public function __construct(
        private MssqlMirrorMapper $mapper,
    ) {}

    /**
     * @return array<string, mixed>|null
     */
    public function entityConfig(string $entityType): ?array
    {
        return config("sync_mirror.entities.{$entityType}");
    }

    /**
     * @param  array<string, mixed>  $record
     * @return array<string, mixed>
     */
    public function normalizeHeaderRow(string $entityType, int $branchId, array $record): array
    {
        $config = $this->entityConfig($entityType);
        if (! $config) {
            throw new \InvalidArgumentException("Unknown mirror entity: {$entityType}");
        }

        $headerOnly = Arr::except($record, ['details', 'lines', 'header']);

        if (isset($record['header']) && is_array($record['header'])) {
            $headerOnly = $record['header'];
        }

        $mapped = $this->mapper->map($config['header_table'], $branchId, $headerOnly);

        $idColumn = $config['header_id_column'];
        if (! empty($mapped[$idColumn])) {
            $mapped[$idColumn] = trim((string) $mapped[$idColumn]);
        }

        if (! empty($config['header_identity_column'])) {
            $this->fillHeaderIdentity($mapped, $headerOnly, (string) $config['header_identity_column']);
        }

        return $mapped;
    }

    /**
     * MS SQL identity columns may arrive as sino/SiNo while cloud stores slno/SlNo.
     *
     * @param  array<string, mixed>  $mapped
     * @param  array<string, mixed>  $raw
     */
    private function fillHeaderIdentity(array &$mapped, array $raw, string $targetColumn): void
    {
        if (! empty($mapped[$targetColumn])) {
            return;
        }

        foreach ($raw as $key => $value) {
            if ($value === null || $value === '') {
                continue;
            }

            if (strcasecmp((string) $key, $targetColumn) === 0
                || strcasecmp((string) $key, 'sino') === 0
                || strcasecmp((string) $key, 'SiNo') === 0) {
                $mapped[$targetColumn] = (int) $value;

                return;
            }
        }
    }

    /**
     * @param  Model  $header
     * @param  array<int, array<string, mixed>>  $lines
     */
    public function syncDetails(string $entityType, Branch $branch, Model $header, array $lines): int
    {
        $config = $this->entityConfig($entityType);
        /** @var class-string<Model> $detailsModel */
        $detailsModel = $config['details_model'];
        $parentColumn = $config['details_parent_column'];
        $parentId = $header->{$parentColumn};
        $lineKey = $config['details_line_key'] ?? null;

        if ($lineKey) {
            return $this->syncDetailsWithLineKey(
                $entityType,
                $branch,
                $detailsModel,
                $config,
                $parentColumn,
                (string) $parentId,
                $lines
            );
        }

        return $this->syncDetailsReplaceAll(
            $branch,
            $detailsModel,
            $config,
            $parentColumn,
            (string) $parentId,
            $lines
        );
    }

    /**
     * @param  class-string<Model>  $detailsModel
     * @param  array<string, mixed>  $config
     * @param  array<int, array<string, mixed>>  $lines
     */
    private function syncDetailsWithLineKey(
        string $entityType,
        Branch $branch,
        string $detailsModel,
        array $config,
        string $parentColumn,
        string $parentId,
        array $lines,
    ): int {
        $syncedKeys = [];
        $count = 0;

        foreach ($lines as $line) {
            if (! is_array($line)) {
                continue;
            }

            $payload = $this->mapper->map($config['details_table'], $branch->id, $line);
            $payload[$parentColumn] = $payload[$parentColumn] ?? $parentId;

            $lineKey = $config['details_line_key'];
            $keyValue = (int) ($payload[$lineKey] ?? 0);
            if ($keyValue < 1) {
                continue;
            }

            $detailsModel::upsertRow(
                [
                    'branch_id' => $branch->id,
                    $lineKey => $keyValue,
                ],
                $payload
            );

            $syncedKeys[] = $keyValue;
            $count++;
        }

        if ($syncedKeys !== []) {
            $detailsModel::query()
                ->where('branch_id', $branch->id)
                ->where($parentColumn, $parentId)
                ->whereNotIn($config['details_line_key'], $syncedKeys)
                ->delete();
        }

        return $count;
    }

    /**
     * @param  class-string<Model>  $detailsModel
     * @param  array<int, array<string, mixed>>  $lines
     */
    /**
     * @param  class-string<Model>  $detailsModel
     * @param  array<string, mixed>  $config
     * @param  array<int, array<string, mixed>>  $lines
     */
    private function syncDetailsReplaceAll(
        Branch $branch,
        string $detailsModel,
        array $config,
        string $parentColumn,
        string $parentId,
        array $lines,
    ): int {
        $detailsModel::query()
            ->where('branch_id', $branch->id)
            ->where($parentColumn, $parentId)
            ->delete();

        $rows = [];
        foreach ($lines as $line) {
            if (! is_array($line) || ! $config) {
                continue;
            }

            $payload = $this->mapper->map($config['details_table'], $branch->id, $line);
            $payload[$parentColumn] = $payload[$parentColumn] ?? $parentId;
            $rows[] = $payload;
        }

        if ($rows === []) {
            return 0;
        }

        foreach (array_chunk($rows, 200) as $chunk) {
            $detailsModel::query()->insert($chunk);
        }

        return count($rows);
    }
}
