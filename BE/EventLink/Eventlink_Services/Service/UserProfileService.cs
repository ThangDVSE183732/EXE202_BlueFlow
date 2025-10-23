using Azure.Core;
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

namespace Eventlink_Services.Service
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepo _userProfileRepository;
        private readonly IUserRepository _userRepository;
        public UserProfileService(IUserProfileRepo userProfileRepository, IUserRepository userRepository)
        {
            _userProfileRepository = userProfileRepository;
            _userRepository = userRepository;
        }

        public async Task CreateAsync(Guid userId, CreateUserProfileRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            await _userProfileRepository.AddAsync(new UserProfile
            {
                UserId = userId,
                CompanyName = request.CompanyName,
                CompanyLogoUrl = request.CompanyLogoUrl,
                Industry = request.Industry,
                CompanySize = request.CompanySize,
                FoundedYear = request.FoundedYear,
                AboutUs = request.AboutUs,
                Mission = request.Mission,
                CompanyDescription = request.CompanyDescription,
                SocialProfile = request.SocialProfile,
                LinkedInProfile = request.LinkedInProfile,
                OfficialEmail = request.OfficialEmail,
                StateProvince = request.StateProvince,
                CountryRegion = request.CountryRegion,
                City = request.City,
                StreetAddress = request.StreetAddress,
                Tags = request.Tags,
                FullName = user.FullName,
                JobTitle = request.JobTitle,
                DirectEmail = user.Email,
                DirectPhone = user.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        public async Task<List<UserProfileResponse>> GetAllUserProfilesAsync()
        {
            var profiles = await _userProfileRepository.GetAllUserProfilesAsync();
            var response = profiles.Select(p => new UserProfileResponse
            {
                UserId = p.UserId,
                CompanyName = p.CompanyName,
                CompanyLogoUrl = p.CompanyLogoUrl,
                Industry = p.Industry,
                CompanySize = p.CompanySize,
                FoundedYear = p.FoundedYear,
                AboutUs = p.AboutUs,
                Mission = p.Mission.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>(),
                CompanyDescription = p.CompanyDescription,
                SocialProfile = p.SocialProfile,
                LinkedInProfile = p.LinkedInProfile,
                OfficialEmail = p.OfficialEmail,
                StateProvince = p.StateProvince,
                CountryRegion = p.CountryRegion,
                City = p.City,
                StreetAddress = p.StreetAddress,
                Tags = p.Tags.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>(),
                FullName = p.FullName,
                JobTitle = p.JobTitle,
                DirectEmail = p.DirectEmail,
                DirectPhone = p.DirectPhone,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Role = p.User.Role
            }).ToList();
            return response;
        }

        public async Task<UserProfileResponse> GetByUserIdAsync(Guid userId)
        {
            var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
            if (userProfile == null) return null;
            var response = new UserProfileResponse
            {
                UserId = userProfile.UserId,
                CompanyName = userProfile.CompanyName,
                CompanyLogoUrl = userProfile.CompanyLogoUrl,
                Industry = userProfile.Industry,
                CompanySize = userProfile.CompanySize,
                FoundedYear = userProfile.FoundedYear,
                AboutUs = userProfile.AboutUs,
                Mission = userProfile.Mission?.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>(),
                CompanyDescription = userProfile.CompanyDescription,
                SocialProfile = userProfile.SocialProfile,
                LinkedInProfile = userProfile.LinkedInProfile,
                OfficialEmail = userProfile.OfficialEmail,
                StateProvince = userProfile.StateProvince,
                CountryRegion = userProfile.CountryRegion,
                City = userProfile.City,
                StreetAddress = userProfile.StreetAddress,
                Tags = userProfile.Tags.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>(),
                FullName = userProfile.FullName,
                JobTitle = userProfile.JobTitle,
                DirectEmail = userProfile.DirectEmail,
                DirectPhone = userProfile.DirectPhone,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Role = userProfile.User.Role
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

            if (existingProfile != null)
            {
                existingProfile.CompanyName = request.CompanyName;
                existingProfile.CompanyLogoUrl = request.CompanyLogoUrl;
                existingProfile.Industry = request.Industry;
                existingProfile.CompanySize = request.CompanySize;
                existingProfile.FoundedYear = request.FoundedYear;
                existingProfile.AboutUs = request.AboutUs;
                existingProfile.Mission = request.Mission;
                existingProfile.CompanyDescription = request.CompanyDescription;
                existingProfile.SocialProfile = request.SocialProfile;
                existingProfile.LinkedInProfile = request.LinkedInProfile;
                existingProfile.OfficialEmail = request.OfficialEmail;
                existingProfile.StateProvince = request.StateProvince;
                existingProfile.CountryRegion = request.CountryRegion;
                existingProfile.City = request.City;
                existingProfile.StreetAddress = request.StreetAddress;
                existingProfile.Tags = request.Tags;
                existingProfile.JobTitle = request.JobTitle;
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
