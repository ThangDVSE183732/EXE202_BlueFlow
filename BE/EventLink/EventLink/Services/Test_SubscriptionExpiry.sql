-- =====================================================
-- Subscription Expiry Background Service - Test Script
-- =====================================================

-- ? Test 1: Create expired subscription
-- This subscription should be deactivated on next check
DECLARE @UserId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Users WHERE IsPremium = 0);
DECLARE @PlanId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM SubscriptionPlans WHERE PlanType = 'monthly');

INSERT INTO UserSubscriptions (Id, UserId, PlanId, StartDate, EndDate, IsActive, AutoRenew, CreatedAt)
VALUES (
    NEWID(),
    @UserId,
    @PlanId,
    DATEADD(DAY, -31, GETDATE()),  -- Started 31 days ago
    DATEADD(DAY, -1, GETDATE()),   -- Expired 1 day ago
    1,                              -- Still active (waiting for background job)
    1,
    DATEADD(DAY, -31, GETDATE())
);

-- Update User to simulate they have premium
UPDATE Users 
SET IsPremium = 1, 
    PremiumExpiryDate = DATEADD(DAY, -1, GETDATE())
WHERE Id = @UserId;

SELECT 
    'Test 1: Expired Subscription Created' AS Test,
    Id, UserId, EndDate, IsActive
FROM UserSubscriptions
WHERE UserId = @UserId AND IsActive = 1 AND EndDate < GETDATE();

-- =====================================================

-- ? Test 2: Create subscription expiring in 7 days (for reminder email)
DECLARE @UserId2 UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Users WHERE Email IS NOT NULL AND IsPremium = 0);

INSERT INTO UserSubscriptions (Id, UserId, PlanId, StartDate, EndDate, IsActive, AutoRenew, CreatedAt)
VALUES (
    NEWID(),
    @UserId2,
    @PlanId,
    GETDATE(),
    DATEADD(DAY, 7, GETDATE()),  -- Expires in 7 days
    1,
    1,
    GETDATE()
);

-- Update User to simulate they have premium
UPDATE Users 
SET IsPremium = 1, 
    PremiumExpiryDate = DATEADD(DAY, 7, GETDATE())
WHERE Id = @UserId2;

SELECT 
    'Test 2: Subscription Expiring in 7 Days' AS Test,
    Id, UserId, EndDate, IsActive,
    DATEDIFF(DAY, GETDATE(), EndDate) AS DaysUntilExpiry
FROM UserSubscriptions
WHERE UserId = @UserId2 AND IsActive = 1;

-- =====================================================

-- ? Test 3: Create subscription expiring in 1 minute (immediate test)
DECLARE @UserId3 UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Users WHERE IsPremium = 0);

INSERT INTO UserSubscriptions (Id, UserId, PlanId, StartDate, EndDate, IsActive, AutoRenew, CreatedAt)
VALUES (
    NEWID(),
    @UserId3,
    @PlanId,
    GETDATE(),
    DATEADD(MINUTE, 1, GETDATE()),  -- Expires in 1 minute
    1,
    1,
    GETDATE()
);

UPDATE Users 
SET IsPremium = 1, 
    PremiumExpiryDate = DATEADD(MINUTE, 1, GETDATE())
WHERE Id = @UserId3;

SELECT 
    'Test 3: Subscription Expiring in 1 Minute' AS Test,
    Id, UserId, EndDate, IsActive,
    DATEDIFF(SECOND, GETDATE(), EndDate) AS SecondsUntilExpiry
FROM UserSubscriptions
WHERE UserId = @UserId3 AND IsActive = 1;

-- =====================================================

-- ?? Verification Queries (Run AFTER background service executes)

-- 1. Check deactivated subscriptions
SELECT 
    'Deactivated Subscriptions (Last 2 Hours)' AS Report,
    us.Id,
    u.Email,
    us.EndDate,
    us.IsActive,
    us.UpdatedAt
FROM UserSubscriptions us
JOIN Users u ON us.UserId = u.Id
WHERE us.IsActive = 0 
  AND us.EndDate <= GETDATE()
  AND us.UpdatedAt >= DATEADD(HOUR, -2, GETDATE())
ORDER BY us.UpdatedAt DESC;

-- 2. Check users who lost premium
SELECT 
    'Users Lost Premium (Last 2 Hours)' AS Report,
    Id,
    Email,
    IsPremium,
    PremiumExpiryDate,
    UpdatedAt
FROM Users
WHERE IsPremium = 0
  AND UpdatedAt >= DATEADD(HOUR, -2, GETDATE())
ORDER BY UpdatedAt DESC;

-- 3. Check subscriptions expiring soon (should get reminder)
SELECT 
    'Subscriptions Expiring in 7 Days' AS Report,
    us.Id,
    u.Email,
    us.EndDate,
    DATEDIFF(DAY, GETDATE(), us.EndDate) AS DaysRemaining,
    us.IsActive
FROM UserSubscriptions us
JOIN Users u ON us.UserId = u.Id
WHERE us.IsActive = 1
  AND us.EndDate > GETDATE()
  AND us.EndDate <= DATEADD(DAY, 7, GETDATE())
ORDER BY us.EndDate;

-- 4. Count statistics
SELECT 
    'Subscription Statistics' AS Report,
    COUNT(*) AS TotalSubscriptions,
    SUM(CASE WHEN IsActive = 1 THEN 1 ELSE 0 END) AS ActiveSubscriptions,
    SUM(CASE WHEN IsActive = 0 THEN 1 ELSE 0 END) AS InactiveSubscriptions,
    SUM(CASE WHEN EndDate < GETDATE() AND IsActive = 1 THEN 1 ELSE 0 END) AS ExpiredButStillActive
FROM UserSubscriptions;

-- =====================================================

-- ?? Cleanup Test Data (Optional - run after testing)

-- Remove test subscriptions
DELETE FROM UserSubscriptions 
WHERE CreatedAt >= DATEADD(HOUR, -2, GETDATE())
  AND UserId IN (
      SELECT Id FROM Users WHERE Email LIKE '%test%' OR IsPremium = 0
  );

-- Reset test users
UPDATE Users 
SET IsPremium = 0, PremiumExpiryDate = NULL
WHERE Id IN (@UserId, @UserId2, @UserId3);

SELECT 'Cleanup Complete' AS Status;
