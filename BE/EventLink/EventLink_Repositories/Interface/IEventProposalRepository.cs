using EventLink_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventLink_Repositories.Interface
{
    public interface IEventProposalRepository : IGenericRepository<EventProposal>
    {
        Task<List<EventProposal>> GetAllEventProposalsAsync();
        Task<EventProposal> GetEventProposalByIdAsync(Guid id);

        // ✅ NEW METHODS
        Task<List<EventProposal>> GetProposalsByEventIdAsync(Guid eventId);
        Task<List<EventProposal>> GetProposalsByProposerIdAsync(Guid proposerId);
        Task<List<EventProposal>> GetProposalsByStatusAsync(string status);
        Task<EventProposal> GetProposalDetailAsync(Guid id);  // Include User navigation
        Task<decimal> GetTotalApprovedSponsorshipAsync(Guid eventId);
        Task<int> GetProposalCountByEventAsync(Guid eventId, string? status = null);
    }
}