<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::create('QuotationEstimationDetails', function (Blueprint $table) {
            $table->foreignId('branch_id')->constrained()->cascadeOnDelete();
            $table->string('Estimationid', 50);
            $table->string('Productid', 20)->nullable();
            $table->string('Rate', 10)->nullable();
            $table->string('Quantity', 10)->nullable();
            $table->string('Amount', 20)->nullable();
            $table->integer('isdelevered')->nullable();
            $table->string('DeliveredQty', 100)->nullable();
            $table->integer('Isreturncheck')->nullable();
            $table->unsignedBigInteger('sino');
            $table->string('Location', 100)->nullable();
            $table->string('Productserialno', 255)->nullable();

            $table->primary(['branch_id', 'sino']);
            $table->index(['branch_id', 'Estimationid']);
        });

        Schema::create('SalesDetails', function (Blueprint $table) {
            $table->foreignId('branch_id')->constrained()->cascadeOnDelete();
            $table->string('Salesid', 50)->nullable();
            $table->string('Productid', 20)->nullable();
            $table->string('Rate', 10)->nullable();
            $table->string('Quantity', 10)->nullable();
            $table->string('Amount', 20)->nullable();
            $table->decimal('gst', 18, 2)->nullable();

            $table->index(['branch_id', 'Salesid']);
        });

        foreach (['SalesPipesDetails', 'SalesTradersDetails'] as $tableName) {
            Schema::create($tableName, function (Blueprint $table) {
                $table->foreignId('branch_id')->constrained()->cascadeOnDelete();
                $table->string('Salesid', 50)->nullable();
                $table->string('Productid', 20)->nullable();
                $table->string('Rate', 10)->nullable();
                $table->string('Quantity', 25)->nullable();
                $table->string('Amount', 20)->nullable();
                $table->decimal('gst', 18, 2)->nullable();

                $table->index(['branch_id', 'Salesid']);
            });
        }

        foreach (['Sales', 'SalesPipes', 'SalesTraders'] as $tableName) {
            if (! Schema::hasTable($tableName)) {
                continue;
            }

            Schema::table($tableName, function (Blueprint $table) use ($tableName) {
                if (! $this->hasIndex($tableName, 'branch_salesid_unique')) {
                    $table->unique(['branch_id', 'Salesid'], 'branch_salesid_unique');
                }
            });
        }
    }

    public function down(): void
    {
        foreach (['Sales', 'SalesPipes', 'SalesTraders'] as $tableName) {
            if (Schema::hasTable($tableName)) {
                Schema::table($tableName, function (Blueprint $table) {
                    $table->dropUnique('branch_salesid_unique');
                });
            }
        }

        Schema::dropIfExists('SalesTradersDetails');
        Schema::dropIfExists('SalesPipesDetails');
        Schema::dropIfExists('SalesDetails');
        Schema::dropIfExists('QuotationEstimationDetails');
    }

    private function hasIndex(string $table, string $indexName): bool
    {
        $indexes = Schema::getIndexes($table);

        foreach ($indexes as $index) {
            if (($index['name'] ?? '') === $indexName) {
                return true;
            }
        }

        return false;
    }
};
