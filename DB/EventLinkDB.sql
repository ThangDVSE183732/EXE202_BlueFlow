-- EventLink Database Creation Script - Fixed Version
-- Handles existing database and foreign key constraints properly

-- Create Database if not exists
IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'EventLinkDB')
BEGIN
    CREATE DATABASE EventLinkDB;
END
GO

USE EventLinkDB;
GO

-- Disable foreign key constraints temporarily for clean drop
EXEC sp_msforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT all"
GO

-- Drop all foreign key constraints first
DECLARE @sql NVARCHAR(max)=''
SELECT @sql = @sql + 'ALTER TABLE ' + QUOTENAME(SCHEMA_NAME(schema_id)) + '.' + QUOTENAME(OBJECT_NAME(parent_object_id)) + ' DROP CONSTRAINT ' + QUOTENAME(name) + ';'
FROM sys.foreign_keys

EXECUTE(@sql)
GO

-- Drop all existing tables
DROP TABLE IF EXISTS DashboardMetrics;
DROP TABLE IF EXISTS UserActivities;
DROP TABLE IF EXISTS Payments;
DROP TABLE IF EXISTS UserSubscriptions;
DROP TABLE IF EXISTS SubscriptionPlans;
DROP TABLE IF EXISTS Reviews;
DROP TABLE IF EXISTS Messages;
DROP TABLE IF EXISTS Partnerships;
DROP TABLE IF EXISTS EventProposals;
DROP TABLE IF EXISTS Events;
DROP TABLE IF EXISTS SponsorPackages;
DROP TABLE IF EXISTS SupplierServices;
DROP TABLE IF EXISTS UserProfiles;
DROP TABLE IF EXISTS Users;
GO

-- Drop all existing indexes (prevent duplicate index errors)
DECLARE @DropIndexSQL NVARCHAR(MAX) = N''
SELECT @DropIndexSQL = @DropIndexSQL + N'DROP INDEX ' + QUOTENAME(i.name) + N' ON ' + QUOTENAME(s.name) + N'.' + QUOTENAME(t.name) + N';' + CHAR(13)
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE i.is_primary_key = 0 
  AND i.index_id > 0 
  AND i.type IN (1, 2)
  AND i.is_unique_constraint = 0
  AND t.is_ms_shipped = 0

IF LEN(@DropIndexSQL) > 0
    EXECUTE(@DropIndexSQL)
GO

-- Re-enable foreign key constraints
EXEC sp_msforeachtable "ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all"
GO

-- 1. Users Table (Base Authentication)
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Email NVARCHAR(255) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Organizer', 'Supplier', 'Sponsor')),
    FullName NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(20),
    AvatarUrl NVARCHAR(500),
    IsActive BIT DEFAULT 1,
    EmailVerified BIT DEFAULT 0,
    LastLoginAt DATETIME2,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE()
);
GO

