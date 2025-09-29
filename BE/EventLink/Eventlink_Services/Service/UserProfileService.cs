using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using Eventlink_Services.Interface;
using Eventlink_Services.Request;
using Eventlink_Services.Response;
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

        public async Task<List<UserProfileResponse>> GetAllUserProfilesAsync()
        {
            var profiles = await _userProfileRepository.GetAllUserProfilesAsync();
            var response = profiles.Select(p => new UserProfileResponse
            {
                Id = p.Id,
                UserId = p.UserId,
                Bio = p.Bio,
                CompanyName = p.CompanyName,
                Website = p.Website,
                Location = p.Location,
                ProfileImageUrl = p.ProfileImageUrl,
                CoverImageUrl = p.CoverImageUrl,
                LinkedInUrl = p.LinkedInUrl,
                FacebookUrl = p.FacebookUrl,
                PortfolioImages = p.PortfolioImages,
                WorkSamples = p.WorkSamples,
                Certifications = p.Certifications,
                YearsOfExperience = p.YearsOfExperience,
                TotalProjectsCompleted = p.TotalProjectsCompleted,
                AverageRating = p.AverageRating,
                VerificationDocuments = p.VerificationDocuments,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList();
            return response;
        }

        public async Task<UserProfileResponse> GetByUserIdAsync(Guid userId)
        {
            var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
            if (userProfile == null) return null;
            var response = new UserProfileResponse
            {
                Id = userProfile.Id,
                UserId = userProfile.UserId,
                Bio = userProfile.Bio,
                CompanyName = userProfile.CompanyName,
                Website = userProfile.Website,
                Location = userProfile.Location,
                ProfileImageUrl = userProfile.ProfileImageUrl,
                CoverImageUrl = userProfile.CoverImageUrl,
                LinkedInUrl = userProfile.LinkedInUrl,
                FacebookUrl = userProfile.FacebookUrl,
                PortfolioImages = userProfile.PortfolioImages,
                WorkSamples = userProfile.WorkSamples,
                Certifications = userProfile.Certifications,
                YearsOfExperience = userProfile.YearsOfExperience,
                TotalProjectsCompleted = userProfile.TotalProjectsCompleted,
                AverageRating = userProfile.AverageRating,
                VerificationDocuments = userProfile.VerificationDocuments,
                CreatedAt = userProfile.CreatedAt,
                UpdatedAt = userProfile.UpdatedAt
            };
            return response;
        }

        public async Task<UserProfile> GetByUserId(Guid userId)
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
