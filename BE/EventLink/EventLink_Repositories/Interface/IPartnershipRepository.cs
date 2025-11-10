using EventLink_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLink_Repositories.Interface
{
    public interface IPartnershipRepository : IGenericRepository<Partnership>
    {
        Task<Partnership> GetByIdAsync(Guid id);
        Task UpdateAsync(Partnership partnership);
        
        /// <summary>
        /// Get all partnerships in the system with Event and Partner navigation properties loaded
        /// </summary>
        Task<List<Partnership>> GetAllPartnershipsAsync();
        
        /// <summary>
        /// Get all partnerships for an event with Event and Partner navigation properties loaded
        /// </summary>
        Task<List<Partnership>> GetPartnershipsByEventAsync(Guid eventId);
        
        /// <summary>
        /// Get all partnerships without an assigned event (EventId is null)
        /// ✅ NEW: For partnerships not yet assigned to any event
        /// </summary>
        Task<List<Partnership>> GetUnassignedPartnershipsAsync();

        /// <summary>
        /// Get all partnerships for a partner with Event and Partner navigation properties loaded
        /// ✅ NEW: For getting partnerships by PartnerId
        /// </summary>
        Task<List<Partnership>> GetPartnershipsByPartnerAsync(Guid partnerId);
    }
}
