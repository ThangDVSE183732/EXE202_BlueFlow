using System.ComponentModel.DataAnnotations;

namespace EventLink_Services.DTOs.Requests
{
    public class SendMessageRequest
    {
        [Required(ErrorMessage = "Receiver ID is required")]
        public Guid ReceiverId { get; set; }

        [Required(ErrorMessage = "Message content is required")]
        [MaxLength(5000, ErrorMessage = "Message cannot exceed 5000 characters")]
        public string Content { get; set; } = string.Empty;

        public Guid? PartnershipId { get; set; }

        [RegularExpression("^(Text|Image|File|Contact|System)$",
            ErrorMessage = "MessageType must be: Text, Image, File, Contact, or System")]
        public string MessageType { get; set; } = "Text"; // Default value

        public string? AttachmentUrl { get; set; }

        public string? AttachmentName { get; set; }
    }
}