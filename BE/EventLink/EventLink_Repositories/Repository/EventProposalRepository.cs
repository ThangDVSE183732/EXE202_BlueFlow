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
    public class EventProposalRepository : GenericRepository<EventProposal>, IEventProposalRepository
    {
        private readonly EventLinkDBContext _context;
        public EventProposalRepository(EventLinkDBContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<EventProposal>> GetAllEventProposalsAsync()
        {
            return await _context.EventProposals.ToListAsync();
        }

        public async Task<EventProposal> GetEventProposalByIdAsync(Guid id)
        {
            return await _context.EventProposals.FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
