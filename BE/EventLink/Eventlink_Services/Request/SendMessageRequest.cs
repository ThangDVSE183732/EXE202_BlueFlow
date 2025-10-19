using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Request
{
    public class SendMessageRequest
    {
        [Required(ErrorMessage = "Receiver ID is required")]
        public Guid ReceiverId { get; set; }

        [Required(ErrorMessage = "Message content is required")]
        [MaxLength(5000, ErrorMessage = "Message cannot exceed 5000 characters")]
        public string Content { get; set; } = string.Empty;

        public Guid? PartnershipId { get; set; }

        [MaxLength(20)]
        public string MessageType { get; set; } = "Text"; // Text, Image, File, Contact, System

        public string? AttachmentUrl { get; set; }

        public string? AttachmentName { get; set; }
    }

    public class GetMessagesRequest
    {
        public Guid? PartnerId { get; set; }
        public Guid? PartnershipId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public bool UnreadOnly { get; set; } = false;
    }
}
