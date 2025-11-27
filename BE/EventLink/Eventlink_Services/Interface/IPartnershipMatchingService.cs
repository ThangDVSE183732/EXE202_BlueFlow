using EventLink_Repositories.Models;
using Eventlink_Services.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eventlink_Services.Interface
{
    public interface IPartnershipMatchingService
    {
        Task<(User user, string role)> GetUserRoleAsync(Guid userId);
        Task<List<SponsorMatchResult>> FindSponsorMatchesAsync(Guid organizerId);
        Task<List<OrganizerMatchResult>> FindOrganizerMatchesAsync(Guid sponsorId);
    }
}

