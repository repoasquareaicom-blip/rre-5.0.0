<?php

namespace App\Services;

use App\Models\Branch;
use App\Support\BranchDateTime;
use Illuminate\Database\Eloquent\Model;

class HeaderSyncCursorService
{
    /**
     * @return array<string, string|null> e.g. quotation_last_sync_on => ISO8601
     */
    public function flatLastSyncOn(Branch $branch): array
    {
        $values = [];

        foreach (config('sync_headers.tables', []) as $definition) {
            $key = (string) ($definition['response_key'] ?? '');
            if ($key === '') {
                continue;
            }

            $values[$key] = $this->lastSyncOnForTable($branch, $definition);
        }

        return $values;
    }

    public function responseKeyForEntity(string $entityType): ?string
    {
        foreach (config('sync_headers.tables', []) as $definition) {
            if (($definition['entity'] ?? '') === $entityType) {
                return (string) ($definition['response_key'] ?? null);
            }
        }

        return null;
    }

    public function lastSyncOnForEntity(Branch $branch, string $entityType): ?string
    {
        foreach (config('sync_headers.tables', []) as $definition) {
            if (($definition['entity'] ?? '') === $entityType) {
                return $this->lastSyncOnForTable($branch, $definition);
            }
        }

        return null;
    }

    public function tableNameForEntity(string $entityType): ?string
    {
        foreach (config('sync_headers.tables', []) as $tableName => $definition) {
            if (($definition['entity'] ?? '') === $entityType) {
                return (string) $tableName;
            }
        }

        return null;
    }

    /**
     * @param  array<string, mixed>  $definition
     */
    private function lastSyncOnForTable(Branch $branch, array $definition): ?string
    {
        /** @var class-string<Model> $modelClass */
        $modelClass = $definition['model'];
        $watermark = (string) $definition['watermark'];

        $row = $modelClass::query()
            ->where('branch_id', $branch->id)
            ->whereNotNull($watermark)
            ->orderByDesc($watermark)
            ->first([$watermark]);

        if (! $row) {
            return null;
        }

        return BranchDateTime::toDisplay($row->getRawOriginal($watermark));
    }

    /**
     * @return list<string>
     */
    public function headerEntityTypes(): array
    {
        return array_values(array_map(
            fn (array $definition): string => (string) $definition['entity'],
            config('sync_headers.tables', [])
        ));
    }

}
