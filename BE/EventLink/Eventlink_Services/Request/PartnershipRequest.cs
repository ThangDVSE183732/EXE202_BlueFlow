using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Request
{
    public class PartnershipRequest
    {
        public class CreatePartnershipRequest
        {
            public Guid EventId { get; set; }
            public Guid PartnerId { get; set; }
            public string PartnerType { get; set; }
            public string InitialMessage { get; set; }
            public decimal? ProposedBudget { get; set; }
            public string ServiceDescription { get; set; }
            public string PreferredContactMethod { get; set; }
            public string OrganizerContactInfo { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? DeadlineDate { get; set; }
        }

        public class UpdatePartnershipStatusRequest
        {
            public string Status { get; set; }
            public string OrganizerResponse { get; set; }
        }

        public class UpdatePartnershipRequest
        {
            public decimal? AgreedBudget { get; set; }
            public string OrganizerNotes { get; set; }
            public string PartnerNotes { get; set; }
            public string SharedNotes { get; set; }
            public DateTime? CompletionDate { get; set; }
        }
    }
}
