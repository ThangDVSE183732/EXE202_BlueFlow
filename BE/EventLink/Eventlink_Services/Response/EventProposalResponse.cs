using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Response
{
    public class EventProposalResponse
    {
        public Guid EventId { get; set; }
        public string ProposalType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Requirements { get; set; }
        public decimal? Budget { get; set; }
        public DateTime? Deadline { get; set; }
        public string ContactInstructions { get; set; }
        public string AttachmentUrls { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
