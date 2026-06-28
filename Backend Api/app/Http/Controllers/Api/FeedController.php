<?php

namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use App\Services\BranchStatusService;
use App\Services\ActivityFeedService;
use App\Support\BranchDateTime;
use Carbon\Carbon;
use Illuminate\Http\JsonResponse;
use Illuminate\Http\Request;

class FeedController extends Controller
{
    public function __construct(
        private BranchStatusService $branchStatus,
        private ActivityFeedService $activityFeed,
    ) {}

    public function index(Request $request): JsonResponse
    {
        $sinceId = (int) $request->input('since_id', 0);
        $limit = min(100, max(10, (int) $request->input('limit', 40)));
        $branchIds = $this->parseBranchIds($request);
        $types = $this->parseTypes($request);
        $date = $request->input('date', BranchDateTime::today());

        try {
            $dateCarbon = Carbon::parse($date)->startOfDay();
        } catch (\Throwable) {
            $dateCarbon = BranchDateTime::now()->startOfDay();
            $date = $dateCarbon->toDateString();
        }

        $result = $this->activityFeed->feed($dateCarbon, $branchIds, $types, $sinceId, $limit);

        return response()->json([
            'latest_id' => $result['latest_id'],
            'date' => $date,
            'items' => $result['items'],
            'branches' => $this->branchStatus->branchesWithStatus()->values(),
        ]);
    }

    /** @return list<int> */
    private function parseBranchIds(Request $request): array
    {
        if ($request->filled('branch_ids')) {
            return array_values(array_filter(array_map(
                'intval',
                explode(',', $request->string('branch_ids')->toString())
            )));
        }

        if ($request->filled('branch_id')) {
            return [(int) $request->input('branch_id')];
        }

        return [];
    }

    /** @return list<string> */
    private function parseTypes(Request $request): array
    {
        if (! $request->filled('types')) {
            return [];
        }

        return array_values(array_filter(explode(',', $request->string('types')->toString())));
    }
}
