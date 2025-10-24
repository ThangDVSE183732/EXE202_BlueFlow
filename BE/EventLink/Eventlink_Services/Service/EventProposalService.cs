using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using Eventlink_Services.Interface;
using Eventlink_Services.Request;
using Eventlink_Services.Response;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static Eventlink_Services.Request.EventProposalRequest;

namespace Eventlink_Services.Service
{
    public class EventProposalService : IEventProposalService
    {
        private readonly IEventProposalRepository _proposalRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<EventProposalService> _logger;

        public EventProposalService(
            IEventProposalRepository proposalRepository,
            IEventRepository eventRepository,
            IUserRepository userRepository,
            ILogger<EventProposalService> logger)
        {
            _proposalRepository = proposalRepository;
            _eventRepository = eventRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        #region Existing Methods

        public async Task<List<EventProposalResponse>> GetAllEventProposalsAsync()
        {
            var proposals = await _proposalRepository.GetAllEventProposalsAsync();
            return proposals.Select(MapToResponse).ToList();
        }

        public async Task<EventProposalResponse> GetEventProposalByIdAsync(Guid id)
        {
            var proposal = await _proposalRepository.GetEventProposalByIdAsync(id);
            if (proposal == null) return null;
            return MapToResponse(proposal);
        }

        public async Task<EventProposalResponse> CreateEventProposalAsync(CreateEventProposalRequest request)
        {
            var proposal = new EventProposal
            {
                Id = Guid.NewGuid(),
                EventId = request.EventId,
                ProposalType = request.ProposalType,
                Title = request.Title,
                Description = request.Description,
                Requirements = request.Requirements,
                Budget = request.Budget,
                Deadline = request.Deadline,
                ContactInstructions = request.ContactInstructions,
                AttachmentUrls = request.AttachmentUrls,
                Status = "Pending",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _proposalRepository.AddAsync(proposal);

            return MapToResponse(proposal);
        }

        public async Task<EventProposalResponse> UpdateEventProposalAsync(Guid id, UpdateEventProposalRequest request)
        {
            var proposal = await _proposalRepository.GetEventProposalByIdAsync(id);
            if (proposal == null) return null;

            proposal.EventId = request.EventId;
            proposal.ProposalType = request.ProposalType;
            proposal.Title = request.Title;
            proposal.Description = request.Description;
            proposal.Requirements = request.Requirements;
            proposal.Budget = request.Budget;
            proposal.Deadline = request.Deadline;
            proposal.ContactInstructions = request.ContactInstructions;
            proposal.AttachmentUrls = request.AttachmentUrls;
            proposal.UpdatedAt = DateTime.UtcNow;

            _proposalRepository.Update(proposal);

            return MapToResponse(proposal);
        }

        public async Task<bool> DeleteEventProposalAsync(Guid id)
        {
            var proposal = await _proposalRepository.GetEventProposalByIdAsync(id);
            if (proposal == null) return false;

            _proposalRepository.Remove(proposal);
            return true;
        }

        #endregion

        #region New Methods for Sponsorship

        /// <summary>
        /// Get proposal details with full user information
        /// </summary>
        public async Task<EventProposalDetailDto> GetProposalDetailAsync(Guid proposalId)
        {
            try
            {
                var proposal = await _proposalRepository.GetProposalDetailAsync(proposalId);
                if (proposal == null)
                    throw new KeyNotFoundException($"Proposal {proposalId} not found");

                return MapToDetailDto(proposal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting proposal detail {ProposalId}", proposalId);
                throw;
            }
        }

        /// <summary>
        /// Get all proposals for an event
        /// </summary>
        public async Task<List<EventProposalDetailDto>> GetProposalsByEventIdAsync(Guid eventId)
        {
            try
            {
                var proposals = await _proposalRepository.GetProposalsByEventIdAsync(eventId);
                return proposals.Select(MapToDetailDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting proposals for event {EventId}", eventId);
                throw;
            }
        }

        /// <summary>
        /// Get proposals by sponsor/supplier
        /// </summary>
        public async Task<List<EventProposalDetailDto>> GetProposalsByProposerIdAsync(Guid proposerId)
        {
            try
            {
                var proposals = await _proposalRepository.GetProposalsByProposerIdAsync(proposerId);
                return proposals.Select(MapToDetailDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting proposals for proposer {ProposerId}", proposerId);
                throw;
            }
        }

        /// <summary>
        /// Create sponsorship proposal (Sponsor only)
        /// </summary>
        public async Task<EventProposalDetailDto> CreateSponsorshipProposalAsync(
            Guid sponsorId,
            SponsorshipProposalRequest request)
        {
            try
            {
                // Verify event exists
                var @event = await _eventRepository.GetEventByIdAsync(request.EventId);
                if (@event == null)
                    throw new KeyNotFoundException($"Event {request.EventId} not found");

                // Verify user is sponsor
                var sponsor = await _userRepository.FirstOrDefaultAsync(u => u.Id == sponsorId);
                if (sponsor == null || sponsor.Role != "Sponsor")
                    throw new UnauthorizedAccessException("Only sponsors can create sponsorship proposals");

                // Serialize funding breakdown
                string fundingBreakdownJson = null;
                if (request.FundingBreakdown != null && request.FundingBreakdown.Any())
                {
                    fundingBreakdownJson = JsonSerializer.Serialize(request.FundingBreakdown);
                }

                // Serialize benefits
                string benefitsJson = null;
                if (request.Benefits != null && request.Benefits.Any())
                {
                    benefitsJson = JsonSerializer.Serialize(request.Benefits);
                }

                var proposal = new EventProposal
                {
                    Id = Guid.NewGuid(),
                    EventId = request.EventId,
                    ProposalType = "Sponsorship",
                    Title = request.Title,
                    Description = request.Description,
                    Budget = request.Budget,
                    ContactInstructions = request.ContactInstructions,
                    AttachmentUrls = request.AttachmentUrls,
                    ProposedBy = sponsorId,
                    SponsorTier = request.SponsorTier,
                    FundingBreakdown = fundingBreakdownJson,
                    Benefits = benefitsJson,
                    Status = "Pending",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _proposalRepository.AddAsync(proposal);

                _logger.LogInformation("Sponsorship proposal {ProposalId} created by {SponsorId} for event {EventId}",
                    proposal.Id, sponsorId, request.EventId);

                // Reload with navigation properties
                return await GetProposalDetailAsync(proposal.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating sponsorship proposal");
                throw;
            }
        }

        /// <summary>
        /// Update sponsorship proposal (Sponsor only, before approval)
        /// </summary>
        public async Task<EventProposalDetailDto> UpdateSponsorshipProposalAsync(
            Guid proposalId,
            Guid sponsorId,
            SponsorshipProposalRequest request)
        {
            try
            {
                var proposal = await _proposalRepository.GetProposalDetailAsync(proposalId);
                if (proposal == null)
                    throw new KeyNotFoundException($"Proposal {proposalId} not found");

                // Check ownership
                if (proposal.ProposedBy != sponsorId)
                    throw new UnauthorizedAccessException("You can only edit your own proposals");

                // Check status
                if (proposal.Status != "Pending")
                    throw new InvalidOperationException("Cannot edit proposal after it has been approved or rejected");

                // Update fields
                proposal.Title = request.Title;
                proposal.Description = request.Description;
                proposal.Budget = request.Budget;
                proposal.SponsorTier = request.SponsorTier;
                proposal.ContactInstructions = request.ContactInstructions;
                proposal.AttachmentUrls = request.AttachmentUrls;

                if (request.FundingBreakdown != null)
                {
                    proposal.FundingBreakdown = JsonSerializer.Serialize(request.FundingBreakdown);
                }

                if (request.Benefits != null)
                {
                    proposal.Benefits = JsonSerializer.Serialize(request.Benefits);
                }

                proposal.UpdatedAt = DateTime.UtcNow;

                _proposalRepository.Update(proposal);

                _logger.LogInformation("Sponsorship proposal {ProposalId} updated by {SponsorId}",
                    proposalId, sponsorId);

                return await GetProposalDetailAsync(proposalId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating sponsorship proposal {ProposalId}", proposalId);
                throw;
            }
        }

        /// <summary>
        /// Approve or reject proposal (Organizer only)
        /// </summary>
        public async Task<EventProposalDetailDto> UpdateProposalStatusAsync(
            Guid proposalId,
            Guid organizerId,
            UpdateProposalStatusRequest request)
        {
            try
            {
                var proposal = await _proposalRepository.GetProposalDetailAsync(proposalId);
                if (proposal == null)
                    throw new KeyNotFoundException($"Proposal {proposalId} not found");

                // Verify user is event organizer
                var @event = await _eventRepository.GetEventByIdAsync(proposal.EventId);
                if (@event == null || @event.OrganizerId != organizerId)
                    throw new UnauthorizedAccessException("Only event organizer can approve/reject proposals");

                // Check current status
                if (proposal.Status != "Pending")
                    throw new InvalidOperationException($"Proposal is already {proposal.Status}");

                // Update status
                proposal.Status = request.Status;
                proposal.ApprovedBy = organizerId;
                proposal.ApprovedAt = DateTime.UtcNow;

                if (request.Status == "Rejected")
                {
                    proposal.RejectionReason = request.RejectionReason;
                }

                proposal.UpdatedAt = DateTime.UtcNow;

                _proposalRepository.Update(proposal);

                // If approved, update event budget
                if (request.Status == "Approved" && proposal.Budget.HasValue)
                {
                    // Note: EventService should handle this
                    // await _eventService.UpdateEventBudgetAsync(proposal.EventId);
                }

                _logger.LogInformation("Proposal {ProposalId} {Status} by organizer {OrganizerId}",
                    proposalId, request.Status, organizerId);

                return await GetProposalDetailAsync(proposalId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating proposal status {ProposalId}", proposalId);
                throw;
            }
        }

        /// <summary>
        /// Check if user can edit proposal
        /// </summary>
        public async Task<bool> CanUserEditProposalAsync(Guid proposalId, Guid userId)
        {
            try
            {
                var proposal = await _proposalRepository.GetProposalDetailAsync(proposalId);
                if (proposal == null) return false;

                // User must be the proposer and proposal must be pending
                return proposal.ProposedBy == userId && proposal.Status == "Pending";
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check if user can approve/reject proposal
        /// </summary>
        public async Task<bool> CanUserApproveProposalAsync(Guid proposalId, Guid userId)
        {
            try
            {
                var proposal = await _proposalRepository.GetProposalDetailAsync(proposalId);
                if (proposal == null) return false;

                var @event = await _eventRepository.GetEventByIdAsync(proposal.EventId);
                if (@event == null) return false;

                // User must be event organizer
                return @event.OrganizerId == userId;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Helper Methods

        private EventProposalResponse MapToResponse(EventProposal proposal)
        {
            return new EventProposalResponse
            {
                EventId = proposal.EventId,
                ProposalType = proposal.ProposalType,
                Title = proposal.Title,
                Description = proposal.Description,
                Requirements = proposal.Requirements,
                Budget = proposal.Budget,
                Deadline = proposal.Deadline,
                ContactInstructions = proposal.ContactInstructions,
                AttachmentUrls = proposal.AttachmentUrls,
                IsActive = proposal.IsActive,
                CreatedAt = proposal.CreatedAt,
                UpdatedAt = proposal.UpdatedAt
            };
        }

        private EventProposalDetailDto MapToDetailDto(EventProposal proposal)
        {
            // Parse funding breakdown
            List<FundingBreakdownItem> breakdown = null;
            if (!string.IsNullOrEmpty(proposal.FundingBreakdown))
            {
                try
                {
                    var dict = JsonSerializer.Deserialize<Dictionary<string, decimal>>(proposal.FundingBreakdown);
                    breakdown = dict?.Select(kvp => new FundingBreakdownItem
                    {
                        Category = kvp.Key,
                        Amount = kvp.Value,
                        Percentage = proposal.Budget.HasValue && proposal.Budget.Value > 0
                            ? (kvp.Value / proposal.Budget.Value) * 100
                            : 0
                    }).ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse funding breakdown for proposal {ProposalId}", proposal.Id);
                }
            }

            // Parse benefits
            List<string> benefits = null;
            if (!string.IsNullOrEmpty(proposal.Benefits))
            {
                try
                {
                    benefits = JsonSerializer.Deserialize<List<string>>(proposal.Benefits);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse benefits for proposal {ProposalId}", proposal.Id);
                }
            }

            return new EventProposalDetailDto
            {
                Id = proposal.Id,
                EventId = proposal.EventId,
                ProposalType = proposal.ProposalType,
                Title = proposal.Title,
                Description = proposal.Description,
                Requirements = proposal.Requirements,
                Budget = proposal.Budget,
                Deadline = proposal.Deadline,
                ContactInstructions = proposal.ContactInstructions,
                AttachmentUrls = proposal.AttachmentUrls,
                ProposedBy = proposal.ProposedBy,
                ProposerName = proposal.Proposer?.FullName,
                ProposerEmail = proposal.Proposer?.Email,
                ProposerAvatar = proposal.Proposer?.AvatarUrl,
                SponsorTier = proposal.SponsorTier,
                FundingBreakdown = breakdown,
                Benefits = benefits,
                Status = proposal.Status,
                ApprovedBy = proposal.ApprovedBy,
                ApproverName = proposal.Approver?.FullName,
                ApprovedAt = proposal.ApprovedAt,
                RejectionReason = proposal.RejectionReason,
                IsActive = proposal.IsActive,
                CreatedAt = proposal.CreatedAt,
                UpdatedAt = proposal.UpdatedAt
            };
        }

        #endregion
    }
}