-- 2. UserProfiles Table (Extended Profiles for all roles)
CREATE TABLE UserProfiles (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    
    -- Common fields
    Bio NVARCHAR(2000),
    CompanyName NVARCHAR(255),
    Website NVARCHAR(255),
    Location NVARCHAR(255),
    ProfileImageUrl NVARCHAR(500),
    CoverImageUrl NVARCHAR(500),
    LinkedInUrl NVARCHAR(500),
    FacebookUrl NVARCHAR(500),
    
    -- Portfolio & Gallery
    PortfolioImages NVARCHAR(MAX), -- JSON array of image URLs
    WorkSamples NVARCHAR(MAX), -- JSON array with descriptions
    Certifications NVARCHAR(MAX), -- JSON array
    
    -- Experience & Stats
    YearsOfExperience INT DEFAULT 0,
    TotalProjectsCompleted INT DEFAULT 0,
    AverageRating DECIMAL(3,2) DEFAULT 0,
    
    -- Verification
    IsVerified BIT DEFAULT 0,
    VerificationDocuments NVARCHAR(MAX), -- JSON array
    
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    
    CONSTRAINT FK_UserProfiles_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
GO

-- 3. SupplierServices Table (Services offered by suppliers)
CREATE TABLE SupplierServices (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SupplierId UNIQUEIDENTIFIER NOT NULL,
    ServiceName NVARCHAR(255) NOT NULL,
    ServiceCategory NVARCHAR(100) NOT NULL, -- Audio, Lighting, Construction, Printing, etc.
    Description NVARCHAR(2000),
    BasePrice DECIMAL(18,2),
    PriceUnit NVARCHAR(50), -- per hour, per day, per project
    MinPrice DECIMAL(18,2),
    MaxPrice DECIMAL(18,2),
    ServiceImages NVARCHAR(MAX), -- JSON array of image URLs
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    
    CONSTRAINT FK_SupplierServices_Users FOREIGN KEY (SupplierId) REFERENCES Users(Id) ON DELETE CASCADE
);
GO

-- 4. SponsorPackages Table (Sponsorship packages offered by sponsors)
CREATE TABLE SponsorPackages (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SponsorId UNIQUEIDENTIFIER NOT NULL,
    PackageName NVARCHAR(255) NOT NULL,
    PackageType NVARCHAR(100), -- Gold, Silver, Bronze, Custom
    Budget DECIMAL(18,2),
    BudgetRange NVARCHAR(100), -- "1M-5M VND"
    SponsorshipBenefits NVARCHAR(MAX), -- JSON array
    TargetAudience NVARCHAR(500),
    PreferredEventTypes NVARCHAR(MAX), -- JSON array
    BrandGuidelines NVARCHAR(MAX), -- JSON requirements
    LogoUrl NVARCHAR(500),
    BrandAssets NVARCHAR(MAX), -- JSON array of URLs
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    
    CONSTRAINT FK_SponsorPackages_Users FOREIGN KEY (SponsorId) REFERENCES Users(Id) ON DELETE CASCADE
);
GO

-- 5. Events Table
CREATE TABLE Events (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    OrganizerId UNIQUEIDENTIFIER NOT NULL,
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    ShortDescription NVARCHAR(500),
    EventDate DATETIME2,
    EndDate DATETIME2,
    Location NVARCHAR(255),
    VenueDetails NVARCHAR(1000),
    
    -- Budget & Financial
    TotalBudget DECIMAL(18,2),
    
    -- Event Details
    ExpectedAttendees INT,
    Category NVARCHAR(100), -- Workshop, Talkshow, Conference, etc.
    EventType NVARCHAR(100), -- Online, Offline, Hybrid
    TargetAudience NVARCHAR(500),
    
    -- Requirements
    RequiredServices NVARCHAR(MAX), -- JSON array of needed services
    SponsorshipNeeds NVARCHAR(MAX), -- JSON array of sponsorship needs
    SpecialRequirements NVARCHAR(1000),
    
    -- Status & Visibility
    Status NVARCHAR(20) DEFAULT 'Draft' CHECK (Status IN ('Draft', 'Published', 'InProgress', 'Completed', 'Cancelled')),
    IsPublic BIT DEFAULT 1,
    IsFeatured BIT DEFAULT 0,
    
    -- Media
    CoverImageUrl NVARCHAR(500),
    EventImages NVARCHAR(MAX), -- JSON array
    
    -- Engagement
    ViewCount INT DEFAULT 0,
    InterestedCount INT DEFAULT 0,
    
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    
    CONSTRAINT FK_Events_Users FOREIGN KEY (OrganizerId) REFERENCES Users(Id) ON DELETE CASCADE
);
GO

-- 6. EventProposals Table (Detailed proposals for partnerships)
CREATE TABLE EventProposals (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EventId UNIQUEIDENTIFIER NOT NULL,
    ProposalType NVARCHAR(20) NOT NULL CHECK (ProposalType IN ('Supplier', 'Sponsor')),
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    Requirements NVARCHAR(MAX), -- JSON detailed requirements
    Budget DECIMAL(18,2),
    Deadline DATETIME2,
    ContactInstructions NVARCHAR(1000),
    AttachmentUrls NVARCHAR(MAX), -- JSON array
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    
    CONSTRAINT FK_EventProposals_Events FOREIGN KEY (EventId) REFERENCES Events(Id) ON DELETE CASCADE
);
GO

-- 7. Partnerships Table (Connection records between parties)
CREATE TABLE Partnerships (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EventId UNIQUEIDENTIFIER NOT NULL,
    PartnerId UNIQUEIDENTIFIER NOT NULL, -- Supplier or Sponsor ID
    PartnerType NVARCHAR(20) NOT NULL CHECK (PartnerType IN ('Supplier', 'Sponsor')),
    
    -- Proposal & Initial Contact
    InitialMessage NVARCHAR(MAX), -- First contact message
    OrganizerResponse NVARCHAR(MAX), -- Organizer's response
    
    -- Basic Agreement Info
    ProposedBudget DECIMAL(18,2),
    AgreedBudget DECIMAL(18,2),
    ServiceDescription NVARCHAR(MAX), -- What will be provided
    
    -- Status Tracking
    Status NVARCHAR(20) DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Accepted', 'Rejected', 'Completed', 'Cancelled')),
    
    -- Contact Information Exchange
    OrganizerContactInfo NVARCHAR(MAX), -- JSON with phone, email, etc.
    PartnerContactInfo NVARCHAR(MAX), -- JSON with phone, email, etc.
    PreferredContactMethod NVARCHAR(100), -- WhatsApp, Zalo, Email, etc.
    ExternalWorkspaceUrl NVARCHAR(500), -- Link to Trello, Slack, etc. if they use
    
    -- Timeline
    StartDate DATETIME2,
    DeadlineDate DATETIME2,
    CompletionDate DATETIME2,
    
    -- Notes
    OrganizerNotes NVARCHAR(MAX), -- Private notes for organizer
    PartnerNotes NVARCHAR(MAX), -- Private notes for partner
    SharedNotes NVARCHAR(MAX), -- Visible to both parties
    
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    
    CONSTRAINT FK_Partnerships_Events FOREIGN KEY (EventId) REFERENCES Events(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Partnerships_Users FOREIGN KEY (PartnerId) REFERENCES Users(Id) ON DELETE NO ACTION
);
GO

-- 8. Messages Table (Simplified 1-on-1 messaging)
CREATE TABLE Messages (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SenderId UNIQUEIDENTIFIER NOT NULL,
    ReceiverId UNIQUEIDENTIFIER NOT NULL,
    PartnershipId UNIQUEIDENTIFIER, -- Optional: link to specific partnership
    Content NVARCHAR(MAX) NOT NULL,
    MessageType NVARCHAR(20) DEFAULT 'Text' CHECK (MessageType IN ('Text', 'Image', 'File', 'Contact', 'System')),
    AttachmentUrl NVARCHAR(500),
    AttachmentName NVARCHAR(255),
    IsRead BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    
    CONSTRAINT FK_Messages_Sender FOREIGN KEY (SenderId) REFERENCES Users(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Messages_Receiver FOREIGN KEY (ReceiverId) REFERENCES Users(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Messages_Partnership FOREIGN KEY (PartnershipId) REFERENCES Partnerships(Id) ON DELETE SET NULL
);
GO

-- 9. Reviews Table (Feedback after partnership completion)
CREATE TABLE Reviews (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PartnershipId UNIQUEIDENTIFIER NOT NULL,
    ReviewerId UNIQUEIDENTIFIER NOT NULL,
    RevieweeId UNIQUEIDENTIFIER NOT NULL,
    
    -- Ratings (1-5 scale)
    OverallRating INT CHECK (OverallRating BETWEEN 1 AND 5),
    CommunicationRating INT CHECK (CommunicationRating BETWEEN 1 AND 5),
    QualityRating INT CHECK (QualityRating BETWEEN 1 AND 5),
    TimelinessRating INT CHECK (TimelinessRating BETWEEN 1 AND 5),
    ProfessionalismRating INT CHECK (ProfessionalismRating BETWEEN 1 AND 5),
    
    -- Written Review
    ReviewTitle NVARCHAR(255),
    ReviewComment NVARCHAR(2000),
    Pros NVARCHAR(1000),
    Cons NVARCHAR(1000),
    
    -- Recommendations
    WouldRecommend BIT,
    WouldWorkAgain BIT,
    
    -- Status
    IsPublic BIT DEFAULT 1,
    IsVerified BIT DEFAULT 0,
    
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    
    CONSTRAINT FK_Reviews_Partnership FOREIGN KEY (PartnershipId) REFERENCES Partnerships(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Reviews_Reviewer FOREIGN KEY (ReviewerId) REFERENCES Users(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Reviews_Reviewee FOREIGN KEY (RevieweeId) REFERENCES Users(Id) ON DELETE NO ACTION
);
GO

-- 10. SubscriptionPlans Table
CREATE TABLE SubscriptionPlans (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PlanName NVARCHAR(100) NOT NULL,
    PlanType NVARCHAR(50) NOT NULL CHECK (PlanType IN ('Free', 'Pro', 'Business')),
    TargetRole NVARCHAR(20) CHECK (TargetRole IN ('Organizer', 'Supplier', 'Sponsor')),
    MonthlyPrice DECIMAL(18,2) NOT NULL,
    YearlyPrice DECIMAL(18,2),
    MaxEvents INT,
    MaxPartnerships INT,
    MaxMessages INT,
    Features NVARCHAR(MAX), -- JSON array of features
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETDATE()
);
GO

-- 11. UserSubscriptions Table
CREATE TABLE UserSubscriptions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    PlanId UNIQUEIDENTIFIER NOT NULL,
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2,
    IsActive BIT DEFAULT 1,
    AutoRenew BIT DEFAULT 1,
    PaymentMethod NVARCHAR(50),
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    
    CONSTRAINT FK_UserSubscriptions_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    CONSTRAINT FK_UserSubscriptions_Plans FOREIGN KEY (PlanId) REFERENCES SubscriptionPlans(Id) ON DELETE CASCADE
);
GO

-- 12. Payments Table (PayOS Integration) - Fixed cascade issue
CREATE TABLE Payments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    SubscriptionId UNIQUEIDENTIFIER,
    
    -- Payment Details
    PaymentType NVARCHAR(50) NOT NULL CHECK (PaymentType IN ('Subscription', 'Featured_Event', 'Premium_Listing')),
    Amount DECIMAL(18,2) NOT NULL,
    Currency NVARCHAR(10) DEFAULT 'VND',
    
    -- PayOS Integration
    PayOSOrderId NVARCHAR(255),
    PayOSTransactionId NVARCHAR(255),
    PaymentMethod NVARCHAR(100),
    
    -- Status
    Status NVARCHAR(50) DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Processing', 'Completed', 'Failed', 'Refunded')),
    PaymentDate DATETIME2,
    
    -- Additional Info
    Description NVARCHAR(500),
    PaymentGatewayResponse NVARCHAR(MAX), -- JSON response
    
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    
    CONSTRAINT FK_Payments_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Payments_Subscriptions FOREIGN KEY (SubscriptionId) REFERENCES UserSubscriptions(Id) ON DELETE NO ACTION
);
GO

-- 13. UserActivities Table (Basic analytics)
CREATE TABLE UserActivities (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    ActivityType NVARCHAR(50) NOT NULL, -- login, view_profile, send_message, create_event, etc.
    EntityType NVARCHAR(50), -- event, partnership, user
    EntityId UNIQUEIDENTIFIER,
    Description NVARCHAR(500),
    IpAddress NVARCHAR(50),
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    
    CONSTRAINT FK_UserActivities_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
GO

-- 14. DashboardMetrics Table (Simple cached metrics)
CREATE TABLE DashboardMetrics (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    MetricDate DATE NOT NULL,
    
    -- Common Metrics
    ProfileViews INT DEFAULT 0,
    MessagesReceived INT DEFAULT 0,
    MessagesSent INT DEFAULT 0,
    
    -- Role-specific Metrics (will be 0 for non-applicable roles)
    ActiveEvents INT DEFAULT 0, -- Organizer
    CompletedEvents INT DEFAULT 0, -- Organizer
    ActivePartnerships INT DEFAULT 0, -- All roles
    CompletedPartnerships INT DEFAULT 0, -- All roles
    AverageRating DECIMAL(3,2) DEFAULT 0, -- All roles
    
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    
    CONSTRAINT FK_DashboardMetrics_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_DashboardMetrics_User_Date UNIQUE(UserId, MetricDate)
);
GO

-- CREATE PERFORMANCE INDEXES
PRINT 'Creating performance indexes...';

-- User indexes
CREATE NONCLUSTERED INDEX IX_Users_Email ON Users(Email);
CREATE NONCLUSTERED INDEX IX_Users_Role ON Users(Role);
CREATE NONCLUSTERED INDEX IX_Users_IsActive ON Users(IsActive);

-- Event indexes
CREATE NONCLUSTERED INDEX IX_Events_OrganizerId ON Events(OrganizerId);
CREATE NONCLUSTERED INDEX IX_Events_Status_IsPublic ON Events(Status, IsPublic);
CREATE NONCLUSTERED INDEX IX_Events_Category ON Events(Category);
CREATE NONCLUSTERED INDEX IX_Events_EventDate ON Events(EventDate);
CREATE NONCLUSTERED INDEX IX_Events_Location ON Events(Location);

-- Partnership indexes
CREATE NONCLUSTERED INDEX IX_Partnerships_EventId ON Partnerships(EventId);
CREATE NONCLUSTERED INDEX IX_Partnerships_PartnerId ON Partnerships(PartnerId);
CREATE NONCLUSTERED INDEX IX_Partnerships_Status ON Partnerships(Status);
CREATE NONCLUSTERED INDEX IX_Partnerships_PartnerType ON Partnerships(PartnerType);

-- Message indexes (simplified)
CREATE NONCLUSTERED INDEX IX_Messages_SenderId_ReceiverId ON Messages(SenderId, ReceiverId);
CREATE NONCLUSTERED INDEX IX_Messages_ReceiverId_IsRead ON Messages(ReceiverId, IsRead);
CREATE NONCLUSTERED INDEX IX_Messages_CreatedAt ON Messages(CreatedAt);
CREATE NONCLUSTERED INDEX IX_Messages_PartnershipId ON Messages(PartnershipId);

-- Service and Package indexes
CREATE NONCLUSTERED INDEX IX_SupplierServices_SupplierId ON SupplierServices(SupplierId);
CREATE NONCLUSTERED INDEX IX_SupplierServices_ServiceCategory ON SupplierServices(ServiceCategory);
CREATE NONCLUSTERED INDEX IX_SponsorPackages_SponsorId ON SponsorPackages(SponsorId);
CREATE NONCLUSTERED INDEX IX_SponsorPackages_PackageType ON SponsorPackages(PackageType);

-- Review indexes
CREATE NONCLUSTERED INDEX IX_Reviews_RevieweeId ON Reviews(RevieweeId);
CREATE NONCLUSTERED INDEX IX_Reviews_PartnershipId ON Reviews(PartnershipId);
CREATE NONCLUSTERED INDEX IX_Reviews_OverallRating ON Reviews(OverallRating);

-- Payment indexes
CREATE NONCLUSTERED INDEX IX_Payments_UserId ON Payments(UserId);
CREATE NONCLUSTERED INDEX IX_Payments_Status ON Payments(Status);
CREATE NONCLUSTERED INDEX IX_UserSubscriptions_UserId ON UserSubscriptions(UserId);

-- Analytics indexes
CREATE NONCLUSTERED INDEX IX_UserActivities_UserId_CreatedAt ON UserActivities(UserId, CreatedAt);
CREATE NONCLUSTERED INDEX IX_DashboardMetrics_UserId_MetricDate ON DashboardMetrics(UserId, MetricDate);

PRINT 'Inserting default subscription plans...';

-- Insert default subscription plans
INSERT INTO SubscriptionPlans (PlanName, PlanType, TargetRole, MonthlyPrice, YearlyPrice, MaxEvents, MaxPartnerships, MaxMessages, Features) VALUES
-- Organizer Plans
('Free Organizer', 'Free', 'Organizer', 0, 0, 2, 3, 20, '["Basic event creation", "Limited partner search", "Basic messaging"]'),
('Pro Organizer', 'Pro', 'Organizer', 199000, 1990000, 10, 20, 200, '["Advanced event management", "Unlimited partner search", "Priority support", "Basic analytics"]'),
('Business Organizer', 'Business', 'Organizer', 399000, 3990000, -1, -1, -1, '["Everything in Pro", "Featured events", "Advanced analytics", "Custom branding"]'),

-- Supplier Plans
('Free Supplier', 'Free', 'Supplier', 0, 0, 2, 3, 20, '["Basic profile", "Limited event applications", "Basic messaging"]'),
('Pro Supplier', 'Pro', 'Supplier', 149000, 1490000, 15, 25, 300, '["Enhanced profile", "Unlimited applications", "Portfolio showcase", "Priority listing"]'),
('Business Supplier', 'Business', 'Supplier', 299000, 2990000, -1, -1, -1, '["Everything in Pro", "Featured supplier", "Advanced analytics", "Lead generation"]'),

-- Sponsor Plans
('Free Sponsor', 'Free', 'Sponsor', 0, 0, 2, 3, 20, '["Basic brand profile", "Limited event discovery", "Basic messaging"]'),
('Pro Sponsor', 'Pro', 'Sponsor', 249000, 2490000, 10, 15, 250, '["Enhanced brand profile", "Advanced event matching", "Campaign tracking"]'),
('Business Sponsor', 'Business', 'Sponsor', 499000, 4990000, -1, -1, -1, '["Everything in Pro", "Featured sponsor", "Custom packages", "ROI analytics"]');

PRINT 'EventLink database setup completed successfully!';
PRINT 'Database focus: Connection platform with simplified messaging and partnership management.';
GO