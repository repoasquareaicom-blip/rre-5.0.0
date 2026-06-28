<?php

namespace App\Services;

use App\Support\BranchDateTime;

class MssqlMirrorMapper
{
    /**
     * @param  array<string, mixed>  $row
     * @return array<string, mixed>
     */
    public function map(string $table, int $branchId, array $row): array
    {
        $schema = config("sync_mirror.tables.{$table}")
            ?? config("quotation_schema.tables.{$table}");

        if (! $schema) {
            throw new \InvalidArgumentException("Unknown mirror table: {$table}");
        }

        $normalized = $this->applyAliases($row, $schema['aliases'] ?? []);

        $payload = ['branch_id' => $branchId];

        foreach ($schema['columns'] as $column) {
            $value = $this->pickValue($normalized, $column);
            if ($value !== null || array_key_exists($column, $normalized)) {
                $payload[$column] = $this->cast($value, $column, $schema);
            }
        }

        return $payload;
    }

    /**
     * @param  array<string, mixed>  $row
     * @param  array<string, string>  $aliases
     * @return array<string, mixed>
     */
    private function applyAliases(array $row, array $aliases): array
    {
        $out = $row;

        foreach ($aliases as $from => $to) {
            if (array_key_exists($from, $out) && ! array_key_exists($to, $out)) {
                $out[$to] = $out[$from];
            }
        }

        return $out;
    }

    /**
     * @param  array<string, mixed>  $row
     */
    private function pickValue(array $row, string $column): mixed
    {
        if (array_key_exists($column, $row)) {
            return $row[$column];
        }

        foreach ($row as $key => $value) {
            if (strcasecmp((string) $key, $column) === 0) {
                return $value;
            }
        }

        return null;
    }

    /**
     * @param  array<string, mixed>  $schema
     */
    private function cast(mixed $value, string $column, array $schema): mixed
    {
        if ($value === null || $value === '') {
            return in_array($column, $schema['integers'] ?? [], true) ? null : null;
        }

        if (in_array($column, $schema['integers'] ?? [], true)) {
            return (int) $value;
        }

        if (in_array($column, $schema['booleans'] ?? [], true)) {
            return filter_var($value, FILTER_VALIDATE_BOOLEAN, FILTER_NULL_ON_FAILURE) ?? (bool) $value;
        }

        if (isset($schema['decimals'][$column])) {
            return round((float) $value, (int) $schema['decimals'][$column]);
        }

        if (in_array($column, $schema['datetimes'] ?? [], true)) {
            return BranchDateTime::toDisplay($value);
        }

        if (is_string($value)) {
            $trimmed = trim($value);

            return $trimmed === '' ? null : $trimmed;
        }

        return $value;
    }
}
