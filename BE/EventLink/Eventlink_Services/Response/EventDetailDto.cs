using System;
using System.Collections.Generic;

namespace Eventlink_Services.Response
{
    /// <summary>
    /// Response DTO for Event Activity (Timeline item)
    /// </summary>
    public class EventActivityDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public string ActivityName { get; set; } = string.Empty;
        public string? ActivityDescription { get; set; }

        // Time as string for easy frontend display
        public string StartTime { get; set; } = string.Empty;  // "09:00"
        public string EndTime { get; set; } = string.Empty;    // "10:00"

        public string? Location { get; set; }
        public string? Speakers { get; set; }
        public string? ActivityType { get; set; }
        public int? MaxParticipants { get; set; }
        public int? CurrentParticipants { get; set; }
        public bool? IsPublic { get; set; }
        public int? DisplayOrder { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Computed properties
        public string TimeRange => $"{StartTime} - {EndTime}";
        public int DurationMinutes
        {
            get
            {
                if (TimeSpan.TryParse(StartTime, out var start) && TimeSpan.TryParse(EndTime, out var end))
                {
                    return (int)(end - start).TotalMinutes;
                }
                return 0;
            }
        }
    }

    /// <summary>
    /// Funding breakdown item for sponsor budget
    /// </summary>
    public class FundingBreakdownItem
    {
        public string Category { get; set; } = string.Empty;  // "Booth Setup", "Media Promotion"
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }

    /// <summary>
    /// Enhanced Event Proposal Response with funding details
    /// </summary>
    public class EventProposalDetailDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public string ProposalType { get; set; } = string.Empty;  // Sponsorship, Supplier
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Requirements { get; set; }
        public decimal? Budget { get; set; }
        public DateTime? Deadline { get; set; }
        public string? ContactInstructions { get; set; }
        public string? AttachmentUrls { get; set; }

        // Sponsor/Proposer info
        public Guid? ProposedBy { get; set; }
        public string? ProposerName { get; set; }
        public string? ProposerEmail { get; set; }
        public string? ProposerAvatar { get; set; }

        // Sponsorship details
        public string? SponsorTier { get; set; }
        public List<FundingBreakdownItem>? FundingBreakdown { get; set; }
        public List<string>? Benefits { get; set; }

        // Status
        public string? Status { get; set; }  // Pending, Approved, Rejected
        public Guid? ApprovedBy { get; set; }
        public string? ApproverName { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectionReason { get; set; }

        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Computed
        public decimal UsedBudgetPercentage
        {
            get
            {
                if (Budget.HasValue && Budget.Value > 0 && FundingBreakdown != null)
                {
                    var totalUsed = 0m;
                    foreach (var item in FundingBreakdown)
                    {
                        totalUsed += item.Amount;
                    }
                    return (totalUsed / Budget.Value) * 100;
                }
                return 0;
            }
        }
    }

    /// <summary>
    /// Enhanced Event Response with timeline and proposals
    /// </summary>
    public class EventDetailDto
    {
        public Guid Id { get; set; }
        public Guid OrganizerId { get; set; }
        public string OrganizerName { get; set; } = string.Empty;
        public string? OrganizerEmail { get; set; }
        public string? OrganizerPhone { get; set; }
        public string? OrganizerWebsite { get; set; }

        // Basic Info
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ShortDescription { get; set; }
        public DateTime? EventDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public string? VenueDetails { get; set; }

        // Budget & Metrics
        public decimal? TotalBudget { get; set; }
        public decimal? TotalSponsorship { get; set; }
        public decimal? RemainingBudget { get; set; }
        public int? ExpectedAttendees { get; set; }

        // Details
        public string? Category { get; set; }
        public string? EventType { get; set; }
        public string? TargetAudience { get; set; }
        public string? RequiredServices { get; set; }
        public string? SponsorshipNeeds { get; set; }
        public string? SpecialRequirements { get; set; }

        // Status & Visibility
        public string? Status { get; set; }
        public bool? IsPublic { get; set; }
        public bool? IsFeatured { get; set; }

        // Media
        public string? CoverImageUrl { get; set; }
        public List<string>? EventImages { get; set; }

        // Engagement
        public int? ViewCount { get; set; }
        public int? InterestedCount { get; set; }
        public decimal? AverageRating { get; set; }
        public int? ReviewCount { get; set; }

        // Dates
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // ✅ NEW: Timeline Activities
        public List<EventActivityDto>? Timeline { get; set; }

        // ✅ NEW: Sponsorship Proposals
        public List<EventProposalDetailDto>? Proposals { get; set; }

        // Computed properties
        public string? Industry => Category;
        public int? DaysUntilEvent
        {
            get
            {
                if (EventDate.HasValue)
                {
                    return (EventDate.Value.Date - DateTime.UtcNow.Date).Days;
                }
                return null;
            }
        }

        public string EventDateFormatted => EventDate?.ToString("MMMM dd-dd, yyyy") ?? string.Empty;
    }
}