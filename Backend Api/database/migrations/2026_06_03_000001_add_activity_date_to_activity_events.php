<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::table('activity_events', function (Blueprint $table) {
            $table->date('activity_date')->nullable()->after('amount');
            $table->index(['activity_date', 'id']);
            $table->index(['branch_id', 'activity_date']);
        });

        DB::table('activity_events')
            ->whereNull('activity_date')
            ->update(['activity_date' => DB::raw('DATE(occurred_at)')]);
    }

    public function down(): void
    {
        Schema::table('activity_events', function (Blueprint $table) {
            $table->dropIndex(['activity_date', 'id']);
            $table->dropIndex(['branch_id', 'activity_date']);
            $table->dropColumn('activity_date');
        });
    }
};
