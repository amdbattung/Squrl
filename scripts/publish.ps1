$ErrorActionPreference = "Stop"

$root = Split-Path $PSScriptRoot -Parent

$appProject = Join-Path $root "src\Squrl.App\Squrl.App.csproj"
$hostProject = Join-Path $root "src\Squrl.WindowsHost\Squrl.WindowsHost.csproj"

$publishRoot = Join-Path $root "publish"
$serverOutput = Join-Path $publishRoot "Server"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Publishing Squrl" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Clean previous publish
if (Test-Path $publishRoot) {
    Write-Host "Cleaning $publishRoot..."
    Remove-Item $publishRoot -Recurse -Force
}

New-Item -ItemType Directory -Path $publishRoot | Out-Null
New-Item -ItemType Directory -Path $serverOutput | Out-Null

# ----------------------------------------
# Publish Squrl.App
# ----------------------------------------

Write-Host ""
Write-Host "Publishing Squrl.App..." -ForegroundColor Yellow

dotnet publish $appProject `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -o $serverOutput

if ($LASTEXITCODE -ne 0) {
    throw "Squrl.App publish failed."
}

# ----------------------------------------
# Publish WindowsHost
# ----------------------------------------

Write-Host ""
Write-Host "Publishing Squrl.WindowsHost..." -ForegroundColor Yellow

dotnet publish $hostProject `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -o $publishRoot

if ($LASTEXITCODE -ne 0) {
    throw "Squrl.WindowsHost publish failed."
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host " Publish completed successfully!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

Write-Host "Output:"
Write-Host "  $publishRoot"
Write-Host ""

Write-Host "WindowsHost:"
Write-Host "  $publishRoot\Squrl.WindowsHost.exe"
Write-Host ""

Write-Host "Squrl.App:"
Write-Host "  $serverOutput\Squrl.App.exe"
Write-Host ""