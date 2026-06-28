<?php

use App\Models\QuotationEstimation;
use App\Models\QuotationEstimationDetails;
use App\Models\QuotationHeader;
use App\Models\QuotationDetails;
use App\Models\Sales;
use App\Models\SalesDetails;
use App\Models\SalesPipes;
use App\Models\SalesPipesDetails;
use App\Models\SalesTraders;
use App\Models\SalesTradersDetails;

/**
 * MS SQL mirror definitions for branch sync POST APIs.
 * Cloud-only columns on headers: branch_id, syncon.
 */
return [

    'chunk_size' => 10,

    'entities' => [

        'quotations' => [
            'header_model' => QuotationHeader::class,
            'details_model' => QuotationDetails::class,
            'header_table' => 'QuotationHeader',
            'details_table' => 'QuotationDetails',
            'header_unique' => ['branch_id', 'Quotationid'],
            'header_id_column' => 'Quotationid',
            'details_parent_column' => 'Quotationid',
            'details_line_key' => 'sino',
            'watermark' => 'Updatedon',
            'response_key' => 'quotation_last_sync_on',
        ],

        'estimations' => [
            'header_model' => QuotationEstimation::class,
            'details_model' => QuotationEstimationDetails::class,
            'header_table' => 'QuotationEstimation',
            'details_table' => 'QuotationEstimationDetails',
            'header_unique' => ['branch_id', 'Estimationid'],
            'header_id_column' => 'Estimationid',
            'header_identity_column' => 'SlNo',
            'details_parent_column' => 'Estimationid',
            'details_line_key' => 'sino',
            'watermark' => 'Updatedon',
            'response_key' => 'estimation_last_sync_on',
        ],

        'sales' => [
            'header_model' => Sales::class,
            'details_model' => SalesDetails::class,
            'header_table' => 'Sales',
            'details_table' => 'SalesDetails',
            'header_unique' => ['branch_id', 'Salesid'],
            'header_id_column' => 'Salesid',
            'header_identity_column' => 'slno',
            'details_parent_column' => 'Salesid',
            'details_line_key' => null,
            'watermark' => 'EnteredOn',
            'response_key' => 'sales_last_sync_on',
        ],

        'sales-pipes' => [
            'header_model' => SalesPipes::class,
            'details_model' => SalesPipesDetails::class,
            'header_table' => 'SalesPipes',
            'details_table' => 'SalesPipesDetails',
            'header_unique' => ['branch_id', 'Salesid'],
            'header_id_column' => 'Salesid',
            'header_identity_column' => 'slno',
            'details_parent_column' => 'Salesid',
            'details_line_key' => null,
            'watermark' => 'EnteredOn',
            'response_key' => 'sales_pipes_last_sync_on',
        ],

        'sales-traders' => [
            'header_model' => SalesTraders::class,
            'details_model' => SalesTradersDetails::class,
            'header_table' => 'SalesTraders',
            'details_table' => 'SalesTradersDetails',
            'header_unique' => ['branch_id', 'Salesid'],
            'header_id_column' => 'Salesid',
            'header_identity_column' => 'slno',
            'details_parent_column' => 'Salesid',
            'details_line_key' => null,
            'watermark' => 'EnteredOn',
            'response_key' => 'sales_traders_last_sync_on',
        ],

    ],

    'tables' => [

        'QuotationHeader' => [
            'columns' => [
                'Quotationid', 'Customerid', 'date', 'Referenceid', 'Assist', 'Status',
                'Updatedon', 'Updatedby', 'isdelete', 'customename', 'City', 'Isestimate',
                'sino', 'Iscancel', 'Ispdi', 'Pdiby', 'pdion', 'AssistName',
            ],
            'aliases' => [
                'customemame' => 'customename',
                'quotationid' => 'Quotationid',
                'external_id' => 'Quotationid',
            ],
            'integers' => ['sino', 'Iscancel', 'Ispdi', 'Pdiby'],
            'datetimes' => ['date', 'Updatedon', 'pdion'],
        ],

        'QuotationDetails' => [
            'columns' => [
                'Quotationid', 'Productid', 'Rate', 'Quantity', 'Amount', 'sino',
                'Status', 'pqty', 'Productserialno',
            ],
            'aliases' => ['quotationid' => 'Quotationid', 'Sino' => 'sino'],
            'integers' => ['sino'],
            'datetimes' => [],
        ],

        'QuotationEstimation' => [
            'columns' => [
                'Estimationid', 'Quotationid', 'Customerid', 'date', 'Referenceid', 'Assist',
                'Status', 'customername', 'City', 'Total', 'LessAmount', 'GrandTotal', 'Remarks',
                'Updatedon', 'Updatedby', 'isdelete', 'Paymentmode', 'Paid', 'Balance',
                'iscreditapproved', 'IsBilled', 'ischecked', 'checkedby', 'IsReturnApproved',
                'IsReturnChecked', 'SlNo', 'returnvalue', 'Iscancel', 'others', 'ismove', 'ispack',
                'AssistName',
            ],
            'aliases' => [
                'SiNo' => 'SlNo',
                'sino' => 'SlNo',
                'estimationid' => 'Estimationid',
            ],
            'integers' => ['SlNo', 'ischecked', 'returnvalue', 'Iscancel', 'ismove', 'ispack'],
            'datetimes' => ['date', 'Updatedon'],
            'booleans' => ['IsBilled'],
            'decimals' => ['others' => 3],
        ],

        'QuotationEstimationDetails' => [
            'columns' => [
                'Estimationid', 'Productid', 'Rate', 'Quantity', 'Amount', 'isdelevered',
                'DeliveredQty', 'Isreturncheck', 'sino', 'Location', 'Productserialno',
            ],
            'aliases' => [
                'estimationid' => 'Estimationid',
                'Sino' => 'sino',
            ],
            'integers' => ['isdelevered', 'Isreturncheck', 'sino'],
            'datetimes' => [],
        ],

        'Sales' => [
            'columns' => [
                'Salesid', 'Customerid', 'customername', 'City', 'Referenceid', 'Assist',
                'Paymentmode', 'Updatedon', 'Updatedby', 'TotalAmount', 'LessAmount', 'GrandTotal',
                'Address1', 'Address2', 'Tin', 'Mobile', 'slno', 'others', 'EnteredOn', 'State', 'GstText',
            ],
            'aliases' => ['salesid' => 'Salesid', 'Sino' => 'slno', 'sino' => 'slno'],
            'integers' => ['slno'],
            'datetimes' => ['Updatedon', 'EnteredOn'],
            'decimals' => ['others' => 2],
        ],

        'SalesDetails' => [
            'columns' => ['Salesid', 'Productid', 'product_name', 'Rate', 'Quantity', 'Amount', 'gst'],
            'aliases' => [
                'SalesId' => 'Salesid',
                'ProductId' => 'Productid',
                'DisplayName' => 'product_name',
                'ItemName' => 'product_name',
                'ProductName' => 'product_name',
            ],
            'integers' => [],
            'datetimes' => [],
            'decimals' => ['gst' => 2],
        ],

        'SalesPipes' => [
            'columns' => [
                'Salesid', 'Customerid', 'customername', 'City', 'Referenceid', 'Assist',
                'Paymentmode', 'Updatedon', 'Updatedby', 'TotalAmount', 'LessAmount', 'GrandTotal',
                'Address1', 'Address2', 'Tin', 'Mobile', 'slno', 'others', 'EnteredOn', 'State', 'GstText',
            ],
            'aliases' => ['salesid' => 'Salesid', 'Sino' => 'slno', 'sino' => 'slno'],
            'integers' => ['slno'],
            'datetimes' => ['Updatedon', 'EnteredOn'],
            'decimals' => ['others' => 2],
        ],

        'SalesPipesDetails' => [
            'columns' => ['Salesid', 'Productid', 'product_name', 'Rate', 'Quantity', 'Amount', 'gst'],
            'aliases' => [
                'SalesId' => 'Salesid',
                'ProductId' => 'Productid',
                'DisplayName' => 'product_name',
                'ItemName' => 'product_name',
                'ProductName' => 'product_name',
            ],
            'integers' => [],
            'datetimes' => [],
            'decimals' => ['gst' => 2],
        ],

        'SalesTraders' => [
            'columns' => [
                'Salesid', 'Customerid', 'customername', 'City', 'Referenceid', 'Assist',
                'Paymentmode', 'Updatedon', 'Updatedby', 'TotalAmount', 'LessAmount', 'GrandTotal',
                'Address1', 'Address2', 'Tin', 'Mobile', 'slno', 'others', 'EnteredOn', 'State', 'GstText',
            ],
            'aliases' => ['salesid' => 'Salesid', 'Sino' => 'slno', 'sino' => 'slno'],
            'integers' => ['slno'],
            'datetimes' => ['Updatedon', 'EnteredOn'],
            'decimals' => ['others' => 2],
        ],

        'SalesTradersDetails' => [
            'columns' => ['Salesid', 'Productid', 'product_name', 'Rate', 'Quantity', 'Amount', 'gst'],
            'aliases' => [
                'SalesId' => 'Salesid',
                'ProductId' => 'Productid',
                'DisplayName' => 'product_name',
                'ItemName' => 'product_name',
                'ProductName' => 'product_name',
            ],
            'integers' => [],
            'datetimes' => [],
            'decimals' => ['gst' => 2],
        ],

    ],

];
