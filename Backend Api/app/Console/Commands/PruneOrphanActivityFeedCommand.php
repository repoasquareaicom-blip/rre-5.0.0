<?php

namespace App\Console\Commands;

use App\Models\ActivityEvent;
use App\Services\MirrorActivityScope;
use Illuminate\Console\Command;

class PruneOrphanActivityFeedCommand extends Command
{
    protected $signature = 'rr:prune-orphan-feed {--dry-run : Show count only, do not delete}';

    protected $description = 'Remove activity feed rows with no matching mirror header (QuotationHeader, QuotationEstimation, Sales, etc.)';

    public function handle(MirrorActivityScope $scope): int
    {
        $mirrorTypes = ['quotation', 'estimation', 'sale', 'sales-pipe', 'sales-trader'];

        $withMirror = ActivityEvent::query()
            ->whereIn('entity_type', $mirrorTypes);

        $scope->applyExistsOnMirror($withMirror);

        $toDelete = ActivityEvent::query()
            ->whereIn('entity_type', $mirrorTypes)
            ->whereNotIn('id', (clone $withMirror)->select('id'));

        $count = (clone $toDelete)->count();

        if ($count === 0) {
            $this->info('No orphan feed events found.');

            return self::SUCCESS;
        }

        if ($this->option('dry-run')) {
            $this->warn("Would delete {$count} orphan feed event(s). Run without --dry-run to apply.");

            return self::SUCCESS;
        }

        $deleted = $toDelete->delete();
        $this->info("Deleted {$deleted} orphan feed event(s).");

        return self::SUCCESS;
    }
}
