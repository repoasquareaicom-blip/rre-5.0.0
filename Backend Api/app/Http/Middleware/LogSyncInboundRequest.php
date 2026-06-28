<?php

namespace App\Http\Middleware;

use App\Services\SyncInboundLogger;
use Closure;
use Illuminate\Http\Request;
use Symfony\Component\HttpFoundation\Response;
use Throwable;

class LogSyncInboundRequest
{
    public function __construct(
        private SyncInboundLogger $logger,
    ) {}

    public function handle(Request $request, Closure $next): Response
    {
        $startedAt = microtime(true);

        try {
            $response = $next($request);
        } catch (Throwable $e) {
            $this->logger->logFromRequest($request, null, $startedAt, $e);
            throw $e;
        }

        $this->logger->logFromRequest(
            $request,
            $response instanceof Response ? $response : null,
            $startedAt,
        );

        return $response;
    }
}
