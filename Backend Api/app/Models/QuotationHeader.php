<?php

namespace App\Models;

use App\Models\Concerns\UpsertsWithoutId;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;
use Illuminate\Database\Eloquent\Relations\HasMany;

class QuotationHeader extends Model
{
    use UpsertsWithoutId;

    protected $table = 'QuotationHeader';

    public $timestamps = false;

    public $incrementing = false;

    /** Composite primary key — no `id` column. */
    public function getKeyName(): string
    {
        return 'Quotationid';
    }

    protected $fillable = [
        'branch_id',
        'Quotationid',
        'Customerid',
        'date',
        'Referenceid',
        'Assist',
        'Status',
        'Updatedon',
        'Updatedby',
        'isdelete',
        'customename',
        'City',
        'Isestimate',
        'sino',
        'Iscancel',
        'Ispdi',
        'Pdiby',
        'pdion',
        'AssistName',
        'syncon',
    ];

    protected $appends = [
        'row_id',
        'external_id',
        'customer_name',
        'document_date',
        'status',
        'amount',
        'line_count',
        'total_amount',
    ];

    protected function casts(): array
    {
        return [
            'branch_id' => 'integer',
            'sino' => 'integer',
            'Iscancel' => 'integer',
            'Ispdi' => 'integer',
            'Pdiby' => 'integer',
        ];
    }

    public function branch(): BelongsTo
    {
        return $this->belongsTo(Branch::class);
    }

    public function details(): HasMany
    {
        return $this->hasMany(QuotationDetails::class, 'Quotationid', 'Quotationid')
            ->where('QuotationDetails.branch_id', $this->branch_id);
    }

    /** Row key for APIs / UI (uses branch header sino). Not named `id` — avoids Eloquent save conflicts. */
    public function getRowIdAttribute(): int
    {
        return (int) $this->sino;
    }

    /** Monitor / feed compatibility */
    public function getExternalIdAttribute(): string
    {
        return (string) $this->Quotationid;
    }

    public function getCustomerNameAttribute(): ?string
    {
        return $this->customename ?? $this->attributes['customemame'] ?? null;
    }

    public function getDocumentDateAttribute()
    {
        $value = $this->date;

        return $value instanceof \DateTimeInterface ? $value->format('Y-m-d') : $value;
    }

    public function getStatusAttribute(): ?string
    {
        return $this->attributes['Status'] ?? null;
    }

    public function getSourceUpdatedAtAttribute()
    {
        return $this->Updatedon;
    }

    public function getAmountAttribute(): float
    {
        if (array_key_exists('total_amount', $this->attributes)) {
            return (float) $this->attributes['total_amount'];
        }

        return (float) $this->details()->get()->sum(fn (QuotationDetails $line) => $line->amount_decimal);
    }

    public function getLineCountAttribute(): int
    {
        return (int) ($this->attributes['line_count'] ?? $this->details()->count());
    }

    public function getTotalAmountAttribute(): float
    {
        return (float) ($this->attributes['total_amount'] ?? $this->amount);
    }
}
