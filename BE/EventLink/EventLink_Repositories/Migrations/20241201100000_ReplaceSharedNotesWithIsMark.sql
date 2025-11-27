-- ============================================
-- Migration: ReplaceSharedNotesWithIsMark
-- Date: 2024-12-01
-- Description: 
--   Replace SharedNotes (nvarchar) with IsMark (bit) in Partnerships table
-- ============================================

USE EventLinkDB;  -- Replace with your database name
GO

BEGIN TRANSACTION;

-- ? Step 1: Check if SharedNotes exists
IF EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.Partnerships') 
    AND name = 'SharedNotes'
)
BEGIN
    PRINT 'Dropping SharedNotes column...';
    
    -- Drop SharedNotes column
    ALTER TABLE [dbo].[Partnerships]
    DROP COLUMN [SharedNotes];
    
    PRINT 'SharedNotes column dropped successfully';
END
ELSE
BEGIN
    PRINT 'WARNING: SharedNotes column not found, skipping drop';
END

-- ? Step 2: Check if IsMark already exists
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.Partnerships') 
    AND name = 'IsMark'
)
BEGIN
    PRINT 'Adding IsMark column...';
    
    -- Add IsMark column (bit/boolean with default value FALSE)
    ALTER TABLE [dbo].[Partnerships]
    ADD [IsMark] bit NULL DEFAULT 0;
    
    PRINT 'IsMark column added successfully';
END
ELSE
BEGIN
    PRINT 'WARNING: IsMark column already exists, skipping add';
END

-- ? Step 3: Verify the changes
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

-- ? Commit transaction
COMMIT TRANSACTION;

PRINT 'Migration completed successfully!';

-- ============================================
-- ROLLBACK SCRIPT (if needed)
-- ============================================
/*
USE EventLinkDB;
GO

BEGIN TRANSACTION;

-- Drop IsMark column
IF EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.Partnerships') 
    AND name = 'IsMark'
)
BEGIN
    ALTER TABLE [dbo].[Partnerships]
    DROP COLUMN [IsMark];
    PRINT 'IsMark column dropped';
END

-- Add back SharedNotes column
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.Partnerships') 
    AND name = 'SharedNotes'
)
BEGIN
    ALTER TABLE [dbo].[Partnerships]
    ADD [SharedNotes] nvarchar(max) NULL;
    PRINT 'SharedNotes column restored';
END

COMMIT TRANSACTION;
PRINT 'Rollback completed: SharedNotes restored, IsMark removed';
*/
