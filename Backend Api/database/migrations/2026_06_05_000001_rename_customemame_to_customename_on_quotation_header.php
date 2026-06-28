<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        if (! Schema::hasTable('QuotationHeader')) {
            return;
        }

        if (Schema::hasColumn('QuotationHeader', 'customemame') && ! Schema::hasColumn('QuotationHeader', 'customename')) {
            Schema::table('QuotationHeader', function (Blueprint $table) {
                $table->renameColumn('customemame', 'customename');
            });
        }
    }

    public function down(): void
    {
        if (Schema::hasColumn('QuotationHeader', 'customename') && ! Schema::hasColumn('QuotationHeader', 'customemame')) {
            Schema::table('QuotationHeader', function (Blueprint $table) {
                $table->renameColumn('customename', 'customemame');
            });
        }
    }
};
