<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class ProductMasterSyncChange extends Model
{
    protected $fillable = [
        'source_branch_id',
        'product_id',
        'item_code',
        'item_name',
        'display_name',
        'sales_price',
        'payload',
    ];

    protected function casts(): array
    {
        return [
            'payload' => 'array',
            'sales_price' => 'decimal:2',
        ];
    }
}
