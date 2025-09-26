// Add these using statements
using BCrypt.Net;
using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using Eventlink_Services.Interface;
using Eventlink_Services.Request;
using Eventlink_Services.Response;
using Microsoft.Extensions.Logging;

namespace EventLink_Services.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IJwtService jwtService,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // Validate role
                if (!await _userRepository.IsValidRoleAsync(request.Role))
                {
                    return ApiResponse<AuthResponse>.ErrorResult("Invalid role. Must be Organizer, Supplier, or Sponsor");
                }

                // Check if email already exists
                if (await _userRepository.EmailExistsAsync(request.Email))
                {
                    return ApiResponse<AuthResponse>.ErrorResult("Email already exists");
                }

                // Create new user
                var user = new User
                {
                    Email = request.Email.ToLower().Trim(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    FullName = request.FullName.Trim(),
                    Role = request.Role,
                    PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim(),
                };

                var createdUser = await _userRepository.CreateUserAsync(user);

                // Generate JWT tokens
                var token = _jwtService.GenerateToken(createdUser);
                var refreshToken = _jwtService.GenerateRefreshToken();

                var authResponse = new AuthResponse
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    Expires = DateTime.UtcNow.AddHours(24),
                    User = MapToUserDto(createdUser)
                };

                _logger.LogInformation("User registered successfully: {Email}, Role: {Role}", createdUser.Email, createdUser.Role);

                return ApiResponse<AuthResponse>.SuccessResult(authResponse, "Registration successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for email: {Email}", request.Email);
                return ApiResponse<AuthResponse>.ErrorResult($"Registration failed: {ex.Message}");
            }
        }

        public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                // Find user by email
                var user = await _userRepository.GetActiveUserByEmailAsync(request.Email);

                if (user == null)
                {
                    _logger.LogWarning("Login attempt with non-existent email: {Email}", request.Email);
                    return ApiResponse<AuthResponse>.ErrorResult("Invalid email or password");
                }

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Invalid password attempt for user: {Email}", request.Email);
                    return ApiResponse<AuthResponse>.ErrorResult("Invalid email or password");
                }

                // Update last login
                user.LastLoginAt = DateTime.UtcNow;
                await _userRepository.UpdateUserAsync(user);

                // Generate JWT tokens
                var token = _jwtService.GenerateToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();

                var authResponse = new AuthResponse
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    Expires = DateTime.UtcNow.AddHours(24),
                    User = MapToUserDto(user)
                };

                _logger.LogInformation("User logged in successfully: {Email}", user.Email);

                return ApiResponse<AuthResponse>.SuccessResult(authResponse, "Login successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for email: {Email}", request.Email);
                return ApiResponse<AuthResponse>.ErrorResult($"Login failed: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UserDto>> GetCurrentUserAsync(Guid userId)
        {
            try
            {
                var user = await _userRepository.GetActiveUserByIdAsync(userId);

                if (user == null)
                {
                    return ApiResponse<UserDto>.ErrorResult("User not found");
                }

                var userDto = MapToUserDto(user);

                return ApiResponse<UserDto>.SuccessResult(userDto, "User retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve user: {UserId}", userId);
                return ApiResponse<UserDto>.ErrorResult($"Failed to retrieve user: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> LogoutAsync(Guid userId)
        {
            try
            {
                _logger.LogInformation("User logged out: {UserId}", userId);
                return ApiResponse<string>.SuccessResult("", "Logout successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout failed for user: {UserId}", userId);
                return ApiResponse<string>.ErrorResult($"Logout failed: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> CheckEmailExistsAsync(string email)
        {
            try
            {
                var exists = await _userRepository.EmailExistsAsync(email);
                return ApiResponse<bool>.SuccessResult(exists, "Email availability checked");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email availability: {Email}", email);
                return ApiResponse<bool>.ErrorResult($"Failed to check email availability: {ex.Message}");
            }
        }

        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                PhoneNumber = user.PhoneNumber,
                AvatarUrl = user.AvatarUrl,
                EmailVerified = user.EmailVerified ?? false, // Handle nullable bool
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };
        }
    }
}