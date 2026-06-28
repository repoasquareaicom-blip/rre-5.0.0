<?php

namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use App\Models\Branch;
use App\Models\ProductMasterSyncChange;
use Illuminate\Http\JsonResponse;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;

class ProductSyncController extends Controller
{
    private const MAIN_OFFICE_BRANCH_CODE = 'RR-SALEM';
    private const SALEM_PRODUCT_TABLE = 'ProductMaster_salem';

    public function publish(Request $request): JsonResponse
    {
        /** @var Branch $branch */
        $branch = $request->attributes->get('sync_branch');

        if ($branch->code !== self::MAIN_OFFICE_BRANCH_CODE) {
            return response()->json([
                'message' => 'Product master changes are accepted only from Salem main office.',
            ], 403);
        }

        $validated = $request->validate([
            'records' => ['required', 'array', 'min:1'],
            'records.*' => ['required', 'array'],
        ]);

        $upserted = 0;

        foreach ($validated['records'] as $record) {
            $payload = $this->normalizeRecord($record);
            $productId = $this->stringValue($payload, 'id');

            if ($productId === null) {
                continue;
            }

            ProductMasterSyncChange::query()->create([
                'source_branch_id' => $branch->id,
                'product_id' => $productId,
                'item_code' => $this->stringValue($payload, 'ItemCode'),
                'item_name' => $this->stringValue($payload, 'ItemName'),
                'display_name' => $this->stringValue($payload, 'DisplayName'),
                'sales_price' => $this->decimalValue($payload, 'SalesPrice'),
                'payload' => $payload,
            ]);

            $this->upsertSalemLookupTable($payload);
            $upserted++;
        }

        return response()->json([
            'ok' => true,
            'branch_code' => $branch->code,
            'records_upserted' => $upserted,
        ]);
    }

    public function changes(Request $request): JsonResponse
    {
        $validated = $request->validate([
            'after_id' => ['sometimes', 'integer', 'min:0'],
            'limit' => ['sometimes', 'integer', 'min:1', 'max:500'],
        ]);

        $afterId = (int) ($validated['after_id'] ?? 0);
        $limit = (int) ($validated['limit'] ?? 100);

        $changes = ProductMasterSyncChange::query()
            ->where('id', '>', $afterId)
            ->orderBy('id')
            ->limit($limit)
            ->get();

        return response()->json([
            'ok' => true,
            'records' => $changes->map(fn (ProductMasterSyncChange $change): array => [
                'change_id' => $change->id,
                'product_id' => $change->product_id,
                'payload' => $change->payload,
                'updated_at' => optional($change->updated_at)->toISOString(),
            ])->values(),
        ]);
    }

    /**
     * @param  array<string, mixed>  $record
     * @return array<string, mixed>
     */
    private function normalizeRecord(array $record): array
    {
        $normalized = [];

        foreach ($record as $key => $value) {
            $normalized[(string) $key] = $value === '' ? null : $value;
        }

        return $normalized;
    }

    /**
     * @param  array<string, mixed>  $payload
     */
    private function stringValue(array $payload, string $key): ?string
    {
        $value = $payload[$key] ?? null;

        if ($value === null) {
            return null;
        }

        $value = trim((string) $value);

        return $value === '' ? null : $value;
    }

    /**
     * @param  array<string, mixed>  $payload
     */
    private function decimalValue(array $payload, string $key): ?string
    {
        $value = $this->stringValue($payload, $key);

        return $value !== null && is_numeric($value) ? $value : null;
    }

    /**
     * @param  array<string, mixed>  $payload
     */
    private function upsertSalemLookupTable(array $payload): void
    {
        if (! Schema::hasTable(self::SALEM_PRODUCT_TABLE)) {
            return;
        }

        $columns = Schema::getColumnListing(self::SALEM_PRODUCT_TABLE);
        $data = array_intersect_key($payload, array_flip($columns));

        if (! isset($data['id'])) {
            return;
        }

        DB::table(self::SALEM_PRODUCT_TABLE)->updateOrInsert(
            ['id' => $data['id']],
            $data,
        );
    }
}
