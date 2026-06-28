<?php

namespace App\Services;

use App\Models\Branch;
use App\Models\SyncInboundLog;
use App\Support\BranchDateTime;
use Illuminate\Http\Request;
use Symfony\Component\HttpFoundation\Response;
use Throwable;

class SyncInboundLogger
{
    public function logFromRequest(
        Request $request,
        ?Response $response,
        float $startedAt,
        ?Throwable $exception = null,
    ): SyncInboundLog {
        /** @var Branch|null $branch */
        $branch = $request->attributes->get('sync_branch');
        $completedAt = BranchDateTime::now();
        $durationMs = (int) round((microtime(true) - $startedAt) * 1000);

        $httpStatus = $response?->getStatusCode();
        $status = $this->resolveStatus($httpStatus, $exception);

        $responseMeta = $this->extractResponseMeta($response);
        $requestMeta = $this->extractRequestMeta($request);

        $errorMessage = $exception?->getMessage()
            ?? ($responseMeta['message'] ?? null);

        return SyncInboundLog::query()->create([
            'branch_id' => $branch?->id,
            'branch_code' => $branch?->code ?? $request->header('X-Branch-Code'),
            'http_method' => $request->method(),
            'path' => '/'.$request->path(),
            'endpoint' => $this->resolveEndpoint($request->path()),
            'http_status' => $httpStatus,
            'status' => $status,
            'records_received' => $requestMeta['records_count'] ?? $responseMeta['received'] ?? null,
            'records_upserted' => $responseMeta['upserted'] ?? null,
            'lines_upserted' => $responseMeta['lines_upserted'] ?? $requestMeta['lines_count'] ?? null,
            'error_message' => $errorMessage ? mb_substr($errorMessage, 0, 2000) : null,
            'request_meta' => $requestMeta ?: null,
            'response_meta' => $responseMeta ?: null,
            'duration_ms' => $durationMs,
            'ip_address' => $request->ip(),
            'started_at' => BranchDateTime::toDisplay($completedAt->copy()->subMilliseconds($durationMs)),
            'completed_at' => BranchDateTime::toDisplay($completedAt),
        ]);
    }

    public function logAuthFailure(Request $request, int $httpStatus, string $message): SyncInboundLog
    {
        $at = BranchDateTime::nowString();

        return SyncInboundLog::query()->create([
            'branch_id' => null,
            'branch_code' => $request->header('X-Branch-Code'),
            'http_method' => $request->method(),
            'path' => '/'.$request->path(),
            'endpoint' => $this->resolveEndpoint($request->path()),
            'http_status' => $httpStatus,
            'status' => $httpStatus === 401 ? 'unauthorized' : 'failed',
            'error_message' => mb_substr($message, 0, 2000),
            'request_meta' => $this->extractRequestMeta($request) ?: null,
            'ip_address' => $request->ip(),
            'started_at' => $at,
            'completed_at' => $at,
            'duration_ms' => 0,
        ]);
    }

    private function resolveStatus(?int $httpStatus, ?Throwable $exception): string
    {
        if ($exception !== null) {
            return 'failed';
        }

        return match (true) {
            $httpStatus === 401 => 'unauthorized',
            $httpStatus === 422 => 'validation_error',
            $httpStatus >= 200 && $httpStatus < 300 => 'success',
            $httpStatus >= 400 => 'failed',
            default => 'unknown',
        };
    }

    private function resolveEndpoint(string $path): ?string
    {
        if (! str_starts_with($path, 'api/sync')) {
            return null;
        }

        $suffix = trim(substr($path, strlen('api/sync')), '/');

        return $suffix === '' ? 'sync' : $suffix;
    }

    /**
     * @return array<string, mixed>
     */
    private function extractRequestMeta(Request $request): array
    {
        $meta = [];

        if ($request->isMethod('GET')) {
            return ['query' => $request->query()];
        }

        $payload = $request->all();
        if ($payload === []) {
            return $meta;
        }

        if (isset($payload['records']) && is_array($payload['records'])) {
            $meta['records_count'] = count($payload['records']);
            $meta['lines_count'] = $this->countDetailLines($payload['records']);
        }

        if (isset($payload['last_synced_at'])) {
            $meta['last_synced_at'] = $payload['last_synced_at'];
        }

        if (isset($payload['users']) && is_array($payload['users'])) {
            $meta['users_count'] = count($payload['users']);
        }

        return $meta;
    }

    /**
     * @param  array<int, mixed>  $records
     */
    private function countDetailLines(array $records): int
    {
        $total = 0;

        foreach ($records as $record) {
            if (! is_array($record)) {
                continue;
            }

            $lines = $record['details'] ?? $record['lines'] ?? [];
            if (isset($record['header']) && is_array($record['header'])) {
                $lines = $record['details'] ?? $record['lines'] ?? [];
            }

            if (is_array($lines)) {
                $total += count($lines);
            }
        }

        return $total;
    }

    /**
     * @return array<string, mixed>
     */
    private function extractResponseMeta(?Response $response): array
    {
        if ($response === null) {
            return [];
        }

        $content = $response->getContent();
        if (! is_string($content) || $content === '') {
            return [];
        }

        $decoded = json_decode($content, true);
        if (! is_array($decoded)) {
            return [];
        }

        $keep = [
            'ok', 'message', 'entity', 'received', 'upserted', 'lines_upserted',
            'headers_created', 'headers_updated',
            'branch_code', 'quotation_last_sync_on', 'estimation_last_sync_on',
            'sales_last_sync_on', 'errors',
        ];

        return array_intersect_key($decoded, array_flip($keep));
    }
}
