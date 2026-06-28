<?php

namespace App\Services;

use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;

class ProductMasterLookupService
{
    /** @var array<string, string> */
    private const BRANCH_TABLES = [
        'RR-SALEM' => 'ProductMaster_salem',
        'RR-NAMAKKAL' => 'ProductMaster_namakkal',
        'RR-KOLATHUR' => 'ProductMaster_kolathur',
    ];

    /**
     * @param  list<string|null>  $productIds
     * @return array<string, string>  product id => display name
     */
    public function namesForBranch(string $branchCode, array $productIds): array
    {
        $table = self::BRANCH_TABLES[$branchCode] ?? null;
        if ($table === null) {
            return [];
        }

        $keys = collect($productIds)
            ->filter(fn ($id) => $id !== null && trim((string) $id) !== '')
            ->map(fn ($id) => trim((string) $id))
            ->unique()
            ->values();

        if ($keys->isEmpty()) {
            return [];
        }

        $names = $this->lookupInTable($table, $keys->all());

        $missing = $keys->filter(fn (string $id) => ! isset($names[$id]))->values()->all();
        if ($missing === []) {
            return $names;
        }

        foreach (self::BRANCH_TABLES as $code => $fallbackTable) {
            if ($code === $branchCode) {
                continue;
            }

            $extra = $this->lookupInTable($fallbackTable, $missing);
            foreach ($extra as $productId => $name) {
                if (! isset($names[$productId])) {
                    $names[$productId] = $name;
                }
            }
        }

        return $names;
    }

    /**
     * @param  list<string>  $productKeys
     * @return array<string, string>
     */
    private function lookupInTable(string $table, array $productKeys): array
    {
        if ($productKeys === [] || ! $this->tableExists($table)) {
            return [];
        }

        $names = [];
        $numericIds = collect($productKeys)
            ->filter(fn (string $id) => ctype_digit($id))
            ->map(fn (string $id) => (int) $id)
            ->filter(fn (int $id) => $id > 0)
            ->unique()
            ->values()
            ->all();

        if ($numericIds !== []) {
            $rows = DB::table($table)
                ->whereIn('id', $numericIds)
                ->get(['id', 'DisplayName', 'ItemName']);

            foreach ($rows as $row) {
                $name = $this->rowName($row);
                if ($name !== null) {
                    $names[(string) $row->id] = $name;
                }
            }
        }

        $codeKeys = collect($productKeys)
            ->reject(fn (string $id) => isset($names[$id]))
            ->values()
            ->all();

        if ($codeKeys !== []) {
            $rows = DB::table($table)
                ->whereIn('ItemCode', $codeKeys)
                ->get(['ItemCode', 'DisplayName', 'ItemName']);

            foreach ($rows as $row) {
                $name = $this->rowName($row);
                if ($name !== null && $row->ItemCode !== null && $row->ItemCode !== '') {
                    $names[(string) $row->ItemCode] = $name;
                }
            }
        }

        return $names;
    }

    private function rowName(object $row): ?string
    {
        $name = trim((string) ($row->DisplayName ?? $row->ItemName ?? ''));

        return $name !== '' ? $name : null;
    }

    private function tableExists(string $table): bool
    {
        return Schema::hasTable($table);
    }

    /**
     * @param  list<array<string, mixed>>  $lines
     * @return list<array<string, mixed>>
     */
    public function attachProductNames(string $branchCode, array $lines): array
    {
        $needsLookup = [];

        foreach ($lines as $index => $line) {
            $existing = trim((string) ($line['product_name'] ?? ''));
            if ($existing !== '') {
                continue;
            }

            $productId = $line['product_id'] ?? null;
            if ($productId !== null && trim((string) $productId) !== '') {
                $needsLookup[$index] = (string) $productId;
            }
        }

        if ($needsLookup === []) {
            return $lines;
        }

        $names = $this->namesForBranch($branchCode, array_values($needsLookup));

        if ($names === []) {
            return $lines;
        }

        foreach ($needsLookup as $index => $productId) {
            if (isset($names[$productId])) {
                $lines[$index]['product_name'] = $names[$productId];
            }
        }

        return $lines;
    }
}
