using Eventlink_Services.Ultility;
using System;
using System.Collections.Generic;

namespace Eventlink_Services.Response
{
    /// <summary>
    /// Data Transfer Object for Message
    /// </summary>
    public class MessageDto
    {
        public Guid Id { get; set; }

        // Sender Information
        public Guid SenderId { get; set; }
        public string? SenderName { get; set; }
        public string? SenderAvatar { get; set; }

        // Receiver Information
        public Guid ReceiverId { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverAvatar { get; set; }

        // Message Content
        public Guid? PartnershipId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string MessageType { get; set; } = "Text";

        // Attachments
        public string? AttachmentUrl { get; set; }
        public string? AttachmentName { get; set; }

        // Status
        public bool IsRead { get; set; }
        public bool IsSentByCurrentUser { get; set; }

        // Timestamp (UTC)
        public DateTime CreatedAt { get; set; }

        // ============ HELPER METHOD FOR TIMEZONE CONVERSION ============
        private DateTime LocalTime => TimeZoneHelper.ToLocalTime(CreatedAt);

        // ============ FORMATTED TIME PROPERTIES FOR FRONTEND ============

        /// <summary>
        /// Time only in 24-hour format: "02:16" (Vietnam timezone)
        /// </summary>
        public string FormattedTime => LocalTime.ToString("HH:mm");

        /// <summary>
        /// Date only: "23/10/2025" (Vietnam timezone)
        /// </summary>
        public string FormattedDate => LocalTime.ToString("dd/MM/yyyy");

        /// <summary>
        /// Full datetime: "23/10/2025 02:16" (Vietnam timezone)
        /// </summary>
        public string FormattedDateTime => LocalTime.ToString("dd/MM/yyyy HH:mm");

        /// <summary>
        /// 12-hour format with AM/PM: "02:16 AM" (Vietnam timezone)
        /// </summary>
        public string FormattedTime12h => LocalTime.ToString("hh:mm tt");

        /// <summary>
        /// Relative time for better UX
        /// </summary>
        public string RelativeTime
        {
            get
            {
                var nowLocal = TimeZoneHelper.GetCurrentLocalTime();
                var timeSpan = nowLocal - LocalTime;

                if (timeSpan.TotalSeconds < 30)
                    return "Just now";
                if (timeSpan.TotalMinutes < 1)
                    return "Less than a minute ago";
                if (timeSpan.TotalMinutes < 60)
                    return $"{(int)timeSpan.TotalMinutes}m ago";
                if (timeSpan.TotalHours < 24)
                    return $"{(int)timeSpan.TotalHours}h ago";
                if (timeSpan.TotalDays < 7)
                    return $"{(int)timeSpan.TotalDays}d ago";
                if (timeSpan.TotalDays < 30)
                    return $"{(int)(timeSpan.TotalDays / 7)}w ago";

                return LocalTime.ToString("dd/MM/yyyy");
            }
        }

        /// <summary>
        /// Short day name: "Mon", "Tue", "Wed"
        /// </summary>
        public string DayOfWeek => LocalTime.ToString("ddd");

        /// <summary>
        /// Check if message was sent today
        /// </summary>
        public bool IsToday => LocalTime.Date == TimeZoneHelper.GetCurrentLocalTime().Date;

        /// <summary>
        /// Check if message was sent yesterday
        /// </summary>
        public bool IsYesterday => LocalTime.Date == TimeZoneHelper.GetCurrentLocalTime().Date.AddDays(-1);
    }

    /// <summary>
    /// Data Transfer Object for Conversation
    /// </summary>
    public class ConversationDto
    {
        public Guid PartnerId { get; set; }
        public string PartnerName { get; set; } = string.Empty;
        public string? PartnerAvatar { get; set; }
        public string PartnerRole { get; set; } = string.Empty;
        public int UnreadCount { get; set; }
        public DateTime? LastMessageTime { get; set; }
        public MessageDto? LastMessage { get; set; }

        // ============ HELPER METHOD FOR TIMEZONE CONVERSION ============
        private DateTime? LastMessageLocalTime => LastMessageTime.HasValue
            ? TimeZoneHelper.ToLocalTime(LastMessageTime.Value)
            : null;

        // ============ FORMATTED TIME PROPERTIES FOR FRONTEND ============

        /// <summary>
        /// Last message time only: "02:16"
        /// </summary>
        public string? LastMessageFormattedTime => LastMessageLocalTime?.ToString("HH:mm");

        /// <summary>
        /// Last message date only: "23/10/2025"
        /// </summary>
        public string? LastMessageFormattedDate => LastMessageLocalTime?.ToString("dd/MM/yyyy");

        /// <summary>
        /// Last message 12-hour format: "02:16 AM"
        /// </summary>
        public string? LastMessageFormattedTime12h => LastMessageLocalTime?.ToString("hh:mm tt");

        /// <summary>
        /// Relative time for conversation list
        /// </summary>
        public string? LastMessageRelativeTime
        {
            get
            {
                if (!LastMessageLocalTime.HasValue)
                    return null;

                var nowLocal = TimeZoneHelper.GetCurrentLocalTime();
                var timeSpan = nowLocal - LastMessageLocalTime.Value;

                if (timeSpan.TotalSeconds < 30)
                    return "Now";
                if (timeSpan.TotalMinutes < 1)
                    return "<1m";
                if (timeSpan.TotalMinutes < 60)
                    return $"{(int)timeSpan.TotalMinutes}m";
                if (timeSpan.TotalHours < 24)
                    return $"{(int)timeSpan.TotalHours}h";
                if (timeSpan.TotalDays < 7)
                    return $"{(int)timeSpan.TotalDays}d";
                if (timeSpan.TotalDays < 30)
                    return $"{(int)(timeSpan.TotalDays / 7)}w";

                return LastMessageLocalTime.Value.ToString("dd/MM");
            }
        }

        /// <summary>
        /// Smart display: "02:16" today, "Yesterday", "Mon", or "dd/MM"
        /// </summary>
        public string? LastMessageSmartDisplay
        {
            get
            {
                if (!LastMessageLocalTime.HasValue)
                    return null;

                var nowLocal = TimeZoneHelper.GetCurrentLocalTime();
                var messageDate = LastMessageLocalTime.Value.Date;

                if (messageDate == nowLocal.Date)
                    return LastMessageLocalTime.Value.ToString("HH:mm");
                if (messageDate == nowLocal.Date.AddDays(-1))
                    return "Yesterday";
                if (messageDate > nowLocal.Date.AddDays(-7))
                    return LastMessageLocalTime.Value.ToString("ddd");

                return LastMessageLocalTime.Value.ToString("dd/MM");
            }
        }

        /// <summary>
        /// Check if last message was today
        /// </summary>
        public bool LastMessageIsToday
        {
            get
            {
                if (!LastMessageLocalTime.HasValue) return false;
                return LastMessageLocalTime.Value.Date == TimeZoneHelper.GetCurrentLocalTime().Date;
            }
        }

        /// <summary>
        /// Check if last message was yesterday
        /// </summary>
        public bool LastMessageIsYesterday
        {
            get
            {
                if (!LastMessageLocalTime.HasValue) return false;
                return LastMessageLocalTime.Value.Date == TimeZoneHelper.GetCurrentLocalTime().Date.AddDays(-1);
            }
        }
    }

    /// <summary>
    /// Data Transfer Object for Message Statistics
    /// </summary>
    public class MessageStatsDto
    {
        public int TotalConversations { get; set; }
        public int UnreadMessages { get; set; }
        public int TodayMessages { get; set; }
        public List<ConversationDto> RecentConversations { get; set; } = new();
    }
}