# Rack Wise Product Stock Report

For the searchable multi-rack enhancement, SQL upgrade and selection semantics, see [RackMultiSelect.md](RackMultiSelect.md).

## Deployment and access

1. Run `DatabaseScripts/2026_10_16_rack_wise_product_stock_report.sql` against each intended branch database using SSMS/sqlcmd. The script creates the report TVP type and creates/alters the report procedure; it does not modify existing procedures or stock records. The optional covering index is commented out for DBA review against existing indexes and actual query plans.
2. Build/deploy Inventory with its normal dependencies, including the .NET 3.5 `WindowsBase` framework assembly. Excel, ACE/Jet, and new NuGet packages are not required for this report.
3. Open **Reports > Rack Wise Product Stock Report**. Access follows the existing **StockReport** menu permission (including Admin). No new menu permission records are required.
4. Product Search accepts a literal partial Product ID, Item Code, Display Name, or Item Name. Press Enter or Search. Open Rack Search, type any caption/location substring, and check one or more racks. No selections means All Racks. Reset clears all filters and reloads.
5. Exports use the last successfully loaded report and its applied filter description. Changing a filter only affects output after Search succeeds. All result rows are exported, including rows outside the visible grid viewport.
6. Print first selects the printer, then opens preview; use the preview toolbar's Print button to send the complete report to that printer. A4 landscape is selected when supported by that printer.

## Verified stock source and behavior

The configured local database was inspected through `sys.columns` and `OBJECT_DEFINITION`:

- `MaterialTranscation.MaterailId` and `RackId` are INT; Quantity is DECIMAL(18,3); LocationId is VARCHAR. The historical spelling `MaterialTranscation` / `MaterailId` is intentional.
- `ProductMaster.id` is INT; names, ItemCode, UOM and IsDeleted are VARCHAR. `UOM.Uomid` resolves stored UOM identifiers to captions, following the existing rack template/allocation screens.
- `RackMaster.LocationId` identifies each rack's location. `ProductRackMapping` is a many-to-many allocation table, with an existing unique ProductId/RackId index.
- `ProductMaster.RackWiseStockMovement` is a BIT configuration field, not a ledger table. The report does not exclude stock merely because that setting has changed.
- `GetStockRackWiseDetail` sums IN minus OUT from MaterialTranscation by product/location/rack. The deployed `GetStockRackWiseReport` aggregates physical rack stock using RackMaster.LocationId. The new procedure uses the same signed ledger quantities and returns one row per physical product/rack. It does not join original sales/PDI/return/note tables or apply their quantities a second time.
- Opening stock, posted purchases, sales/PDI, reversals/sales returns, notes and stock-upload adjustments flow through that ledger. Unposted source documents are not additional stock. Existing scripts inspected include purchase receipt, PDI/reverse PDI, sales return, delivery/receipt note, material movement, warranty, and rack stock upload scripts.
- NULL RackId history is excluded. Real positive rack balances remain visible even when the allocation was later removed or the rack deactivated. Zero balances are included only for existing active rack/product/location allocations; no all-products/all-racks cross join is used. Negative balances are excluded in both modes, as requested by the positive/zero filters.
- Duplicate mappings cannot multiply movements: allocations are deduplicated after aggregation. Separate ledger postings are counted separately, even if their values look identical; the report cannot determine that a posted transaction is erroneous. Duplicate ProductMaster IDs are collapsed deterministically to one master record.
- Totals are grouped by normalized UOM. Missing UOMs have separate total buckets per product, so unrelated unknown units are not added together. Product and rack counts exclude headings/subtotals.

## Files

New production files:

- `DatabaseScripts/2026_10_16_rack_wise_product_stock_report.sql`: parameterized, set-based report procedure, literal partial search and optional index guidance.
- `Report/RackWiseProductStockReport.cs`: background loading/export, filter handling, shared immutable-after-load report snapshot, typed DataTable binding, counts and per-UOM summaries.
- `Report/RackWiseProductStockReport.Designer.cs`: wrapping filters/toolbars, scrolling read-only grid, numeric formatting and highlighted groups/totals.
- `Report/RackStockOutput.cs`: Office-free XLSX, landscape PDF, shared complete-row pagination and direct print drawing.

Modified production files:

