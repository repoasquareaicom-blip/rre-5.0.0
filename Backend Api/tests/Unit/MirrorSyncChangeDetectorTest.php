<?php

namespace Tests\Unit;

use App\Models\QuotationHeader;
use App\Services\MirrorSyncChangeDetector;
use Carbon\Carbon;
use Tests\TestCase;

class MirrorSyncChangeDetectorTest extends TestCase
{
    public function test_header_unchanged_when_payload_matches_existing(): void
    {
        $detector = app(MirrorSyncChangeDetector::class);

        $header = new QuotationHeader([
            'branch_id' => 1,
            'Quotationid' => 'Q-100',
            'customename' => 'ACME',
            'Status' => 'Open',
            'sino' => 42,
        ]);
        $header->syncOriginal();

        $payload = [
            'branch_id' => 1,
            'Quotationid' => 'Q-100',
            'customename' => 'ACME',
            'Status' => 'Open',
            'sino' => 42,
        ];

        $this->assertTrue($detector->headerUnchanged($header, $payload));
    }

    public function test_header_changed_when_status_differs(): void
    {
        $detector = app(MirrorSyncChangeDetector::class);

        $header = new QuotationHeader([
            'branch_id' => 1,
            'Quotationid' => 'Q-100',
            'Status' => 'Open',
        ]);
        $header->syncOriginal();

        $payload = [
            'branch_id' => 1,
            'Quotationid' => 'Q-100',
            'Status' => 'Closed',
        ];

        $this->assertFalse($detector->headerUnchanged($header, $payload));
    }

    public function test_datetime_normalization_treats_equivalent_values_as_equal(): void
    {
        $detector = app(MirrorSyncChangeDetector::class);

        $header = new QuotationHeader([
            'branch_id' => 1,
            'Quotationid' => 'Q-100',
            'date' => Carbon::parse('2026-05-21 10:00:00'),
        ]);
        $header->syncOriginal();

        $payload = [
            'branch_id' => 1,
            'Quotationid' => 'Q-100',
            'date' => Carbon::parse('2026-05-21 10:00:00'),
        ];

        $this->assertTrue($detector->headerUnchanged($header, $payload));
    }
}
