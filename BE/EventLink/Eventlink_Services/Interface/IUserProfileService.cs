using EventLink_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Eventlink_Services.Request.UserProfileRequest;

namespace Eventlink_Services.Interface
{
    public interface IUserProfileService
    {
        Task<UserProfile> GetByUserIdAsync(Guid userId);
        Task<bool> UserProfileExistsAsync(Guid userId);
        Task CreateAsync(CreateUserProfileRequest request);
        Task Update(Guid id, UpdateUserProfileRequest request);
        Task Remove(UserProfile userProfile);
        Task<List<UserProfile>> GetAllUserProfilesAsync();
    }
}
