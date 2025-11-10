-- ============================================
-- Check Partnerships table constraints and triggers
-- ============================================

USE EventLinkDB;
GO

PRINT '========================================';
PRINT 'CHECKING PARTNERSHIPS TABLE CONFIGURATION';
PRINT '========================================';
PRINT '';

-- ? Check DEFAULT constraints
PRINT '1. DEFAULT CONSTRAINTS:';
PRINT '----------------------------------------';
SELECT 
    c.name AS ColumnName,
    dc.name AS ConstraintName,
    dc.definition AS DefaultValue
FROM sys.default_constraints dc
INNER JOIN sys.columns c ON dc.parent_column_id = c.column_id
INNER JOIN sys.tables t ON dc.parent_object_id = t.object_id
WHERE t.name = 'Partnerships'
  AND c.name IN ('Status', 'IsMark', 'CreatedAt', 'UpdatedAt')
ORDER BY c.name;

PRINT '';

-- ? Check TRIGGERS
PRINT '2. TRIGGERS ON PARTNERSHIPS TABLE:';
PRINT '----------------------------------------';
SELECT 
    t.name AS TriggerName,
    OBJECT_DEFINITION(t.object_id) AS TriggerDefinition
FROM sys.triggers t
INNER JOIN sys.tables tb ON t.parent_id = tb.object_id
WHERE tb.name = 'Partnerships';

PRINT '';

-- ? Check current column properties
PRINT '3. COLUMN PROPERTIES:';
PRINT '----------------------------------------';
SELECT 
    c.name AS ColumnName,
    ty.name AS DataType,
    c.max_length AS MaxLength,
    c.is_nullable AS IsNullable,
    ISNULL(dc.definition, 'NO DEFAULT') AS DefaultValue
FROM sys.columns c
INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
INNER JOIN sys.tables t ON c.object_id = t.object_id
LEFT JOIN sys.default_constraints dc ON c.default_object_id = dc.object_id
WHERE t.name = 'Partnerships'
  AND c.name IN ('Status', 'IsMark', 'PartnershipImage', 'CreatedAt', 'UpdatedAt')
ORDER BY c.name;

PRINT '';
PRINT '========================================';
PRINT 'CHECK COMPLETED';
PRINT '========================================';
