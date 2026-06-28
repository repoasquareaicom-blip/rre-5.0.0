<?php

namespace App\Console\Commands;

use App\Models\Branch;
use App\Services\BranchPresenceService;
use Illuminate\Console\Command;

class EnsureBranchesCommand extends Command
{
    protected $signature = 'rr:ensure-branches';

    protected $description = 'Create or update RR branch locations (Salem, Namakkal, Kolathur) with short codes';

    public function handle(BranchPresenceService $presence): int
    {
        $branches = [
            ['code' => 'RR-SALEM', 'short_code' => 'SLM', 'name' => 'Salem', 'api_key' => 'rr-salem-sync-dev-key'],
            ['code' => 'RR-NAMAKKAL', 'short_code' => 'NAM', 'name' => 'Namakkal', 'api_key' => 'rr-namakkal-sync-dev-key'],
            ['code' => 'RR-KOLATHUR', 'short_code' => 'KOL', 'name' => 'Kolathur', 'api_key' => 'rr-kolathur-sync-dev-key'],
        ];

        $codes = array_column($branches, 'code');

        $legacy = Branch::query()->where('code', 'RR-MAMAKKA')->first();
        if ($legacy && ! Branch::query()->where('code', 'RR-NAMAKKAL')->where('id', '!=', $legacy->id)->exists()) {
            $legacy->update([
                'code' => 'RR-NAMAKKAL',
                'short_code' => 'NAM',
                'name' => 'Namakkal',
                'api_key' => 'rr-namakkal-sync-dev-key',
            ]);
        }

        Branch::query()->whereNotIn('code', $codes)->update(['is_active' => false]);

        foreach ($branches as $data) {
            $branch = Branch::updateOrCreate(
                ['code' => $data['code']],
                [
                    'name' => $data['name'],
                    'short_code' => $data['short_code'],
                    'api_key' => $data['api_key'],
                    'is_active' => true,
                ]
            );

            $this->line("  {$branch->short_code}  {$branch->name}  ({$branch->code})");
        }

        $this->seedDemoPresence($presence);

        $this->info('Branches updated.');

        return self::SUCCESS;
    }

    private function seedDemoPresence(BranchPresenceService $presence): void
    {
        $counts = [
            'RR-SALEM' => 4,
            'RR-NAMAKKAL' => 2,
            'RR-KOLATHUR' => 1,
        ];

        foreach ($counts as $code => $count) {
            $branch = Branch::query()->where('code', $code)->first();
            if (! $branch) {
                continue;
            }

            $users = [];
            for ($i = 1; $i <= $count; $i++) {
                $users[] = [
                    'external_id' => "demo-{$i}",
                    'user_name' => "Staff {$i}",
                    'login_flag' => 1,
                ];
            }

            $presence->syncActiveUsers($branch, $users);
        }
    }
}
