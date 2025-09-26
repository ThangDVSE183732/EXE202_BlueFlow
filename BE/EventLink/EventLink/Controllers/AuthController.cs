using Eventlink_Services.Interface;
using Eventlink_Services.Request;
using Eventlink_Services.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EventLink_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .SelectMany(x => x.Value.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<AuthResponse>.ErrorResult("Validation failed", errors));
            }

            var result = await _authService.RegisterAsync(request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .SelectMany(x => x.Value.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<AuthResponse>.ErrorResult("Validation failed", errors));
            }

            var result = await _authService.LoginAsync(request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get current user info from JWT claims (Fast - No DB query)
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public ActionResult<ApiResponse<UserDto>> GetCurrentUser()
        {
            try
            {
                // Lấy thông tin từ JWT claims thay vì query database
                var userIdClaim = User.FindFirst("UserId")?.Value;
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var fullName = User.FindFirst(ClaimTypes.Name)?.Value;
                var role = User.FindFirst("Role")?.Value;
                var emailVerified = User.FindFirst("EmailVerified")?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(ApiResponse<UserDto>.ErrorResult("Invalid token"));
                }

                var userDto = new UserDto
                {
                    Id = userId,
                    Email = email ?? string.Empty,
                    FullName = fullName ?? string.Empty,
                    Role = role ?? string.Empty,
                    EmailVerified = bool.TryParse(emailVerified, out bool isVerified) && isVerified,
                    // Claims không chứa: PhoneNumber, AvatarUrl, CreatedAt, LastLoginAt
                    // Dùng /profile endpoint nếu cần thông tin đầy đủ
                };

                return Ok(ApiResponse<UserDto>.SuccessResult(userDto, "User retrieved from token"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve current user from claims");
                return StatusCode(500, ApiResponse<UserDto>.ErrorResult("Failed to retrieve user information"));
            }
        }

        /// <summary>
        /// Get complete user profile (with database query for full info)
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUserProfile()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized(ApiResponse<UserDto>.ErrorResult("Invalid token"));
            }

            var result = await _authService.GetCurrentUserAsync(userId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<string>>> Logout()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized(ApiResponse<string>.ErrorResult("Invalid token"));
            }

            var result = await _authService.LogoutAsync(userId);

            return Ok(result);
        }

        [HttpGet("check-email")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckEmailExists([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(ApiResponse<bool>.ErrorResult("Email is required"));
            }

            var result = await _authService.CheckEmailExistsAsync(email);

            return Ok(result);
        }
    }
}