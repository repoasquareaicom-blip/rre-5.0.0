<?php

namespace App\Models;

use App\Models\Concerns\UpsertsWithoutId;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

class QuotationEstimationDetails extends Model
{
    use UpsertsWithoutId;

    protected $table = 'QuotationEstimationDetails';

    public $timestamps = false;

    public $incrementing = false;

    protected $fillable = [
        'branch_id',
        'Estimationid',
        'Productid',
        'Rate',
        'Quantity',
        'Amount',
        'isdelevered',
        'DeliveredQty',
        'Isreturncheck',
        'sino',
        'Location',
        'Productserialno',
    ];

    protected function casts(): array
    {
        return [
            'branch_id' => 'integer',
            'sino' => 'integer',
            'isdelevered' => 'integer',
            'Isreturncheck' => 'integer',
        ];
    }

    public function branch(): BelongsTo
    {
        return $this->belongsTo(Branch::class);
    }
}
