using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Request
{
    public class PartnershipRequest
    {
        /// <summary>
        /// Form request for creating partnership with file upload
        /// </summary>
        public class CreatePartnershipFormRequest
        {
            // ✅ CHANGED: EventId is now optional (nullable)
            public Guid? EventId { get; set; }
            
            [Required]
            public Guid PartnerId { get; set; }
            
            [Required]
            [MaxLength(20)]
            public string PartnerType { get; set; }
            
            // ✅ Explicitly nullable
            public string? InitialMessage { get; set; }
            
            public decimal? ProposedBudget { get; set; }
            
            // ✅ Explicitly nullable
            public string? ServiceDescription { get; set; }
            
            [MaxLength(100)]
            // ✅ Explicitly nullable
            public string? PreferredContactMethod { get; set; }
            
            // ✅ Explicitly nullable
            public string? OrganizerContactInfo { get; set; }
            
            public DateTime? StartDate { get; set; }
            
            public DateTime? DeadlineDate { get; set; }
            
            // ✅ File upload for partnership image
            public string? PartnershipImageFile { get; set; }
        }

        /// <summary>
        /// JSON request for creating partnership (backward compatibility)
        /// </summary>
        public class CreatePartnershipRequest
        {
            // ✅ CHANGED: EventId is now optional (nullable)
            public Guid? EventId { get; set; }
            public Guid PartnerId { get; set; }
            public string PartnerType { get; set; }
            // ✅ Explicitly nullable
            public string? InitialMessage { get; set; }
            public decimal? ProposedBudget { get; set; }
            // ✅ Explicitly nullable
            public string? ServiceDescription { get; set; }
            // ✅ Explicitly nullable
            public string? PreferredContactMethod { get; set; }
            // ✅ Explicitly nullable
            public string? OrganizerContactInfo { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? DeadlineDate { get; set; }
            // ✅ Explicitly nullable
            public string? PartnershipImage { get; set; }
        }

        public class UpdatePartnershipStatusRequest
        {
            public string Status { get; set; }
            // ✅ Explicitly nullable
            public string? OrganizerResponse { get; set; }
            // ✅ Explicitly nullable
            public string? PartnershipImage { get; set; }
        }

        public class UpdatePartnershipRequest
        {
            public decimal? AgreedBudget { get; set; }
            // ✅ Explicitly nullable
            public string? OrganizerNotes { get; set; }
            // ✅ Explicitly nullable
            public string? PartnershipImage { get; set; }
            // ✅ CHANGED: SharedNotes (string) -> IsMark (bool?)
            public bool? IsMark { get; set; }
            public DateTime? CompletionDate { get; set; }
        }
    }
}
