# Partnership Status Update - Documentation

## ?? **Status Changes**

### **OLD Status Values (5 statuses):**
```csharp
public const string Pending = "Pending";       
public const string Accepted = "Accepted";     // ? REMOVED
public const string Rejected = "Rejected";     // ? REMOVED
public const string Completed = "Completed";    
public const string Cancelled = "Cancelled";
```

### **NEW Status Values (4 statuses):**
```csharp
public const string Pending = "Pending";       
public const string Ongoing = "Ongoing";       // ? NEW (replaces Accepted)
public const string Completed = "Completed";    
public const string Cancelled = "Cancelled";   // ? (also replaces Rejected)
```

## ?? **Status Mapping:**

| Old Status | New Status | Notes |
|------------|------------|-------|
| **Pending** | **Pending** | ? No change |
| **Accepted** | **Ongoing** | ? Partnership is now active/in progress |
| **Rejected** | **Cancelled** | ? Rejection is now a type of cancellation |
| **Completed** | **Completed** | ? No change |
| **Cancelled** | **Cancelled** | ? No change |

## ?? **New Workflow:**

```
1. Pending (Initial request)
   ?
2. Ongoing (Accepted and active)
   ?
3. Completed (Successfully finished)

OR at any point:
? Cancelled (Rejected or cancelled by either party)
```

### **Valid Status Transitions:**

| From | To | Allowed? | Notes |
|------|----|---------:|-------|
| **Pending** | Ongoing | ? Yes | Organizer accepts the partnership |
| **Pending** | Cancelled | ? Yes | Organizer rejects or either party cancels |
| **Ongoing** | Completed | ? Yes | Partnership work is done |
| **Ongoing** | Cancelled | ? Yes | Either party cancels |
| **Completed** | Any | ? No | Cannot change finalized status |
| **Cancelled** | Any | ? No | Cannot change finalized status |

## ?? **Database Migration:**

### **Automatic Data Updates:**
```sql
-- Accepted -> Ongoing
UPDATE Partnerships
SET Status = 'Ongoing', UpdatedAt = GETDATE()
WHERE Status = 'Accepted';

-- Rejected -> Cancelled
UPDATE Partnerships
SET Status = 'Cancelled', UpdatedAt = GETDATE()
WHERE Status = 'Rejected';
```

## ?? **API Changes:**

### **1. Create Partnership (No Change):**
```javascript
POST /api/Partnerships
FormData:
- EventId: ...
- PartnerId: ...
- PartnerType: Sponsor

Response:
{
  "success": true,
  "data": {
    "status": "Pending"  // ? Always starts as Pending
  }
}
```

### **2. Update Status (New Values):**

#### **Accept Partnership (Pending -> Ongoing):**
```json
PUT /api/Partnerships/{id}/status
{
  "status": "Ongoing",  // ? NEW: Use instead of "Accepted"
  "organizerResponse": "We are happy to work with you!"
}
```

#### **Reject Partnership (Pending -> Cancelled):**
```json
PUT /api/Partnerships/{id}/status
{
  "status": "Cancelled",  // ? Use instead of "Rejected"
  "organizerResponse": "Sorry, we cannot proceed with this partnership"
}
```

#### **Complete Partnership (Ongoing -> Completed):**
```json
PUT /api/Partnerships/{id}/status
{
  "status": "Completed",
  "organizerResponse": "Partnership completed successfully. Thank you!"
}
```

#### **Cancel Active Partnership (Ongoing -> Cancelled):**
```json
PUT /api/Partnerships/{id}/status
{
  "status": "Cancelled",
  "organizerResponse": "We need to cancel this partnership due to..."
}
```

## ?? **Breaking Changes:**

### **Frontend Changes Required:**

#### **OLD CODE (Frontend):**
```javascript
// ? OLD - These values no longer exist
if (partnership.status === 'Accepted') {
  // Show active partnership UI
}

if (partnership.status === 'Rejected') {
  // Show rejection message
}
```

