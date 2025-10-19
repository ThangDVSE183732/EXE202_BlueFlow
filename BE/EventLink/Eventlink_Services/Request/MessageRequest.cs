using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Request
{
    public class MessageRequest
    {
        public Guid ReceiverId { get; set; }

        public Guid? PartnershipId { get; set; }

        public string Content { get; set; }

        public string AttachmentUrl { get; set; }

        public string AttachmentName { get; set; }
    }
}
