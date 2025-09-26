using Eventlink_Services.Request;
using Eventlink_Services.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Interface
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
        Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);
        Task<ApiResponse<UserDto>> GetCurrentUserAsync(Guid userId);
        Task<ApiResponse<string>> LogoutAsync(Guid userId);
        Task<ApiResponse<bool>> CheckEmailExistsAsync(string email);
    }
}
