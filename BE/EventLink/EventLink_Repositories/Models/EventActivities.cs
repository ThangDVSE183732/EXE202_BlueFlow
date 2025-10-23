using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventLink_Repositories.Models
{
    [Table("EventActivities")]
    public class EventActivity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid EventId { get; set; }

        [Required]
        [StringLength(255)]
        public string ActivityName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? ActivityDescription { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }  // 09:00, 10:00

        [Required]
        public TimeSpan EndTime { get; set; }    // 10:00, 11:30

        [StringLength(255)]
        public string? Location { get; set; }

        [StringLength(500)]
        public string? Speakers { get; set; }

        [StringLength(50)]
        public string? ActivityType { get; set; }  // Registration, Keynote, Panel, Workshop, etc.

        public int? MaxParticipants { get; set; }

        public int? CurrentParticipants { get; set; } = 0;

        public bool? IsPublic { get; set; } = true;

        public int? DisplayOrder { get; set; } = 0;

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("EventId")]
        public virtual Event? Event { get; set; }
    }
}