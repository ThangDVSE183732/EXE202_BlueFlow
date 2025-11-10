using System;
using System.Collections.Generic;

namespace Eventlink_Services.Response
{
    /// <summary>
    /// Response DTO for Partnership with Event details
    /// </summary>
    public class PartnershipResponse
    {
        // Partnership Info
        public Guid Id { get; set; }
        // ? CHANGED: EventId is now nullable
        public Guid? EventId { get; set; }
        public Guid PartnerId { get; set; }
        public string PartnerType { get; set; }
        // ? Explicitly nullable
        public string? InitialMessage { get; set; }
        // ? Explicitly nullable
        public string? OrganizerResponse { get; set; }
        public decimal? ProposedBudget { get; set; }
        public decimal? AgreedBudget { get; set; }
        // ? Explicitly nullable
        public string? ServiceDescription { get; set; }
        // ? Explicitly nullable
        public string? Status { get; set; }
        // ? Explicitly nullable
        public string? OrganizerContactInfo { get; set; }
        // ? Explicitly nullable
        public string? PartnerContactInfo { get; set; }
        // ? Explicitly nullable
        public string? PreferredContactMethod { get; set; }
        // ? Explicitly nullable
        public string? ExternalWorkspaceUrl { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DeadlineDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        // ? Explicitly nullable
        public string? OrganizerNotes { get; set; }
        // ? Explicitly nullable
        public string? PartnershipImage { get; set; }
        // ? CHANGED: SharedNotes (string) -> IsMark (bool?)
        public bool? IsMark { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // ? CHANGED: Event Info is now nullable (can be null if EventId is null)
        public EventBasicInfo? Event { get; set; }

        // Partner Info
        public PartnerBasicInfo Partner { get; set; }
    }

    /// <summary>
    /// Complete event information for partnership response
    /// ? Includes ALL Event fields
    /// </summary>
    public class EventBasicInfo
    {
        public Guid Id { get; set; }
        public Guid OrganizerId { get; set; }
        
        // Basic Info
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? ShortDescription { get; set; }
        public DateTime? EventDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public string? VenueDetails { get; set; }
        
        // Budget & Attendees
        public decimal? TotalBudget { get; set; }
        public int? ExpectedAttendees { get; set; }
        
        // Classification
        public string? Category { get; set; }
        public string? EventType { get; set; }
        public string? TargetAudience { get; set; }
        
        // Requirements
        public string? RequiredServices { get; set; }
        public string? SponsorshipNeeds { get; set; }
        public string? SpecialRequirements { get; set; }
        
        // Status & Visibility
        public string? Status { get; set; }
        public bool? IsPublic { get; set; }
        public bool? IsFeatured { get; set; }
        
        // Media
        public string? CoverImageUrl { get; set; }
        public string? EventImages { get; set; }
        
        // Engagement Metrics
        public int? ViewCount { get; set; }
        public int? InterestedCount { get; set; }
        public decimal? AverageRating { get; set; }
        public int? ReviewCount { get; set; }
        
        // Overview Fields (JSON arrays)
        public string? EventHighlights { get; set; }
        public string? Tags { get; set; }
        public string? TargetAudienceList { get; set; }
        
        // Timestamps
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Basic partner information for partnership response
    /// ? Simplified to only include essential fields
    /// </summary>
    public class PartnerBasicInfo
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
        
        // ? NEW: BrandProfile information
        public BrandProfileBasicInfo? BrandProfile { get; set; }
    }
    
    /// <summary>
    /// Basic brand profile information for partnership response
    /// ? NEW: Include key brand profile details
    /// </summary>
    public class BrandProfileBasicInfo
    {
        public Guid Id { get; set; }
        public string BrandName { get; set; }
        public string BrandLogo { get; set; }
        public string Industry { get; set; }
        public string Location { get; set; }
        public bool? IsPublic { get; set; }
        public bool? HasPartnership { get; set; }
    }
}
