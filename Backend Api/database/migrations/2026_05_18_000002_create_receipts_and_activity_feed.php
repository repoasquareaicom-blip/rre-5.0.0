<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::create('receipts', function (Blueprint $table) {
            $table->id();
            $table->foreignId('branch_id')->constrained()->cascadeOnDelete();
            $table->string('external_id', 64);
            $table->string('receipt_type', 64)->nullable();
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

        Schema::create('activity_events', function (Blueprint $table) {
            $table->id();
            $table->foreignId('branch_id')->constrained()->cascadeOnDelete();
            $table->string('entity_type', 32);
            $table->unsignedBigInteger('entity_record_id')->nullable();
            $table->string('external_id', 64);
            $table->string('event_kind', 16)->default('created');
            $table->string('title');
            $table->string('subtitle')->nullable();
            $table->decimal('amount', 14, 2)->nullable();
            $table->timestamp('occurred_at');
            $table->timestamps();

            $table->index(['occurred_at', 'id']);
            $table->index(['branch_id', 'occurred_at']);
            $table->index('entity_type');
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('activity_events');
        Schema::dropIfExists('receipts');
    }
};
