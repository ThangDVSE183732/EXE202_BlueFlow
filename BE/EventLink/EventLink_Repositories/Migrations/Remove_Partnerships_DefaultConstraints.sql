-- ============================================
-- Remove DEFAULT constraints from Partnerships table
-- To allow C# code to fully control values
-- ============================================

USE EventLinkDB;
GO

BEGIN TRANSACTION;

PRINT 'Removing DEFAULT constraints from Partnerships table...';
PRINT '';

-- ? Step 1: Find and drop Status DEFAULT constraint
DECLARE @StatusConstraint NVARCHAR(200);
SELECT @StatusConstraint = dc.name
FROM sys.default_constraints dc
INNER JOIN sys.columns c ON dc.parent_column_id = c.column_id
INNER JOIN sys.tables t ON dc.parent_object_id = t.object_id
WHERE t.name = 'Partnerships' AND c.name = 'Status';

IF @StatusConstraint IS NOT NULL
BEGIN
    EXEC('ALTER TABLE Partnerships DROP CONSTRAINT ' + @StatusConstraint);
    PRINT '? Dropped Status DEFAULT constraint: ' + @StatusConstraint;
END
ELSE
BEGIN
    PRINT '??  No Status DEFAULT constraint found';
END

PRINT '';

-- ? Step 2: Find and drop IsMark DEFAULT constraint
DECLARE @IsMarkConstraint NVARCHAR(200);
SELECT @IsMarkConstraint = dc.name
FROM sys.default_constraints dc
INNER JOIN sys.columns c ON dc.parent_column_id = c.column_id
INNER JOIN sys.tables t ON dc.parent_object_id = t.object_id
WHERE t.name = 'Partnerships' AND c.name = 'IsMark';

IF @IsMarkConstraint IS NOT NULL
BEGIN
    EXEC('ALTER TABLE Partnerships DROP CONSTRAINT ' + @IsMarkConstraint);
    PRINT '? Dropped IsMark DEFAULT constraint: ' + @IsMarkConstraint;
END
ELSE
BEGIN
    PRINT '??  No IsMark DEFAULT constraint found';
END

PRINT '';

-- ? Step 3: Verify constraints are removed
PRINT 'Verifying constraints...';
SELECT 
    c.name AS ColumnName,
    ISNULL(dc.definition, 'NO DEFAULT') AS DefaultValue
FROM sys.columns c
INNER JOIN sys.tables t ON c.object_id = t.object_id
LEFT JOIN sys.default_constraints dc ON c.default_object_id = dc.object_id
WHERE t.name = 'Partnerships'
  AND c.name IN ('Status', 'IsMark');

COMMIT TRANSACTION;

PRINT '';
PRINT '========================================';
PRINT '? DEFAULT constraints removed successfully!';
PRINT 'C# code now has full control over Status and IsMark values';
PRINT '========================================';
