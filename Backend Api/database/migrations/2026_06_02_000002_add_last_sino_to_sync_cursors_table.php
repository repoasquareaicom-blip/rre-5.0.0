<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::table('sync_cursors', function (Blueprint $table) {
            $table->unsignedBigInteger('last_sino')->default(0)->after('entity_type');
        });
    }

    public function down(): void
    {
        Schema::table('sync_cursors', function (Blueprint $table) {
            $table->dropColumn('last_sino');
        });
    }
};
