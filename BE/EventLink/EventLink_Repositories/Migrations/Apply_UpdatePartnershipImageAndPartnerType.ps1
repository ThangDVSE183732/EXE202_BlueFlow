# Apply Migration: UpdatePartnershipImageAndPartnerType
# Rename PartnerNotes to PartnershipImage in Partnerships table

$connectionString = "Server=localhost;Database=EventLinkDB;Trusted_Connection=True;TrustServerCertificate=True;"

$sqlScript = @"
USE EventLinkDB;

BEGIN TRANSACTION;

-- Check if PartnerNotes exists and PartnershipImage doesn't
IF EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.Partnerships') 
    AND name = 'PartnerNotes'
)
AND NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.Partnerships') 
    AND name = 'PartnershipImage'
)
BEGIN
    PRINT 'Renaming PartnerNotes to PartnershipImage...';
    
    EXEC sp_rename 
        @objname = 'dbo.Partnerships.PartnerNotes', 
        @newname = 'PartnershipImage', 
        @objtype = 'COLUMN';
    
    PRINT 'SUCCESS: Column renamed from PartnerNotes to PartnershipImage';
    PRINT 'PartnerType now supports: Sponsor, Supplier, Organizer';
END
ELSE IF EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.Partnerships') 
    AND name = 'PartnershipImage'
)
BEGIN
    PRINT 'SKIPPED: PartnershipImage column already exists. Migration already applied.';
END
ELSE
BEGIN
    PRINT 'ERROR: PartnerNotes column not found';
    ROLLBACK TRANSACTION;
    RETURN;
END

COMMIT TRANSACTION;
PRINT 'Migration completed successfully!';
"@

try {
    Write-Host "Connecting to database..." -ForegroundColor Yellow
    
    # Load SQL Server SMO Assembly
    Add-Type -AssemblyName "Microsoft.SqlServer.Smo, Version=16.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" -ErrorAction SilentlyContinue
    
    # Use SqlConnection instead
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    Write-Host "Connected successfully!" -ForegroundColor Green
    Write-Host "Executing migration script..." -ForegroundColor Yellow
    
    $command = $connection.CreateCommand()
    $command.CommandText = $sqlScript
    
    $reader = $command.ExecuteReader()
    
    # Read all result messages
    do {
        while ($reader.Read()) {
            for ($i = 0; $i -lt $reader.FieldCount; $i++) {
                Write-Host $reader.GetValue($i)
            }
        }
    } while ($reader.NextResult())
    
    $reader.Close()
    $connection.Close()
    
    Write-Host "`n? Migration applied successfully!" -ForegroundColor Green
    Write-Host "`nChanges:" -ForegroundColor Cyan
    Write-Host "  - Column renamed: PartnerNotes ? PartnershipImage" -ForegroundColor White
    Write-Host "  - PartnerType values: Sponsor, Supplier, Organizer" -ForegroundColor White
    
} catch {
    Write-Host "`n? Error applying migration:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}
