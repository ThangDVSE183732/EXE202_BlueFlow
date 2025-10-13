using EventLink_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLink_Repositories.Interface
{
    public interface ISponsorPackageRepo : IGenericRepository<SponsorPackage>
    {
        Task<List<SponsorPackage>> GetAllSponsorPackagesAsync();
        Task<SponsorPackage> GetSponsorPackageByIdAsync(Guid id);
        Task<List<SponsorPackage>> GetSponsorPackageBySponsorIdAsync(Guid sponsorId);
        Task<List<SponsorPackage>> GetActiveSponsorPackagesAsync();
        Task<List<SponsorPackage>> GetSponsorPackageByBudgetRangeAsync(decimal min, decimal max);
        Task<List<SponsorPackage>> GetSponsorPackageByTypeAsync(string packageType);
        Task<List<SponsorPackage>> GetSponsorPackageByTargetAudienceAsync(string targetAudience);
        Task<List<SponsorPackage>> GetSponsorPackageByPreferredEventTypesAsync(string eventType);
        Task<List<SponsorPackage>> SearchSponsorPackage(string packageName, string packageType, decimal? minBudget, decimal? maxBudget, bool? isActive);

    }
}
