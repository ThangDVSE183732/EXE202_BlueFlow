using Eventlink_Services.Request;
using Eventlink_Services.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Interface
{
    public interface IMessageService
    {
        Task<ApiResponse<MessageDto>> SendMessageAsync(Guid senderId, SendMessageRequest request);
        Task<ApiResponse<List<MessageDto>>> GetConversationAsync(Guid userId, Guid partnerId, int page, int pageSize);
        Task<ApiResponse<List<ConversationDto>>> GetUserConversationsAsync(Guid userId);
        Task<ApiResponse<MessageStatsDto>> GetMessageStatsAsync(Guid userId);
        Task<ApiResponse<string>> MarkMessageAsReadAsync(Guid userId, Guid messageId);
        Task<ApiResponse<string>> MarkConversationAsReadAsync(Guid userId, Guid partnerId);
        Task<ApiResponse<int>> GetUnreadCountAsync(Guid userId);
    }
}
