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
        Task<List<Partnership>> GetPartnershipsByEventIdAsync(Guid eventId);
        Task<Partnership> GetPartnershipByIdAsync(Guid partnershipId);
        Task CreatePartnershipAsync(CreatePartnershipRequest request);
        Task UpdatePartnershipAsync(Guid id, UpdatePartnershipRequest request);
        void DeletePartnershipAsync(Guid partnershipId);
    }
}
