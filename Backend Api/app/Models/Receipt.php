<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

class Receipt extends Model
{
    protected $fillable = [
        'branch_id',
        'external_id',
        'receipt_type',
        'document_date',
        'party_name',
        'status',
        'amount',
        'source_updated_at',
        'meta',
    ];

    protected function casts(): array
    {
        return [
            'document_date' => 'date',
            'amount' => 'decimal:2',
            'source_updated_at' => 'datetime',
            'meta' => 'array',
        ];
    }

    public function branch(): BelongsTo
    {
        return $this->belongsTo(Branch::class);
    }
}
