<?php

namespace App\Models;

use App\Models\Concerns\BranchHeaderTable;
use App\Models\Concerns\UpsertsWithoutId;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

class Sales extends Model
{
    use BranchHeaderTable, UpsertsWithoutId;

    protected $table = 'Sales';

    protected $fillable = [
        'branch_id',
        'Salesid',
        'Customerid',
        'customername',
        'City',
        'Referenceid',
        'Assist',
        'Paymentmode',
        'Updatedon',
        'Updatedby',
        'TotalAmount',
        'LessAmount',
        'GrandTotal',
        'Address1',
        'Address2',
        'Tin',
        'Mobile',
        'slno',
        'others',
        'EnteredOn',
        'State',
        'GstText',
        'syncon',
    ];

    protected function casts(): array
    {
        return [
            'branch_id' => 'integer',
            'slno' => 'integer',
            'others' => 'decimal:2',
        ];
    }

    public function branch(): BelongsTo
    {
        return $this->belongsTo(Branch::class);
    }
}
