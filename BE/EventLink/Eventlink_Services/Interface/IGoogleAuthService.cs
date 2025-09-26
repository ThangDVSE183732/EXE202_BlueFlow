using Eventlink_Services.Request;
using Eventlink_Services.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Interface
{
    public interface IGoogleAuthService
    {
        Task<ApiResponse<GoogleUserInfo>> VerifyGoogleTokenAsync(string idToken);
        Task<ApiResponse<AuthResponse>> AuthenticateWithGoogleAsync(GoogleAuthRequest request);
    }
}
