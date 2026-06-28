<?php

namespace App\Http\Middleware;

use App\Models\Branch;
use App\Services\SyncInboundLogger;
use Closure;
use Illuminate\Http\Request;
use Symfony\Component\HttpFoundation\Response;

class AuthenticateBranchSync
{
    public function __construct(
        private SyncInboundLogger $inboundLogger,
    ) {}

    public function handle(Request $request, Closure $next): Response
    {
        $apiKey = $request->header('X-Api-Key') ?? $request->bearerToken();
        $branchCode = $request->header('X-Branch-Code');

        if (! $apiKey || ! $branchCode) {
            $this->inboundLogger->logAuthFailure(
                $request,
                401,
                'Missing X-Api-Key and X-Branch-Code headers.',
            );

            return response()->json([
                'message' => 'Missing X-Api-Key and X-Branch-Code headers.',
            ], 401);
        }

        $branch = Branch::query()
            ->where('code', $branchCode)
            ->where('api_key', $apiKey)
            ->where('is_active', true)
            ->first();

        if (! $branch) {
            $this->inboundLogger->logAuthFailure(
                $request,
                401,
                'Invalid branch credentials.',
            );

            return response()->json([
                'message' => 'Invalid branch credentials.',
            ], 401);
        }

        $request->attributes->set('sync_branch', $branch);

        return $next($request);
    }
}
