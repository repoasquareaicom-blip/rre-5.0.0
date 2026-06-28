<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::create('product_master_sync_changes', function (Blueprint $table) {
            $table->id();
            $table->foreignId('source_branch_id')->constrained('branches')->cascadeOnDelete();
            $table->string('product_id', 50);
            $table->string('item_code', 100)->nullable();
            $table->string('item_name', 255)->nullable();
            $table->string('display_name', 255)->nullable();
            $table->decimal('sales_price', 18, 2)->nullable();
            $table->json('payload');
            $table->timestamps();

            $table->index('product_id');
            $table->index('updated_at');
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('product_master_sync_changes');
    }
};
