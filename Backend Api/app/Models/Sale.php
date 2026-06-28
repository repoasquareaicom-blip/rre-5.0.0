<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

class Sale extends Model
{
    protected $fillable = [
        'branch_id',
        'external_id',
        'estimation_external_id',
        'quotation_external_id',
        'customer_name',
        'document_date',
        'status',
        'payment_mode',
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
