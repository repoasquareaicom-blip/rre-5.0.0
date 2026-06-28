# RR Backend API (Laravel)

## Setup

1. Copy `.env.example` to `.env` and set MySQL:

```
DB_CONNECTION=mysql
DB_DATABASE=rr_monitor
```

2. `php artisan migrate`
3. `php artisan db:seed`
4. `php artisan serve`

## API overview

### Monitor (admin UI)

- `GET /api/monitor/dashboard?from=&to=&branch_id=`
- `GET /api/monitor/quotations|estimations|sales|vouchers`
- `GET /api/monitor/settlements/summary`

### Sync (branch service)

Headers: `X-Branch-Code`, `X-Api-Key` (branch is stored in cloud by header — not in MS SQL)

- `GET /api/sync/last-sync-on` — five fields: `quotation_last_sync_on`, `estimation_last_sync_on`, `sales_last_sync_on`, `sales_pipes_last_sync_on`, `sales_traders_last_sync_on` (from max `Updatedon` / `EnteredOn` in each header table for the branch)
- `GET /api/sync/cursor/{entity}` — `branch_code` + the matching `*_last_sync_on` field for one entity
- `POST /api/sync/quotations` — `QuotationHeader` + `QuotationDetails` (MS SQL column names)
- `POST /api/sync/estimations` — `QuotationEstimation` + `QuotationEstimationDetails`
- `POST /api/sync/sales` — `Sales` + `SalesDetails`
- `POST /api/sync/sales-pipes` — `SalesPipes` + `SalesPipesDetails`
- `POST /api/sync/sales-traders` — `SalesTraders` + `SalesTradersDetails`
- `POST /api/sync/vouchers` / `receipts` / `settlements` — legacy monitor tables (`external_id` + `source_updated_at`)
- `GET /api/sync/status` — includes `inbound_logs` (last 30 API calls with `status`: `success`, `failed`, `validation_error`, `unauthorized`)

Mirror POST body (same as quotations):

```json
{
  "records": [
    {
      "header": { "...": "MS SQL header columns" },
      "details": [ { "...": "line columns" } ]
    }
  ]
}
```

Headers upsert by **`branch_id` + business id** (`Quotationid`, `Estimationid`, or `Salesid`). Detail lines with `sino` upsert per line; sales detail tables replace all lines for that `Salesid` on each sync. Responses include `headers_created`, `headers_updated`, `lines_upserted`, and the matching `*_last_sync_on` field.
