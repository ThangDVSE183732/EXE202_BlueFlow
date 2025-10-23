using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Eventlink_Services.Request
{
    /// <summary>
    /// Request to create/update timeline activity
    /// </summary>
    public class EventActivityRequest
    {
        [Required(ErrorMessage = "Activity name is required")]
        [StringLength(255)]
        public string ActivityName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? ActivityDescription { get; set; }

        [Required(ErrorMessage = "Start time is required")]
        [RegularExpression(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Invalid time format. Use HH:mm")]
        public string StartTime { get; set; } = string.Empty;  // "09:00"

        [Required(ErrorMessage = "End time is required")]
        [RegularExpression(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Invalid time format. Use HH:mm")]
        public string EndTime { get; set; } = string.Empty;    // "10:00"

        [StringLength(255)]
        public string? Location { get; set; }

        [StringLength(500)]
        public string? Speakers { get; set; }

        [StringLength(50)]
        public string? ActivityType { get; set; }

        [Range(0, 100000)]
        public int? MaxParticipants { get; set; }

        public bool? IsPublic { get; set; } = true;

        public int? DisplayOrder { get; set; }
    }

    /// <summary>
    /// Request to create/update sponsorship proposal
    /// </summary>
    public class SponsorshipProposalRequest
    {
        [Required(ErrorMessage = "Event ID is required")]
        public Guid EventId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Budget is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Budget must be greater than 0")]
        public decimal Budget { get; set; }

        [Required(ErrorMessage = "Sponsor tier is required")]
        [StringLength(50)]
        public string SponsorTier { get; set; } = string.Empty;  // Silver, Gold, Platinum

        /// <summary>
        /// Funding breakdown: { "BoothSetup": 3000000, "MediaPromotion": 2500000, ... }
        /// </summary>
        public Dictionary<string, decimal>? FundingBreakdown { get; set; }

        /// <summary>
        /// Benefits: ["Logo on select materials", "Small booth space", ...]
        /// </summary>
        public List<string>? Benefits { get; set; }

        public string? ContactInstructions { get; set; }
        public string? AttachmentUrls { get; set; }
    }

    /// <summary>
    /// Request to approve/reject proposal (Organizer only)
    /// </summary>
    public class UpdateProposalStatusRequest
    {
        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("Approved|Rejected", ErrorMessage = "Status must be 'Approved' or 'Rejected'")]
        public string Status { get; set; } = string.Empty;

        public string? RejectionReason { get; set; }
    }

    /// <summary>
    /// Request to create event with all details
    /// </summary>
    public class CreateEventWithDetailsRequest : EventRequest.CreateEventRequest
    {
        /// <summary>
        /// Timeline activities to create with event
        /// </summary>
        public List<EventActivityRequest>? InitialActivities { get; set; }
    }

    /// <summary>
    /// Request to update event with timeline
    /// </summary>
    public class UpdateEventWithDetailsRequest : EventRequest.UpdateEventRequest
    {
        /// <summary>
        /// Updated timeline activities (will replace existing)
        /// </summary>
        public List<EventActivityRequest>? UpdatedActivities { get; set; }
    }
}