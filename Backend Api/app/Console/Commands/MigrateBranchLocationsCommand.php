<?php

namespace App\Console\Commands;

use App\Models\ActivityEvent;
use App\Models\Branch;
use App\Models\Estimation;
use App\Models\QuotationHeader;
use App\Models\Receipt;
use App\Models\Sale;
use App\Models\Settlement;
use App\Models\SyncLog;
use App\Models\Voucher;
use Illuminate\Console\Command;
class MigrateBranchLocationsCommand extends Command
{
    protected $signature = 'rr:migrate-branch-locations {--dry-run : Show counts only}';

    protected $description = 'Move legacy branch data (RR-ELECTRICAL, RR-PIPES) to Salem / Namakkal and seed Kolathur demo sync';

    /** @var array<string, string> */
    private array $codeMap = [
        'RR-ELECTRICAL' => 'RR-SALEM',
        'RR-PIPES' => 'RR-NAMAKKAL',
    ];

    public function handle(): int
    {
        $dryRun = (bool) $this->option('dry-run');

        foreach ($this->codeMap as $fromCode => $toCode) {
            $from = Branch::where('code', $fromCode)->first();
            $to = Branch::where('code', $toCode)->first();

            if (! $from || ! $to) {
                $this->warn("Skip {$fromCode} → {$toCode} (branch missing)");

                continue;
            }

            if ($from->id === $to->id) {
                continue;
            }

            $tables = [
                ActivityEvent::class,
                QuotationHeader::class,
                Estimation::class,
                Sale::class,
                Voucher::class,
                Receipt::class,
                Settlement::class,
                SyncLog::class,
            ];

            foreach ($tables as $model) {
                $count = $model::where('branch_id', $from->id)->count();
                if ($count === 0) {
                    continue;
                }

                $this->line("  {$fromCode} → {$toCode}: {$count} ".class_basename($model));

                if (! $dryRun) {
                    $model::where('branch_id', $from->id)->update(['branch_id' => $to->id]);
                }
            }
        }

        if (! $dryRun) {
            $this->seedSyncHeartbeats();
            $this->call('rr:ensure-branches');
        }

        $this->info($dryRun ? 'Dry run complete.' : 'Branch location migration complete.');

        return self::SUCCESS;
    }

    private function seedSyncHeartbeats(): void
    {
        foreach (Branch::query()->where('is_active', true)->get() as $branch) {
            $hasRecent = SyncLog::query()
                ->where('branch_id', $branch->id)
                ->where('status', 'success')
                ->where('completed_at', '>=', now()->subMinutes(20))
                ->exists();

            if ($hasRecent) {
                continue;
            }

            foreach (['quotations', 'estimations', 'sales'] as $entity) {
                SyncLog::create([
                    'branch_id' => $branch->id,
                    'entity_type' => $entity,
                    'records_received' => 0,
                    'records_upserted' => 0,
                    'status' => 'success',
                    'started_at' => now()->subMinutes(5),
                    'completed_at' => now()->subMinutes(4),
                ]);
            }
        }
    }
}
