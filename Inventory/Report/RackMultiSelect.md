# Searchable multi-rack filter enhancement

## Use

Open the Rack Search field. Type any part of a rack name/code (the existing `RackCaption`) or its location, ignoring case. Check racks, then press Done or click outside the popup. The report's Search button applies the selection.

- Search only changes the visible list; it never selects, clears or generates a report.
- Checked RackIds remain selected when hidden by a search. Clear the search or find a rack again to uncheck it individually.
- **Select All** checks every rack, including hidden racks, even while filtered.
- **Clear All** removes every selection, including hidden racks, even while filtered.
- No checked racks means **All Racks**, with no restriction. Checking every rack explicitly restricts to the current list of IDs; the report header lists those captions.
- The field shows one selected caption or `N Racks Selected`. Hover to see full selections.
- Reset clears Product Search, selected racks and the popup's search, unchecks Show Zero Stock, and reloads all applicable racks.
- Typing in the popup and changing checks do not alter the last loaded report. Exports/print use that report's applied filters, including the complete selected rack captions.

The popup is sized/positioned within the current monitor's working area and can open above the field when necessary. Matching occurs in memory with no per-keystroke database calls. IDs are deduplicated independently of captions, so identically named racks at different locations remain distinct.

## SQL deployment

Run the updated complete script `DatabaseScripts/2026_10_16_rack_wise_product_stock_report.sql` **before deploying the enhanced application**. It idempotently creates `dbo.RackStockReportRackIds (RackId INT NOT NULL PRIMARY KEY)` and alters only `dbo.sp_RackWiseProductStockReport`. No stock calculation or source-data changes are introduced.

The new `@RackIds dbo.RackStockReportRackIds READONLY` parameter is passed with `SqlDbType.Structured` and `TypeName = "dbo.RackStockReportRackIds"`. No ID values are concatenated into SQL. Membership is applied to both ledger aggregation and valid zero-stock allocations. An empty TVP means all racks; unknown IDs match no racks. The legacy `@RackId INT = NULL` remains supported for existing callers. If both are supplied, their restrictions intersect.

```sql
DECLARE @Selected dbo.RackStockReportRackIds;
-- Empty @Selected returns all applicable racks.
EXEC dbo.sp_RackWiseProductStockReport @RackIds = @Selected;

-- Example IDs: replace with actual RackMaster.RackId values.
INSERT INTO @Selected (RackId) VALUES (1), (5), (14);
EXEC dbo.sp_RackWiseProductStockReport
    @ProductSearch = NULL,
    @ShowZeroStock = 0,
    @RackIds = @Selected;
```

Applications using restricted SQL roles need permission to use the table type (`EXECUTE` and `REFERENCES` on the type, or equivalent schema/database permission), alongside their existing procedure permission.

## Modified code

- `Report/RackWiseProductStockReport.cs`: contains the native WinForms `RackMultiSelect` checked popup and selection model alongside the report. No separate selector source file or new package is required.
- `Report/RackWiseProductStockReport.Designer.cs`: replaces only the rack ComboBox with the compact selector.
- `Report/RackWiseProductStockReport.cs`: loads the same rack list, captures selected IDs/captions before background work, passes the TVP, resets selection.
- `DatabaseScripts/2026_10_16_rack_wise_product_stock_report.sql`: complete type/procedure deployment script; signed ledger calculation and report columns retained.
- `Report/RackStockOutput.cs`: wraps extended filter headers in Excel; paginates very long selected-rack captions into filter-detail pages shared by PDF, preview and printing. Ordinary short headers keep the existing layout. All selected names and data rows are retained.
- `Inventory.csproj`: uses the existing report source entry; no separate selector compile entry is required.
- `Tests/RackWiseProductStockReportHarness.cs`, `Tests/Test-RackWiseProductStockOutput.ps1`, `Tests/Test-RackWiseProductStockSql.ps1`: updated regression coverage.

Product search, zero-stock rules, rack/location grouping, UOM totals and menu integration retain their existing implementation.

## Validation

```powershell
& .\Inventory\Tests\Test-RackWiseProductStockSql.ps1
& .\Inventory\Tests\Test-RackWiseProductStockOutput.ps1
```

The SQL test temporarily installs the procedure in a transaction and always rolls it back. If the table type is absent, it creates the type before that transaction (SQL Server TVP metadata resolution requires a committed type) and drops that test-owned type after rollback. Existing types are retained. No stock rows are changed. Run on a test database since the procedure test holds schema locks.

Checks cover actual Structured TVP calls, multiple racks, empty selections, invalid IDs, positive/zero filters, uniqueness and equality with the original signed balances. UI checks cover case-insensitive substring matches, hidden checked items, unchecking one rack, Select All / Clear All while filtered, Reset, duplicate IDs and screen-edge popup positioning. The output harness continues to validate 30,005 records and small-window layouts; additional Excel/PDF fixtures use 1,500 selected captions to verify the final rack name and unchanged totals survive long headers.

The full Debug application build passed with the existing local dependencies. SQL tests passed against the configured local database and left no persistent changes. Independent XLSX/PDF readers confirmed all 1,500 selected names and fixture totals. Deployment to branch databases and physical printer acceptance remain rollout steps.
