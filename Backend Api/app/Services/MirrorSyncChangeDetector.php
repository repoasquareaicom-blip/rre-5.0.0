<?php

namespace App\Services;

use App\Models\Branch;
use Carbon\Carbon;
use Illuminate\Database\Eloquent\Model;

/**
 * Detect whether an incoming mirror sync record differs from cloud storage.
 * Used to skip writes and preserve syncon when nothing changed.
 */
class MirrorSyncChangeDetector
{
    /** Cloud-enriched columns — ignore when comparing incoming branch payload. */
    private const IGNORE_DETAIL_COLUMNS = ['product_name'];

    public function __construct(
        private MssqlMirrorMapper $mapper,
    ) {}

    /**
     * @param  array<string, mixed>  $headerPayload
     * @param  list<array<string, mixed>>|null  $lines
     * @param  array<string, mixed>  $config
     */
    public function isUnchanged(
        ?Model $existing,
        array $headerPayload,
        ?array $lines,
        array $config,
        Branch $branch,
    ): bool {
        if (! $existing) {
            return false;
        }

        if (! $this->headerUnchanged($existing, $headerPayload)) {
            return false;
        }

        if ($lines === null || $lines === []) {
            return true;
        }

        return $this->detailsUnchanged($existing, $lines, $config, $branch);
    }

    /**
     * @param  array<string, mixed>  $headerPayload
     */
    public function headerUnchanged(Model $existing, array $headerPayload): bool
    {
        foreach ($headerPayload as $column => $incoming) {
            if ($column === 'branch_id') {
                continue;
            }

            $current = $existing->getRawOriginal($column) ?? $existing->getAttribute($column);
            if (! $this->valuesEqual($current, $incoming)) {
                return false;
            }
        }

        return true;
    }

    /**
     * @param  list<array<string, mixed>>  $lines
     * @param  array<string, mixed>  $config
     */
    public function detailsUnchanged(Model $header, array $lines, array $config, Branch $branch): bool
    {
        /** @var class-string<Model> $detailsModel */
        $detailsModel = $config['details_model'];
        $parentColumn = $config['details_parent_column'];
        $parentId = (string) $header->{$parentColumn};
        $lineKey = $config['details_line_key'] ?? null;

        $incomingPayloads = $this->mapDetailPayloads($lines, $config, $branch, $parentId);

        if ($lineKey) {
            return $this->lineKeyedDetailsUnchanged(
                $detailsModel,
                $branch,
                $parentColumn,
                $parentId,
                $lineKey,
                $incomingPayloads
            );
        }

        return $this->replaceAllDetailsUnchanged(
            $detailsModel,
            $branch,
            $parentColumn,
            $parentId,
            $incomingPayloads
        );
    }

    /**
     * @param  list<array<string, mixed>>  $lines
     * @param  array<string, mixed>  $config
     * @return list<array<string, mixed>>
     */
    private function mapDetailPayloads(array $lines, array $config, Branch $branch, string $parentId): array
    {
        $payloads = [];
        $parentColumn = $config['details_parent_column'];
        $lineKey = $config['details_line_key'] ?? null;

        foreach ($lines as $line) {
            if (! is_array($line)) {
                continue;
            }

            $payload = $this->mapper->map($config['details_table'], $branch->id, $line);
            $payload[$parentColumn] = $payload[$parentColumn] ?? $parentId;

            if ($lineKey) {
                $keyValue = (int) ($payload[$lineKey] ?? 0);
                if ($keyValue < 1) {
                    continue;
                }
            }

            $payloads[] = $payload;
        }

        return $payloads;
    }

