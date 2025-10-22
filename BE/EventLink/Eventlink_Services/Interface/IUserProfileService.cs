using EventLink_Repositories.Models;
using Eventlink_Services.Request;
using Eventlink_Services.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Interface
{
    public interface IUserProfileService
    {
        Task<UserProfileResponse> GetByUserIdAsync(Guid userId);
        Task<UserProfile> GetByUserId(Guid userId);
        Task<bool> UserProfileExistsAsync(Guid userId);
        Task CreateAsync(Guid userId, CreateUserProfileRequest request);
        Task Update(Guid id, UpdateUserProfileRequest request);
        Task Remove(UserProfile userProfile);
        Task<List<UserProfileResponse>> GetAllUserProfilesAsync();
    }
}
