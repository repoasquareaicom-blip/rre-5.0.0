<?php

namespace App\Services;

use App\Models\QuotationEstimation;
use App\Models\QuotationHeader;
use App\Models\Sales;
use App\Models\SalesPipes;
use App\Models\SalesTraders;
use Illuminate\Database\Eloquent\Builder;
use Illuminate\Database\Query\Builder as QueryBuilder;

/**
 * Restrict activity feed / monitor UI to rows that still exist in MSSQL mirror header tables.
 */
class MirrorActivityScope
{
    /**
     * @param  Builder<\App\Models\ActivityEvent>  $query
     */
    public function applyExistsOnMirror(Builder $query): void
    {
        $query->where(function (Builder $outer) {
            $outer
                ->where(function (Builder $q) {
                    $q->where('entity_type', 'quotation')
                        ->whereExists($this->headerExistsSubquery(
                            QuotationHeader::query(),
                            'Quotationid'
                        ));
                })
                ->orWhere(function (Builder $q) {
                    $q->where('entity_type', 'estimation')
                        ->whereExists($this->headerExistsSubquery(
                            QuotationEstimation::query(),
                            'Estimationid'
                        ));
                })
                ->orWhere(function (Builder $q) {
                    $q->where('entity_type', 'sale')
                        ->whereExists($this->headerExistsSubquery(Sales::query(), 'Salesid'));
                })
                ->orWhere(function (Builder $q) {
                    $q->where('entity_type', 'sales-pipe')
                        ->whereExists($this->headerExistsSubquery(SalesPipes::query(), 'Salesid'));
                })
                ->orWhere(function (Builder $q) {
                    $q->where('entity_type', 'sales-trader')
                        ->whereExists($this->headerExistsSubquery(SalesTraders::query(), 'Salesid'));
                })
                ->orWhereNotIn('entity_type', [
                    'quotation',
                    'estimation',
                    'sale',
                    'sales-pipe',
                    'sales-trader',
                ]);
        });
    }

    /**
     * @param  Builder<\Illuminate\Database\Eloquent\Model>  $headerQuery
     */
    private function headerExistsSubquery(Builder $headerQuery, string $idColumn): \Closure
    {
        $table = $headerQuery->getModel()->getTable();

        return function (QueryBuilder $sub) use ($table, $idColumn) {
            $sub->selectRaw('1')
                ->from($table)
                ->whereColumn("{$table}.branch_id", 'activity_events.branch_id')
                ->whereColumn("{$table}.{$idColumn}", 'activity_events.external_id');
        };
    }
}
