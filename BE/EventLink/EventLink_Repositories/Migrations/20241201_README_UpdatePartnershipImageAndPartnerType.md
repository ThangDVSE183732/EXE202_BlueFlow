# Migration: UpdatePartnershipImageAndPartnerType

## ?? **Changes Overview**

### **1. Model Changes - `Partnership.cs`**
- ? **Renamed:** `PartnerNotes` ? `PartnershipImage`
- ? **Added:** `PartnerType.Organizer` constant (in addition to Sponsor and Supplier)

### **2. Database Schema Changes**
- ? **Column Rename:** `Partnerships.PartnerNotes` ? `Partnerships.PartnershipImage`
- ?? **No schema change needed** for PartnerType (already `nvarchar(20)`, can store "Organizer")

---

## ?? **How to Apply Migration**

### **Option 1: Using SQL Script (Recommended)**

```bash
# 1. Open SQL Server Management Studio (SSMS)
# 2. Connect to your database
# 3. Open the SQL script:
EventLink_Repositories/Migrations/20241201000000_UpdatePartnershipImageAndPartnerType.sql

# 4. Update database name in script (line 8):
USE EventLinkDB;  -- Replace with YOUR database name

# 5. Execute the script
```

### **Option 2: Using EF Core Migrations (if DbContext is configured)**

```bash
# Navigate to solution root
cd "C:\Users\ASUS\Music\CN 8\EXE202\EXE202_BlueFlow\BE\EventLink"

# Apply migration
dotnet ef database update --project EventLink_Repositories --startup-project EventLink
```

---

## ?? **Files Created/Modified**

### **Modified:**
1. ? `EventLink_Repositories/Models/Partnership.cs`
   - Renamed property: `PartnerNotes` ? `PartnershipImage`
   - Added constant: `PartnerType.Organizer`

2. ? `EventLink_Repositories/Migrations/EventLinkDBContextModelSnapshot.cs`
   - Updated Partnership model snapshot

### **Created:**
3. ? `EventLink_Repositories/Migrations/20241201000000_UpdatePartnershipImageAndPartnerType.cs`
   - Migration class (for EF Core)

4. ? `EventLink_Repositories/Migrations/20241201000000_UpdatePartnershipImageAndPartnerType.sql`
   - SQL script (for manual execution)

---

## ? **Verification Steps**

### **After applying migration:**

```sql
-- Check column exists
SELECT * 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Partnerships' 
  AND COLUMN_NAME = 'PartnershipImage';

-- Verify data type
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Partnerships' 
  AND COLUMN_NAME = 'PartnershipImage';
-- Expected: nvarchar(max)

-- Test insert with new PartnerType
INSERT INTO Partnerships (Id, EventId, PartnerId, PartnerType, Status, PartnershipImage)
VALUES (
    NEWID(),
    '...',  -- Replace with valid EventId
    '...',  -- Replace with valid PartnerId
    'Organizer',  -- ? New value
    'Pending',
    'https://cloudinary.com/partnership-image.jpg'
);
```

---

## ?? **Rollback (if needed)**

### **SQL Rollback:**
```sql
-- Rename column back to PartnerNotes
EXEC sp_rename 
    @objname = 'dbo.Partnerships.PartnershipImage', 
    @newname = 'PartnerNotes', 
    @objtype = 'COLUMN';
```

### **EF Core Rollback:**
```bash
# Revert to previous migration
dotnet ef database update 20251026082131_UpdateOverview --project EventLink_Repositories --startup-project EventLink
```

---

## ?? **Usage in Code**

### **Before:**
```csharp
var partnership = new Partnership
{
    PartnerNotes = "Some notes",  // ? Old property
    PartnerType = PartnerType.Sponsor  // Only Sponsor/Supplier
};
```

### **After:**
```csharp
var partnership = new Partnership
{
    PartnershipImage = "https://cloudinary.com/image.jpg",  // ? New property
    PartnerType = PartnerType.Organizer  // ? New value available
};

// Available PartnerType values:
// - PartnerType.Sponsor
// - PartnerType.Supplier
// - PartnerType.Organizer  (NEW!)
```

---

## ?? **Important Notes**

1. **Backup Database First!**
   ```sql
   BACKUP DATABASE EventLinkDB 
   TO DISK = 'C:\Backup\EventLinkDB_BeforeMigration.bak';
   ```

2. **Update Frontend Code:** If you're storing partnership images, update the field name:
   - Old: `partnerNotes`
   - New: `partnershipImage`

3. **API Changes:** Update any DTOs/Requests/Responses that reference `PartnerNotes`

---

## ?? **Support**

If you encounter issues during migration:
1. Check SQL error messages
2. Verify database connection string
3. Ensure no active transactions are blocking the column rename
4. Review migration rollback script above

---

**Created:** 2024-12-01  
**Version:** 1.0.0  
**Status:** ? Ready to apply
