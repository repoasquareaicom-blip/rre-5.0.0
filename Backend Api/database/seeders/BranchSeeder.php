<?php

namespace Database\Seeders;

use App\Models\Branch;
use App\Models\Receipt;
use App\Models\Settlement;
use App\Models\SyncLog;
use App\Models\Voucher;
use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\Artisan;
class BranchSeeder extends Seeder
{
    public function run(): void
    {
        $branches = [
            [
                'code' => 'RR-SALEM',
                'short_code' => 'SLM',
                'name' => 'Salem',
                'api_key' => 'rr-salem-sync-dev-key',
            ],
            [
                'code' => 'RR-NAMAKKAL',
                'short_code' => 'NAM',
                'name' => 'Namakkal',
                'api_key' => 'rr-namakkal-sync-dev-key',
            ],
            [
                'code' => 'RR-KOLATHUR',
                'short_code' => 'KOL',
                'name' => 'Kolathur',
                'api_key' => 'rr-kolathur-sync-dev-key',
            ],
        ];

        $codes = array_column($branches, 'code');

        $this->renameLegacyNamakkalBranch();

        Branch::query()
            ->whereNotIn('code', $codes)
            ->update(['is_active' => false]);

        foreach ($branches as $data) {
            Branch::updateOrCreate(
                ['code' => $data['code']],
                [
                    'name' => $data['name'],
                    'short_code' => $data['short_code'],
                    'api_key' => $data['api_key'],
                    'is_active' => true,
                ]
            );
        }

        if (! app()->environment('local')) {
            return;
        }

        $this->seedDemoData();
        $this->seedBranchSyncHeartbeats();
        Artisan::call('rr:backfill-feed');
    }

    private function seedBranchSyncHeartbeats(): void
    {
        foreach (Branch::all() as $branch) {
            foreach (['quotations', 'estimations', 'sales'] as $entity) {
                SyncLog::create([
                    'branch_id' => $branch->id,
                    'entity_type' => $entity,
                    'records_received' => 1,
                    'records_upserted' => 1,
                    'status' => 'success',
                    'started_at' => now()->subMinutes(5),
                    'completed_at' => now()->subMinutes(4),
                ]);
            }
        }
    }

    private function renameLegacyNamakkalBranch(): void
    {
        $legacy = Branch::query()->where('code', 'RR-MAMAKKA')->first();
        if (! $legacy) {
            return;
        }

        if (Branch::query()->where('code', 'RR-NAMAKKAL')->where('id', '!=', $legacy->id)->exists()) {
            $legacy->update(['is_active' => false]);

            return;
        }

        $legacy->update([
            'code' => 'RR-NAMAKKAL',
            'short_code' => 'NAM',
            'name' => 'Namakkal',
            'api_key' => 'rr-namakkal-sync-dev-key',
            'is_active' => true,
        ]);
    }

    private function seedDemoData(): void
    {
        $activeBranches = Branch::query()
            ->where('is_active', true)
            ->whereIn('code', ['RR-SALEM', 'RR-NAMAKKAL', 'RR-KOLATHUR'])
            ->orderBy('code')
            ->get();

        if ($activeBranches->isEmpty()) {
            return;
        }

        foreach ($activeBranches as $index => $branch) {
            for ($i = 1; $i <= 8; $i++) {
                $day = now()->subDays($i)->toDateString();
                $qid = $branch->code.'-Q-'.str_pad((string) $i, 4, '0', STR_PAD_LEFT);
                $eid = $branch->code.'-E-'.str_pad((string) $i, 4, '0', STR_PAD_LEFT);

                // Header sync tables (QuotationHeader, QuotationEstimation, Sales, …) are filled
                // only by branch sync POST — not seeded, so last-sync-on stays null until real data.

                Voucher::updateOrCreate(
                    ['branch_id' => $branch->id, 'external_id' => $branch->code.'-V-'.str_pad((string) $i, 4, '0', STR_PAD_LEFT)],
                    [
                        'voucher_type' => 'Payment',
                        'document_date' => $day,
                        'party_name' => 'Supplier '.$i,
                        'status' => 'Approved',
                        'amount' => 1200 + ($i * 100),
                        'source_updated_at' => now()->subDays($i),
                    ]
                );

                Receipt::updateOrCreate(
                    ['branch_id' => $branch->id, 'external_id' => $branch->code.'-R-'.str_pad((string) $i, 4, '0', STR_PAD_LEFT)],
                    [
                        'receipt_type' => 'Cash Receipt',
                        'document_date' => $day,
                        'party_name' => 'Customer '.$i,
                        'status' => 'Received',
                        'amount' => 3000 + ($i * 150),
                        'source_updated_at' => now()->subDays($i),
                    ]
                );

                Settlement::updateOrCreate(
                    ['branch_id' => $branch->id, 'external_id' => $branch->code.'-SET-'.$day],
                    [
                        'settlement_date' => $day,
                        'beginning_cash' => 10000,
                        'cash_in' => 25000 + ($i * 500),
                        'cash_out' => 8000 + ($i * 200),
                        'net_cash' => 27000 + ($i * 300),
                        'actual_cash' => 26950 + ($i * 300),
                        'over_short' => -50,
                        'status' => 'Closed',
                        'source_updated_at' => now()->subDays($i),
                    ]
                );
            }
        }
    }
}
