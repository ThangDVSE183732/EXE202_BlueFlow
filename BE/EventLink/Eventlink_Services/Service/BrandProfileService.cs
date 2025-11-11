using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using Eventlink_Services.Interface;
using Eventlink_Services.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Eventlink_Services.Request.BrandProfileRequest;

namespace Eventlink_Services.Service
{
    public class BrandProfileService : IBrandProfileService
    {
        private readonly IBrandProfileRepository _brandProfileRepository;
        private readonly CloudinaryService _cloudinaryService;
        public BrandProfileService(IBrandProfileRepository brandProfileRepository, CloudinaryService cloudinaryService)
        {
            _brandProfileRepository = brandProfileRepository;
            _cloudinaryService = cloudinaryService;
        }

        public async Task AddAsync(Guid userId, CreateBrandProfileRequest request)
        {
            var brandProfile = new BrandProfile
            {
                UserId = userId,
                BrandName = request.BrandName,
                Industry = request.Industry,
                CompanySize = request.CompanySize,
                FoundedYear = request.FoundedYear,
                Location = request.Location,
                AboutUs = request.AboutUs,
                OurMission = request.OurMission,
                Website = request.Website,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Tags = request.Tags,
                IsPublic = request.IsPublic ?? false, // ✅ Default to false (private)
                HasPartnership = request.HasPartnership ?? false, // ✅ NEW: Default to false (no partnership)
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if(request.BrandLogo != null)
            {
                var brandLogo = await _cloudinaryService.UploadImageAsync(request.BrandLogo);
                brandProfile.BrandLogo = brandLogo;
            }

            await _brandProfileRepository.AddAsync(brandProfile);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existingProfile = await _brandProfileRepository.GetByIdAsync(id);

            if (existingProfile == null)
            {
                throw new Exception("Brand profile not found");
            }

            await _cloudinaryService.DeleteImageAsync(existingProfile.BrandLogo);
            _brandProfileRepository.Remove(existingProfile);
        }

        public async Task<List<BrandProfileResponse>> GetAllAsync()
        {
            var brandProfiles = await _brandProfileRepository.GetAllAsync();

            var result = brandProfiles.Select(p => new BrandProfileResponse
            {
                Id = p.Id,
                BrandName = p.BrandName,
                BrandLogo = p.BrandLogo,
                Industry = p.Industry,
                CompanySize = p.CompanySize,
                FoundedYear = p.FoundedYear,
                Location = p.Location,
                AboutUs = p.AboutUs,
                // ✅ FIX: Handle null OurMission
                OurMission = !string.IsNullOrEmpty(p.OurMission) 
                    ? p.OurMission.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() 
                    : new List<string>(),
                Website = p.Website,
                Email = p.Email,
                PhoneNumber = p.PhoneNumber,
                // ✅ FIX: Handle null Tags
                Tags = !string.IsNullOrEmpty(p.Tags) 
                    ? p.Tags.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() 
                    : new List<string>(),
                IsPublic = p.IsPublic,
                HasPartnership = p.HasPartnership,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList();

            return result;
        }

        public async Task<BrandProfileResponse> GetByIdAsync(Guid id)
        {
            var brandProfile = await _brandProfileRepository.GetByIdAsync(id);

            var result = new BrandProfileResponse
            {
                Id = brandProfile.Id,
                BrandName = brandProfile.BrandName,
                BrandLogo = brandProfile.BrandLogo,
                Industry = brandProfile.Industry,
                CompanySize = brandProfile.CompanySize,
                FoundedYear = brandProfile.FoundedYear,
                Location = brandProfile.Location,
                AboutUs = brandProfile.AboutUs,
                // ✅ FIX: Handle null OurMission
                OurMission = !string.IsNullOrEmpty(brandProfile.OurMission) 
                    ? brandProfile.OurMission.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() 
                    : new List<string>(),
                Website = brandProfile.Website,
                Email = brandProfile.Email,
                PhoneNumber = brandProfile.PhoneNumber,
                // ✅ FIX: Handle null Tags
                Tags = !string.IsNullOrEmpty(brandProfile.Tags) 
                    ? brandProfile.Tags.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() 
                    : new List<string>(),
                IsPublic = brandProfile.IsPublic,
                HasPartnership = brandProfile.HasPartnership,
                CreatedAt = brandProfile.CreatedAt,
                UpdatedAt = brandProfile.UpdatedAt
            };

            return result;
        }

        public async Task<BrandProfileResponse> GetByUserIdAsync(Guid userId)
        {
            var brandProfile = await _brandProfileRepository.GetByUserIdAsync(userId);

            if (brandProfile == null)
            {
                return null;
            }

            var result = new BrandProfileResponse
            {
                Id = brandProfile.Id,
                BrandName = brandProfile.BrandName,
                BrandLogo = brandProfile.BrandLogo,
                Industry = brandProfile.Industry,
                CompanySize = brandProfile.CompanySize,
                FoundedYear = brandProfile.FoundedYear,
                Location = brandProfile.Location,
                AboutUs = brandProfile.AboutUs,
                // ✅ FIX: Handle null OurMission
                OurMission = !string.IsNullOrEmpty(brandProfile.OurMission) 
                    ? brandProfile.OurMission.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() 
                    : new List<string>(),
                Website = brandProfile.Website,
                Email = brandProfile.Email,
                PhoneNumber = brandProfile.PhoneNumber,
                // ✅ FIX: Handle null Tags
                Tags = !string.IsNullOrEmpty(brandProfile.Tags) 
                    ? brandProfile.Tags.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() 
                    : new List<string>(),
                IsPublic = brandProfile.IsPublic,
                HasPartnership = brandProfile.HasPartnership,
                CreatedAt = brandProfile.CreatedAt,
                UpdatedAt = brandProfile.UpdatedAt
            };

            return result;
        }

        public async Task UpdateAsync(Guid id, UpdateBrandProfileRequest request)
        {
            // ✅ FIX: Use GetByIdAsync instead of GetByUserIdAsync
            // id parameter is BrandProfileId, not UserId
            var existingProfile = await _brandProfileRepository.GetByIdAsync(id);

            if (existingProfile == null)
            {
                throw new Exception("Brand profile not found");
            }

            existingProfile.BrandName = request.BrandName;
            existingProfile.Industry = request.Industry;
            existingProfile.CompanySize = request.CompanySize;
            existingProfile.FoundedYear = request.FoundedYear;
            existingProfile.Location = request.Location;
            existingProfile.AboutUs = request.AboutUs;
            existingProfile.OurMission = request.OurMission;
            existingProfile.Website = request.Website;
            existingProfile.Email = request.Email;
            existingProfile.PhoneNumber = request.PhoneNumber;
            existingProfile.Tags = request.Tags;
            existingProfile.IsPublic = request.IsPublic ?? false;
            existingProfile.HasPartnership = request.HasPartnership ?? false;
            // ✅ FIX: Remove duplicate CreatedAt assignment
            existingProfile.UpdatedAt = DateTime.UtcNow;

            if (request.BrandLogo != null)
            {
                // Xóa logo cũ nếu có
                if (!string.IsNullOrEmpty(existingProfile.BrandLogo))
                {
                    try
                    {
                        await _cloudinaryService.DeleteImageAsync(existingProfile.BrandLogo);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Không thể xóa logo cũ: {ex.Message}");
                    }
                }

                // Upload logo mới
                var newLogoUrl = await _cloudinaryService.UploadImageAsync(request.BrandLogo);
                existingProfile.BrandLogo = newLogoUrl;
            }

            _brandProfileRepository.Update(existingProfile);
        }

        /// <summary>
        /// Toggle BrandProfile visibility status (IsPublic) by BrandProfile ID
        /// ✅ SIMPLE: Only updates IsPublic field, no side effects
        /// </summary>
        public async Task<BrandProfile> ToggleBrandProfileStatusByIdAsync(Guid brandProfileId)
        {
            var brandProfile = await _brandProfileRepository.GetByIdAsync(brandProfileId);

            if (brandProfile == null)
                throw new Exception("Brand profile not found.");

            // ✅ Toggle logic: true ↔ false
            var currentStatus = brandProfile.IsPublic ?? false;
            var newStatus = !currentStatus;

            // ✅ ONLY update IsPublic
            brandProfile.IsPublic = newStatus;

            _brandProfileRepository.Update(brandProfile);
            return brandProfile;
        }

        /// <summary>
        /// Toggle both IsPublic and HasPartnership status by BrandProfile ID
        /// ✅ NEW: Toggle both visibility and partnership status at once
        /// </summary>
        public async Task<BrandProfile> ToggleBrandProfileAllStatusAsync(Guid brandProfileId)
        {
            var brandProfile = await _brandProfileRepository.GetByIdAsync(brandProfileId);

            if (brandProfile == null)
                throw new Exception("Brand profile not found.");

            // ✅ Toggle both properties
            var currentIsPublic = brandProfile.IsPublic ?? false;
            var currentHasPartnership = brandProfile.HasPartnership ?? false;
            
            brandProfile.IsPublic = !currentIsPublic;
            brandProfile.HasPartnership = !currentHasPartnership;

            _brandProfileRepository.Update(brandProfile);
            return brandProfile;
        }
    }
}
