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
using static Eventlink_Services.Request.EventRequest;

namespace Eventlink_Services.Service
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventActivityRepository _activityRepository;
        private readonly IEventProposalRepository _proposalRepository;
        private readonly IUserRepository _userRepository;
        private readonly CloudinaryService _cloudinaryService;
        private readonly ILogger<EventService> _logger;

        public EventService(
            IEventRepository eventRepository,
            IEventActivityRepository activityRepository,
            IEventProposalRepository proposalRepository,
            IUserRepository userRepository,
            CloudinaryService cloudinaryService,
            ILogger<EventService> logger)
        {
            _eventRepository = eventRepository;
            _activityRepository = activityRepository;
            _proposalRepository = proposalRepository;
            _userRepository = userRepository;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        #region Existing Methods

        public async Task<List<EventResponse>> GetAllEventsAsync()
        {
            var events = await _eventRepository.GetAllEventsAsync();
            return events.Select(MapToResponse).ToList();
        }

        public async Task<EventResponse> GetEventByIdAsync(Guid id)
        {
            var @event = await _eventRepository.GetEventByIdAsync(id);
            if (@event == null) return null;
            return MapToResponse(@event);
        }

        public async Task<Event> GetEventById(Guid id)
        {
            return await _eventRepository.GetEventByIdAsync(id);
        }

        public async Task<List<EventResponse>> GetEventsByOrganizerIdAsync(Guid organizerId)
        {
            var events = await _eventRepository.GetEventsByOrganizerIdAsync(organizerId);
            return events.Select(MapToResponse).ToList();
        }

        public async Task<List<EventResponse>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var events = await _eventRepository.GetEventsByDateRangeAsync(startDate, endDate);
            return events.Select(MapToResponse).ToList();
        }

        public async Task<List<EventResponse>> GetEventsByLocationAsync(string location)
        {
            var events = await _eventRepository.GetEventsByLocationAsync(location);
            return events.Select(MapToResponse).ToList();
        }

        public async Task<List<EventResponse>> GetEventsByTypeAsync(string eventType)
        {
            var events = await _eventRepository.GetEventsByTypeAsync(eventType);
            return events.Select(MapToResponse).ToList();
        }

        public async Task<List<EventResponse>> SearchEvents(string name, string location, DateTime? startDate, DateTime? endDate, string eventType)
        {
            var events = await _eventRepository.SearchEvents(name, location, startDate, endDate, eventType);
            return events.Select(MapToResponse).ToList();
        }

        public async Task Create(Guid userId, CreateEventRequest request)
        {
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                OrganizerId = userId,
                Title = request.Title,
                Description = request.Description,
                ShortDescription = request.ShortDescription,
                Location = request.Location,
                VenueDetails = request.VenueDetails,
                EventDate = request.EventDate,
                EndDate = request.EndDate,
                EventType = request.EventType,
                Category = request.Category,
                ExpectedAttendees = request.ExpectedAttendees,
                TotalBudget = request.TotalBudget,
                TargetAudience = request.TargetAudience,
                RequiredServices = request.RequiredServices,
                SponsorshipNeeds = request.SponsorshipNeeds,
                SpecialRequirements = request.SpecialRequirements,
                IsPublic = true,
                IsFeatured = false,
                Status = "Draft",
                ViewCount = 0,
                InterestedCount = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Set Overview fields
            if (request.EventHighlights != null && request.EventHighlights.Count > 0)
            {
                newEvent.EventHighlights = JsonSerializer.Serialize(request.EventHighlights);
            }

            if (request.Tags != null && request.Tags.Count > 0)
            {
                newEvent.Tags = JsonSerializer.Serialize(request.Tags);
            }

            if (request.TargetAudienceList != null && request.TargetAudienceList.Count > 0)
            {
                newEvent.TargetAudienceList = JsonSerializer.Serialize(request.TargetAudienceList);
            }

            // Handle cover image
            if (!string.IsNullOrEmpty(request.CoverImageUrl))
            {
                newEvent.CoverImageUrl = request.CoverImageUrl;
            }

            // Handle event images
            if (request.EventImages != null && request.EventImages.Any())
            {
                newEvent.EventImages = JsonSerializer.Serialize(request.EventImages);
            }

            await _eventRepository.AddAsync(newEvent);
        }

        public async Task Update(Guid id, UpdateEventRequest request)
        {
            var existingEvent = await _eventRepository.GetEventByIdAsync(id);
            if (existingEvent == null)
                throw new KeyNotFoundException("Event not found");

            // Update basic info
            existingEvent.Title = request.Title;
            existingEvent.Description = request.Description;
            existingEvent.ShortDescription = request.ShortDescription;
            existingEvent.Location = request.Location;
            existingEvent.VenueDetails = request.VenueDetails;
            existingEvent.EventDate = request.EventDate;
            existingEvent.EndDate = request.EndDate;
            existingEvent.EventType = request.EventType;
            existingEvent.Category = request.Category;
            existingEvent.ExpectedAttendees = request.ExpectedAttendees;
            existingEvent.TotalBudget = request.TotalBudget;
            existingEvent.TargetAudience = request.TargetAudience;
            existingEvent.RequiredServices = request.RequiredServices;
            existingEvent.SponsorshipNeeds = request.SponsorshipNeeds;
            existingEvent.SpecialRequirements = request.SpecialRequirements;
            existingEvent.UpdatedAt = DateTime.UtcNow;

            // Update Overview fields
            if (request.EventHighlights != null)
            {
                existingEvent.EventHighlights = JsonSerializer.Serialize(request.EventHighlights);
            }

            if (request.Tags != null)
            {
                existingEvent.Tags = JsonSerializer.Serialize(request.Tags);
            }

            if (request.TargetAudienceList != null)
            {
                existingEvent.TargetAudienceList = JsonSerializer.Serialize(request.TargetAudienceList);
            }

            // Handle cover image
            if (!string.IsNullOrEmpty(request.CoverImageUrl))
            {
                existingEvent.CoverImageUrl = request.CoverImageUrl;
            }

            // Handle event images
            if (request.NewImages != null && request.NewImages.Any())
            {
                var existingImages = new List<string>();
                if (!string.IsNullOrEmpty(existingEvent.EventImages))
                {
                    try
                    {
                        existingImages = JsonSerializer.Deserialize<List<string>>(existingEvent.EventImages) ?? new List<string>();
                    }
                    catch
                    {
                        existingImages = new List<string>();
                    }
                }

                // Add new images
                existingImages.AddRange(request.NewImages);
                existingEvent.EventImages = JsonSerializer.Serialize(existingImages);
            }

            _eventRepository.Update(existingEvent);
        }

        public void Remove(Event @event)
        {
            _eventRepository.Remove(@event);
        }

        public async Task UpdateStatus(Guid id, string status)
        {
            var @event = await _eventRepository.GetEventByIdAsync(id);
            if (@event == null)
                throw new KeyNotFoundException("Event not found");

            @event.Status = status;
            @event.UpdatedAt = DateTime.UtcNow;
            _eventRepository.Update(@event);
        }

        #endregion

        #region Enhanced Methods

        public async Task<EventDetailDto> GetEventDetailAsync(Guid eventId, Guid? currentUserId = null)
        {
            try
            {
                var eventEntity = await _eventRepository.GetEventByIdAsync(eventId);
                if (eventEntity == null)
                    throw new KeyNotFoundException("Event not found");

                // Get organizer info
                var organizer = eventEntity.Organizer;

                // Get timeline activities
                var activities = await _activityRepository.GetActivitiesByEventIdAsync(eventId);
                var activityDtos = activities.Select(MapActivityToDto).ToList();

                // Get proposals (only if user is organizer)
                List<EventProposalDetailDto>? proposalDtos = null;
                if (currentUserId.HasValue && eventEntity.OrganizerId == currentUserId.Value)
                {
                    var proposals = await _proposalRepository.GetProposalsByEventIdAsync(eventId);
                    proposalDtos = proposals.Select(MapProposalToDetailDto).ToList();
                }

                // Calculate total sponsorship
                var totalSponsorship = await _proposalRepository.GetTotalApprovedSponsorshipAsync(eventId);

                // Parse EventImages
                List<string> eventImages = new List<string>();
                if (!string.IsNullOrEmpty(eventEntity.EventImages))
                {
                    try
                    {
                        eventImages = JsonSerializer.Deserialize<List<string>>(eventEntity.EventImages) ?? new List<string>();
                    }
                    catch { }
                }

                // Parse Overview fields
                List<string>? eventHighlights = null;
                if (!string.IsNullOrEmpty(eventEntity.EventHighlights))
                {
                    try
                    {
                        eventHighlights = JsonSerializer.Deserialize<List<string>>(eventEntity.EventHighlights);
                    }
                    catch { }
                }

                List<string>? tags = null;
                if (!string.IsNullOrEmpty(eventEntity.Tags))
                {
                    try
                    {
                        tags = JsonSerializer.Deserialize<List<string>>(eventEntity.Tags);
                    }
                    catch { }
                }

                List<string>? targetAudienceList = null;
                if (!string.IsNullOrEmpty(eventEntity.TargetAudienceList))
                {
                    try
                    {
                        targetAudienceList = JsonSerializer.Deserialize<List<string>>(eventEntity.TargetAudienceList);
                    }
                    catch { }
                }

                // Map to DTO
                var eventDetailDto = new EventDetailDto
                {
                    Id = eventEntity.Id,
                    OrganizerId = eventEntity.OrganizerId,
                    OrganizerName = organizer?.FullName ?? string.Empty,
                    OrganizerEmail = organizer?.Email,
                    OrganizerPhone = organizer?.PhoneNumber,

                    // Basic info
                    Title = eventEntity.Title ?? string.Empty,
                    Description = eventEntity.Description,
                    ShortDescription = eventEntity.ShortDescription,
                    EventDate = eventEntity.EventDate,
                    EndDate = eventEntity.EndDate,
                    Location = eventEntity.Location,
                    VenueDetails = eventEntity.VenueDetails,

                    // Budget
                    TotalBudget = eventEntity.TotalBudget,
                    TotalSponsorship = totalSponsorship,
                    RemainingBudget = (eventEntity.TotalBudget ?? 0) - totalSponsorship,
                    ExpectedAttendees = eventEntity.ExpectedAttendees,

                    // Details
                    Category = eventEntity.Category,
                    EventType = eventEntity.EventType,
                    TargetAudience = eventEntity.TargetAudience,
                    RequiredServices = eventEntity.RequiredServices,
                    SponsorshipNeeds = eventEntity.SponsorshipNeeds,
                    SpecialRequirements = eventEntity.SpecialRequirements,

                    // Overview fields
                    EventHighlights = eventHighlights,
                    Tags = tags,
                    TargetAudienceList = targetAudienceList,

                    // Status
                    Status = eventEntity.Status,
                    IsPublic = eventEntity.IsPublic,
                    IsFeatured = eventEntity.IsFeatured,

                    // Media
                    CoverImageUrl = eventEntity.CoverImageUrl,
                    EventImages = eventImages,

                    // Engagement
                    ViewCount = eventEntity.ViewCount,
                    InterestedCount = eventEntity.InterestedCount,
                    AverageRating = eventEntity.AverageRating,
                    ReviewCount = eventEntity.ReviewCount,

                    // Dates
                    CreatedAt = eventEntity.CreatedAt,
                    UpdatedAt = eventEntity.UpdatedAt,

                    // Related data
                    Timeline = activityDtos,
                    Proposals = proposalDtos
                };

                return eventDetailDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting event detail for event {EventId}", eventId);
                throw;
            }
        }

        public async Task<bool> CanUserEditEventAsync(Guid eventId, Guid userId)
        {
            var @event = await _eventRepository.GetEventByIdAsync(eventId);
            return @event != null && @event.OrganizerId == userId;
        }

        public async Task UpdateEventBudgetAsync(Guid eventId)
        {
            try
            {
                var @event = await _eventRepository.GetEventByIdAsync(eventId);
                if (@event == null) return;

                var totalSponsorship = await _proposalRepository.GetTotalApprovedSponsorshipAsync(eventId);

                _eventRepository.Update(@event);
                _logger.LogInformation("Budget updated for event {EventId}", eventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating budget for event {EventId}", eventId);
                throw;
            }
        }

        #endregion

        #region Helper Methods

        private EventResponse MapToResponse(Event @event)
        {
            return new EventResponse
            {
                Id = @event.Id,
                OrganizerId = @event.OrganizerId,
                Title = @event.Title,
                Description = @event.Description,
                ShortDescription = @event.ShortDescription,
                EventDate = @event.EventDate,
                EndDate = @event.EndDate,
                Location = @event.Location,
                VenueDetails = @event.VenueDetails,
                TotalBudget = @event.TotalBudget,
                ExpectedAttendees = @event.ExpectedAttendees,
                Category = @event.Category,
                EventType = @event.EventType,
                TargetAudience = @event.TargetAudience,
                RequiredServices = @event.RequiredServices,
                SponsorshipNeeds = @event.SponsorshipNeeds,
                SpecialRequirements = @event.SpecialRequirements,
                Status = @event.Status,
                IsPublic = @event.IsPublic,
                IsFeatured = @event.IsFeatured,
                CoverImageUrl = @event.CoverImageUrl,
                EventImages = @event.EventImages,
                ViewCount = @event.ViewCount,
                InterestedCount = @event.InterestedCount,
                CreatedAt = @event.CreatedAt,
                UpdatedAt = @event.UpdatedAt
            };
        }

        private EventActivityDto MapActivityToDto(EventActivity activity)
        {
            return new EventActivityDto
            {
                Id = activity.Id,
                EventId = activity.EventId,
                ActivityName = activity.ActivityName ?? string.Empty,
                ActivityDescription = activity.ActivityDescription,
                // ✅ FIX: StartTime and EndTime are TimeSpan (NOT nullable), so no ? operator
                StartTime = activity.StartTime.ToString(@"hh\:mm"),
                EndTime = activity.EndTime.ToString(@"hh\:mm"),
                Location = activity.Location,
                Speakers = activity.Speakers,
                ActivityType = activity.ActivityType,
                MaxParticipants = activity.MaxParticipants,
                CurrentParticipants = activity.CurrentParticipants,
                IsPublic = activity.IsPublic,
                DisplayOrder = activity.DisplayOrder,
                CreatedAt = activity.CreatedAt,
                UpdatedAt = activity.UpdatedAt
            };
        }

        private EventProposalDetailDto MapProposalToDetailDto(EventProposal proposal)
        {
            // Parse funding breakdown
            List<FundingBreakdownItem>? breakdown = null;
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
                catch { }
            }

            // Parse benefits
            List<string>? benefits = null;
            if (!string.IsNullOrEmpty(proposal.Benefits))
            {
                try
                {
                    benefits = JsonSerializer.Deserialize<List<string>>(proposal.Benefits);
                }
                catch { }
            }

            return new EventProposalDetailDto
            {
                Id = proposal.Id,
                EventId = proposal.EventId,
                ProposalType = proposal.ProposalType ?? string.Empty,
                Title = proposal.Title ?? string.Empty,
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