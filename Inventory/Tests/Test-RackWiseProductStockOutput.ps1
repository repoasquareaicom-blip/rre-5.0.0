param([string]$OutputDirectory)
$ErrorActionPreference = 'Stop'
$inventory = Split-Path $PSScriptRoot -Parent
$root = Split-Path $inventory -Parent
if (!$OutputDirectory) { $OutputDirectory = Join-Path $root '.codex\tmp\rack-report-qa' }
[void][IO.Directory]::CreateDirectory($OutputDirectory)
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v3.5\csc.exe'
$packaging = Join-Path ${env:ProgramFiles(x86)} 'Reference Assemblies\Microsoft\Framework\v3.0\WindowsBase.dll'
if (!(Test-Path $packaging)) { $packaging = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v3.0\WPF\WindowsBase.dll' }
$executable = Join-Path $OutputDirectory 'RackStockHarness.exe'
$sources = @('Tests\RackWiseProductStockReportHarness.cs', 'Report\RackWiseProductStockReport.cs', 'Report\RackWiseProductStockReport.Designer.cs', 'Report\RackStockOutput.cs', 'Tool\SimplePdf.cs') | ForEach-Object { Join-Path $inventory $_ }
& $compiler /nologo /target:exe "/out:$executable" /r:System.dll /r:System.Core.dll /r:System.Data.dll /r:System.Drawing.dll /r:System.Windows.Forms.dll /r:System.Xml.dll "/r:$packaging" $sources
if ($LASTEXITCODE -ne 0) { throw 'Regression harness compilation failed' }
& $executable $OutputDirectory
if ($LASTEXITCODE -ne 0) { throw 'Regression harness failed' }
Write-Output "QA workbooks, PDFs and layout/print images: $OutputDirectory"
