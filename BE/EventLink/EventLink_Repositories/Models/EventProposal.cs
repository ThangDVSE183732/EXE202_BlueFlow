using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventLink_Repositories.Models
{
    [Table("EventProposals")]
    public class EventProposal
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid EventId { get; set; }

        /// <summary>
        /// Sponsorship or Supplier
        /// </summary>
        [Required]
        [StringLength(20)]
        public string ProposalType { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Requirements { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Budget { get; set; }

        public DateTime? Deadline { get; set; }

        [StringLength(1000)]
        public string? ContactInstructions { get; set; }

        public string? AttachmentUrls { get; set; }

        // ✅ NEW FIELDS FOR SPONSORSHIP
        /// <summary>
        /// Sponsor/Supplier who made this proposal
        /// </summary>
        public Guid? ProposedBy { get; set; }

        /// <summary>
        /// Silver, Gold, Platinum
        /// </summary>
        [StringLength(50)]
        public string? SponsorTier { get; set; }

        /// <summary>
        /// JSON: {"BoothSetup": 30, "MediaPromotion": 25, "MerchandiseAndGifts": 20, ...}
        /// </summary>
        public string? FundingBreakdown { get; set; }

        /// <summary>
        /// JSON array: ["Logo on select materials", "Small booth space", ...]
        /// </summary>
        public string? Benefits { get; set; }

        /// <summary>
        /// Pending, Approved, Rejected
        /// </summary>
        [StringLength(20)]
        public string? Status { get; set; } = "Pending";

        /// <summary>
        /// Organizer who approved/rejected
        /// </summary>
        public Guid? ApprovedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }

        [StringLength(1000)]
        public string? RejectionReason { get; set; }

        public bool? IsActive { get; set; } = true;

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("EventId")]
        public virtual Event? Event { get; set; }

        [ForeignKey("ProposedBy")]
        public virtual User? Proposer { get; set; }

        [ForeignKey("ApprovedBy")]
        public virtual User? Approver { get; set; }
    }
}