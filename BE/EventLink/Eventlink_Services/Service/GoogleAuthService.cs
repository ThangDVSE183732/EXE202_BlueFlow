using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using Eventlink_Services.Interface;
using Eventlink_Services.Request;
using Eventlink_Services.Response;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Service
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly ILogger<GoogleAuthService> _logger;

        public GoogleAuthService(
            IConfiguration configuration,
            IUserRepository userRepository,
            IJwtService jwtService,
            ILogger<GoogleAuthService> logger)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<ApiResponse<GoogleUserInfo>> VerifyGoogleTokenAsync(string idToken)
        {
            try
            {
                var clientId = _configuration["GoogleAuth:ClientId"];

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { clientId }
                });

                var googleUserInfo = new GoogleUserInfo
                {
                    Email = payload.Email,
                    Name = payload.Name,
                    Picture = payload.Picture,
                    GoogleId = payload.Subject,
                    EmailVerified = payload.EmailVerified
                };

                return ApiResponse<GoogleUserInfo>.SuccessResult(googleUserInfo, "Google token verified successfully");
            }
            catch (InvalidJwtException ex)
            {
                _logger.LogWarning(ex, "Invalid Google JWT token");
                return ApiResponse<GoogleUserInfo>.ErrorResult("Invalid Google token");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying Google token");
                return ApiResponse<GoogleUserInfo>.ErrorResult("Failed to verify Google token");
            }
        }

        public async Task<ApiResponse<AuthResponse>> AuthenticateWithGoogleAsync(GoogleAuthRequest request)
        {
            try
            {
                // Verify Google token
                var verificationResult = await VerifyGoogleTokenAsync(request.IdToken);
                if (!verificationResult.Success)
                {
                    return ApiResponse<AuthResponse>.ErrorResult(verificationResult.Message);
                }

                var googleUser = verificationResult.Data!;

                // Check if user exists
                var existingUser = await _userRepository.GetByEmailAsync(googleUser.Email);

                User user;

                if (existingUser != null)
                {
                    // Update existing user's Google info if needed
                    if (string.IsNullOrEmpty(existingUser.AvatarUrl) && !string.IsNullOrEmpty(googleUser.Picture))
                    {
                        existingUser.AvatarUrl = googleUser.Picture;
                    }

                    if (!existingUser.EmailVerified.GetValueOrDefault())
                    {
                        existingUser.EmailVerified = googleUser.EmailVerified;
                    }

                    existingUser.LastLoginAt = DateTime.UtcNow;
                    user = await _userRepository.UpdateUserAsync(existingUser);

                    _logger.LogInformation("Existing user logged in with Google: {Email}", user.Email);
                }
                else
                {
                    // Create new user
                    user = new User
                    {
                        Email = googleUser.Email.ToLower().Trim(),
                        FullName = googleUser.Name,
                        Role = request.Role,
                        AvatarUrl = googleUser.Picture,
                        PasswordHash = Guid.NewGuid().ToString(), // Random password for Google users
                        EmailVerified = googleUser.EmailVerified,
                        LastLoginAt = DateTime.UtcNow
                    };

                    user = await _userRepository.CreateUserAsync(user);

                    _logger.LogInformation("New user registered with Google: {Email}, Role: {Role}", user.Email, user.Role);
                }

                // Generate JWT token
                var token = _jwtService.GenerateToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();

                var authResponse = new AuthResponse
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    Expires = DateTime.UtcNow.AddHours(24),
                    User = MapToUserDto(user)
                };

                return ApiResponse<AuthResponse>.SuccessResult(authResponse, "Google authentication successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Google authentication failed");
                return ApiResponse<AuthResponse>.ErrorResult($"Google authentication failed: {ex.Message}");
            }
        }

        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
                Role = user.Role ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                AvatarUrl = user.AvatarUrl,
                EmailVerified = user.EmailVerified ?? false,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };
        }
    }
}
