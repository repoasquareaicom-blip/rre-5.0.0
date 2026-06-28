<?php

namespace App\Support;

use Carbon\Carbon;
use Illuminate\Database\Eloquent\Model;

/**
 * Branch DB datetimes are naive IST wall-clock values — return as stored, no timezone shift.
 */
class BranchDateTime
{
    public const TZ = 'Asia/Kolkata';

    /** Current IST wall-clock time (matches MySQL NOW() on an IST server). */
    public static function now(): Carbon
    {
        return Carbon::now(self::TZ);
    }

    public static function today(): string
    {
        return self::now()->toDateString();
    }

    /** Current wall-clock as `Y-m-d H:i:s` for DB columns (no UTC conversion). */
    public static function nowString(): string
    {
        return self::now()->format('Y-m-d H:i:s');
    }

    public static function toDisplay(mixed $value): ?string
    {
        if ($value === null || $value === '') {
            return null;
        }

        if ($value instanceof Model) {
            return null;
        }

        $raw = $value instanceof \DateTimeInterface
            ? $value->format('Y-m-d H:i:s')
            : trim((string) $value);

        if ($raw === '') {
            return null;
        }

        if (preg_match('/^(\d{4}-\d{2}-\d{2})[ T](\d{2}:\d{2}:\d{2})/', $raw, $matches)) {
            return "{$matches[1]} {$matches[2]}";
        }

        if (preg_match('/^(\d{4}-\d{2}-\d{2})[ T](\d{2}:\d{2})(?:\D|$)/', $raw, $matches)) {
            return "{$matches[1]} {$matches[2]}:00";
        }

        if (preg_match('/^\d{4}-\d{2}-\d{2}$/', $raw)) {
            return "{$raw} 00:00:00";
        }

        return $raw;
    }

    public static function fromModel(Model $model, string $column): ?string
    {
        $raw = $model->getRawOriginal($column);

        return self::toDisplay($raw);
    }

    public static function toDateString(mixed $value): ?string
    {
        $display = self::toDisplay($value);

        return $display ? substr($display, 0, 10) : null;
    }

    public static function dateFromModel(Model $model, string $column): ?string
    {
        $display = self::fromModel($model, $column);

        return $display ? substr($display, 0, 10) : null;
    }
}
