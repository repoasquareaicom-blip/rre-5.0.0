<?php

namespace App\Console\Commands;

use App\Models\Branch;
use App\Models\SalesDetails;
use App\Models\SalesPipesDetails;
use App\Models\SalesTradersDetails;
use App\Services\ProductMasterLookupService;
use Illuminate\Console\Command;

class BackfillSalesProductNamesCommand extends Command
{
    protected $signature = 'rr:backfill-sales-product-names';

    protected $description = 'Fill product_name on sales line tables from ProductMaster lookups';

    public function handle(ProductMasterLookupService $lookup): int
    {
        $branches = Branch::query()->get(['id', 'code']);
        $updated = 0;

        foreach ([SalesDetails::class, SalesPipesDetails::class, SalesTradersDetails::class] as $modelClass) {
            /** @var class-string<\Illuminate\Database\Eloquent\Model> $modelClass */
            $table = (new $modelClass)->getTable();

            foreach ($branches as $branch) {
                $lines = $modelClass::query()
                    ->where('branch_id', $branch->id)
                    ->whereNotNull('Productid')
                    ->where(function ($q) {
                        $q->whereNull('product_name')->orWhere('product_name', '');
                    })
                    ->get(['branch_id', 'Salesid', 'Productid', 'product_name']);

                if ($lines->isEmpty()) {
                    continue;
                }

                $names = $lookup->namesForBranch(
                    (string) $branch->code,
                    $lines->pluck('Productid')->map(fn ($id) => (string) $id)->all(),
                );

                foreach ($lines as $line) {
                    $productId = (string) $line->Productid;
                    $name = $names[$productId] ?? null;
                    if ($name === null) {
                        continue;
                    }

                    $modelClass::query()
                        ->where('branch_id', $line->branch_id)
                        ->where('Salesid', $line->Salesid)
                        ->where('Productid', $line->Productid)
                        ->update(['product_name' => $name]);

                    $updated++;
                }
            }

            $this->line("Checked {$table}");
        }

        $this->info("Updated {$updated} sales line(s) with product names.");

        if ($updated === 0) {
            $this->comment('If names are still missing, refresh ProductMaster_* tables from the branch DB, then run this again.');
        }

        return self::SUCCESS;
    }
}
