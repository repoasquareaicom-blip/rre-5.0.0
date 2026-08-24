param(
    [string]$DeploymentKey = $env:RRE_BRANCH_DEPLOYMENT_KEY,
    [string]$ScriptPath = ".\Inventory\DatabaseScripts\2026_08_18_deploy_proc_product_sales_analysis.sql",
    [string]$DeploymentId = "2026_08_22_proc_product_sales_analysis_detail",
    [string[]]$BranchUrls = @(
        "https://salem.rreconnect.in",
        "https://namakkal.rreconnect.in",
        "https://kolathur.rreconnect.in"
    )
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($DeploymentKey)) {
    throw "Deployment key is required. Set RRE_BRANCH_DEPLOYMENT_KEY or pass -DeploymentKey."
}

$resolvedScriptPath = Resolve-Path -LiteralPath $ScriptPath
$script = Get-Content -LiteralPath $resolvedScriptPath -Raw

$body = @{
    deploymentId = $DeploymentId
    script = $script
} | ConvertTo-Json -Depth 4

foreach ($branchUrl in $BranchUrls) {
    $url = "$($branchUrl.TrimEnd('/'))/api/admin/deployscript"
    Write-Host "Deploying $DeploymentId to $url"

    try {
        $response = Invoke-RestMethod `
            -Uri $url `
            -Method Post `
            -ContentType "application/json" `
            -Headers @{ "X-Deployment-Key" = $DeploymentKey } `
            -Body $body

        Write-Host "  Success: $($response.message)"
    }
    catch {
        Write-Host "  Failed: $($_.Exception.Message)" -ForegroundColor Red
    }
}
