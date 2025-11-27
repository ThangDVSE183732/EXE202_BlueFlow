-- ============================================
-- Migration: UpdatePartnershipImageAndPartnerType
-- Date: 2024-12-01
-- Description: 
--   1. Rename column PartnerNotes to PartnershipImage
--   2. Add 'Organizer' to PartnerType enum (no schema change needed)
-- ============================================

USE EventLinkDB;  -- Replace with your database name
GO

BEGIN TRANSACTION;

-- ? Step 1: Rename PartnerNotes to PartnershipImage
EXEC sp_rename 
    @objname = 'dbo.Partnerships.PartnerNotes', 
    @newname = 'PartnershipImage', 
    @objtype = 'COLUMN';

-- ? Step 2: Add comment about PartnerType enum values
-- Note: PartnerType is nvarchar(20), so 'Organizer' is already supported
-- No schema change needed - just update application code to use new value
PRINT 'PartnerType now supports: Sponsor, Supplier, Organizer';

-- ? Step 3: Verify the change
IF EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.Partnerships') 
    AND name = 'PartnershipImage'
)
BEGIN
    PRINT 'SUCCESS: Column renamed from PartnerNotes to PartnershipImage';
END
ELSE
BEGIN
    PRINT 'ERROR: Column rename failed';
    ROLLBACK TRANSACTION;
    RETURN;
END

-- ? Commit transaction
COMMIT TRANSACTION;

PRINT 'Migration completed successfully!';

-- ============================================
-- ROLLBACK SCRIPT (if needed)
-- ============================================
/*
BEGIN TRANSACTION;

EXEC sp_rename 
    @objname = 'dbo.Partnerships.PartnershipImage', 
    @newname = 'PartnerNotes', 
    @objtype = 'COLUMN';

COMMIT TRANSACTION;
PRINT 'Rollback completed: PartnershipImage renamed back to PartnerNotes';
*/