#### **NEW CODE (Frontend):**
```javascript
// ? NEW - Use updated status values
if (partnership.status === 'Ongoing') {
  // Show active partnership UI
}

if (partnership.status === 'Cancelled') {
  // Show cancellation message
  // Check OrganizerResponse to see if it was rejection or cancellation
}
```

### **Status Display Mapping:**
```javascript
const statusLabels = {
  'Pending': 'Awaiting Response',
  'Ongoing': 'Active Partnership',      // ? NEW
  'Completed': 'Completed',
  'Cancelled': 'Cancelled/Rejected'     // ? UPDATED
};

const statusColors = {
  'Pending': 'warning',    // Yellow
  'Ongoing': 'success',    // Green (was 'Accepted')
  'Completed': 'info',     // Blue
  'Cancelled': 'danger'    // Red (covers both rejected & cancelled)
};
```

## ?? **Backend Service Changes:**

### **Status Validation:**
```csharp
// OLD
if (request.Status != PartnershipStatus.Accepted && 
    request.Status != PartnershipStatus.Rejected)
{
    throw new ArgumentException("Invalid status update.");
}

// NEW
var validStatuses = new[] { 
    PartnershipStatus.Pending, 
    PartnershipStatus.Ongoing,
    PartnershipStatus.Completed,
    PartnershipStatus.Cancelled
};

if (!validStatuses.Contains(request.Status))
{
    throw new ArgumentException($"Invalid status. Allowed: {string.Join(", ", validStatuses)}");
}
```

### **Status-Specific Logic:**
```csharp
switch (request.Status)
{
    case PartnershipStatus.Ongoing:  // ? NEW (was Accepted)
        partnership.AgreedBudget = partnership.ProposedBudget;
        partnership.StartDate = DateTime.UtcNow;
        // Set partner contact info...
        break;
        
    case PartnershipStatus.Completed:
        partnership.CompletionDate = DateTime.UtcNow;
        break;
        
    case PartnershipStatus.Cancelled:  // ? Handles both rejection & cancellation
        if (string.IsNullOrEmpty(partnership.OrganizerResponse))
        {
            partnership.OrganizerResponse = "Partnership cancelled";
        }
        break;
}
```

## ?? **Migration Checklist:**

### **Backend:**
- [x] Update `PartnershipStatus` constants in `Partnership.cs`
- [x] Update `PartnershipService.UpdateStatusAsync()` validation
- [x] Add status transition validation logic
- [x] Update status-specific logic (Ongoing instead of Accepted)
- [x] Create database migration script
- [x] Test all status transitions

### **Frontend:**
- [ ] Update status constants/enums
- [ ] Update UI status display labels
- [ ] Update status filter dropdowns
- [ ] Update status color coding
- [ ] Update status transition buttons
- [ ] Test all partnership workflows

### **Database:**
- [ ] Backup database before migration
- [ ] Run migration script to update existing data
- [ ] Verify all partnerships have valid statuses
- [ ] Update any reports/analytics referencing old statuses

## ?? **How to Apply Migration:**

### **1. Backup Database:**
```sql
BACKUP DATABASE EventLinkDB 
TO DISK = 'C:\Backup\EventLinkDB_BeforeStatusUpdate.bak';
```

### **2. Run Migration:**
```powershell
cd EventLink_Repositories\Migrations
.\Apply_UpdatePartnershipStatusValues.ps1
```

### **3. Verify Results:**
```sql
-- Check status distribution
SELECT Status, COUNT(*) as Count
FROM Partnerships
GROUP BY Status;

-- Should only see: Pending, Ongoing, Completed, Cancelled
```

## ? **Benefits of New Status Model:**

1. **Clearer Workflow:** "Ongoing" better represents active partnerships than "Accepted"
2. **Simplified Logic:** Fewer status values to handle
3. **Better Semantics:** "Cancelled" covers both rejection and cancellation scenarios
4. **Easier State Management:** 4 states instead of 5

## ?? **Support:**

If you encounter issues:
1. Check migration logs for errors
2. Verify no partnerships have invalid statuses
3. Review frontend code for hardcoded status values
4. Test status transitions thoroughly

---

**Created:** 2024-12-01  
**Version:** 2.0.0  
**Status:** ? Ready to deploy
