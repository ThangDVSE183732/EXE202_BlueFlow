using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using Eventlink_Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Eventlink_Services.Request.PartnershipRequest;

namespace Eventlink_Services.Service
{
    public class PartnershipService : IPartnershipService
    {
        private readonly IPartnershipRepository _partnershipRepository;
        public PartnershipService(IPartnershipRepository partnershipRepository)
        {
            _partnershipRepository = partnershipRepository;
        }

        public async Task CreatePartnershipAsync(CreatePartnershipRequest request)
        {
            var partnership = new Partnership
            {
                Id = Guid.NewGuid(),
                EventId = request.EventId,
                PartnerId = request.PartnerId,
                PartnerType = request.PartnerType,
                InitialMessage = request.InitialMessage,
                OrganizerResponse = request.OrganizerResponse,
                ProposedBudget = request.ProposedBudget,
                AgreedBudget = request.AgreedBudget,
                ServiceDescription = request.ServiceDescription,
                Status = "Pending",
                OrganizerContactInfo = request.OrganizerContactInfo,
                PartnerContactInfo = request.PartnerContactInfo,
                PreferredContactMethod = request.PreferredContactMethod,
                ExternalWorkspaceUrl = request.ExternalWorkspaceUrl,
                StartDate = request.StartDate,
                DeadlineDate = request.DeadlineDate,
                CompletionDate = request.CompletionDate,
                OrganizerNotes = request.OrganizerNotes,
                PartnerNotes = request.PartnerNotes,
                SharedNotes = request.SharedNotes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _partnershipRepository.AddAsync(partnership);
        }

        public async void DeletePartnershipAsync(Guid partnershipId)
        {
            var partnership = await _partnershipRepository.GetPartnershipByIdAsync(partnershipId);
            if (partnership != null)
            {
                _partnershipRepository.Remove(partnership);
            }
        }

        public async Task<Partnership> GetPartnershipByIdAsync(Guid partnershipId)
        {
            return await _partnershipRepository.GetPartnershipByIdAsync(partnershipId);
        }

        public async Task<List<Partnership>> GetPartnershipsByEventIdAsync(Guid eventId)
        {
            return await _partnershipRepository.GetPartnershipsByEventIdAsync(eventId);
        }

        public async Task UpdatePartnershipAsync(Guid id, UpdatePartnershipRequest request)
        {
            var existingPartnership = await _partnershipRepository.GetPartnershipByIdAsync(id);
            if (existingPartnership != null)
            {
                var updatedPartnership = new Partnership
                {
                    Id = existingPartnership.Id,
                    EventId = request.EventId,
                    PartnerId = request.PartnerId,
                    PartnerType = request.PartnerType,
                    InitialMessage = request.InitialMessage,
                    OrganizerResponse = request.OrganizerResponse,
                    ProposedBudget = request.ProposedBudget,
                    AgreedBudget = request.AgreedBudget,
                    ServiceDescription = request.ServiceDescription,
                    Status = "Pending",
                    OrganizerContactInfo = request.OrganizerContactInfo,
                    PartnerContactInfo = request.PartnerContactInfo,
                    PreferredContactMethod = request.PreferredContactMethod,
                    ExternalWorkspaceUrl = request.ExternalWorkspaceUrl,
                    StartDate = request.StartDate,
                    DeadlineDate = request.DeadlineDate,
                    CompletionDate = request.CompletionDate,
                    OrganizerNotes = request.OrganizerNotes,
                    PartnerNotes = request.PartnerNotes,
                    SharedNotes = request.SharedNotes,
                    CreatedAt = existingPartnership.CreatedAt,
                    UpdatedAt = DateTime.UtcNow
                };

                _partnershipRepository.Update(updatedPartnership);
            }
        }

        public async Task PartnershipSuggestionResponse(Guid partnershipId, string organizerResponse, string status)
        {
            var existingPartnership = await _partnershipRepository.GetPartnershipByIdAsync(partnershipId);
            if (existingPartnership != null)
            {
                existingPartnership.OrganizerResponse = organizerResponse;
                existingPartnership.Status = status;
                existingPartnership.UpdatedAt = DateTime.UtcNow;
                _partnershipRepository.Update(existingPartnership);
            }
        }
    }
}
