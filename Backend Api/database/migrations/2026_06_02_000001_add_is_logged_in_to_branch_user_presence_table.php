<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::table('branch_user_presence', function (Blueprint $table) {
            $table->boolean('is_logged_in')->default(false)->after('machine_name');
            $table->index(['branch_id', 'is_logged_in']);
        });
    }

    public function down(): void
    {
        Schema::table('branch_user_presence', function (Blueprint $table) {
            $table->dropIndex(['branch_id', 'is_logged_in']);
            $table->dropColumn('is_logged_in');
        });
    }
};
