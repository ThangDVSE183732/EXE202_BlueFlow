using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using Eventlink_Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Service
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepo _userProfileRepository;
        public UserProfileService(IUserProfileRepo userProfileRepository)
        {
            _userProfileRepository = userProfileRepository;
        }
        public async Task<int> CreateAsync(UserProfile userProfile)
        {
            return await _userProfileRepository.CreateAsync(userProfile);
        }
        public async Task<bool> RemoveAsync(UserProfile userProfile)
        {
            return await _userProfileRepository.RemoveAsync(userProfile);
        }

        public async Task<List<UserProfile>> GetAllUserProfilesAsync()
        {
            return await _userProfileRepository.GetAllUserProfilesAsync();
        }

        public async Task<UserProfile> GetByUserIdAsync(Guid userId)
        {
            return await _userProfileRepository.GetByUserIdAsync(userId);
        }

        public async Task<int> UpdateAsync(UserProfile userProfile)
        {
            return await _userProfileRepository.UpdateAsync(userProfile);
        }

        public async Task<bool> UserProfileExistsAsync(Guid userId)
        {
            return await _userProfileRepository.UserProfileExistsAsync(userId);
        }
    }
}
