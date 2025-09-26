using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using EventLink_Repositories.Repository;
using Eventlink_Services.Interface;
using Eventlink_Services.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Service
{
    public class SponsorPackageService : ISponsorPackageService
    {
        private readonly ISponsorPackageRepo _sponsorPackageRepo;
        public SponsorPackageService(ISponsorPackageRepo sponsorPackageRepo)
        {
            _sponsorPackageRepo = sponsorPackageRepo;
        }
        public async Task<List<SponsorPackage>> GetAllSponsorPackagesAsync()
        {
            return await _sponsorPackageRepo.GetAllSponsorPackagesAsync();
        }
        public async Task<SponsorPackage> GetSponsorPackageByIdAsync(Guid id)
        {
            return await _sponsorPackageRepo.GetSponsorPackageByIdAsync(id);
        }
        public async Task<List<SponsorPackage>> GetSponsorPackageBySponsorIdAsync(Guid sponsorId)
        {
            return await _sponsorPackageRepo.GetSponsorPackageBySponsorIdAsync(sponsorId);
        }
        public async Task<List<SponsorPackage>> SearchSponsorPackage(string packageName, string packageType, decimal? minBudget, decimal? maxBudget, bool? isActive)
        {
            return await _sponsorPackageRepo.SearchSponsorPackage(packageName, packageType, minBudget, maxBudget, isActive);
        }
        public async Task<int> CreateAsync(SponsorPackage sponsorPackage)
        {
            return await _sponsorPackageRepo.CreateAsync(sponsorPackage);
        }
        public async Task<int> UpdateAsync(SponsorPackage sponsorPackage)
        {
            return await _sponsorPackageRepo.UpdateAsync(sponsorPackage);
        }
        public async Task<bool> RemoveAsync(SponsorPackage sponsorPackage)
        {
            return await _sponsorPackageRepo.RemoveAsync(sponsorPackage);
        }

        public async Task<List<SponsorPackage>> GetActiveSponsorPackagesAsync()
        {
            return await _sponsorPackageRepo.GetActiveSponsorPackagesAsync();
        }

        public async Task<List<SponsorPackage>> GetSponsorPackageByBudgetRangeAsync(decimal min, decimal max)
        {
            return await _sponsorPackageRepo.GetSponsorPackageByBudgetRangeAsync(min, max);
        }

        public async Task<List<SponsorPackage>> GetSponsorPackageByTypeAsync(string packageType)
        {
            return await _sponsorPackageRepo.GetSponsorPackageByTypeAsync(packageType);
        }

        public async Task<List<SponsorPackage>> GetSponsorPackageByTargetAudienceAsync(string targetAudience)
        {
            return await _sponsorPackageRepo.GetSponsorPackageByTargetAudienceAsync(targetAudience);
        }

        public async Task<List<SponsorPackage>> GetSponsorPackageByPreferredEventTypesAsync(string eventType)
        {
            return await _sponsorPackageRepo.GetSponsorPackageByPreferredEventTypesAsync(eventType);
        }
    }
}
