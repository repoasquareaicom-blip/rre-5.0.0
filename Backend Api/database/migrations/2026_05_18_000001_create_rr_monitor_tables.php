<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::create('branches', function (Blueprint $table) {
            $table->id();
            $table->string('code', 32)->unique();
            $table->string('name');
            $table->string('api_key', 64)->unique();
            $table->boolean('is_active')->default(true);
            $table->timestamps();
        });

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

        Schema::create('estimations', function (Blueprint $table) {
            $table->id();
            $table->foreignId('branch_id')->constrained()->cascadeOnDelete();
            $table->string('external_id', 64);
            $table->string('quotation_external_id', 64)->nullable();
            $table->string('customer_name')->nullable();
            $table->date('document_date')->nullable();
            $table->string('status', 64)->nullable();
            $table->decimal('amount', 14, 2)->default(0);
            $table->timestamp('source_updated_at')->nullable();
            $table->json('meta')->nullable();
            $table->timestamps();

            $table->unique(['branch_id', 'external_id']);
            $table->index(['branch_id', 'document_date']);
        });

        Schema::create('sales', function (Blueprint $table) {
            $table->id();
            $table->foreignId('branch_id')->constrained()->cascadeOnDelete();
            $table->string('external_id', 64);
            $table->string('estimation_external_id', 64)->nullable();
            $table->string('quotation_external_id', 64)->nullable();
            $table->string('customer_name')->nullable();
            $table->date('document_date')->nullable();
            $table->string('status', 64)->nullable();
            $table->string('payment_mode', 64)->nullable();
            $table->decimal('amount', 14, 2)->default(0);
            $table->timestamp('source_updated_at')->nullable();
            $table->json('meta')->nullable();
            $table->timestamps();

            $table->unique(['branch_id', 'external_id']);
            $table->index(['branch_id', 'document_date']);
        });

        Schema::create('vouchers', function (Blueprint $table) {
            $table->id();
            $table->foreignId('branch_id')->constrained()->cascadeOnDelete();
            $table->string('external_id', 64);
            $table->string('voucher_type', 64)->nullable();
            $table->date('document_date')->nullable();
            $table->string('party_name')->nullable();
            $table->string('status', 64)->nullable();
            $table->decimal('amount', 14, 2)->default(0);
            $table->timestamp('source_updated_at')->nullable();
            $table->json('meta')->nullable();
            $table->timestamps();

            $table->unique(['branch_id', 'external_id']);
            $table->index(['branch_id', 'document_date']);
        });

        Schema::create('settlements', function (Blueprint $table) {
            $table->id();
            $table->foreignId('branch_id')->constrained()->cascadeOnDelete();
            $table->string('external_id', 64);
            $table->date('settlement_date');
            $table->decimal('beginning_cash', 14, 2)->default(0);
            $table->decimal('cash_in', 14, 2)->default(0);
            $table->decimal('cash_out', 14, 2)->default(0);
            $table->decimal('net_cash', 14, 2)->default(0);
            $table->decimal('actual_cash', 14, 2)->default(0);
            $table->decimal('over_short', 14, 2)->default(0);
            $table->string('status', 64)->nullable();
            $table->timestamp('source_updated_at')->nullable();
            $table->json('meta')->nullable();
            $table->timestamps();

            $table->unique(['branch_id', 'external_id']);
            $table->index(['branch_id', 'settlement_date']);
        });

        Schema::create('sync_logs', function (Blueprint $table) {
            $table->id();
            $table->foreignId('branch_id')->constrained()->cascadeOnDelete();
            $table->string('entity_type', 32);
            $table->unsignedInteger('records_received')->default(0);
            $table->unsignedInteger('records_upserted')->default(0);
            $table->string('status', 16)->default('success');
            $table->text('error_message')->nullable();
            $table->timestamp('started_at')->nullable();
            $table->timestamp('completed_at')->nullable();
            $table->timestamps();

            $table->index(['branch_id', 'entity_type', 'created_at']);
        });

        Schema::create('sync_cursors', function (Blueprint $table) {
            $table->id();
            $table->foreignId('branch_id')->constrained()->cascadeOnDelete();
            $table->string('entity_type', 32);
            $table->timestamp('last_synced_at')->nullable();
            $table->string('last_external_id', 64)->nullable();
            $table->timestamps();

            $table->unique(['branch_id', 'entity_type']);
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('sync_cursors');
        Schema::dropIfExists('sync_logs');
        Schema::dropIfExists('settlements');
        Schema::dropIfExists('vouchers');
        Schema::dropIfExists('sales');
        Schema::dropIfExists('estimations');
        Schema::dropIfExists('quotations');
        Schema::dropIfExists('branches');
    }
};
