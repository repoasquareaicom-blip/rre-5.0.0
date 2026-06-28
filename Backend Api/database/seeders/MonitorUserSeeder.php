<?php

namespace Database\Seeders;

use App\Models\User;
use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\Hash;

class MonitorUserSeeder extends Seeder
{
    public function run(): void
    {
        $users = [
            [
                'username' => 'admin',
                'email' => 'admin@asquareai.com',
                'name' => 'Admin',
                'password' => 'admin123',
            ],
            [
                'username' => 'rre',
                'email' => 'rre@asquareai.com',
                'name' => 'RRE Monitor',
                'password' => 'rre',
            ],
        ];

        foreach ($users as $row) {
            User::query()->updateOrCreate(
                ['username' => strtolower($row['username'])],
                [
                    'email' => $row['email'],
                    'name' => $row['name'],
                    'password' => Hash::make($row['password']),
                    'email_verified_at' => now(),
                ]
            );
        }
    }
}
