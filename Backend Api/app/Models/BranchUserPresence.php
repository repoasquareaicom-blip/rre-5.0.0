<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

class BranchUserPresence extends Model
{
    protected $table = 'branch_user_presence';

    protected $fillable = [
        'branch_id',
        'external_user_id',
        'user_name',
        'machine_name',
        'is_logged_in',
        'last_seen_at',
    ];

    protected function casts(): array
    {
        return [
            'is_logged_in' => 'boolean',
            'last_seen_at' => 'datetime',
        ];
    }

    public function branch(): BelongsTo
    {
        return $this->belongsTo(Branch::class);
    }
}
