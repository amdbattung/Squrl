$ErrorActionPreference = "Stop"

$root = Split-Path $PSScriptRoot -Parent

$appProject = Join-Path $root "src\Squrl.App\Squrl.App.csproj"
$hostProject = Join-Path $root "src\Squrl.WindowsHost\Squrl.WindowsHost.csproj"

$publishRoot = Join-Path (Join-Path $root "publish") "windows"
$serverOutput = Join-Path $publishRoot "server"

# Inno Setup script
$installerScript = Join-Path $root "scripts\installer-windows.iss"

# Inno Setup compiler
$iscc = "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe"


Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Publishing Squrl" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""


# ----------------------------------------
# Get application version
# ----------------------------------------

Write-Host "Getting application version..." -ForegroundColor Yellow

$appVersion = (
    dotnet msbuild $appProject -getProperty:AppVersion
).Trim()

if ([string]::IsNullOrWhiteSpace($appVersion)) {
    throw "Could not determine AppVersion from MSBuild."
}

Write-Host "  Version: $appVersion" -ForegroundColor Green


# ----------------------------------------
# Clean previous publish
# ----------------------------------------

if (Test-Path $publishRoot) {
    Write-Host ""
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


# ----------------------------------------
# Build installer
# ----------------------------------------
$installerOutputDir = Join-Path $root "artifacts"

Write-Host ""
Write-Host "Building installer..." -ForegroundColor Yellow

if (-not (Test-Path $iscc)) {
    throw "Inno Setup compiler not found: $iscc"
}

if (-not (Test-Path $installerScript)) {
    throw "Inno Setup script not found: $installerScript"
}

& $iscc `
    "/DMyAppVersion=$appVersion" `
    "/DOutputDir=$installerOutputDir" `
    "/DSourceDir=$publishRoot" `
    $installerScript

if ($LASTEXITCODE -ne 0) {
    throw "Inno Setup compilation failed."
}


# ----------------------------------------
# Rename Squrl.App executable
# ----------------------------------------

Write-Host ""
Write-Host "Renaming Squrl.App executable..." -ForegroundColor Yellow

$appExe = Join-Path $serverOutput "Squrl.App.exe"
$appExeRenamed = Join-Path $serverOutput "squrl-server.exe"

if (-not (Test-Path $appExe)) {
    throw "Expected Squrl.App executable not found: $appExe"
}

if (Test-Path $appExeRenamed) {
    Remove-Item $appExeRenamed -Force
}

Rename-Item `
    -Path $appExe `
    -NewName "squrl-server.exe"

Write-Host "  $appExe" -ForegroundColor DarkGray
Write-Host "       ->" -ForegroundColor DarkGray
Write-Host "  $appExeRenamed" -ForegroundColor Green


# ----------------------------------------
# Rename Squrl.WindowsHost executable
# ----------------------------------------

Write-Host ""
Write-Host "Renaming Squrl.WindowsHost executable..." -ForegroundColor Yellow

$hostExe = Join-Path $publishRoot "Squrl.WindowsHost.exe"
$hostExeRenamed = Join-Path $publishRoot "squrl.exe"

if (-not (Test-Path $hostExe)) {
    throw "Expected Squrl.WindowsHost executable not found: $hostExe"
}

if (Test-Path $hostExeRenamed) {
    Remove-Item $hostExeRenamed -Force
}

Rename-Item `
    -Path $hostExe `
    -NewName "squrl.exe"

Write-Host "  $hostExe" -ForegroundColor DarkGray
Write-Host "       ->" -ForegroundColor DarkGray
Write-Host "  $hostExeRenamed" -ForegroundColor Green


# ----------------------------------------
# Done
# ----------------------------------------

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host " Publish completed successfully!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

Write-Host "Version:"
Write-Host "  $appVersion"
Write-Host ""

Write-Host "Output:"
Write-Host "  $publishRoot"
Write-Host ""

Write-Host "Executable:"
Write-Host "  $hostExeRenamed"
Write-Host ""

Write-Host "Server:"
Write-Host "  $appExeRenamed"
Write-Host ""
