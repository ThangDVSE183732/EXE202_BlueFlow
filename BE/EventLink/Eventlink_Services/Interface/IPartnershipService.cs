using EventLink_Repositories.Models;
using Eventlink_Services.Response;
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
        /// <summary>
        /// Get all partnerships in the system with event and partner details
        /// </summary>
        Task<IEnumerable<PartnershipResponse>> GetAllPartnershipsAsync();
        
        /// <summary>
        /// Get all partnerships with event and partner details for a specific event
        /// </summary>
        Task<IEnumerable<PartnershipResponse>> GetPartnershipsByEventAsync(Guid eventId);
        
        /// <summary>
        /// Create partnership from FormData with file upload support
        /// </summary>
        Task<Partnership> CreateAsync(Guid organizerId, CreatePartnershipFormRequest request);
        
        /// <summary>
        /// Create partnership from JSON request (backward compatibility)
        /// </summary>
        Task<Partnership> CreateAsync(Guid organizerId, CreatePartnershipRequest request);
        
        /// <summary>
        /// Update partnership status (Accept/Reject) with optional PartnershipImage
        /// </summary>
        Task<Partnership> UpdateStatusAsync(Guid partnershipId, UpdatePartnershipStatusRequest request);
        
        /// <summary>
        /// Toggle partnership status between Ongoing and Pending by EventId
        /// </summary>
        Task<Partnership> TogglePartnershipStatusByEventAsync(Guid eventId);

        /// <summary>
        /// Toggle partnership status between Ongoing and Pending by PartnerId
        /// ✅ NEW: Toggle status for a specific partner's partnership
        /// </summary>
        Task<Partnership> TogglePartnershipStatusByPartnerAsync(Guid partnerId);

        Task UpdateAsync(Guid partnershipId, UpdatePartnershipRequest request);

        /// <summary>
        /// Get all partnerships without an assigned event (EventId is null)
        /// ✅ NEW: For partnerships marketplace - not yet assigned to any event
        /// </summary>
        Task<IEnumerable<PartnershipResponse>> GetUnassignedPartnershipsAsync();

        /// <summary>
        /// Assign a partnership to an event
        /// ✅ NEW: Organizer can assign unassigned partnership to their event
        /// </summary>
        Task<Partnership> AssignPartnershipToEventAsync(Guid partnershipId, Guid eventId);
    }
}
