using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using Eventlink_Services.Interface;
using Eventlink_Services.Request;
using Eventlink_Services.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Eventlink_Services.Request.EventProposalRequest;

namespace Eventlink_Services.Service
{
    public class EventProposalService : IEventProposalService
    {
        private readonly IEventProposalRepository _eventProposalRepository;
        public EventProposalService(IEventProposalRepository eventProposalRepository)
        {
            _eventProposalRepository = eventProposalRepository;
        }
        public async Task<EventProposalResponse> CreateEventProposalAsync(CreateEventProposalRequest request)
        {
            await _eventProposalRepository.AddAsync(new EventProposal
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
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            return new EventProposalResponse
            {
                EventId = request.EventId,
                ProposalType = request.ProposalType,
                Title = request.Title,
                Description = request.Description,
                Requirements = request.Requirements,
                Budget = request.Budget,
                Deadline = request.Deadline,
                ContactInstructions = request.ContactInstructions,
                AttachmentUrls = request.AttachmentUrls,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public async Task<bool> DeleteEventProposalAsync(Guid id)
        {
            var eventProposal = await _eventProposalRepository.GetEventProposalByIdAsync(id);
            if (eventProposal == null) return false;
            _eventProposalRepository.Remove(eventProposal);
            return true;
        }

        public Task<List<EventProposalResponse>> GetAllEventProposalsAsync()
        {
            var eventProposals = _eventProposalRepository.GetAllEventProposalsAsync();
            if (eventProposals == null) return null;
            return eventProposals.ContinueWith(t => t.Result.Select(ep => new EventProposalResponse
            {
                EventId = ep.EventId,
                ProposalType = ep.ProposalType,
                Title = ep.Title,
                Description = ep.Description,
                Requirements = ep.Requirements,
                Budget = ep.Budget,
                Deadline = ep.Deadline,
                ContactInstructions = ep.ContactInstructions,
                AttachmentUrls = ep.AttachmentUrls,
                IsActive = ep.IsActive,
                CreatedAt = ep.CreatedAt,
                UpdatedAt = ep.UpdatedAt
            }).ToList());
        }

        public async Task<EventProposal> GetEventProposalById(Guid id)
        {
            return await _eventProposalRepository.GetEventProposalByIdAsync(id);
        }

        public async Task<EventProposalResponse> GetEventProposalByIdAsync(Guid id)
        {
            var eventProposal = await _eventProposalRepository.GetEventProposalByIdAsync(id);
            if (eventProposal == null) return null;
            return new EventProposalResponse
            {
                EventId = eventProposal.EventId,
                ProposalType = eventProposal.ProposalType,
                Title = eventProposal.Title,
                Description = eventProposal.Description,
                Requirements = eventProposal.Requirements,
                Budget = eventProposal.Budget,
                Deadline = eventProposal.Deadline,
                ContactInstructions = eventProposal.ContactInstructions,
                AttachmentUrls = eventProposal.AttachmentUrls,
                IsActive = eventProposal.IsActive,
                CreatedAt = eventProposal.CreatedAt,
                UpdatedAt = eventProposal.UpdatedAt
            };
        }   

        public async Task<EventProposalResponse> UpdateEventProposalAsync(Guid id, UpdateEventProposalRequest request)
        {
            var existingProposal = await _eventProposalRepository.GetEventProposalByIdAsync(id);
            if(existingProposal == null) return null;
            existingProposal.EventId = request.EventId;
            existingProposal.ProposalType = request.ProposalType;
            existingProposal.Title = request.Title;
            existingProposal.Description = request.Description;
            existingProposal.Requirements = request.Requirements;
            existingProposal.Budget = request.Budget;
            existingProposal.Deadline = request.Deadline;
            existingProposal.ContactInstructions = request.ContactInstructions;
            existingProposal.AttachmentUrls = request.AttachmentUrls;
            existingProposal.UpdatedAt = DateTime.UtcNow;
            _eventProposalRepository.Update(existingProposal);
            return new EventProposalResponse
            {
                EventId = existingProposal.EventId,
                ProposalType = existingProposal.ProposalType,
                Title = existingProposal.Title,
                Description = existingProposal.Description,
                Requirements = existingProposal.Requirements,
                Budget = existingProposal.Budget,
                Deadline = existingProposal.Deadline,
                ContactInstructions = existingProposal.ContactInstructions,
                AttachmentUrls = existingProposal.AttachmentUrls,
                IsActive = existingProposal.IsActive,
                CreatedAt = existingProposal.CreatedAt,
                UpdatedAt = existingProposal.UpdatedAt
            };
        }
    }
}
