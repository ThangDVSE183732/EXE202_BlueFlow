using EventLink_Repositories.Models;
using Eventlink_Services.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Eventlink_Services.Request.BrandProfileRequest;

namespace Eventlink_Services.Interface
{
    public interface IBrandProfileService
    {
        Task<BrandProfileResponse> GetByUserIdAsync(Guid userId);
        Task<List<BrandProfileResponse>> GetAllAsync();
        Task<BrandProfileResponse> GetByIdAsync(Guid id);
        Task AddAsync(Guid userId, CreateBrandProfileRequest request);
        Task UpdateAsync(Guid id, UpdateBrandProfileRequest request);
        Task DeleteAsync(Guid id);
    }
}
