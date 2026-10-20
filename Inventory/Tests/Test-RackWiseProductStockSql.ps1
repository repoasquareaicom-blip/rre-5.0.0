param([string]$ConfigPath = (Join-Path $PSScriptRoot '..\App.config'))
$ErrorActionPreference = 'Stop'
[xml]$config = Get-Content -LiteralPath $ConfigPath
$connectionString = ($config.configuration.connectionStrings.add | Where-Object { $_.name -eq 'con' }).connectionString
$connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
$transaction = $null
$createdRackType = $false
function Assert-Report($condition, [string]$message) { if (!$condition) { throw $message } }
try {
    $connection.Open()
    # SQL Server resolves TVP metadata on a separate worker. Commit the type
    # before the test transaction, then remove it after rollback if we created it.
    $typeCommand = $connection.CreateCommand()
    $typeCommand.CommandText = "SELECT CASE WHEN TYPE_ID(N'dbo.RackStockReportRackIds') IS NULL THEN 0 ELSE 1 END"
    if ([int]$typeCommand.ExecuteScalar() -eq 0) {
        $typeCommand.CommandText = 'CREATE TYPE dbo.RackStockReportRackIds AS TABLE (RackId INT NOT NULL PRIMARY KEY)'
        [void]$typeCommand.ExecuteNonQuery()
        $createdRackType = $true
    }
    $typeCommand.Dispose()
    $transaction = $connection.BeginTransaction()
    # Validate deployment and query compilation against the configured schema.
    # Roll back even on success. Never commit procedure changes or stock data.
    $script = Get-Content -Raw -LiteralPath (Join-Path $PSScriptRoot '..\DatabaseScripts\2026_10_16_rack_wise_product_stock_report.sql')
    foreach ($batch in [regex]::Split($script, '(?im)^\s*GO\s*$')) {
        if ([string]::IsNullOrWhiteSpace($batch)) { continue }
        $command = $connection.CreateCommand()
        $command.Transaction = $transaction
        $command.CommandText = $batch
        $command.CommandTimeout = 180
        try { [void]$command.ExecuteNonQuery() } finally { $command.Dispose() }
    }
    function Query-Report([string]$procedure, $parameters) {
        $command = $connection.CreateCommand()
        $command.Transaction = $transaction
        $command.CommandText = $procedure
        $command.CommandType = [System.Data.CommandType]::StoredProcedure
        $command.CommandTimeout = 180
        foreach ($name in $parameters.Keys) {
            if ($name -eq '@RackIds') {
                $idsTable = New-Object System.Data.DataTable
                [void]$idsTable.Columns.Add('RackId', [int])
                foreach ($id in @($parameters[$name] | Select-Object -Unique)) { [void]$idsTable.Rows.Add([int]$id) }
                $parameter = $command.Parameters.Add('@RackIds', [System.Data.SqlDbType]::Structured)
                $parameter.TypeName = 'dbo.RackStockReportRackIds'
                $parameter.Value = $idsTable
            } else { [void]$command.Parameters.AddWithValue($name, $parameters[$name]) }
        }
        $adapter = New-Object System.Data.SqlClient.SqlDataAdapter($command)
        $table = New-Object System.Data.DataTable
        try { [void]$adapter.Fill($table); return ,$table } finally { $adapter.Dispose(); $command.Dispose() }
    }
    $watch = [Diagnostics.Stopwatch]::StartNew()
    $positive = Query-Report 'dbo.sp_RackWiseProductStockReport' @{ '@ShowZeroStock' = $false }
    $watch.Stop()
    Write-Output ("Positive report: {0} rows in {1} ms" -f $positive.Rows.Count, $watch.ElapsedMilliseconds)
    $zero = Query-Report 'dbo.sp_RackWiseProductStockReport' @{ '@ShowZeroStock' = $true }
    $keys = @{}
    foreach ($row in $zero.Rows) {
        $key = "$($row.RackId):$($row.ProductId)"
        Assert-Report (!$keys.ContainsKey($key)) "Duplicate product-rack row: $key"
        Assert-Report ([decimal]$row.CurrentStockQuantity -ge 0) 'Unexpected negative stock'
        $keys[$key] = [decimal]$row.CurrentStockQuantity
    }
    foreach ($row in $positive.Rows) {
        Assert-Report ([decimal]$row.CurrentStockQuantity -gt 0) 'Positive-only filter returned zero or negative stock'
        Assert-Report ($keys["$($row.RackId):$($row.ProductId)"] -eq [decimal]$row.CurrentStockQuantity) 'Zero toggle changed a positive balance'
    }
    # The legacy detail procedure is the regression oracle; combine locations for
    # each physical RackId, as GetStockRackWiseReport does. Exclude no-rack history.
    $ids = @($positive.Rows | Select-Object -ExpandProperty ProductId -Unique | Select-Object -First 8)
    foreach ($id in $ids) {
        $detail = Query-Report 'dbo.GetStockRackWiseDetail' @{ '@ProductId' = [int]$id }
        $expected = @{}
        foreach ($row in $detail.Rows) {
            if ($row.RackId -eq [DBNull]::Value) { continue }
            $rackKey = [int]$row.RackId
            if (!$expected.ContainsKey($rackKey)) { $expected[$rackKey] = [decimal]0 }
            $expected[$rackKey] += [decimal]$row.Quantity
        }
        foreach ($row in $positive.Rows | Where-Object { $_.ProductId -eq $id }) {
            Assert-Report ($expected[[int]$row.RackId] -eq [decimal]$row.CurrentStockQuantity) 'Balance differs from GetStockRackWiseDetail'
        }
    }
    if ($positive.Rows.Count -gt 0) {
        $rackId = [int]$positive.Rows[0].RackId
        $filtered = Query-Report 'dbo.sp_RackWiseProductStockReport' @{ '@RackId' = $rackId }
        Assert-Report ($filtered.Rows.Count -eq @($positive.Rows | Where-Object { $_.RackId -eq $rackId }).Count) 'Rack filter count mismatch'
        foreach ($row in $filtered.Rows) { Assert-Report ($row.RackId -eq $rackId) 'Rack filter leaked another rack' }
        $idSearch = [string]$positive.Rows[0].ProductId
        $searched = Query-Report 'dbo.sp_RackWiseProductStockReport' @{ '@ProductSearch' = $idSearch }
        Assert-Report (@($searched.Rows | Where-Object { $_.ProductId -eq [int]$idSearch }).Count -gt 0) 'Product ID search failed'
    }
    $selectedRackIds = @($zero.Rows | Select-Object -ExpandProperty RackId -Unique | Select-Object -First 3)
    Assert-Report ($selectedRackIds.Count -gt 1) 'Need at least two allocated racks for multi-rack SQL regression'
    foreach ($includeZero in @($false, $true)) {
        $multi = Query-Report 'dbo.sp_RackWiseProductStockReport' @{ '@RackIds' = $selectedRackIds; '@ShowZeroStock' = $includeZero }
        if ($includeZero) { $allRows = $zero } else { $allRows = $positive }
        $expectedRows = @($allRows.Rows | Where-Object { $selectedRackIds -contains [int]$_.RackId })
        Assert-Report ($multi.Rows.Count -eq $expectedRows.Count) 'Multi-rack result count mismatch'
        $seen = @{}
        foreach ($row in $multi.Rows) {
            Assert-Report ($selectedRackIds -contains [int]$row.RackId) 'Multi-rack filter leaked another rack'
            $key = "$($row.RackId):$($row.ProductId)"
            Assert-Report (!$seen.ContainsKey($key)) 'Duplicate multi-rack record'
            $seen[$key] = $true
            Assert-Report ($keys[$key] -eq [decimal]$row.CurrentStockQuantity) 'Multi-rack filter changed quantity'
        }
    }
    $explicitEmpty = Query-Report 'dbo.sp_RackWiseProductStockReport' @{ '@RackIds' = @() }
    Assert-Report ($explicitEmpty.Rows.Count -eq $positive.Rows.Count) 'Empty TVP must mean all racks'
    $invalid = Query-Report 'dbo.sp_RackWiseProductStockReport' @{ '@RackIds' = @([int]::MaxValue); '@ShowZeroStock' = $true }
    Assert-Report ($invalid.Rows.Count -eq 0) 'Unknown selected rack must not fall back to all racks'
    Write-Output 'PASS: actual Structured TVP, multiple racks, positive/zero balances, empty selection, invalid rack and unique rows.'
    $empty = Query-Report 'dbo.sp_RackWiseProductStockReport' @{ '@ProductSearch' = [guid]::NewGuid().ToString() }
    Assert-Report ($empty.Rows.Count -eq 0) 'Empty search returned records'
    $spaces = Query-Report 'dbo.sp_RackWiseProductStockReport' @{ '@ProductSearch' = '   ' }
    Assert-Report ($spaces.Rows.Count -eq $positive.Rows.Count) 'Blank search does not mean all products'
    $literal = Query-Report 'dbo.sp_RackWiseProductStockReport' @{ '@ProductSearch' = '%' }
    Write-Output ("PASS: deployment/query compilation, unique product-rack rows, positive/zero toggle ({0}/{1}), {2} legacy-detail comparisons, rack/product/empty/blank/literal wildcard queries." -f $positive.Rows.Count, $zero.Rows.Count, $ids.Count)
} finally {
    if ($transaction) { $transaction.Rollback(); $transaction.Dispose() }
    if ($createdRackType -and $connection.State -eq [System.Data.ConnectionState]::Open) {
        $cleanup = $connection.CreateCommand()
        $cleanup.CommandText = 'DROP TYPE dbo.RackStockReportRackIds'
        try { [void]$cleanup.ExecuteNonQuery() } finally { $cleanup.Dispose() }
    }
    $connection.Dispose()
    Write-Output 'Rolled back test deployment; no persistent database changes.'
}
