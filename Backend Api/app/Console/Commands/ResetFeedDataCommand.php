<?php

namespace App\Console\Commands;

use App\Models\ActivityEvent;
use App\Models\QuotationDetails;
use App\Models\QuotationEstimation;
use App\Models\QuotationEstimationDetails;
use App\Models\QuotationHeader;
use App\Models\Sales;
use App\Models\SalesDetails;
use App\Models\SalesPipes;
use App\Models\SalesPipesDetails;
use App\Models\SalesTraders;
use App\Models\SalesTradersDetails;
use App\Models\SyncCursor;
use Illuminate\Console\Command;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;

class ResetFeedDataCommand extends Command
{
    protected $signature = 'rr:reset-feed
                            {--force : Skip confirmation}';

    protected $description = 'Clear live feed mirror data (quotations, estimations, sales) and sync cursors. Keeps branches, users, presence, and logs.';

    public function handle(): int
    {
        if (! $this->option('force') && ! $this->confirm(
            'Clear all feed mirror tables and sync cursors? Branches and users are kept. Branch sync will repopulate from the next push.'
        )) {
            return self::SUCCESS;
        }

        Schema::disableForeignKeyConstraints();

        $counts = [];

        foreach ($this->detailTables() as $label => $model) {
            if (! Schema::hasTable((new $model)->getTable())) {
                continue;
            }
            $counts[$label] = $model::query()->delete();
        }

        foreach ($this->headerTables() as $label => $model) {
            if (! Schema::hasTable((new $model)->getTable())) {
                continue;
            }
            $counts[$label] = $model::query()->delete();
        }

        if (Schema::hasTable('activity_events')) {
            $counts['activity_events'] = ActivityEvent::query()->delete();
        }

        if (Schema::hasTable('quotation_lines')) {
            $counts['quotation_lines'] = DB::table('quotation_lines')->delete();
        }

        if (Schema::hasTable('quotations')) {
            $counts['quotations'] = DB::table('quotations')->delete();
        }

        if (Schema::hasTable('sync_cursors')) {
            $counts['sync_cursors'] = SyncCursor::query()->delete();
        }

        Schema::enableForeignKeyConstraints();

        $this->info('Feed mirror data cleared. Branches, users, presence, and logs were kept.');
        foreach ($counts as $table => $n) {
            $this->line("  {$table}: {$n} row(s)");
        }

        $this->newLine();
        $this->comment('Feed is empty until branches sync again. New activity will appear from the next branch push.');

        return self::SUCCESS;
    }

    /**
     * @return array<string, class-string>
     */
    private function detailTables(): array
    {
        return [
            'QuotationDetails' => QuotationDetails::class,
            'QuotationEstimationDetails' => QuotationEstimationDetails::class,
            'SalesDetails' => SalesDetails::class,
            'SalesPipesDetails' => SalesPipesDetails::class,
            'SalesTradersDetails' => SalesTradersDetails::class,
        ];
    }

    /**
     * @return array<string, class-string>
     */
    private function headerTables(): array
    {
        return [
            'QuotationHeader' => QuotationHeader::class,
            'QuotationEstimation' => QuotationEstimation::class,
            'Sales' => Sales::class,
            'SalesPipes' => SalesPipes::class,
            'SalesTraders' => SalesTraders::class,
        ];
    }
}
