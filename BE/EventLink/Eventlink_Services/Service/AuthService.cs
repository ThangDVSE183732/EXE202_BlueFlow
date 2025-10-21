// Add these using statements
using BCrypt.Net;
using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using Eventlink_Services.Interface;
using Eventlink_Services.Request;
using Eventlink_Services.Response;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using static Eventlink_Services.Request.MailForm;
using static System.Net.WebRequestMethods;

namespace EventLink_Services.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IEmailService _emailService;

        public AuthService(
            IUserRepository userRepository,
            IJwtService jwtService,
            ILogger<AuthService> logger,
            IMemoryCache memoryCache,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _logger = logger;
            _memoryCache = memoryCache;
            _emailService = emailService;
        }

        public async Task<ApiResponse<string>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // Validate role
                if (!await _userRepository.IsValidRoleAsync(request.Role))
                {
                    return ApiResponse<string>.ErrorResult("Invalid role. Must be Organizer, Supplier, or Sponsor");
                }

                // Check if email already exists
                if (await _userRepository.EmailExistsAsync(request.Email))
                {
                    return ApiResponse<string>.ErrorResult("Email already exists");
                }

                await SendOtpEmailAsync(request.Email);

                var key = $"register:{request.Email.Trim().ToLowerInvariant()}";
                _memoryCache.Set(key, request, TimeSpan.FromMinutes(5));

                _logger.LogInformation("OTP sent for registration: {Email}", request.Email);

                return ApiResponse<string>.SuccessResult(null, "OTP sent to your email. Please verify to complete registration.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Register request failed for email: {Email}", request.Email);
                return ApiResponse<string>.ErrorResult($"Registration failed: {ex.Message}");
            }
        }


        public async Task<ApiResponse<AuthResponse>> VerifyRegistrationOtpAsync(VerifyOtpRequest request)
        {
            try
            {
                // 1. Kiểm tra OTP
                if (!VerifyOtp(request.Email, request.Otp))
                {
                    return ApiResponse<AuthResponse>.ErrorResult("Invalid or expired OTP");
                }

                // 2. Lấy RegisterRequest từ cache
                var key = $"register:{request.Email.Trim().ToLowerInvariant()}";
                if (!_memoryCache.TryGetValue(key, out RegisterRequest registerRequest))
                {
                    return ApiResponse<AuthResponse>.ErrorResult("Registration data expired. Please register again.");
                }

                // 3. Tạo user
                var user = new User
                {
                    Email = registerRequest.Email.ToLower().Trim(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password),
                    FullName = registerRequest.FullName.Trim(),
                    Role = registerRequest.Role,
                    PhoneNumber = string.IsNullOrWhiteSpace(registerRequest.PhoneNumber) ? null : registerRequest.PhoneNumber.Trim(),
                };

                var createdUser = await _userRepository.CreateUserAsync(user);

                // 4. Sinh token
                var token = _jwtService.GenerateToken(createdUser);
                var refreshToken = _jwtService.GenerateRefreshToken();

                var authResponse = new AuthResponse
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    Expires = DateTime.UtcNow.AddHours(24),
                    User = MapToUserDto(createdUser)
                };

                _logger.LogInformation("User registered successfully after OTP verification: {Email}", createdUser.Email);

                return ApiResponse<AuthResponse>.SuccessResult(authResponse, "Registration successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed after OTP verification for email: {Email}", request.Email);
                return ApiResponse<AuthResponse>.ErrorResult($"Registration failed: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> LoginAsync(LoginRequest request)
        {
            try
            {
                // Find user by email
                var user = await _userRepository.GetActiveUserByEmailAsync(request.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Invalid login attempt for email: {Email}", request.Email);
                    return ApiResponse<string>.ErrorResult("Invalid email or password");
                }

                // Gửi OTP qua email
                //await SendOtpEmailAsync(request.Email);

                //return ApiResponse<string>.SuccessResult(null, "OTP sent to your email. Please verify.");

                user.LastLoginAt = DateTime.UtcNow;
                await _userRepository.UpdateUserAsync(user);
                // Generate JWT
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
                return ApiResponse<string>.SuccessResult(null, "Login successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login OTP request failed for email: {Email}", request.Email);
                return ApiResponse<string>.ErrorResult($"Login failed: {ex.Message}");
            }
        }
        //public async Task<ApiResponse<AuthResponse>> VerifyOtpAsync(VerifyOtpRequest request)
        //{
        //    try
        //    {
        //        // Tìm user
        //        var user = await _userRepository.GetActiveUserByEmailAsync(request.Email);
        //        if (user == null)
        //        {
        //            return ApiResponse<AuthResponse>.ErrorResult("User not found");
        //        }

        //        // Kiểm tra OTP
        //        if (!VerifyOtp(request.Email, request.Otp))
        //        {
        //            return ApiResponse<AuthResponse>.ErrorResult("Invalid or expired OTP");
        //        }

        //        // Update last login
        //        user.LastLoginAt = DateTime.UtcNow;
        //        await _userRepository.UpdateUserAsync(user);

        //        // Generate JWT
        //        var token = _jwtService.GenerateToken(user);
        //        var refreshToken = _jwtService.GenerateRefreshToken();

        //        var authResponse = new AuthResponse
        //        {
        //            Token = token,
        //            RefreshToken = refreshToken,
        //            Expires = DateTime.UtcNow.AddHours(24),
        //            User = MapToUserDto(user)
        //        };

        //        _logger.LogInformation("User logged in successfully after OTP: {Email}", user.Email);

        //        return ApiResponse<AuthResponse>.SuccessResult(authResponse, "Login successful");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Verify OTP failed for email: {Email}", request.Email);
        //        return ApiResponse<AuthResponse>.ErrorResult($"Login failed: {ex.Message}");
        //    }
        //}

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

        public void SaveOtp(string email, string otp)
        {
            var key = email.Trim().ToLowerInvariant();
            _memoryCache.Set(key, otp, TimeSpan.FromMinutes(5));
        }

        public bool VerifyOtp(string email, string otp)
        {
            var key = email.Trim().ToLowerInvariant();
            if (_memoryCache.TryGetValue(key, out string savedOtp))
            {
                return savedOtp == otp;
            }
            return false;
        }
        public async Task SendOtpEmailAsync(string email)
        {
            // 1. Sinh OTP 6 số
            var otp = new Random().Next(100000, 999999).ToString();

            // 2. Lưu OTP vào cache (hết hạn sau 5 phút)
            SaveOtp(email, otp);

            // 3. Gửi email
            var subject = "Your OTP Code";
            var body = $@"
            <h2>Email Verification</h2>
            <p>Your OTP code is: <b>{otp}</b></p>
            <p>This code will expire in 5 minutes.</p>";

            await _emailService.SendEmailAsync(email, subject, body);
        }
    }
}