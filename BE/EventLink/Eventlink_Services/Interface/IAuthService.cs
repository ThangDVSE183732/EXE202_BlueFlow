using Eventlink_Services.Request;
using Eventlink_Services.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Eventlink_Services.Request.MailForm;

namespace Eventlink_Services.Interface
{
    public interface IAuthService
    {
        Task<ApiResponse<string>> RegisterAsync(RegisterRequest request);
        Task<ApiResponse<AuthResponse>> VerifyRegistrationOtpAsync(VerifyOtpRequest request);
        Task<ApiResponse<string>> LoginAsync(LoginRequest request);
        //Task<ApiResponse<AuthResponse>> VerifyOtpAsync(VerifyOtpRequest request);
        Task<ApiResponse<UserDto>> GetCurrentUserAsync(Guid userId);
        Task<ApiResponse<string>> LogoutAsync(Guid userId);
        Task<ApiResponse<bool>> CheckEmailExistsAsync(string email);
        void SaveOtp(string email, string otp);
        bool VerifyOtp(string email, string otp);
        Task SendOtpEmailAsync(string email);
    }
}
