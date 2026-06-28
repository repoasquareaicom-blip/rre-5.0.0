<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::create('branch_user_presence', function (Blueprint $table) {
            $table->id();
            $table->foreignId('branch_id')->constrained()->cascadeOnDelete();
            $table->string('external_user_id', 64);
            $table->string('user_name')->nullable();
            $table->string('machine_name')->nullable();
            $table->timestamp('last_seen_at');
            $table->timestamps();

            $table->unique(['branch_id', 'external_user_id']);
            $table->index(['branch_id', 'last_seen_at']);
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('branch_user_presence');
    }
};
