-- ============================================
-- Migration: UpdatePartnershipStatusValues
-- Date: 2024-12-01
-- Description: 
--   Update Partnership Status values
--   OLD: Pending, Accepted, Rejected, Completed, Cancelled
--   NEW: Pending, Ongoing, Completed, Cancelled
--   
--   Mapping:
--   - Accepted -> Ongoing
--   - Rejected -> Cancelled
--   - Others remain the same
-- ============================================

USE EventLinkDB;  -- Replace with your database name
GO

BEGIN TRANSACTION;

PRINT 'Starting Partnership Status migration...';
PRINT '';

-- ? Step 1: Update Accepted -> Ongoing
IF EXISTS (SELECT 1 FROM Partnerships WHERE Status = 'Accepted')
BEGIN
    UPDATE Partnerships
    SET Status = 'Ongoing',
        UpdatedAt = GETDATE()
    WHERE Status = 'Accepted';
    
    DECLARE @acceptedCount INT = @@ROWCOUNT;
    PRINT CONCAT('? Updated ', @acceptedCount, ' partnerships: Accepted -> Ongoing');
END
ELSE
BEGIN
    PRINT '??  No partnerships with status "Accepted" found';
END

-- ? Step 2: Update Rejected -> Cancelled
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
    
    DECLARE @rejectedCount INT = @@ROWCOUNT;
    PRINT CONCAT('? Updated ', @rejectedCount, ' partnerships: Rejected -> Cancelled');
END
ELSE
BEGIN
    PRINT '??  No partnerships with status "Rejected" found';
END

PRINT '';
PRINT '? Step 3: Verify status values';

-- Show current status distribution
SELECT 
    Status,
    COUNT(*) as Count
FROM Partnerships
GROUP BY Status
ORDER BY Status;

PRINT '';
PRINT '? Step 4: Check for invalid statuses';

-- Check if any invalid statuses remain
DECLARE @invalidCount INT;
SELECT @invalidCount = COUNT(*)
FROM Partnerships
WHERE Status NOT IN ('Pending', 'Ongoing', 'Completed', 'Cancelled')
  AND Status IS NOT NULL;

IF @invalidCount > 0
BEGIN
    PRINT CONCAT('??  WARNING: Found ', @invalidCount, ' partnerships with invalid status values');
    
    SELECT 
        Id,
        Status,
        PartnerType,
        CreatedAt
    FROM Partnerships
    WHERE Status NOT IN ('Pending', 'Ongoing', 'Completed', 'Cancelled')
      AND Status IS NOT NULL;
      
    -- Optionally set invalid statuses to Pending
    -- UPDATE Partnerships
    -- SET Status = 'Pending'
    -- WHERE Status NOT IN ('Pending', 'Ongoing', 'Completed', 'Cancelled')
    --   AND Status IS NOT NULL;
END
ELSE
BEGIN
    PRINT '? All partnership statuses are valid';
END

-- ? Commit transaction
COMMIT TRANSACTION;

PRINT '';
PRINT '========================================';
PRINT '? Migration completed successfully!';
PRINT 'New status values: Pending, Ongoing, Completed, Cancelled';
PRINT '========================================';

GO

-- ============================================
-- ROLLBACK SCRIPT (if needed)
-- ============================================
/*
USE EventLinkDB;
GO

BEGIN TRANSACTION;

PRINT 'Rolling back Partnership Status migration...';

-- Revert Ongoing -> Accepted
UPDATE Partnerships
SET Status = 'Accepted',
    UpdatedAt = GETDATE()
WHERE Status = 'Ongoing';

PRINT CONCAT('Reverted ', @@ROWCOUNT, ' partnerships: Ongoing -> Accepted');

-- Note: Cannot accurately revert Cancelled back to Rejected
-- as Cancelled might have been original or converted from Rejected
PRINT 'WARNING: Cannot accurately revert all Cancelled to Rejected';
PRINT 'Manual review may be needed for Cancelled partnerships';

COMMIT TRANSACTION;

PRINT 'Rollback completed';
*/
