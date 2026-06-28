<?php

namespace App\Console\Commands;

use App\Models\QuotationDetails;
use App\Models\QuotationEstimation;
use App\Models\QuotationHeader;
use App\Models\Sales;
use App\Models\SalesPipes;
use App\Models\SalesTraders;
use Illuminate\Console\Command;
use Illuminate\Support\Facades\Schema;

class ClearHeaderSyncTablesCommand extends Command
{
    protected $signature = 'rr:clear-header-sync-tables {--force : Skip confirmation}';

    protected $description = 'Alias for rr:reset-feed (clears live feed mirror tables only)';

    public function handle(): int
    {
        return $this->call('rr:reset-feed', [
            '--force' => $this->option('force'),
        ]);
    }
}
