# Apply Migration: UpdatePartnershipStatusValues
# Update Partnership status values: Accepted->Ongoing, Rejected->Cancelled

$connectionString = "Server=localhost;Database=EventLinkDB;Trusted_Connection=True;TrustServerCertificate=True;"

$sqlScript = @"
USE EventLinkDB;

BEGIN TRANSACTION;

PRINT 'Starting Partnership Status migration...';
PRINT '';

-- Update Accepted -> Ongoing
IF EXISTS (SELECT 1 FROM Partnerships WHERE Status = 'Accepted')
BEGIN
    UPDATE Partnerships
    SET Status = 'Ongoing',
        UpdatedAt = GETDATE()
    WHERE Status = 'Accepted';
    
    PRINT 'Updated partnerships: Accepted -> Ongoing';
END

-- Update Rejected -> Cancelled
IF EXISTS (SELECT 1 FROM Partnerships WHERE Status = 'Rejected')
BEGIN
    UPDATE Partnerships
    SET Status = 'Cancelled',
        UpdatedAt = GETDATE(),
        OrganizerResponse = CASE 
            WHEN OrganizerResponse IS NULL OR OrganizerResponse = '' 
            THEN 'Partnership rejected' 
            ELSE OrganizerResponse 
        END
    WHERE Status = 'Rejected';
    
    PRINT 'Updated partnerships: Rejected -> Cancelled';
END

PRINT '';
PRINT 'Verifying status values...';

-- Show current status distribution
SELECT 
    Status,
    COUNT(*) as Count
FROM Partnerships
GROUP BY Status
ORDER BY Status;

-- Check for invalid statuses
DECLARE @invalidCount INT;
SELECT @invalidCount = COUNT(*)
FROM Partnerships
WHERE Status NOT IN ('Pending', 'Ongoing', 'Completed', 'Cancelled')
  AND Status IS NOT NULL;

IF @invalidCount > 0
BEGIN
    PRINT 'WARNING: Found invalid status values';
END
ELSE
BEGIN
    PRINT 'All partnership statuses are valid';
END

COMMIT TRANSACTION;

PRINT '';
PRINT 'Migration completed successfully!';
PRINT 'New status values: Pending, Ongoing, Completed, Cancelled';
"@

try {
    Write-Host "Connecting to database..." -ForegroundColor Yellow
    
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    Write-Host "Connected successfully!" -ForegroundColor Green
    Write-Host "Executing migration script..." -ForegroundColor Yellow
    Write-Host ""
    
    $command = $connection.CreateCommand()
    $command.CommandText = $sqlScript
    $command.CommandTimeout = 60
    
    $reader = $command.ExecuteReader()
    
    # Read all result messages
    do {
        while ($reader.Read()) {
            for ($i = 0; $i -lt $reader.FieldCount; $i++) {
                $value = $reader.GetValue($i)
                if ($null -ne $value) {
                    Write-Host $value
                }
            }
        }
    } while ($reader.NextResult())
    
    $reader.Close()
    $connection.Close()
    
    Write-Host ""
    Write-Host "? Migration applied successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Status Changes:" -ForegroundColor Cyan
    Write-Host "  Accepted  -> Ongoing    ?" -ForegroundColor White
    Write-Host "  Rejected  -> Cancelled  ?" -ForegroundColor White
    Write-Host "  Pending   -> Pending    (no change)" -ForegroundColor Gray
    Write-Host "  Completed -> Completed  (no change)" -ForegroundColor Gray
    Write-Host "  Cancelled -> Cancelled  (no change)" -ForegroundColor Gray
    
} catch {
    Write-Host ""
    Write-Host "? Error applying migration:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}
