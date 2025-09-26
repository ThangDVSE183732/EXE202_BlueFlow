using EventLink_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Interface
{
    public interface IUserProfileService
    {
        Task<UserProfile> GetByUserIdAsync(Guid userId);
        Task<bool> UserProfileExistsAsync(Guid userId);
        Task CreateAsync(UserProfile userProfile);
        void Update(UserProfile userProfile);
        void Remove(UserProfile userProfile);
        Task<List<UserProfile>> GetAllUserProfilesAsync();
    }
}
