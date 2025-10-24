# Get the directory where this script is located
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $ScriptDir

# Helper function to wait for key press
function Wait-ForKey {
    Write-Host ""
    Write-Host "Press any key to continue..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}

# Helper function to check if a file exists and exit with error if not
function Test-RequiredFile {
    param (
        [string]$FilePath,
        [string]$Description
    )
    
    if (-not (Test-Path $FilePath)) {
        $FullPath = Join-Path $ScriptDir $FilePath
        Write-Host "$Description not found!" -ForegroundColor Red
        Write-Host "Please place the file at: $FullPath" -ForegroundColor Yellow
        Wait-ForKey
        exit 1
    }
    Write-Host "Found: $Description" -ForegroundColor Green
}

# Function to run CLI commands
function Invoke-PatcherCli {
    param (
        [string[]]$Arguments
    )
    
    & dotnet "Sewer56.Patcher.Riders.Cli.dll" @Arguments
}

Write-Host "=== SRDX Self-Contained Patch Generator ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "This script generates a self-contained patch bundle for SRDX." -ForegroundColor Gray
Write-Host "The bundle contains the full game and uses a hash check to verify" -ForegroundColor Gray
Write-Host "legal ownership of an existing Riders ROM." -ForegroundColor Gray
Write-Host ""

# Step 1: Check for required ISO file
Write-Host "Step 1: Checking for required ISO file..." -ForegroundColor Yellow
Test-RequiredFile "Mod\Sonic Riders (USA) (En,Ja,Fr,De,Es,It).iso" "Modified/SRDX ISO"
Write-Host ""

# Step 2: Create Assets folder if it doesn't exist
Write-Host "Step 2: Ensuring Assets folder exists..." -ForegroundColor Yellow
$AssetsDir = "Assets"
if (-not (Test-Path $AssetsDir)) {
    New-Item -Path $AssetsDir -ItemType Directory -Force | Out-Null
    Write-Host "Created Assets folder." -ForegroundColor Green
} else {
    Write-Host "Assets folder already exists." -ForegroundColor Green
}
Write-Host ""

# Step 3: Create self-contained bundle
Write-Host "Step 3: Creating self-contained bundle..." -ForegroundColor Yellow
Write-Host "This may take a while (4GB RAM required). Go watch an anime episode!" -ForegroundColor Gray
Write-Host ""

$BundlePath = "Assets\Bundle.patch"
$SourceIso = "Mod\Sonic Riders (USA) (En,Ja,Fr,De,Es,It).iso"
$XorKey = "LB2GsjDxia6Po08yC2GoUX8oD3bhDbh"

# Remove old bundle if it exists
if (Test-Path $BundlePath) {
    Write-Host "Removing old bundle..." -ForegroundColor Gray
    Remove-Item -Path $BundlePath -Force
}

Invoke-PatcherCli @("CreateSelfContained", "--file", $SourceIso, "--output", $BundlePath, "--key", $XorKey)

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "Failed to create self-contained bundle!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Make sure you built the patcher with DEV flag enabled:" -ForegroundColor Yellow
    Write-Host "  <DefineConstants>DEV</DefineConstants>" -ForegroundColor Cyan
    Write-Host ""
    Wait-ForKey
    exit 1
}

Write-Host ""
Write-Host "Self-contained bundle created successfully!" -ForegroundColor Green
Write-Host ""

# Step 4: Cleanup
Write-Host "Step 4: Cleaning up..." -ForegroundColor Yellow
Write-Host "About to remove MAKE_PATCH scripts and ISO files..." -ForegroundColor Gray
Write-Host ""
Write-Host "Press any key to continue with cleanup, or Ctrl+C to cancel..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
Write-Host ""

# Remove MAKE_PATCH scripts
Remove-Item -Path "MAKE_PATCH.bat" -Force -ErrorAction SilentlyContinue
Remove-Item -Path "MAKE_PATCH.sh" -Force -ErrorAction SilentlyContinue
Remove-Item -Path "MAKE_PATCH.ps1" -Force -ErrorAction SilentlyContinue

# Remove Mod folder with ISO
Remove-Item -Path "Mod" -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "Cleanup complete!" -ForegroundColor Green
Write-Host ""

# Display next steps
Write-Host "=== NEXT STEPS ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "Rebuild with <DefineConstants>`$(DefineConstants);SRDXSelfContained</DefineConstants>" -ForegroundColor Yellow
Write-Host "Replace all files in this folder with the built output." -ForegroundColor Gray
Write-Host ""
Write-Host "Bundle location: $BundlePath" -ForegroundColor Green
Write-Host ""

Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
