<?php

use App\Models\QuotationEstimation;
use App\Models\QuotationHeader;
use App\Models\Sales;
use App\Models\SalesPipes;
use App\Models\SalesTraders;

return [

    /*
    |--------------------------------------------------------------------------
    | Branch header tables (mirror MSSQL names + branch_id + syncon)
    |--------------------------------------------------------------------------
    |
    | last_sync_on for the sync API = MAX(watermark) per branch in that table.
    | syncon = when the row was last written by branch sync (cloud receive time).
    |
    */

    'tables' => [
        'QuotationHeader' => [
            'model' => QuotationHeader::class,
            'entity' => 'quotations',
            'response_key' => 'quotation_last_sync_on',
            'watermark' => 'Updatedon',
            'syncon' => 'syncon',
        ],
        'QuotationEstimation' => [
            'model' => QuotationEstimation::class,
            'entity' => 'estimations',
            'response_key' => 'estimation_last_sync_on',
            'watermark' => 'Updatedon',
            'syncon' => 'syncon',
        ],
        'Sales' => [
            'model' => Sales::class,
            'entity' => 'sales',
            'response_key' => 'sales_last_sync_on',
            'watermark' => 'EnteredOn',
            'syncon' => 'syncon',
        ],
        'SalesPipes' => [
            'model' => SalesPipes::class,
            'entity' => 'sales-pipes',
            'response_key' => 'sales_pipes_last_sync_on',
            'watermark' => 'EnteredOn',
            'syncon' => 'syncon',
        ],
        'SalesTraders' => [
            'model' => SalesTraders::class,
            'entity' => 'sales-traders',
            'response_key' => 'sales_traders_last_sync_on',
            'watermark' => 'EnteredOn',
            'syncon' => 'syncon',
        ],
    ],

];
