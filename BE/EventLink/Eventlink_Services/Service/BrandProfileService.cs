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
                BrandName = p.BrandName,
                BrandLogo = p.BrandLogo,
                Industry = p.Industry,
                CompanySize = p.CompanySize,
                FoundedYear = p.FoundedYear,
                Location = p.Location,
                AboutUs = p.AboutUs,
                OurMission = p.OurMission.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>(),
                Website = p.Website,
                Email = p.Email,
                PhoneNumber = p.PhoneNumber,
                Tags = p.Tags.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>(),
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
                BrandName = brandProfile.BrandName,
                BrandLogo = brandProfile.BrandLogo,
                Industry = brandProfile.Industry,
                CompanySize = brandProfile.CompanySize,
                FoundedYear = brandProfile.FoundedYear,
                Location = brandProfile.Location,
                AboutUs = brandProfile.AboutUs,
                OurMission = brandProfile.OurMission.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>(),
                Website = brandProfile.Website,
                Email = brandProfile.Email,
                PhoneNumber = brandProfile.PhoneNumber,
                Tags = brandProfile.Tags.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>(),
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
                BrandName = brandProfile.BrandName,
                BrandLogo = brandProfile.BrandLogo,
                Industry = brandProfile.Industry,
                CompanySize = brandProfile.CompanySize,
                FoundedYear = brandProfile.FoundedYear,
                Location = brandProfile.Location,
                AboutUs = brandProfile.AboutUs,
                OurMission = brandProfile.OurMission.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>(),
                Website = brandProfile.Website,
                Email = brandProfile.Email,
                PhoneNumber = brandProfile.PhoneNumber,
                Tags = brandProfile.Tags.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>(),
                CreatedAt = brandProfile.CreatedAt,
                UpdatedAt = brandProfile.UpdatedAt
            };

            return result;
        }

        public async Task UpdateAsync(Guid id, UpdateBrandProfileRequest request)
        {
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
            existingProfile.UpdatedAt = DateTime.UtcNow;
            existingProfile.CreatedAt = DateTime.UtcNow;
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
    }
}
