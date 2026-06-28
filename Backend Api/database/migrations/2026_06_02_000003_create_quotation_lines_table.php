<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::create('quotation_lines', function (Blueprint $table) {
            $table->id();
            $table->foreignId('branch_id')->constrained()->cascadeOnDelete();
            $table->foreignId('quotation_id')->constrained()->cascadeOnDelete();
            $table->unsignedBigInteger('line_sino');
            $table->string('product_id', 64)->nullable();
            $table->decimal('rate', 14, 4)->default(0);
            $table->decimal('quantity', 14, 4)->default(0);
            $table->decimal('amount', 14, 2)->default(0);
            $table->string('status', 100)->nullable();
            $table->string('pqty', 100)->nullable();
            $table->string('product_serial_no', 255)->nullable();
            $table->json('meta')->nullable();
            $table->timestamps();

            $table->unique(['quotation_id', 'line_sino']);
            $table->index(['branch_id', 'line_sino']);
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('quotation_lines');
    }
};
