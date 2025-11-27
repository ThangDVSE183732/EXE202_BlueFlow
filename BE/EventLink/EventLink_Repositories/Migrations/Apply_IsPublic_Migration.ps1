# =============================================
# Apply Migration: Add IsPublic to UserProfiles
# =============================================

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$migrationFile = Join-Path $scriptPath "Add_IsPublic_To_UserProfiles.sql"

# Database connection (from .env)
$server = "localhost"
$database = "EventLinkDB"
$trustedConnection = "True"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Apply Migration: Add IsPublic Column" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if migration file exists
if (-Not (Test-Path $migrationFile)) {
    Write-Host "ERROR: Migration file not found: $migrationFile" -ForegroundColor Red
    exit 1
}

Write-Host "Migration file: $migrationFile" -ForegroundColor Yellow
Write-Host "Database: $database on $server" -ForegroundColor Yellow
Write-Host ""

# Confirm before applying
$confirmation = Read-Host "Do you want to apply this migration? (Y/N)"
if ($confirmation -ne 'Y' -and $confirmation -ne 'y') {
    Write-Host "Migration cancelled." -ForegroundColor Yellow
    exit 0
}

try {
    Write-Host "Applying migration..." -ForegroundColor Green
    
    # Execute SQL file using sqlcmd
    sqlcmd -S $server -d $database -E -i $migrationFile
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Green
        Write-Host " Migration Applied Successfully!" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Green
        Write-Host ""
        Write-Host "Changes:" -ForegroundColor Cyan
        Write-Host "  - Added IsPublic column to UserProfiles table" -ForegroundColor White
        Write-Host "  - Default value: 1 (true - public)" -ForegroundColor White
        Write-Host "  - Existing records updated to public" -ForegroundColor White
    } else {
        Write-Host ""
        Write-Host "Migration failed with exit code: $LASTEXITCODE" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host ""
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
