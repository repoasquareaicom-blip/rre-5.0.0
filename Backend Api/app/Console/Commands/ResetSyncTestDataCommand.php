<?php

namespace App\Console\Commands;

use App\Models\ActivityEvent;
use App\Models\BranchUserPresence;
use App\Models\Estimation;
use App\Models\MonitorApiToken;
use App\Models\QuotationDetails;
use App\Models\QuotationEstimation;
use App\Models\QuotationEstimationDetails;
use App\Models\QuotationHeader;
use App\Models\Receipt;
use App\Models\Sale;
use App\Models\Sales;
use App\Models\SalesDetails;
use App\Models\SalesPipes;
use App\Models\SalesPipesDetails;
use App\Models\SalesTraders;
use App\Models\SalesTradersDetails;
use App\Models\Settlement;
use App\Models\SyncCursor;
use App\Models\SyncInboundLog;
use App\Models\SyncLog;
use App\Models\Voucher;
use Illuminate\Console\Command;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;

class ResetSyncTestDataCommand extends Command
{
    protected $signature = 'rr:reset-test-data
                            {--force : Skip confirmation}
                            {--keep-logs : Keep sync_inbound_logs and sync_logs}';

    protected $description = 'Clear all synced business data (mirror tables, feed, presence, logs). Keeps branches and users.';

    public function handle(): int
    {
        if (! $this->option('force') && ! $this->confirm(
            'Delete all synced data (mirror tables, feed, presence, vouchers, settlements, logs)? Branches and users are kept.'
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

        if (! $this->option('keep-logs')) {
            if (Schema::hasTable('sync_inbound_logs')) {
                $counts['sync_inbound_logs'] = SyncInboundLog::query()->delete();
            }
            if (Schema::hasTable('sync_logs')) {
                $counts['sync_logs'] = SyncLog::query()->delete();
            }
        }

        if (Schema::hasTable('sync_cursors')) {
            $counts['sync_cursors'] = SyncCursor::query()->delete();
        }

        foreach ($this->legacySummaryTables() as $label => $model) {
            if (! Schema::hasTable((new $model)->getTable())) {
                continue;
            }
            $counts[$label] = $model::query()->delete();
        }

        if (Schema::hasTable('quotations')) {
            $counts['quotations'] = DB::table('quotations')->delete();
        }

        if (Schema::hasTable('quotation_lines')) {
            $counts['quotation_lines'] = DB::table('quotation_lines')->delete();
        }

        if (Schema::hasTable('branch_user_presence')) {
            $counts['branch_user_presence'] = BranchUserPresence::query()->delete();
        }

        if (Schema::hasTable('monitor_api_tokens')) {
            $counts['monitor_api_tokens'] = MonitorApiToken::query()->delete();
        }

        Schema::enableForeignKeyConstraints();

        $this->info('All synced data cleared (branches and users kept).');
        foreach ($counts as $table => $n) {
            $this->line("  {$table}: {$n} row(s)");
        }

        $this->newLine();
        $this->comment('Next: run branch sync, then check GET /api/sync/last-sync-on and the monitor UI.');

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

    /**
     * @return array<string, class-string>
     */
    private function legacySummaryTables(): array
    {
        return [
            'estimations' => Estimation::class,
            'sales' => Sale::class,
            'vouchers' => Voucher::class,
            'settlements' => Settlement::class,
            'receipts' => Receipt::class,
        ];
    }
}
