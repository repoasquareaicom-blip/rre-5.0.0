<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

class SyncCursor extends Model
{
    protected $fillable = [
        'branch_id',
        'entity_type',
        'last_sino',
        'last_synced_at',
        'last_external_id',
    ];

    protected $attributes = [
        'last_sino' => 0,
    ];

    protected function casts(): array
    {
        return [
            'last_sino' => 'integer',
            'last_synced_at' => 'datetime',
        ];
    }

    public function branch(): BelongsTo
    {
        return $this->belongsTo(Branch::class);
    }
}
