using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using Eventlink_Services.Interface;
using Eventlink_Services.Request;
using Eventlink_Services.Response;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Service
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<MessageService> _logger;

        public MessageService(
            IMessageRepository messageRepository,
            IUserRepository userRepository,
            ILogger<MessageService> logger)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<MessageDto>> SendMessageAsync(Guid senderId, SendMessageRequest request)
        {
            try
            {
                // Validate users
                if (senderId == request.ReceiverId)
                {
                    return ApiResponse<MessageDto>.ErrorResult("Cannot send message to yourself");
                }

                if (!await _messageRepository.CanUserSendMessageAsync(senderId, request.ReceiverId))
                {
                    return ApiResponse<MessageDto>.ErrorResult("Cannot send message to this user");
                }

                // Create message
                var message = new Message
                {
                    Id = Guid.NewGuid(),
                    SenderId = senderId,
                    ReceiverId = request.ReceiverId,
                    PartnershipId = request.PartnershipId,
                    Content = request.Content.Trim(),
                    MessageType = request.MessageType,
                    AttachmentUrl = request.AttachmentUrl,
                    AttachmentName = request.AttachmentName,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _messageRepository.AddAsync(message);

                // Get user info for response
                var sender = await _userRepository.GetByIdAsync(senderId);
                var receiver = await _userRepository.GetByIdAsync(request.ReceiverId);

                var messageDto = new MessageDto
                {
                    Id = message.Id,
                    SenderId = message.SenderId,
                    SenderName = sender?.FullName ?? "Unknown",
                    SenderAvatar = sender?.AvatarUrl,
                    ReceiverId = message.ReceiverId,
                    ReceiverName = receiver?.FullName ?? "Unknown",
                    ReceiverAvatar = receiver?.AvatarUrl,
                    PartnershipId = message.PartnershipId,
                    Content = message.Content,
                    MessageType = message.MessageType ?? "Text",
                    AttachmentUrl = message.AttachmentUrl,
                    AttachmentName = message.AttachmentName,
                    IsRead = message.IsRead ?? false,
                    CreatedAt = message.CreatedAt,
                    IsSentByCurrentUser = true
                };

                _logger.LogInformation("Message sent from {SenderId} to {ReceiverId}", senderId, request.ReceiverId);

                return ApiResponse<MessageDto>.SuccessResult(messageDto, "Message sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send message from {SenderId} to {ReceiverId}", senderId, request.ReceiverId);
                return ApiResponse<MessageDto>.ErrorResult($"Failed to send message: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<MessageDto>>> GetConversationAsync(Guid userId, Guid partnerId, int page, int pageSize)
        {
            try
            {
                var messages = await _messageRepository.GetConversationMessagesAsync(userId, partnerId, page, pageSize);

                var messageDtos = messages.Select(m => new MessageDto
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    SenderName = m.Sender?.FullName ?? "Unknown",
                    SenderAvatar = m.Sender?.AvatarUrl,
                    ReceiverId = m.ReceiverId,
                    ReceiverName = m.Receiver?.FullName ?? "Unknown",
                    ReceiverAvatar = m.Receiver?.AvatarUrl,
                    PartnershipId = m.PartnershipId,
                    Content = m.Content ?? string.Empty,
                    MessageType = m.MessageType ?? "Text",
                    AttachmentUrl = m.AttachmentUrl,
                    AttachmentName = m.AttachmentName,
                    IsRead = m.IsRead ?? false,
                    CreatedAt = m.CreatedAt,
                    IsSentByCurrentUser = m.SenderId == userId
                }).Reverse().ToList(); // Reverse to show oldest first

                return ApiResponse<List<MessageDto>>.SuccessResult(messageDtos, "Conversation retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get conversation between {UserId} and {PartnerId}", userId, partnerId);
                return ApiResponse<List<MessageDto>>.ErrorResult($"Failed to retrieve conversation: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<ConversationDto>>> GetUserConversationsAsync(Guid userId)
        {
            try
            {
                var partnerIds = await _messageRepository.GetUserConversationPartnersAsync(userId);
                var conversations = new List<ConversationDto>();

                foreach (var partnerId in partnerIds)
                {
                    var partner = await _userRepository.GetByIdAsync(partnerId);
                    if (partner == null) continue;

                    var lastMessage = await _messageRepository.GetLastMessageWithUserAsync(userId, partnerId);
                    var unreadCount = await _messageRepository.GetUnreadCountFromUserAsync(userId, partnerId);

                    var conversation = new ConversationDto
                    {
                        PartnerId = partnerId,
                        PartnerName = partner.FullName ?? "Unknown",
                        PartnerAvatar = partner.AvatarUrl,
                        PartnerRole = partner.Role ?? "Unknown",
                        UnreadCount = unreadCount,
                        LastMessageTime = lastMessage?.CreatedAt
                    };

                    if (lastMessage != null)
                    {
                        conversation.LastMessage = new MessageDto
                        {
                            Id = lastMessage.Id,
                            Content = lastMessage.Content ?? string.Empty,
                            MessageType = lastMessage.MessageType ?? "Text",
                            CreatedAt = lastMessage.CreatedAt,
                            IsSentByCurrentUser = lastMessage.SenderId == userId,
                            IsRead = lastMessage.IsRead ?? false
                        };
                    }

                    conversations.Add(conversation);
                }

                // Sort by last message time
                conversations = conversations
                    .OrderByDescending(c => c.LastMessageTime)
                    .ToList();

                return ApiResponse<List<ConversationDto>>.SuccessResult(conversations, "Conversations retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get conversations for user {UserId}", userId);
                return ApiResponse<List<ConversationDto>>.ErrorResult($"Failed to retrieve conversations: {ex.Message}");
            }
        }

        public async Task<ApiResponse<MessageStatsDto>> GetMessageStatsAsync(Guid userId)
        {
            try
            {
                var unreadCount = await _messageRepository.GetUnreadCountAsync(userId);
                var partnerIds = await _messageRepository.GetUserConversationPartnersAsync(userId);

                var stats = new MessageStatsDto
                {
                    TotalConversations = partnerIds.Count,
                    UnreadMessages = unreadCount,
                    TodayMessages = 0 // Can be implemented if needed
                };

                return ApiResponse<MessageStatsDto>.SuccessResult(stats, "Message stats retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get message stats for user {UserId}", userId);
                return ApiResponse<MessageStatsDto>.ErrorResult($"Failed to retrieve message stats: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> MarkMessageAsReadAsync(Guid userId, Guid messageId)
        {
            try
            {
                var message = await _messageRepository.GetByIdAsync(messageId);

                if (message == null)
                {
                    return ApiResponse<string>.ErrorResult("Message not found");
                }

                if (message.ReceiverId != userId)
                {
                    return ApiResponse<string>.ErrorResult("You cannot mark this message as read");
                }

                await _messageRepository.MarkAsReadAsync(messageId);

                return ApiResponse<string>.SuccessResult("", "Message marked as read");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark message {MessageId} as read", messageId);
                return ApiResponse<string>.ErrorResult($"Failed to mark message as read: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> MarkConversationAsReadAsync(Guid userId, Guid partnerId)
        {
            try
            {
                await _messageRepository.MarkConversationAsReadAsync(userId, partnerId);

                return ApiResponse<string>.SuccessResult("", "Conversation marked as read");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark conversation as read for user {UserId} with partner {PartnerId}", userId, partnerId);
                return ApiResponse<string>.ErrorResult($"Failed to mark conversation as read: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> GetUnreadCountAsync(Guid userId)
        {
            try
            {
                var count = await _messageRepository.GetUnreadCountAsync(userId);
                return ApiResponse<int>.SuccessResult(count, "Unread count retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get unread count for user {UserId}", userId);
                return ApiResponse<int>.ErrorResult($"Failed to get unread count: {ex.Message}");
            }
        }
    }
}