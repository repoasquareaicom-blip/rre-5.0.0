<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::table('QuotationHeader', function (Blueprint $table) {
            $table->timestamp('syncon')->nullable()->after('AssistName');
            $table->index(['branch_id', 'syncon']);
        });
    }

    public function down(): void
    {
        Schema::table('QuotationHeader', function (Blueprint $table) {
            $table->dropIndex(['branch_id', 'syncon']);
            $table->dropColumn('syncon');
        });
    }
};
