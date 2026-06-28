<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        foreach (['SalesDetails', 'SalesPipesDetails', 'SalesTradersDetails'] as $tableName) {
            if (! Schema::hasTable($tableName)) {
                continue;
            }

            Schema::table($tableName, function (Blueprint $table) use ($tableName) {
                if (! Schema::hasColumn($tableName, 'product_name')) {
                    $table->string('product_name', 255)->nullable()->after('Productid');
                }
            });
        }
    }

    public function down(): void
    {
        foreach (['SalesDetails', 'SalesPipesDetails', 'SalesTradersDetails'] as $tableName) {
            if (! Schema::hasTable($tableName)) {
                continue;
            }

            Schema::table($tableName, function (Blueprint $table) use ($tableName) {
                if (Schema::hasColumn($tableName, 'product_name')) {
                    $table->dropColumn('product_name');
                }
            });
        }
    }
};
