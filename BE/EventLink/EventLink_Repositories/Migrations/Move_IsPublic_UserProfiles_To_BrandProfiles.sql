-- =============================================
-- Migration: Move IsPublic from UserProfiles to BrandProfiles
-- Description: Remove IsPublic from UserProfiles and add it to BrandProfiles
-- Date: 2024-01-20
-- =============================================

USE EventLinkDB;
GO

PRINT '========================================';
PRINT 'Migration: Move IsPublic Column';
PRINT '========================================';
PRINT '';

-- =============================================
-- STEP 1: Add IsPublic to BrandProfiles
-- =============================================
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'BrandProfiles' 
    AND COLUMN_NAME = 'IsPublic'
)
BEGIN
    PRINT 'Step 1: Adding IsPublic column to BrandProfiles...';
    
    ALTER TABLE BrandProfiles
    ADD IsPublic BIT NULL;
    
    PRINT '? IsPublic column added to BrandProfiles.';
    
    -- Update existing records to be private by default (0 = false)
    UPDATE BrandProfiles
    SET IsPublic = 0;
    
    PRINT '? Updated existing BrandProfiles to private (IsPublic = 0).';
    
    -- Add default constraint with 0 (false/private)
    ALTER TABLE BrandProfiles
    ADD CONSTRAINT DF_BrandProfiles_IsPublic DEFAULT 0 FOR IsPublic;
    
    PRINT '? Added default constraint (default = 0/private).';
    PRINT '';
END
ELSE
BEGIN
    PRINT '?? IsPublic column already exists in BrandProfiles.';
    PRINT '';
END

-- =============================================
-- STEP 2: Remove IsPublic from UserProfiles
-- =============================================
IF EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'UserProfiles' 
    AND COLUMN_NAME = 'IsPublic'
)
BEGIN
    PRINT 'Step 2: Removing IsPublic column from UserProfiles...';
    
    -- Check for default constraint first
    DECLARE @ConstraintName NVARCHAR(200);
    SELECT @ConstraintName = dc.name
    FROM sys.default_constraints dc
    INNER JOIN sys.columns c ON dc.parent_column_id = c.column_id
    INNER JOIN sys.tables t ON dc.parent_object_id = t.object_id
    WHERE t.name = 'UserProfiles' 
    AND c.name = 'IsPublic';

    -- Drop default constraint if exists
    IF @ConstraintName IS NOT NULL
    BEGIN
        DECLARE @DropConstraintSQL NVARCHAR(500);
        SET @DropConstraintSQL = 'ALTER TABLE UserProfiles DROP CONSTRAINT ' + @ConstraintName;
        EXEC sp_executesql @DropConstraintSQL;
        PRINT '? Dropped default constraint: ' + @ConstraintName;
    END

    -- Drop the column
    ALTER TABLE UserProfiles
    DROP COLUMN IsPublic;
    
    PRINT '? IsPublic column removed from UserProfiles.';
    PRINT '';
END
ELSE
BEGIN
    PRINT '?? IsPublic column does not exist in UserProfiles.';
    PRINT '';
END

-- =============================================
-- STEP 3: Verify Changes
-- =============================================
PRINT '========================================';
PRINT 'Verification:';
PRINT '========================================';

-- Check BrandProfiles
PRINT 'BrandProfiles.IsPublic:';
IF EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'BrandProfiles' AND COLUMN_NAME = 'IsPublic'
)
BEGIN
    PRINT '  ? Column exists';
    SELECT 
        '  ' + COLUMN_NAME AS 'Column Name', 
        DATA_TYPE AS 'Data Type', 
        IS_NULLABLE AS 'Nullable', 
        COLUMN_DEFAULT AS 'Default'
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'BrandProfiles' AND COLUMN_NAME = 'IsPublic';
END
ELSE
    PRINT '  ? Column does NOT exist';

PRINT '';

-- Check UserProfiles
PRINT 'UserProfiles.IsPublic:';
IF EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'UserProfiles' AND COLUMN_NAME = 'IsPublic'
)
BEGIN
    PRINT '  ? Column still exists (should be removed)';
END
ELSE
    PRINT '  ? Column removed successfully';

PRINT '';
PRINT '========================================';
PRINT 'Migration Completed Successfully!';
PRINT '========================================';
GO
