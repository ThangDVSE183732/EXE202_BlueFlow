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
        private readonly CloudinaryService _cloudinaryService;
        public UserProfileService(IUserProfileRepo userProfileRepository, IUserRepository userRepository, CloudinaryService cloudinaryService)
        {
            _userProfileRepository = userProfileRepository;
            _userRepository = userRepository;
            _cloudinaryService = cloudinaryService;
        }

        public async Task CreateAsync(Guid userId, CreateUserProfileRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            // ✅ Handle CompanyLogoUrl upload or set default
            string companyLogoUrl = string.Empty;
            
            if (request.CompanyLogoUrl != null)
            {
                companyLogoUrl = await _cloudinaryService.UploadImageAsync(request.CompanyLogoUrl);
            }

            var profile = new UserProfile
            {
                Id = Guid.NewGuid(), // ✅ Generate Id for UserProfile
                UserId = userId,
                CompanyName = request.CompanyName ?? string.Empty,
                CompanyLogoUrl = companyLogoUrl,
                Industry = request.Industry ?? string.Empty,
                CompanySize = request.CompanySize ?? string.Empty,
                FoundedYear = request.FoundedYear,
                CompanyDescription = request.CompanyDescription ?? string.Empty,
                SocialProfile = request.SocialProfile ?? string.Empty,
                LinkedInProfile = request.LinkedInProfile ?? string.Empty,
                OfficialEmail = request.OfficialEmail ?? string.Empty,
                StateProvince = request.StateProvince ?? string.Empty,
                CountryRegion = request.CountryRegion ?? string.Empty,
                City = request.City ?? string.Empty,
                StreetAddress = request.StreetAddress ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
                JobTitle = request.JobTitle ?? string.Empty,
                DirectEmail = user.Email ?? string.Empty,
                DirectPhone = user.PhoneNumber ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userProfileRepository.AddAsync(profile);
        }

        public async Task<List<UserProfileResponse>> GetAllUserProfilesAsync()
        {
            var profiles = await _userProfileRepository.GetAllUserProfilesAsync();
            var response = profiles.Select(p => new UserProfileResponse
            {
                Id = p.Id,
                CompanyName = p.CompanyName,
                CompanyLogoUrl = p.CompanyLogoUrl,
                Industry = p.Industry,
                CompanySize = p.CompanySize,
                FoundedYear = p.FoundedYear,
                CompanyDescription = p.CompanyDescription,
                SocialProfile = p.SocialProfile,
                LinkedInProfile = p.LinkedInProfile,
                OfficialEmail = p.OfficialEmail,
                StateProvince = p.StateProvince,
                CountryRegion = p.CountryRegion,
                City = p.City,
                StreetAddress = p.StreetAddress,
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
                Id = userProfile.Id,
                CompanyName = userProfile.CompanyName,
                CompanyLogoUrl = userProfile.CompanyLogoUrl,
                Industry = userProfile.Industry,
                CompanySize = userProfile.CompanySize,
                FoundedYear = userProfile.FoundedYear,
                CompanyDescription = userProfile.CompanyDescription,
                SocialProfile = userProfile.SocialProfile,
                LinkedInProfile = userProfile.LinkedInProfile,
                OfficialEmail = userProfile.OfficialEmail,
                StateProvince = userProfile.StateProvince,
                CountryRegion = userProfile.CountryRegion,
                City = userProfile.City,
                StreetAddress = userProfile.StreetAddress,
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
            // ✅ Query by UserProfile.Id (Primary Key) instead of UserId
            var existingProfile = await _userProfileRepository.GetByIdAsync(id);

            if (existingProfile == null)
            {
                throw new Exception("User profile not found");
            }

            existingProfile.CompanyName = request.CompanyName;
            existingProfile.Industry = request.Industry;
            existingProfile.CompanySize = request.CompanySize;
            existingProfile.FoundedYear = request.FoundedYear;
            existingProfile.CompanyDescription = request.CompanyDescription;
            existingProfile.SocialProfile = request.SocialProfile;
            existingProfile.LinkedInProfile = request.LinkedInProfile;
            existingProfile.OfficialEmail = request.OfficialEmail;
            existingProfile.StateProvince = request.StateProvince;
            existingProfile.CountryRegion = request.CountryRegion;
            existingProfile.City = request.City;
            existingProfile.StreetAddress = request.StreetAddress;
            existingProfile.JobTitle = request.JobTitle;
            existingProfile.UpdatedAt = DateTime.UtcNow;

            if (request.CompanyLogoUrl != null)
            {
                // Xóa logo cũ nếu có
                if (!string.IsNullOrEmpty(existingProfile.CompanyLogoUrl))
                {
                    try
                    {
                        await _cloudinaryService.DeleteImageAsync(existingProfile.CompanyLogoUrl);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Không thể xóa logo cũ: {ex.Message}");
                    }
                }

                // Upload logo mới
                var newLogoUrl = await _cloudinaryService.UploadImageAsync(request.CompanyLogoUrl);
                existingProfile.CompanyLogoUrl = newLogoUrl;
            }

            _userProfileRepository.Update(existingProfile);
        }

        public async Task<bool> UserProfileExistsAsync(Guid userId)
        {
            return await _userProfileRepository.UserProfileExistsAsync(userId);
        }
    }
}
