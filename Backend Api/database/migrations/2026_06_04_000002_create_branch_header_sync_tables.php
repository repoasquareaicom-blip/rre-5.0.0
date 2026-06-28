<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::dropIfExists('QuotationEstimation');

        Schema::create('QuotationEstimation', function (Blueprint $table) {
            $table->foreignId('branch_id')->constrained()->cascadeOnDelete();
            $table->string('Estimationid', 20);
            $table->string('Quotationid', 20)->nullable();
            $table->string('Customerid', 10)->nullable();
            $table->dateTime('date')->nullable();
            $table->string('Referenceid', 10)->nullable();
            $table->string('Assist', 20)->nullable();
            $table->string('Status', 100)->nullable();
            $table->string('customername', 200)->nullable();
            $table->string('City', 200)->nullable();
            $table->string('Total', 200)->nullable();
            $table->string('LessAmount', 200)->nullable();
            $table->string('GrandTotal', 200)->nullable();
            $table->text('Remarks')->nullable();
            $table->dateTime('Updatedon')->nullable();
            $table->string('Updatedby', 10)->nullable();
            $table->string('isdelete', 10)->nullable();
            $table->string('Paymentmode', 100)->nullable();
            $table->string('Paid', 100)->nullable();
            $table->string('Balance', 100)->nullable();
            $table->string('iscreditapproved', 100)->nullable();
            $table->boolean('IsBilled')->nullable();
            $table->integer('ischecked')->nullable();
            $table->string('checkedby', 10)->nullable();
            $table->string('IsReturnApproved', 100)->nullable();
            $table->string('IsReturnChecked', 10)->nullable();
            $table->unsignedBigInteger('SlNo');
            $table->integer('returnvalue')->nullable();
            $table->integer('Iscancel')->nullable();
            $table->decimal('others', 18, 3)->nullable();
            $table->integer('ismove')->nullable();
            $table->integer('ispack')->nullable();
            $table->string('AssistName', 100)->nullable();
            $table->timestamp('syncon')->nullable();

            $table->primary(['branch_id', 'Estimationid']);
            $table->unique(['branch_id', 'SlNo']);
            $table->index(['branch_id', 'Updatedon']);
            $table->index(['branch_id', 'syncon']);
        });

        // Legacy monitor table `sales` conflicts with MSSQL-mirror `Sales` on case-insensitive MySQL.
        Schema::dropIfExists('sales');

        foreach (['Sales', 'SalesPipes', 'SalesTraders'] as $tableName) {
            Schema::dropIfExists($tableName);

            Schema::create($tableName, function (Blueprint $table) {
                $table->foreignId('branch_id')->constrained()->cascadeOnDelete();
                $table->string('Salesid', 20)->nullable();
                $table->string('Customerid', 10)->nullable();
                $table->string('customername', 200)->nullable();
                $table->string('City', 200)->nullable();
                $table->string('Referenceid', 10)->nullable();
                $table->string('Assist', 20)->nullable();
                $table->string('Paymentmode', 200)->nullable();
                $table->dateTime('Updatedon')->nullable();
                $table->string('Updatedby', 10)->nullable();
                $table->string('TotalAmount', 100)->nullable();
                $table->string('LessAmount', 100)->nullable();
                $table->string('GrandTotal', 100)->nullable();
                $table->string('Address1', 1000)->nullable();
                $table->string('Address2', 1000)->nullable();
                $table->string('Tin', 100)->nullable();
                $table->string('Mobile', 40)->nullable();
                $table->unsignedBigInteger('slno');
                $table->decimal('others', 18, 2)->nullable();
                $table->dateTime('EnteredOn')->nullable();
                $table->string('State', 100)->nullable();
                $table->string('GstText', 100)->nullable();
                $table->timestamp('syncon')->nullable();

                $table->primary(['branch_id', 'slno']);
                $table->index(['branch_id', 'Salesid']);
                $table->index(['branch_id', 'EnteredOn']);
                $table->index(['branch_id', 'syncon']);
            });
        }
    }

    public function down(): void
    {
        Schema::dropIfExists('SalesTraders');
        Schema::dropIfExists('SalesPipes');
        Schema::dropIfExists('Sales');
        Schema::dropIfExists('QuotationEstimation');
    }
};
