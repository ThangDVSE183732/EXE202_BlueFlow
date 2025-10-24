using EventLink_Repositories.Models;
using Eventlink_Services.Request;
using Eventlink_Services.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Eventlink_Services.Request.EventProposalRequest;

namespace Eventlink_Services.Interface
{
    public interface IEventProposalService
    {
        // Existing methods
        Task<List<EventProposalResponse>> GetAllEventProposalsAsync();
        Task<EventProposalResponse> GetEventProposalByIdAsync(Guid id);
        Task<EventProposalResponse> CreateEventProposalAsync(CreateEventProposalRequest request);
        Task<EventProposalResponse> UpdateEventProposalAsync(Guid id, UpdateEventProposalRequest request);
        Task<bool> DeleteEventProposalAsync(Guid id);

        // ✅ NEW METHODS FOR SPONSORSHIP
        /// <summary>
        /// Get proposal details with full user information
        /// </summary>
        Task<EventProposalDetailDto> GetProposalDetailAsync(Guid proposalId);

        /// <summary>
        /// Get all proposals for an event (for event detail page)
        /// </summary>
        Task<List<EventProposalDetailDto>> GetProposalsByEventIdAsync(Guid eventId);

        /// <summary>
        /// Get proposals by sponsor/supplier
        /// </summary>
        Task<List<EventProposalDetailDto>> GetProposalsByProposerIdAsync(Guid proposerId);

        /// <summary>
        /// Create sponsorship proposal (Sponsor only)
        /// </summary>
        Task<EventProposalDetailDto> CreateSponsorshipProposalAsync(Guid sponsorId, SponsorshipProposalRequest request);

        /// <summary>
        /// Update sponsorship proposal (Sponsor only, before approval)
        /// </summary>
        Task<EventProposalDetailDto> UpdateSponsorshipProposalAsync(Guid proposalId, Guid sponsorId, SponsorshipProposalRequest request);

        /// <summary>
        /// Approve or reject proposal (Organizer only)
        /// </summary>
        Task<EventProposalDetailDto> UpdateProposalStatusAsync(Guid proposalId, Guid organizerId, UpdateProposalStatusRequest request);

        /// <summary>
        /// Check if user can edit proposal
        /// </summary>
        Task<bool> CanUserEditProposalAsync(Guid proposalId, Guid userId);

        /// <summary>
        /// Check if user can approve/reject proposal (must be event organizer)
        /// </summary>
        Task<bool> CanUserApproveProposalAsync(Guid proposalId, Guid userId);
    }
}