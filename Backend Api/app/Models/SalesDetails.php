<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

class SalesDetails extends Model
{
    protected $table = 'SalesDetails';

    public $timestamps = false;

    protected $fillable = [
        'branch_id',
        'Salesid',
        'Productid',
        'product_name',
        'Rate',
        'Quantity',
        'Amount',
        'gst',
    ];

    protected function casts(): array
    {
        return [
            'branch_id' => 'integer',
            'gst' => 'decimal:2',
        ];
    }

    public function branch(): BelongsTo
    {
        return $this->belongsTo(Branch::class);
    }
}
