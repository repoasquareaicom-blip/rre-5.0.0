<?php

namespace App\Console\Commands;

use App\Models\QuotationEstimation;
use App\Models\QuotationHeader;
use App\Models\Receipt;
use App\Models\Sales;
use App\Models\SalesPipes;
use App\Models\SalesTraders;
use App\Models\Settlement;
use App\Models\Voucher;
use App\Services\ActivityFeedLogger;
use App\Support\BranchDateTime;
use Illuminate\Console\Command;
use Illuminate\Support\Facades\Schema;

class BackfillActivityFeedCommand extends Command
{
    protected $signature = 'rr:backfill-feed';

    protected $description = 'Create activity feed entries from existing synced records';

    public function handle(ActivityFeedLogger $logger): int
    {
        $stamped = QuotationHeader::query()
            ->whereNull('syncon')
            ->update(['syncon' => BranchDateTime::now()]);

        if ($stamped > 0) {
            $this->info("Set syncon on {$stamped} quotation headers (system time).");
        }

        $map = [
            'quotations' => [QuotationHeader::class, 'sino'],
            'estimations' => [QuotationEstimation::class, 'SlNo'],
            'sales' => [Sales::class, 'slno'],
            'sales-pipes' => [SalesPipes::class, 'slno'],
            'sales-traders' => [SalesTraders::class, 'slno'],
            'vouchers' => [Voucher::class, 'id'],
            'receipts' => [Receipt::class, 'id'],
            'settlements' => [Settlement::class, 'id'],
        ];

        $count = 0;

        foreach ($map as $type => [$modelClass, $orderColumn]) {
            $table = (new $modelClass)->getTable();
            if (! Schema::hasTable($table)) {
                continue;
            }

            $modelClass::query()
                ->with('branch')
                ->orderBy('branch_id')
                ->orderBy($orderColumn)
                ->chunk(200, function ($rows) use ($logger, $type, &$count) {
                    foreach ($rows as $model) {
                        if (! $model->branch) {
                            continue;
                        }
                        $logger->log($model->branch, $type, $model, true);
                        $count++;
                    }
                });
        }

        $this->info("Backfilled {$count} feed events.");

        return self::SUCCESS;
    }
}
