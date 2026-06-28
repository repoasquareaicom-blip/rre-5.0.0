<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::dropIfExists('quotation_lines');
        Schema::dropIfExists('quotations');

        Schema::create('QuotationHeader', function (Blueprint $table) {
            $table->foreignId('branch_id')->constrained()->cascadeOnDelete();
            $table->string('Quotationid', 20);
            $table->string('Customerid', 10)->nullable();
            $table->dateTime('date')->nullable();
            $table->string('Referenceid', 10)->nullable();
            $table->string('Assist', 20)->nullable();
            $table->string('Status', 100)->nullable();
            $table->dateTime('Updatedon')->nullable();
            $table->string('Updatedby', 10)->nullable();
            $table->string('isdelete', 10)->nullable();
            $table->string('customename', 200)->nullable();
            $table->string('City', 200)->nullable();
            $table->string('Isestimate', 10)->nullable();
            $table->unsignedBigInteger('sino');
            $table->integer('Iscancel')->nullable();
            $table->integer('Ispdi')->nullable();
            $table->integer('Pdiby')->nullable();
            $table->dateTime('pdion')->nullable();
            $table->string('AssistName', 100)->nullable();

            $table->primary(['branch_id', 'Quotationid']);
            $table->unique(['branch_id', 'sino']);
            $table->index(['branch_id', 'date']);
        });

        Schema::create('QuotationDetails', function (Blueprint $table) {
            $table->foreignId('branch_id')->constrained()->cascadeOnDelete();
            $table->string('Quotationid', 50);
            $table->string('Productid', 20)->nullable();
            $table->string('Rate', 10)->nullable();
            $table->string('Quantity', 10)->nullable();
            $table->string('Amount', 20)->nullable();
            $table->unsignedBigInteger('sino');
            $table->string('Status', 100)->nullable();
            $table->string('pqty', 100)->nullable();
            $table->string('Productserialno', 255)->nullable();

            $table->primary(['branch_id', 'sino']);
            $table->index(['branch_id', 'Quotationid']);
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('QuotationDetails');
        Schema::dropIfExists('QuotationHeader');

        Schema::create('quotations', function (Blueprint $table) {
            $table->id();
            $table->foreignId('branch_id')->constrained()->cascadeOnDelete();
            $table->string('external_id', 64);
            $table->string('customer_name')->nullable();
            $table->date('document_date')->nullable();
            $table->string('status', 64)->nullable();
            $table->string('payment_mode', 64)->nullable();
            $table->decimal('amount', 14, 2)->default(0);
            $table->string('reference_name')->nullable();
            $table->string('assist_by')->nullable();
            $table->timestamp('source_updated_at')->nullable();
            $table->json('meta')->nullable();
            $table->timestamps();
            $table->unique(['branch_id', 'external_id']);
            $table->index(['branch_id', 'document_date']);
        });
    }
};
