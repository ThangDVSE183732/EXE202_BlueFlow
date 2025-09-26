using EventLink_Repositories.DBContext;
using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using Microsoft.EntityFrameworkCore;
using SHBTrading.Repositories.DinhTH.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLink_Repositories.Repository
{
    public class SponsorPackageRepo : GenericRepository<SponsorPackage>, ISponsorPackageRepo
    {
        private readonly EventLinkDBContext _context;
        public SponsorPackageRepo(EventLinkDBContext context)
        {
            _context = context;
        }

        public async Task<List<SponsorPackage>> GetAllSponsorPackagesAsync()
        {
            return await _context.SponsorPackages.ToListAsync();
        }

        public async Task<SponsorPackage> GetSponsorPackageByIdAsync(Guid id)
        {
            return await _context.SponsorPackages.FirstOrDefaultAsync(sp => sp.Id == id);
        }

        public async Task<List<SponsorPackage>> GetSponsorPackageBySponsorIdAsync(Guid sponsorId)
        {
            return await _context.SponsorPackages
                .Where(sp => sp.SponsorId == sponsorId)
                .ToListAsync();
        }

        public async Task<List<SponsorPackage>> GetActiveSponsorPackagesAsync()
        {
            return await _context.SponsorPackages
                .Where(sp => sp.IsActive == true)
                .ToListAsync();
        }

        public async Task<List<SponsorPackage>> SearchSponsorPackage(string packageName, string packageType, decimal? minBudget, decimal? maxBudget, bool? isActive)
        {
            var query = _context.SponsorPackages.AsQueryable();
            if (!string.IsNullOrEmpty(packageName))
            {
                query = query.Where(sp => sp.PackageName.Contains(packageName));
            }
            if (!string.IsNullOrEmpty(packageType))
            {
                query = query.Where(sp => sp.PackageType == packageType);
            }
            if (minBudget.HasValue)
            {
                query = query.Where(sp => sp.Budget >= minBudget.Value);
            }
            if (maxBudget.HasValue)
            {
                query = query.Where(sp => sp.Budget <= maxBudget.Value);
            }
            if (isActive.HasValue)
            {
                query = query.Where(sp => sp.IsActive == isActive.Value);
            }
            return await query.ToListAsync();
        }

        public async Task<List<SponsorPackage>> GetSponsorPackageByBudgetRangeAsync(decimal min, decimal max)
        {
            return await _context.SponsorPackages
                .Where(sp => sp.Budget >= min && sp.Budget <= max)
                .ToListAsync();
        }

        public Task<List<SponsorPackage>> GetSponsorPackageByTypeAsync(string packageType)
        {
            return _context.SponsorPackages
                .Where(sp => sp.PackageType == packageType)
                .ToListAsync();
        }

        public Task<List<SponsorPackage>> GetSponsorPackageByTargetAudienceAsync(string targetAudience)
        {
            return _context.SponsorPackages
                .Where(sp => sp.TargetAudience.Contains(targetAudience))
                .ToListAsync();
        }

        public Task<List<SponsorPackage>> GetSponsorPackageByPreferredEventTypesAsync(string eventType)
        {
            return _context.SponsorPackages
                .Where(sp => sp.PreferredEventTypes.Contains(eventType))
                .ToListAsync();
        }
    }
}
