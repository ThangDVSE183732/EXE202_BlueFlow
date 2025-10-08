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
        private readonly List<string> _validStatuses = new() { "Pending", "Accepted", "Rejected", "In Progress", "Completed", "Cancelled" };

        private readonly EventLinkDBContext _context;
        public PartnershipRepository(EventLinkDBContext context) : base(context)
        {
        }
        public async Task<List<Partnership>> GetPartnershipsByEventIdAsync(Guid eventId)
        {
            return await _context.Partnerships
                .Where(p => p.EventId == eventId)
                .ToListAsync();
        }
        public async Task<Partnership> GetPartnershipByIdAsync(Guid partnershipId)
        {
            return await FirstOrDefaultAsync(p => p.Id == partnershipId);
        }
    }
}
