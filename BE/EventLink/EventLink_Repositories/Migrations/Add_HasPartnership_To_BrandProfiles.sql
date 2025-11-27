-- =============================================
-- Migration: Add HasPartnership column to BrandProfiles
-- Description: Add indicator to track if brand has any partnerships
-- Date: 2024-01-20
-- =============================================

USE EventLinkDB;
GO

PRINT '========================================';
PRINT 'Migration: Add HasPartnership Column';
PRINT '========================================';
PRINT '';

-- =============================================
-- STEP 1: Add HasPartnership Column
-- =============================================
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'BrandProfiles' 
    AND COLUMN_NAME = 'HasPartnership'
)
BEGIN
    PRINT 'Step 1: Adding HasPartnership column to BrandProfiles...';
    
    ALTER TABLE BrandProfiles
    ADD HasPartnership BIT NULL;
    
    PRINT '? HasPartnership column added to BrandProfiles.';
    PRINT '';
    
    -- ? Update existing records to false (no partnership by default)
    UPDATE BrandProfiles
    SET HasPartnership = 0;
    
    PRINT '? Updated existing BrandProfiles to HasPartnership = false (0).';
    PRINT '';
    
    -- ? Add default constraint with 0 (false)
    ALTER TABLE BrandProfiles
    ADD CONSTRAINT DF_BrandProfiles_HasPartnership DEFAULT 0 FOR HasPartnership;
    
    PRINT '? Added default constraint (default = 0/false).';
    PRINT '';
END
ELSE
BEGIN
    PRINT '?? HasPartnership column already exists in BrandProfiles.';
    PRINT '';
END
GO

-- =============================================
-- STEP 2: Verify Changes
-- =============================================
PRINT '========================================';
PRINT 'Verification:';
PRINT '========================================';
PRINT '';

-- Check column info
PRINT 'BrandProfiles.HasPartnership column info:';
SELECT 
    COLUMN_NAME AS 'Column Name',
    DATA_TYPE AS 'Data Type',
    IS_NULLABLE AS 'Nullable',
    COLUMN_DEFAULT AS 'Default'
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'BrandProfiles' 
AND COLUMN_NAME = 'HasPartnership';
PRINT '';

-- Check statistics
PRINT 'Statistics:';
SELECT 
    COUNT(*) AS 'Total BrandProfiles',
    SUM(CASE WHEN HasPartnership = 1 THEN 1 ELSE 0 END) AS 'Has Partnership',
    SUM(CASE WHEN HasPartnership = 0 OR HasPartnership IS NULL THEN 1 ELSE 0 END) AS 'No Partnership'
FROM BrandProfiles;
PRINT '';

PRINT '========================================';
PRINT 'Migration Completed Successfully!';
PRINT '========================================';
PRINT '';
PRINT 'Notes:';
PRINT '  - HasPartnership column added with default value = false (0)';
PRINT '  - All existing BrandProfiles set to HasPartnership = false';
PRINT '  - Use this field to track if brand has any active partnerships';
PRINT '  - Update to true when partnership is created/accepted';
GO
