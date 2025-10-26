using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventLink_Repositories.Models
{
    /// <summary>
    /// Event entity - FIXED all errors
    /// </summary>
    [Table("Events")]
    public partial class Event
    {
        public Guid Id { get; set; }
        public Guid OrganizerId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? ShortDescription { get; set; }
        public DateTime? EventDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public string? VenueDetails { get; set; }
        public decimal? TotalBudget { get; set; }
        public int? ExpectedAttendees { get; set; }
        public string? Category { get; set; }
        public string? EventType { get; set; }
        public string? TargetAudience { get; set; }
        public string? RequiredServices { get; set; }
        public string? SponsorshipNeeds { get; set; }
        public string? SpecialRequirements { get; set; }
        public string? Status { get; set; }
        public bool? IsPublic { get; set; }
        public bool? IsFeatured { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? EventImages { get; set; }
        public int? ViewCount { get; set; }
        public int? InterestedCount { get; set; }
        public decimal? AverageRating { get; set; }
        public int? ReviewCount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // ✅ NEW FIELDS FOR OVERVIEW (added by ALTER TABLE script)
        /// <summary>
        /// JSON array of event highlights - "What to Expect"
        /// Example: ["50+ Expert Speakers", "Interactive Workshops"]
        /// </summary>
        public string? EventHighlights { get; set; }

        /// <summary>
        /// JSON array of tags/keywords
        /// Example: ["Artificial Intelligence", "Machine Learning"]
        /// </summary>
        public string? Tags { get; set; }

        /// <summary>
        /// JSON array of target audience segments
        /// Example: ["Tech executives", "Entrepreneurs"]
        /// </summary>
        public string? TargetAudienceList { get; set; }

        // Navigation properties
        public virtual User? Organizer { get; set; }
        public virtual ICollection<EventActivity>? EventActivities { get; set; }
        public virtual ICollection<EventProposal>? EventProposals { get; set; }

        // ❌ REMOVED: Partnerships navigation (doesn't exist in database)
        // public virtual ICollection<Partnership>? Partnerships { get; set; }
    }
}