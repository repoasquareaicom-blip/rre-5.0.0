<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

class Settlement extends Model
{
    protected $fillable = [
        'branch_id',
        'external_id',
        'settlement_date',
        'beginning_cash',
        'cash_in',
        'cash_out',
        'net_cash',
        'actual_cash',
        'over_short',
        'status',
        'source_updated_at',
        'meta',
    ];

    protected function casts(): array
    {
        return [
            'settlement_date' => 'date',
            'beginning_cash' => 'decimal:2',
            'cash_in' => 'decimal:2',
            'cash_out' => 'decimal:2',
            'net_cash' => 'decimal:2',
            'actual_cash' => 'decimal:2',
            'over_short' => 'decimal:2',
            'source_updated_at' => 'datetime',
            'meta' => 'array',
        ];
    }

    public function branch(): BelongsTo
    {
        return $this->belongsTo(Branch::class);
    }
}
