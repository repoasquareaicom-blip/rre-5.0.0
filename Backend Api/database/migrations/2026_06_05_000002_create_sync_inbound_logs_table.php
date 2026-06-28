<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::create('sync_inbound_logs', function (Blueprint $table) {
            $table->id();
            $table->foreignId('branch_id')->nullable()->constrained()->nullOnDelete();
            $table->string('branch_code', 32)->nullable();
            $table->string('http_method', 10);
            $table->string('path', 255);
            $table->string('endpoint', 64)->nullable();
            $table->unsignedSmallInteger('http_status')->nullable();
            $table->string('status', 32);
            $table->unsignedInteger('records_received')->nullable();
            $table->unsignedInteger('records_upserted')->nullable();
            $table->unsignedInteger('lines_upserted')->nullable();
            $table->text('error_message')->nullable();
            $table->json('request_meta')->nullable();
            $table->json('response_meta')->nullable();
            $table->unsignedInteger('duration_ms')->nullable();
            $table->string('ip_address', 45)->nullable();
            $table->timestamp('started_at');
            $table->timestamp('completed_at')->nullable();

            $table->index(['branch_id', 'started_at']);
            $table->index(['status', 'started_at']);
            $table->index(['endpoint', 'started_at']);
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('sync_inbound_logs');
    }
};
