<?php

use App\Http\Controllers\Api\AuthController;
use App\Http\Controllers\Api\FeedController;
use App\Http\Controllers\Api\MonitorController;
use App\Http\Controllers\Api\ProductSyncController;
use App\Http\Controllers\Api\SyncController;
use Illuminate\Support\Facades\Route;

Route::prefix('auth')->group(function () {
    Route::post('/login', [AuthController::class, 'login']);
    Route::middleware('monitor.auth')->group(function () {
        Route::get('/me', [AuthController::class, 'me']);
        Route::post('/logout', [AuthController::class, 'logout']);
    });
});

Route::middleware('monitor.auth')->prefix('monitor')->group(function () {
    Route::get('/feed', [FeedController::class, 'index']);
    Route::get('/feed/detail', [MonitorController::class, 'feedDetail']);
    Route::get('/dashboard', [MonitorController::class, 'dashboard']);
    Route::get('/quotations', [MonitorController::class, 'quotations']);
    Route::get('/estimations', [MonitorController::class, 'estimations']);
    Route::get('/sales', [MonitorController::class, 'sales']);
    Route::get('/sales-pipes', [MonitorController::class, 'salesPipes']);
    Route::get('/sales-traders', [MonitorController::class, 'salesTraders']);
    Route::get('/vouchers', [MonitorController::class, 'vouchers']);
    Route::get('/settlements/summary', [MonitorController::class, 'settlementsSummary']);
});

Route::middleware(['sync.branch', 'sync.inbound.log'])->prefix('sync')->group(function () {
    Route::get('/last-sync-on', [SyncController::class, 'lastSyncOn']);
    Route::get('/cursor/{entity}', [SyncController::class, 'cursor']);
    Route::post('/quotations', [SyncController::class, 'quotations']);
    Route::post('/estimations', [SyncController::class, 'estimations']);
    Route::post('/sales', [SyncController::class, 'sales']);
    Route::post('/sales-pipes', [SyncController::class, 'salesPipes']);
    Route::post('/sales-traders', [SyncController::class, 'salesTraders']);
    Route::post('/vouchers', [SyncController::class, 'vouchers']);
    Route::post('/receipts', [SyncController::class, 'receipts']);
    Route::post('/settlements', [SyncController::class, 'settlements']);
    Route::post('/presence', [SyncController::class, 'presence']);
    Route::post('/products/publish', [ProductSyncController::class, 'publish']);
    Route::get('/products/changes', [ProductSyncController::class, 'changes']);
    Route::get('/status', [SyncController::class, 'status']);
});
