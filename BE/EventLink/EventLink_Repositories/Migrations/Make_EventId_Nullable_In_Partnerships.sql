-- =============================================
-- Migration: Make EventId Nullable in Partnerships
-- Description: Allow partnerships to exist without being assigned to an event
-- Date: 2024-01-20
-- Use Case: Partnership marketplace - Partners can create partnership profiles
--           that Organizers can browse and assign to their events later
-- =============================================

USE EventLinkDB;
GO

PRINT '========================================';
PRINT 'Migration: Make EventId Nullable';
PRINT '========================================';
PRINT '';

-- =============================================
-- STEP 1: Drop Foreign Key Constraint
-- =============================================
PRINT 'Step 1: Checking for FK constraint on EventId...';

DECLARE @FKName NVARCHAR(200);
SELECT @FKName = fk.name
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.tables t ON fk.parent_object_id = t.object_id
INNER JOIN sys.columns c ON fkc.parent_column_id = c.column_id AND fkc.parent_object_id = c.object_id
WHERE t.name = 'Partnerships' 
AND c.name = 'EventId';

IF @FKName IS NOT NULL
BEGIN
    DECLARE @DropFKSQL NVARCHAR(500);
    SET @DropFKSQL = 'ALTER TABLE Partnerships DROP CONSTRAINT ' + @FKName;
    EXEC sp_executesql @DropFKSQL;
    PRINT '? Dropped FK constraint: ' + @FKName;
    PRINT '';
END
ELSE
BEGIN
    PRINT '?? No FK constraint found on EventId';
    PRINT '';
END

-- =============================================
-- STEP 2: Make EventId Column Nullable
-- =============================================
PRINT 'Step 2: Making EventId column nullable...';

ALTER TABLE Partnerships
ALTER COLUMN EventId UNIQUEIDENTIFIER NULL;

PRINT '? EventId column is now nullable';
PRINT '';

-- =============================================
-- STEP 3: Recreate Foreign Key with ON DELETE SET NULL
-- =============================================
PRINT 'Step 3: Recreating FK constraint with ON DELETE SET NULL...';

ALTER TABLE Partnerships
ADD CONSTRAINT FK_Partnerships_Events
FOREIGN KEY (EventId) REFERENCES Events(Id)
ON DELETE SET NULL;  -- ? When event is deleted, set EventId to NULL instead of cascade delete

PRINT '? FK constraint recreated with ON DELETE SET NULL';
PRINT '';

-- =============================================
-- STEP 4: Create Index for Better Query Performance
-- =============================================
PRINT 'Step 4: Creating index for NULL EventId queries...';

-- Check if index exists
IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'IX_Partnerships_EventId_Null' 
    AND object_id = OBJECT_ID('Partnerships')
)
BEGIN
    SET QUOTED_IDENTIFIER ON;
    
    CREATE NONCLUSTERED INDEX IX_Partnerships_EventId_Null
    ON Partnerships(EventId)
    WHERE EventId IS NULL;
    
    PRINT '? Index created for unassigned partnerships queries';
    PRINT '';
END
ELSE
BEGIN
    PRINT '?? Index already exists';
    PRINT '';
END
GO

-- =============================================
-- STEP 5: Verify Changes
-- =============================================
PRINT '========================================';
PRINT 'Verification:';
PRINT '========================================';
PRINT '';

-- Check column nullability
PRINT 'Partnerships.EventId column info:';
SELECT 
    COLUMN_NAME AS 'Column Name',
    DATA_TYPE AS 'Data Type',
    IS_NULLABLE AS 'Nullable',
    COLUMN_DEFAULT AS 'Default'
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Partnerships' 
AND COLUMN_NAME = 'EventId';
PRINT '';

-- Check FK constraint
PRINT 'Foreign Key constraint:';
SELECT 
    fk.name AS 'Constraint Name',
    OBJECT_NAME(fk.parent_object_id) AS 'Table',
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS 'Column',
    OBJECT_NAME(fk.referenced_object_id) AS 'Referenced Table',
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS 'Referenced Column',
    fk.delete_referential_action_desc AS 'On Delete Action'
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
WHERE fk.name = 'FK_Partnerships_Events';
PRINT '';

-- Count partnerships without events
PRINT 'Statistics:';
SELECT 
    COUNT(*) AS 'Total Partnerships',
    SUM(CASE WHEN EventId IS NULL THEN 1 ELSE 0 END) AS 'Unassigned Partnerships',
    SUM(CASE WHEN EventId IS NOT NULL THEN 1 ELSE 0 END) AS 'Assigned Partnerships'
FROM Partnerships;
PRINT '';

PRINT '========================================';
PRINT 'Migration Completed Successfully!';
PRINT '========================================';
PRINT '';
PRINT 'Notes:';
PRINT '  - Partnerships can now be created without EventId';
PRINT '  - Use GET /api/Partnerships/unassigned to browse unassigned partnerships';
PRINT '  - Use PATCH /api/Partnerships/{id}/assign-event/{eventId} to assign to event';
PRINT '  - When an Event is deleted, partnerships EventId will be set to NULL';
GO
