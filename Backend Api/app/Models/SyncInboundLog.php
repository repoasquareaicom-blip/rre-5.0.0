<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

class SyncInboundLog extends Model
{
    public $timestamps = false;

    protected $fillable = [
        'branch_id',
        'branch_code',
        'http_method',
        'path',
        'endpoint',
        'http_status',
        'status',
        'records_received',
        'records_upserted',
        'lines_upserted',
        'error_message',
        'request_meta',
        'response_meta',
        'duration_ms',
        'ip_address',
        'started_at',
        'completed_at',
    ];

    protected function casts(): array
    {
        return [
            'request_meta' => 'array',
            'response_meta' => 'array',
        ];
    }

    public function branch(): BelongsTo
    {
        return $this->belongsTo(Branch::class);
    }
}
