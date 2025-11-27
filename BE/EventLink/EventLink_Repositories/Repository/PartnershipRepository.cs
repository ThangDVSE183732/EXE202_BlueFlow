using EventLink_Repositories.DBContext;
using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLink_Repositories.Repository
{
    public class PartnershipRepository : GenericRepository<Partnership>, IPartnershipRepository
    {
        private readonly EventLinkDBContext _context;
        public PartnershipRepository(EventLinkDBContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Update partnership and save changes to database
        /// </summary>
        public async Task UpdateAsync(Partnership partnership)
        {
            _context.Update(partnership);
            await _context.SaveChangesAsync();  // ✅ FIX: Save changes to database
        }

        public async Task<Partnership> GetByIdAsync(Guid id)
        {
            return await _context.Partnerships.FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// Get all partnerships in the system with Event and Partner navigation properties loaded
        /// </summary>
        public async Task<List<Partnership>> GetAllPartnershipsAsync()
        {
            return await _context.Partnerships
                .Include(p => p.Event)      // ✅ Load Event details
                .Include(p => p.Partner)    // ✅ Load Partner (User) details
                    .ThenInclude(u => u.BrandProfiles)  // ✅ Load BrandProfile details
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Get all partnerships for an event with Event and Partner navigation properties loaded
        /// </summary>
        public async Task<List<Partnership>> GetPartnershipsByEventAsync(Guid eventId)
        {
            return await _context.Partnerships
                .Where(p => p.EventId == eventId)
                .Include(p => p.Event)      // ✅ Load Event details
                .Include(p => p.Partner)    // ✅ Load Partner (User) details
                    .ThenInclude(u => u.BrandProfiles)  // ✅ Load BrandProfile details
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Get all partnerships without an assigned event (EventId is null)
        /// ✅ NEW: For partnerships not yet assigned to any event
        /// </summary>
        public async Task<List<Partnership>> GetUnassignedPartnershipsAsync()
        {
            return await _context.Partnerships
                .Where(p => p.EventId == null)  // ✅ Filter partnerships without event
                .Include(p => p.Partner)         // ✅ Load Partner (User) details
                    .ThenInclude(u => u.BrandProfiles)  // ✅ Load BrandProfile details
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Get all partnerships for a partner with Event and Partner navigation properties loaded
        /// ✅ NEW: For getting partnerships by PartnerId
        /// </summary>
        public async Task<List<Partnership>> GetPartnershipsByPartnerAsync(Guid partnerId)
        {
            return await _context.Partnerships
                .Where(p => p.PartnerId == partnerId)
                .Include(p => p.Event)      // ✅ Load Event details
                .Include(p => p.Partner)    // ✅ Load Partner (User) details
                    .ThenInclude(u => u.BrandProfiles)  // ✅ Load BrandProfile details
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }
}
