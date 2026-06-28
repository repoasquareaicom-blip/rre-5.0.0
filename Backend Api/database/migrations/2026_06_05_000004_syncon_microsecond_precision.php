<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        if (DB::getDriverName() !== 'mysql') {
            return;
        }

        foreach (['QuotationHeader', 'QuotationEstimation', 'Sales', 'SalesPipes', 'SalesTraders'] as $table) {
            if (! Schema::hasTable($table) || ! Schema::hasColumn($table, 'syncon')) {
                continue;
            }

            DB::statement("ALTER TABLE `{$table}` MODIFY `syncon` DATETIME(6) NULL");
        }
    }

    public function down(): void
    {
        if (DB::getDriverName() !== 'mysql') {
            return;
        }

        foreach (['QuotationHeader', 'QuotationEstimation', 'Sales', 'SalesPipes', 'SalesTraders'] as $table) {
            if (! Schema::hasTable($table) || ! Schema::hasColumn($table, 'syncon')) {
                continue;
            }

            DB::statement("ALTER TABLE `{$table}` MODIFY `syncon` TIMESTAMP NULL");
        }
    }
};
