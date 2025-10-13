using EventLink_Repositories.Models;
using EventLink_Repositories.Repository;
using Eventlink_Services.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Interface
{
    public interface ISponsorPackageService
    {
        Task<List<SponsorPackage>> GetAllSponsorPackagesAsync();
        Task<SponsorPackage> GetSponsorPackageByIdAsync(Guid id);
        Task<List<SponsorPackage>> SearchSponsorPackage(string packageName, string packageType, decimal? minBudget, decimal? maxBudget, bool? isActive);
        Task<List<SponsorPackage>> GetSponsorPackageBySponsorIdAsync(Guid sponsorId);
        Task<List<SponsorPackage>> GetActiveSponsorPackagesAsync();
        Task<List<SponsorPackage>> GetSponsorPackageByBudgetRangeAsync(decimal min, decimal max);
        Task<List<SponsorPackage>> GetSponsorPackageByTypeAsync(string packageType);
        Task<List<SponsorPackage>> GetSponsorPackageByTargetAudienceAsync(string targetAudience);
        Task<List<SponsorPackage>> GetSponsorPackageByPreferredEventTypesAsync(string eventType);
        Task CreateAsync(SponsorPackage request);
        void Update(SponsorPackage request);
        void Remove(SponsorPackage sponsorPackage);
    }
}
