using EventLink_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Eventlink_Services.Request.PartnershipRequest;

namespace Eventlink_Services.Interface
{
    public interface IPartnershipService
    {
        Task<IEnumerable<User>> GetPartnersByEventAsync(Guid eventId);
        Task<Partnership> CreateAsync(Guid organizerId, CreatePartnershipRequest request);
        Task<Partnership> UpdateStatusAsync(Guid partnershipId, string status, string response);
        Task UpdateAsync(Guid partnershipId, UpdatePartnershipRequest request);
    }
}
