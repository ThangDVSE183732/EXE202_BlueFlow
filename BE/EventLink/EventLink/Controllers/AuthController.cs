using Eventlink_Services.Interface;
using Eventlink_Services.Request;
using Eventlink_Services.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventLink.Controllers
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

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="request">Registration information</param>
        /// <returns>Authentication response with JWT token</returns>
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

        /// <summary>
        /// Login user
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>Authentication response with JWT token</returns>
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
        /// Get current authenticated user information
        /// </summary>
        /// <returns>Current user information</returns>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetCurrentUser()
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

        /// <summary>
        /// Logout current user
        /// </summary>
        /// <returns>Logout confirmation</returns>
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

        /// <summary>
        /// Check if email exists (for registration form validation)
        /// </summary>
        /// <param name="email">Email to check</param>
        /// <returns>Boolean indicating if email exists</returns>
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
