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

Write-Host "=== SRTE Patch Generator ===" -ForegroundColor Cyan
Write-Host ""

# Step 1: Check for required ISO files
Write-Host "Step 1: Checking for required ISO files..." -ForegroundColor Yellow
Test-RequiredFile "Original\Sonic Riders (USA) (En,Ja,Fr,De,Es,It).iso" "Original Sonic Riders NTSC-U ISO"
Test-RequiredFile "Mod\Sonic Riders (USA) (En,Ja,Fr,De,Es,It).iso" "Modified/SRTE ISO"
Write-Host ""

# Step 2: Check for Assets folder
Write-Host "Step 2: Checking for Assets folder..." -ForegroundColor Yellow
Test-RequiredFile "Assets\TE\Hashes\Original_ROM\hashes.json" "Original ROM hashes"
Write-Host ""

# Step 3: Verify original ROM
Write-Host "Step 3: Verifying original ROM..." -ForegroundColor Yellow
Write-Host "This ensures you have a good dump (e.g., from Vimm's Lair)" -ForegroundColor Gray

Invoke-PatcherCli @("VerifyHashes", "--tgt", "Original", "--hash", "Assets\TE\Hashes\Original_ROM")

if ($LASTEXITCODE -ne 0) {
    Write-Host "Original ROM verification FAILED!" -ForegroundColor Red
    Write-Host "Please ensure you have a valid NTSC-U Sonic Riders ISO." -ForegroundColor Yellow
    Wait-ForKey
    exit 1
}

Write-Host "Original ROM verified successfully!" -ForegroundColor Green
Write-Host ""

# Step 4: Generate hashes for MOD.iso
Write-Host "Step 4: Generating hashes for MOD.iso..." -ForegroundColor Yellow
$ModHashesDir = "Assets\TE\Hashes\TE_ROM"
if (-not (Test-Path $ModHashesDir)) {
    New-Item -Path $ModHashesDir -ItemType Directory -Force | Out-Null
}

Invoke-PatcherCli @("GenerateHashes", "--src", "Mod", "--tgt", $ModHashesDir)

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to generate hashes for MOD.iso!" -ForegroundColor Red
    Wait-ForKey
    exit 1
}

Write-Host "Hashes generated successfully!" -ForegroundColor Green
Write-Host ""

# Step 5: Generate patch from Original to MOD
Write-Host "Step 5: Generating patch from Original to MOD..." -ForegroundColor Yellow
Write-Host "This may take a while (6GB RAM required). Go watch an anime episode!" -ForegroundColor Gray
Write-Host "Give it 15-20 minutes. No, it's not stuck." -ForegroundColor Gray
$PatchDir = "Assets\TE\Patches\Vanilla_to_TE\Patch"
if (-not (Test-Path $PatchDir)) {
    New-Item -Path $PatchDir -ItemType Directory -Force | Out-Null
}

# Clear existing patch files
if (Test-Path $PatchDir) {
    Get-ChildItem -Path $PatchDir -File | Remove-Item -Force
}

Invoke-PatcherCli @("GeneratePatch", "--src", "Original", "--tgt", "Mod", "--out", $PatchDir)

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to generate patch!" -ForegroundColor Red
    Wait-ForKey
    exit 1
}

Write-Host "Patch generated successfully!" -ForegroundColor Green
Write-Host ""

Write-Host "=== PATCH GENERATION COMPLETE ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "Generated files:" -ForegroundColor Yellow
Write-Host "  - Hashes: $ModHashesDir\hashes.json" -ForegroundColor Gray
Write-Host "  - Patch: $PatchDir" -ForegroundColor Gray
Write-Host ""

# Step 6: Cleanup
Write-Host "Complete! Press any key to clean up..." -ForegroundColor Yellow
Write-Host ""
Write-Host "The following files will be deleted:" -ForegroundColor Red
Write-Host "  - Original\Sonic Riders (USA) (En,Ja,Fr,De,Es,It).iso" -ForegroundColor Gray
Write-Host "  - Mod\Sonic Riders (USA) (En,Ja,Fr,De,Es,It).iso" -ForegroundColor Gray
Write-Host "  - MAKE_PATCH.bat" -ForegroundColor Gray
Write-Host "  - MAKE_PATCH.ps1" -ForegroundColor Gray
Write-Host "  - MAKE_PATCH.sh" -ForegroundColor Gray
Write-Host ""
Wait-ForKey

Write-Host "Cleaning up..." -ForegroundColor Yellow

# Delete ROM files
$FilesToDelete = @(
    "Original\Sonic Riders (USA) (En,Ja,Fr,De,Es,It).iso",
    "Mod\Sonic Riders (USA) (En,Ja,Fr,De,Es,It).iso",
    "MAKE_PATCH.bat",
    "MAKE_PATCH.ps1",
    "MAKE_PATCH.sh"
)

foreach ($File in $FilesToDelete) {
    if (Test-Path $File) {
        Remove-Item $File -Force
        Write-Host "Deleted: $File" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "Cleanup complete! Package is ready for upload." -ForegroundColor Cyan