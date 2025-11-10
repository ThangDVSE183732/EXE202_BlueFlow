# Apply Migration: ReplaceSharedNotesWithIsMark
# Replace SharedNotes (string) with IsMark (boolean) in Partnerships table

$connectionString = "Server=localhost;Database=EventLinkDB;Trusted_Connection=True;TrustServerCertificate=True;"

$sqlScript = @"
USE EventLinkDB;

BEGIN TRANSACTION;

-- Check if SharedNotes exists
IF EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.Partnerships') 
    AND name = 'SharedNotes'
)
BEGIN
    PRINT 'Dropping SharedNotes column...';
    
    ALTER TABLE [dbo].[Partnerships]
    DROP COLUMN [SharedNotes];
    
    PRINT 'SharedNotes column dropped successfully';
END
ELSE
BEGIN
    PRINT 'WARNING: SharedNotes column not found, skipping drop';
END

-- Check if IsMark already exists
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.Partnerships') 
    AND name = 'IsMark'
)
BEGIN
    PRINT 'Adding IsMark column...';
    
    ALTER TABLE [dbo].[Partnerships]
    ADD [IsMark] bit NULL DEFAULT 0;
    
    PRINT 'IsMark column added successfully';
END
ELSE
BEGIN
    PRINT 'WARNING: IsMark column already exists, skipping add';
END

-- Verify the changes
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.Partnerships') 
    AND name = 'SharedNotes'
)
AND EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.Partnerships') 
    AND name = 'IsMark'
)
BEGIN
    PRINT 'SUCCESS: Migration completed successfully!';
    PRINT '  - SharedNotes column removed';
    PRINT '  - IsMark column (bit) added';
END
ELSE
BEGIN
    PRINT 'ERROR: Migration verification failed';
    ROLLBACK TRANSACTION;
    RETURN;
END

COMMIT TRANSACTION;
PRINT 'Migration completed successfully!';
"@

try {
    Write-Host "Connecting to database..." -ForegroundColor Yellow
    
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
    Write-Host "  - Removed: SharedNotes (nvarchar(max))" -ForegroundColor White
    Write-Host "  - Added: IsMark (bit, nullable, default: false)" -ForegroundColor White
    
} catch {
    Write-Host "`n? Error applying migration:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}
