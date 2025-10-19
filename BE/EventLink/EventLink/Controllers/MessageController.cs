using Eventlink_Services.Interface;
using Eventlink_Services.Request;
using Eventlink_Services.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventLink.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly ILogger<MessageController> _logger;

        public MessageController(IMessageService messageService, ILogger<MessageController> logger)
        {
            _messageService = messageService;
            _logger = logger;
        }

        /// <summary>
        /// Send a message to another user
        /// </summary>
        [HttpPost("send")]
        public async Task<ActionResult<ApiResponse<MessageDto>>> SendMessage([FromBody] SendMessageRequest request)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<MessageDto>.ErrorResult("Invalid token"));
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).ToList();
                return BadRequest(ApiResponse<MessageDto>.ErrorResult("Validation failed", errors));
            }

            var result = await _messageService.SendMessageAsync(userId.Value, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get conversation with specific user
        /// </summary>
        [HttpGet("conversation/{partnerId}")]
        public async Task<ActionResult<ApiResponse<List<MessageDto>>>> GetConversation(
            Guid partnerId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<List<MessageDto>>.ErrorResult("Invalid token"));
            }

            var result = await _messageService.GetConversationAsync(userId.Value, partnerId, page, pageSize);

            return Ok(result);
        }

        /// <summary>
        /// Get all conversations for current user
        /// </summary>
        [HttpGet("conversations")]
        public async Task<ActionResult<ApiResponse<List<ConversationDto>>>> GetConversations()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<List<ConversationDto>>.ErrorResult("Invalid token"));
            }

            var result = await _messageService.GetUserConversationsAsync(userId.Value);

            return Ok(result);
        }

        /// <summary>
        /// Get message statistics
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<ApiResponse<MessageStatsDto>>> GetMessageStats()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<MessageStatsDto>.ErrorResult("Invalid token"));
            }

            var result = await _messageService.GetMessageStatsAsync(userId.Value);

            return Ok(result);
        }

        /// <summary>
        /// Mark message as read
        /// </summary>
        [HttpPut("{messageId}/read")]
        public async Task<ActionResult<ApiResponse<string>>> MarkAsRead(Guid messageId)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<string>.ErrorResult("Invalid token"));
            }

            var result = await _messageService.MarkMessageAsReadAsync(userId.Value, messageId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Mark entire conversation as read
        /// </summary>
        [HttpPut("conversation/{partnerId}/read")]
        public async Task<ActionResult<ApiResponse<string>>> MarkConversationAsRead(Guid partnerId)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<string>.ErrorResult("Invalid token"));
            }

            var result = await _messageService.MarkConversationAsReadAsync(userId.Value, partnerId);

            return Ok(result);
        }

        /// <summary>
        /// Get unread message count
        /// </summary>
        [HttpGet("unread/count")]
        public async Task<ActionResult<ApiResponse<int>>> GetUnreadCount()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<int>.ErrorResult("Invalid token"));
            }

            var result = await _messageService.GetUnreadCountAsync(userId.Value);

            return Ok(result);
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return Guid.TryParse(userIdClaim, out Guid userId) ? userId : null;
        }
    }
}