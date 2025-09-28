using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using Eventlink_Services.Interface;
using Eventlink_Services.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Eventlink_Services.Request.UserProfileRequest;

namespace Eventlink_Services.Service
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepo _userProfileRepository;
        public UserProfileService(IUserProfileRepo userProfileRepository)
        {
            _userProfileRepository = userProfileRepository;
        }

        public async Task CreateAsync(Guid userId, CreateUserProfileRequest request)
        {
            await _userProfileRepository.AddAsync(new UserProfile
            {
                UserId = userId,
                Bio = request.Bio,
                CompanyName = request.CompanyName,
                Website = request.Website,
                Location = request.Location,
                ProfileImageUrl = request.ProfileImageUrl,
                CoverImageUrl = request.CoverImageUrl,
                LinkedInUrl = request.LinkedInUrl,
                FacebookUrl = request.FacebookUrl,
                PortfolioImages = request.PortfolioImages,
                WorkSamples = request.WorkSamples,
                Certifications = request.Certifications,
                YearsOfExperience = request.YearsOfExperience,
                TotalProjectsCompleted = request.TotalProjectsCompleted,
                AverageRating = request.AverageRating,
                VerificationDocuments = request.VerificationDocuments,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        public async Task<List<UserProfile>> GetAllUserProfilesAsync()
        {
            return await _userProfileRepository.GetAllUserProfilesAsync();
        }

        public async Task<UserProfile> GetByUserIdAsync(Guid userId)
        {
            return await _userProfileRepository.GetByUserIdAsync(userId);
        }

        public async Task Remove(UserProfile userProfile)
        {
            var existingProfile = await _userProfileRepository.GetByUserIdAsync(userProfile.Id);
            if (existingProfile != null)
            {
                _userProfileRepository.Remove(userProfile);
            }
        }

        public async Task Update(Guid id, UpdateUserProfileRequest request)
        {
            var existingProfile = await _userProfileRepository.GetByUserIdAsync(id);
            if(existingProfile != null)
            {
                existingProfile.Bio = request.Bio;
                existingProfile.CompanyName = request.CompanyName;
                existingProfile.Website = request.Website;
                existingProfile.Location = request.Location;
                existingProfile.ProfileImageUrl = request.ProfileImageUrl;
                existingProfile.CoverImageUrl = request.CoverImageUrl;
                existingProfile.LinkedInUrl = request.LinkedInUrl;
                existingProfile.FacebookUrl = request.FacebookUrl;
                existingProfile.PortfolioImages = request.PortfolioImages;
                existingProfile.WorkSamples = request.WorkSamples;
                existingProfile.Certifications = request.Certifications;
                existingProfile.YearsOfExperience = request.YearsOfExperience;
                existingProfile.TotalProjectsCompleted = request.TotalProjectsCompleted;
                existingProfile.AverageRating = request.AverageRating;
                existingProfile.VerificationDocuments = request.VerificationDocuments;
                existingProfile.UpdatedAt = DateTime.UtcNow;
                _userProfileRepository.Update(existingProfile);
            }
        }

        public async Task<bool> UserProfileExistsAsync(Guid userId)
        {
            return await _userProfileRepository.UserProfileExistsAsync(userId);
        }
    }
}