- `FrmMain.cs`: Reports menu entry using existing StockReport permission and single-instance MDI activation.
- `Inventory.csproj`: compile entries and WindowsBase reference.
- `Tool/SimplePdf.cs`: optional page dimensions; its default constructor retains the existing portrait dimensions and behavior for other reports.

Validation/support files:

- `Tests/RackWiseProductStockReportHarness.cs`
- `Tests/Test-RackWiseProductStockOutput.ps1`
- `Tests/Test-RackWiseProductStockSql.ps1`
- This document.

Existing unrelated working-tree changes were retained.

## Automated validation

Run from the repository root in Windows PowerShell:

```powershell
& .\Inventory\Tests\Test-RackWiseProductStockSql.ps1
& .\Inventory\Tests\Test-RackWiseProductStockOutput.ps1
```

The SQL check uses the configured `con` connection in App.config (or `-ConfigPath`). It creates/alters the procedure **inside a transaction and always rolls back**, so the test does not deploy the feature. An absent TVP type is temporarily created before the transaction and removed after rollback; existing types are retained. It checks actual schema compatibility, duplicate rows, positive/zero filters, selected-rack filtering, Product ID search, empty/blank/literal wildcard searches, and balances against the existing GetStockRackWiseDetail procedure. Run on a test database: its transaction holds schema locks until completion.

The output harness compiles production report sources against .NET 3.5, tests multiple products per rack and a product in multiple racks, zero rows, fractional/per-UOM totals and empty results, then exercises 30,005 detail records. It checks complete pagination with no orphan rack headings, creates XLSX/PDF outputs and print/layout images, and checks controls at 1366, 800 and 640 pixel client widths without opening an interactive test window.

Completed in this workspace:

- Full Debug application build succeeded using existing `Inventory/bin/Debug` dependencies via MSBuild ReferencePath; pre-existing warnings remain.
- SQL validation passed: 3 positive rows, 72 rows with zero inclusion, 3 sampled product comparisons against the existing detail procedure; all schema/procedure changes rolled back.
- 30,005-row snapshot generated in approximately 85 ms on this machine (not an end-to-end database or client performance guarantee).
- Output harness passed. Independent openpyxl inspection confirmed valid XLSX, numeric cells, formatting, all 30,005 details and exact fixture totals (NOS 65.500 / MTR 370.375). pypdf inspection confirmed a 9-page A4 landscape PDF with repeated headings and matching totals. Small/large UI layout images and shared print rendering were inspected.

## Manual acceptance checks on a staging branch

1. Open at 1366x768, then resize down to 800 and 640 pixels wide. Ensure controls wrap, all buttons remain available, scrolling works, and quantities are right-aligned with up to three decimals.
2. Pick a rack with multiple products and a product allocated to two racks. Compare its rack balances with the existing Stock Report's rack details. Search independently using partial ID, ItemCode, DisplayName, and ItemName, including literal `%`, `_`, `[` and `~` characters.
3. Select a rack; verify every detail belongs to it. Toggle Show Zero Stock: only valid allocations may introduce zero rows. Ensure products with no rack allocation/history do not appear under an invented rack. Verify empty search results and Reset.
4. Use normal staging workflows to post fractional opening stock, purchase/receipt IN, stock OUT, PDI, sales return/reverse PDI, delivery OUT, receipt-note IN and applicable adjustments. Refresh this report and the existing report after each committed transaction; both must change by the same signed amount. Check that sale/PDI or source note plus ledger posting is counted once. Do not run these posting checks against production data.
5. Verify duplicate allocations do not multiply rows; valid repeated ledger postings remain part of the balance. Verify mixed UOMs get separate rack/grand totals and unknown UOMs remain separate per product.
6. Export XLSX/PDF and preview/print a multi-page report. Compare detail counts, filter captions, zero rows and each UOM total with the grid. Confirm continuation rack headings and page numbers, readable long names, and that quantities remain inside the page. Change filters without pressing Search and confirm output still describes the loaded snapshot.
7. Test Admin, a StockReport-authorized user, and a user without StockReport access. Test database failures and locked export files; the screen should recover and retain the last successfully loaded snapshot. Confirm closing the form during background work does not produce an error dialog.

Physical printer output, real staging transaction posting, and production-scale database query plans remain deployment acceptance checks. PDF text follows the existing SimplePdf ASCII font behavior; non-ASCII characters are substituted. XLSX and Windows printing preserve Unicode. If branch product names require non-ASCII PDF text, add an embedded Unicode font capability before rollout to that branch.
