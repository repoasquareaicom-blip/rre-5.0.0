<?php

namespace App\Models;

use App\Models\Concerns\UpsertsWithoutId;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

class QuotationDetails extends Model
{
    use UpsertsWithoutId;

    protected $table = 'QuotationDetails';

    public $timestamps = false;

    public $incrementing = false;

    protected $fillable = [
        'branch_id',
        'Quotationid',
        'Productid',
        'Rate',
        'Quantity',
        'Amount',
        'sino',
        'Status',
        'pqty',
        'Productserialno',
    ];

    protected function casts(): array
    {
        return [
            'branch_id' => 'integer',
            'sino' => 'integer',
        ];
    }

    public function branch(): BelongsTo
    {
        return $this->belongsTo(Branch::class);
    }

    public function header(): BelongsTo
    {
        return $this->belongsTo(QuotationHeader::class, 'Quotationid', 'Quotationid')
            ->where('QuotationHeader.branch_id', $this->branch_id);
    }

    public function getAmountDecimalAttribute(): float
    {
        return (float) preg_replace('/[^0-9.\-]/', '', (string) ($this->Amount ?? '0'));
    }
}
