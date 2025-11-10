-- =============================================
-- Migration: Add IsPublic column to UserProfiles
-- Description: Add visibility control for user profiles
-- Date: 2024-01-20
-- =============================================

USE EventLinkDB;
GO

-- Check if column exists before adding
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'UserProfiles' 
    AND COLUMN_NAME = 'IsPublic'
)
BEGIN
    PRINT 'Adding IsPublic column to UserProfiles...';
    
    ALTER TABLE UserProfiles
    ADD IsPublic BIT NULL DEFAULT 1;
    
    PRINT 'IsPublic column added successfully.';
    
    -- Update existing records to be public by default
    UPDATE UserProfiles
    SET IsPublic = 1
    WHERE IsPublic IS NULL;
    
    PRINT 'Updated existing profiles to public.';
END
ELSE
BEGIN
    PRINT 'IsPublic column already exists in UserProfiles.';
END
GO

-- Verify the change
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    IS_NULLABLE, 
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'UserProfiles'
AND COLUMN_NAME = 'IsPublic';
GO

PRINT 'Migration completed successfully!';
GO
