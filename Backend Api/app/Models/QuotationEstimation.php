<?php

namespace App\Models;

use App\Models\Concerns\BranchHeaderTable;
use App\Models\Concerns\UpsertsWithoutId;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

class QuotationEstimation extends Model
{
    use BranchHeaderTable, UpsertsWithoutId;

    protected $table = 'QuotationEstimation';

    protected $fillable = [
        'branch_id',
        'Estimationid',
        'Quotationid',
        'Customerid',
        'date',
        'Referenceid',
        'Assist',
        'Status',
        'customername',
        'City',
        'Total',
        'LessAmount',
        'GrandTotal',
        'Remarks',
        'Updatedon',
        'Updatedby',
        'isdelete',
        'Paymentmode',
        'Paid',
        'Balance',
        'iscreditapproved',
        'IsBilled',
        'ischecked',
        'checkedby',
        'IsReturnApproved',
        'IsReturnChecked',
        'SlNo',
        'returnvalue',
        'Iscancel',
        'others',
        'ismove',
        'ispack',
        'AssistName',
        'syncon',
    ];

    protected function casts(): array
    {
        return [
            'branch_id' => 'integer',
            'SlNo' => 'integer',
            'IsBilled' => 'boolean',
            'ischecked' => 'integer',
            'returnvalue' => 'integer',
            'Iscancel' => 'integer',
            'others' => 'decimal:3',
            'ismove' => 'integer',
            'ispack' => 'integer',
        ];
    }

    public function branch(): BelongsTo
    {
        return $this->belongsTo(Branch::class);
    }
}
