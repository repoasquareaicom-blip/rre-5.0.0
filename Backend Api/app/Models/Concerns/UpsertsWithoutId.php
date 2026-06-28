<?php

namespace App\Models\Concerns;

trait UpsertsWithoutId
{
    /**
     * Insert or update by unique key (e.g. branch_id + Quotationid).
     *
     * @param  array<string, mixed>  $unique
     * @param  array<string, mixed>  $attributes
     * @return array{model: static, created: bool}
     */
    public static function upsertRow(array $unique, array $attributes): array
    {
        $values = array_merge($unique, $attributes);
        $created = ! static::query()->where($unique)->exists();

        static::query()->updateOrInsert($unique, $values);

        /** @var static $model */
        $model = static::query()->where($unique)->firstOrFail();

        return [
            'model' => $model,
            'created' => $created,
        ];
    }
}
