<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        if (! Schema::hasColumn('QuotationHeader', 'synced_at') || Schema::hasColumn('QuotationHeader', 'syncon')) {
            return;
        }

        Schema::table('QuotationHeader', function (Blueprint $table) {
            $table->dropIndex(['branch_id', 'synced_at']);
        });

        Schema::table('QuotationHeader', function (Blueprint $table) {
            $table->renameColumn('synced_at', 'syncon');
        });

        Schema::table('QuotationHeader', function (Blueprint $table) {
            $table->index(['branch_id', 'syncon']);
        });
    }

    public function down(): void
    {
        if (! Schema::hasColumn('QuotationHeader', 'syncon') || Schema::hasColumn('QuotationHeader', 'synced_at')) {
            return;
        }

        Schema::table('QuotationHeader', function (Blueprint $table) {
            $table->dropIndex(['branch_id', 'syncon']);
        });

        Schema::table('QuotationHeader', function (Blueprint $table) {
            $table->renameColumn('syncon', 'synced_at');
        });

        Schema::table('QuotationHeader', function (Blueprint $table) {
            $table->index(['branch_id', 'synced_at']);
        });
    }
};
