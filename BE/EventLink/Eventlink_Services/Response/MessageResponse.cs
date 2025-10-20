using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Response
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string? SenderAvatar { get; set; }
        public Guid ReceiverId { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string? ReceiverAvatar { get; set; }
        public Guid? PartnershipId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string MessageType { get; set; } = "Text";
        public string? AttachmentUrl { get; set; }
        public string? AttachmentName { get; set; }
        public bool IsRead { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool IsSentByCurrentUser { get; set; }
    }

    public class ConversationDto
    {
        public Guid PartnerId { get; set; }
        public string PartnerName { get; set; } = string.Empty;
        public string? PartnerAvatar { get; set; }
        public string PartnerRole { get; set; } = string.Empty;
        public MessageDto? LastMessage { get; set; }
        public int UnreadCount { get; set; }
        public DateTime? LastMessageTime { get; set; }
    }

    public class MessageStatsDto
    {
        public int TotalConversations { get; set; }
        public int UnreadMessages { get; set; }
        public int TodayMessages { get; set; }
        public List<ConversationDto> RecentConversations { get; set; } = new();
    }
}
