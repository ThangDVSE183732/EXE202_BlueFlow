using EventLink_Repositories.DBContext;
using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
            return await _context.EventProposals
                .Include(p => p.Event)
                .Include(p => p.Proposer)
                .Include(p => p.Approver)
                .ToListAsync();
        }

        public async Task<EventProposal> GetEventProposalByIdAsync(Guid id)
        {
            return await _context.EventProposals
                .Include(p => p.Event)
                .Include(p => p.Proposer)
                .Include(p => p.Approver)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<EventProposal>> GetProposalsByEventIdAsync(Guid eventId)
        {
            return await _context.EventProposals
                .Include(p => p.Proposer)
                .Include(p => p.Approver)
                .Where(p => p.EventId == eventId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<EventProposal>> GetProposalsByProposerIdAsync(Guid proposerId)
        {
            return await _context.EventProposals
                .Include(p => p.Event)
                .ThenInclude(e => e.Organizer)
                .Where(p => p.ProposedBy == proposerId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<EventProposal>> GetProposalsByStatusAsync(string status)
        {
            return await _context.EventProposals
                .Include(p => p.Event)
                .Include(p => p.Proposer)
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<EventProposal> GetProposalDetailAsync(Guid id)
        {
            return await _context.EventProposals
                .Include(p => p.Event)
                    .ThenInclude(e => e.Organizer)
                .Include(p => p.Proposer)
                .Include(p => p.Approver)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<decimal> GetTotalApprovedSponsorshipAsync(Guid eventId)
        {
            return await _context.EventProposals
                .Where(p => p.EventId == eventId &&
                           p.ProposalType == "Sponsorship" &&
                           p.Status == "Approved" &&
                           p.Budget.HasValue)
                .SumAsync(p => p.Budget.Value);
        }

        public async Task<int> GetProposalCountByEventAsync(Guid eventId, string? status = null)
        {
            var query = _context.EventProposals.Where(p => p.EventId == eventId);

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(p => p.Status == status);
            }

            return await query.CountAsync();
        }
    }
}