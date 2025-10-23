-- Migration: Make CompanyLogoUrl nullable in UserProfiles table
USE EventLinkDB;
GO

PRINT 'Making CompanyLogoUrl nullable...';

-- Check if column exists and is NOT NULL
IF EXISTS (
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'UserProfiles' 
    AND COLUMN_NAME = 'CompanyLogoUrl'
    AND IS_NULLABLE = 'NO'
)
BEGIN
    -- Alter column to allow NULL
    ALTER TABLE UserProfiles 
    ALTER COLUMN CompanyLogoUrl NVARCHAR(500) NULL;
    
    PRINT '? CompanyLogoUrl is now nullable';
END
ELSE
BEGIN
    PRINT '?? CompanyLogoUrl is already nullable or does not exist';
END

PRINT '';
PRINT '? Migration completed successfully!';
GO