    /**
     * @param  class-string<Model>  $detailsModel
     * @param  list<array<string, mixed>>  $incomingPayloads
     */
    private function lineKeyedDetailsUnchanged(
        string $detailsModel,
        Branch $branch,
        string $parentColumn,
        string $parentId,
        string $lineKey,
        array $incomingPayloads,
    ): bool {
        $existingRows = $detailsModel::query()
            ->where('branch_id', $branch->id)
            ->where($parentColumn, $parentId)
            ->get();

        if ($existingRows->count() !== count($incomingPayloads)) {
            return false;
        }

        $incomingByKey = [];
        foreach ($incomingPayloads as $payload) {
            $incomingByKey[(int) $payload[$lineKey]] = $payload;
        }

        foreach ($existingRows as $row) {
            $key = (int) $row->{$lineKey};
            $incoming = $incomingByKey[$key] ?? null;
            if ($incoming === null || ! $this->payloadsEqual($row, $incoming)) {
                return false;
            }
        }

        return true;
    }

    /**
     * @param  class-string<Model>  $detailsModel
     * @param  list<array<string, mixed>>  $incomingPayloads
     */
    private function replaceAllDetailsUnchanged(
        string $detailsModel,
        Branch $branch,
        string $parentColumn,
        string $parentId,
        array $incomingPayloads,
    ): bool {
        $existingRows = $detailsModel::query()
            ->where('branch_id', $branch->id)
            ->where($parentColumn, $parentId)
            ->get();

        if ($existingRows->count() !== count($incomingPayloads)) {
            return false;
        }

        $existingFingerprints = $existingRows
            ->map(fn (Model $row) => $this->payloadFingerprint($row, array_keys($incomingPayloads[0] ?? [])))
            ->sort()
            ->values()
            ->all();

        $incomingFingerprints = collect($incomingPayloads)
            ->map(fn (array $payload) => $this->payloadFingerprintFromArray($payload))
            ->sort()
            ->values()
            ->all();

        return $existingFingerprints === $incomingFingerprints;
    }

    /**
     * @param  array<string, mixed>  $payload
     */
    private function payloadFingerprintFromArray(array $payload): string
    {
        $normalized = [];
        foreach ($payload as $key => $value) {
            if ($key === 'branch_id' || in_array($key, self::IGNORE_DETAIL_COLUMNS, true)) {
                continue;
            }
            $normalized[$key] = $this->normalizeValue($value);
        }
        ksort($normalized);

        return json_encode($normalized, JSON_THROW_ON_ERROR);
    }

    /**
     * @param  list<string>  $columns
     */
    private function payloadFingerprint(Model $row, array $columns): string
    {
        $normalized = [];
        foreach ($columns as $column) {
            if ($column === 'branch_id' || in_array($column, self::IGNORE_DETAIL_COLUMNS, true)) {
                continue;
            }
            $current = $row->getRawOriginal($column) ?? $row->getAttribute($column);
            $normalized[$column] = $this->normalizeValue($current);
        }
        ksort($normalized);

        return json_encode($normalized, JSON_THROW_ON_ERROR);
    }

    /**
     * @param  array<string, mixed>  $incoming
     */
    private function payloadsEqual(Model $existing, array $incoming): bool
    {
        foreach ($incoming as $column => $value) {
            if ($column === 'branch_id' || in_array($column, self::IGNORE_DETAIL_COLUMNS, true)) {
                continue;
            }

            $current = $existing->getRawOriginal($column) ?? $existing->getAttribute($column);
            if (! $this->valuesEqual($current, $value)) {
                return false;
            }
        }

        return true;
    }

    private function valuesEqual(mixed $current, mixed $incoming): bool
    {
        return $this->normalizeValue($current) === $this->normalizeValue($incoming);
    }

    private function normalizeValue(mixed $value): ?string
    {
        if ($value === null || $value === '') {
            return null;
        }

        if ($value instanceof \DateTimeInterface) {
            return Carbon::instance($value)->format('Y-m-d H:i:s');
        }

        if (is_bool($value)) {
            return $value ? '1' : '0';
        }

        if (is_int($value) || is_float($value)) {
            return (string) $value;
        }

        if (is_string($value)) {
            $trimmed = trim($value);

            return $trimmed === '' ? null : $trimmed;
        }

        if (is_array($value)) {
            return json_encode($value, JSON_THROW_ON_ERROR);
        }

        return (string) $value;
    }
}
