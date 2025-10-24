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
using static EventLink_Repositories.Models.Event;
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
                Status = EventStatus.Draft,
                ViewCount = 0,
                InterestedCount = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Upload cover image
            if (request.CoverImageUrl != null)
            {
                var coverUrl = await _cloudinaryService.UploadImageAsync(request.CoverImageUrl);
                newEvent.CoverImageUrl = coverUrl;
            }

            // Upload event images
            if (request.EventImages != null && request.EventImages.Any())
            {
                var urls = new List<string>();
                foreach (var image in request.EventImages)
                {
                    var url = await _cloudinaryService.UploadImageAsync(image);
                    urls.Add(url);
                }
                newEvent.EventImages = JsonSerializer.Serialize(urls);
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

            // Handle images
            var existingImages = new List<string>();
            if (!string.IsNullOrEmpty(existingEvent.EventImages))
            {
                try
                {
                    existingImages = JsonSerializer.Deserialize<List<string>>(existingEvent.EventImages) ?? new List<string>();
                }
                catch
                {
                    existingImages = existingEvent.EventImages.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                }
            }

            if (request.NewImages != null && request.NewImages.Any())
            {
                foreach (var image in request.NewImages)
                {
                    var newUrl = await _cloudinaryService.UploadImageAsync(image);
                    existingImages.Add(newUrl);
                }
            }

            if (request.ExistingImages != null && request.ExistingImages.Any())
            {
                existingImages = existingImages.Where(img => request.ExistingImages.Contains(img)).ToList();
            }

            existingEvent.EventImages = JsonSerializer.Serialize(existingImages);
            _eventRepository.Update(existingEvent);
        }

        public void Remove(Event @event)
        {
            _eventRepository.Remove(@event);
        }

        public async Task UpdateStatus(Guid id, string status)
        {
            var existingEvent = await _eventRepository.GetEventByIdAsync(id);
            if (existingEvent == null)
                throw new KeyNotFoundException("Event not found");

            existingEvent.Status = status;
            existingEvent.UpdatedAt = DateTime.UtcNow;
            _eventRepository.Update(existingEvent);
        }

        #endregion

        #region New Methods for Enhanced Functionality

        /// <summary>
        /// Get complete event details with timeline and proposals
        /// </summary>
        public async Task<EventDetailDto> GetEventDetailAsync(Guid eventId, Guid? currentUserId = null)
        {
            try
            {
                var @event = await _eventRepository.GetEventByIdAsync(eventId);
                if (@event == null)
                    throw new KeyNotFoundException($"Event {eventId} not found");

                var organizer = await _userRepository.FirstOrDefaultAsync(u => u.Id == @event.OrganizerId);

                // Get timeline activities
                var activities = await _activityRepository.GetActivitiesByEventIdAsync(eventId);

                // Get proposals
                var proposals = await _proposalRepository.GetProposalsByEventIdAsync(eventId);

                // Filter proposals based on user role
                List<EventProposal> filteredProposals = proposals;
                if (currentUserId.HasValue && @event.OrganizerId != currentUserId.Value)
                {
                    // Non-organizers can only see their own proposals
                    filteredProposals = proposals.Where(p => p.ProposedBy == currentUserId.Value).ToList();
                }

                // Parse event images
                List<string> eventImages = null;
                if (!string.IsNullOrEmpty(@event.EventImages))
                {
                    try
                    {
                        eventImages = JsonSerializer.Deserialize<List<string>>(@event.EventImages);
                    }
                    catch
                    {
                        eventImages = @event.EventImages.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                    }
                }

                return new EventDetailDto
                {
                    Id = @event.Id,
                    OrganizerId = @event.OrganizerId,
                    OrganizerName = organizer?.FullName ?? "Unknown",
                    OrganizerEmail = organizer?.Email,
                    OrganizerPhone = organizer?.PhoneNumber,
                    Title = @event.Title,
                    Description = @event.Description,
                    ShortDescription = @event.ShortDescription,
                    EventDate = @event.EventDate,
                    EndDate = @event.EndDate,
                    Location = @event.Location,
                    VenueDetails = @event.VenueDetails,
                    TotalBudget = @event.TotalBudget,
                    TotalSponsorship = await _proposalRepository.GetTotalApprovedSponsorshipAsync(eventId),
                    RemainingBudget = @event.TotalBudget - await _proposalRepository.GetTotalApprovedSponsorshipAsync(eventId),
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
                    EventImages = eventImages,
                    ViewCount = @event.ViewCount,
                    InterestedCount = @event.InterestedCount,
                    CreatedAt = @event.CreatedAt,
                    UpdatedAt = @event.UpdatedAt,
                    Timeline = activities.Select(MapActivityToDto).ToList(),
                    Proposals = filteredProposals.Select(MapProposalToDetailDto).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting event detail for {EventId}", eventId);
                throw;
            }
        }

        /// <summary>
        /// Create event with initial timeline activities
        /// </summary>
        public async Task<EventDetailDto> CreateEventWithDetailsAsync(Guid organizerId, CreateEventWithDetailsRequest request)
        {
            try
            {
                // Create event first
                var newEvent = new Event
                {
                    Id = Guid.NewGuid(),
                    OrganizerId = organizerId,
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
                    Status = EventStatus.Draft,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Upload images
                if (request.CoverImageUrl != null)
                {
                    newEvent.CoverImageUrl = await _cloudinaryService.UploadImageAsync(request.CoverImageUrl);
                }

                if (request.EventImages != null && request.EventImages.Any())
                {
                    var urls = new List<string>();
                    foreach (var image in request.EventImages)
                    {
                        urls.Add(await _cloudinaryService.UploadImageAsync(image));
                    }
                    newEvent.EventImages = JsonSerializer.Serialize(urls);
                }

                await _eventRepository.AddAsync(newEvent);

                // Create initial activities if provided
                if (request.InitialActivities != null && request.InitialActivities.Any())
                {
                    var activities = new List<EventActivity>();
                    int displayOrder = 0;

                    foreach (var actReq in request.InitialActivities.OrderBy(a => a.StartTime))
                    {
                        activities.Add(new EventActivity
                        {
                            Id = Guid.NewGuid(),
                            EventId = newEvent.Id,
                            ActivityName = actReq.ActivityName,
                            ActivityDescription = actReq.ActivityDescription,
                            StartTime = TimeSpan.Parse(actReq.StartTime),
                            EndTime = TimeSpan.Parse(actReq.EndTime),
                            Location = actReq.Location,
                            Speakers = actReq.Speakers,
                            ActivityType = actReq.ActivityType,
                            MaxParticipants = actReq.MaxParticipants,
                            IsPublic = actReq.IsPublic ?? true,
                            DisplayOrder = displayOrder++,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }

                    await _activityRepository.BulkInsertActivitiesAsync(activities);
                }

                _logger.LogInformation("Event {EventId} created with timeline by {OrganizerId}", newEvent.Id, organizerId);

                return await GetEventDetailAsync(newEvent.Id, organizerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event with details");
                throw;
            }
        }

        /// <summary>
        /// Update event and replace timeline activities
        /// </summary>
        public async Task<EventDetailDto> UpdateEventWithDetailsAsync(Guid eventId, Guid organizerId, UpdateEventWithDetailsRequest request)
        {
            try
            {
                var existingEvent = await _eventRepository.GetEventByIdAsync(eventId);
                if (existingEvent == null)
                    throw new KeyNotFoundException("Event not found");

                if (existingEvent.OrganizerId != organizerId)
                    throw new UnauthorizedAccessException("Only event organizer can update event");

                // Update event info
                await Update(eventId, request);

                // Update timeline if provided
                if (request.UpdatedActivities != null)
                {
                    await _activityRepository.DeleteActivitiesByEventIdAsync(eventId);

                    if (request.UpdatedActivities.Any())
                    {
                        var activities = new List<EventActivity>();
                        int displayOrder = 0;

                        foreach (var actReq in request.UpdatedActivities.OrderBy(a => a.StartTime))
                        {
                            activities.Add(new EventActivity
                            {
                                Id = Guid.NewGuid(),
                                EventId = eventId,
                                ActivityName = actReq.ActivityName,
                                ActivityDescription = actReq.ActivityDescription,
                                StartTime = TimeSpan.Parse(actReq.StartTime),
                                EndTime = TimeSpan.Parse(actReq.EndTime),
                                Location = actReq.Location,
                                Speakers = actReq.Speakers,
                                ActivityType = actReq.ActivityType,
                                MaxParticipants = actReq.MaxParticipants,
                                IsPublic = actReq.IsPublic ?? true,
                                DisplayOrder = displayOrder++,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            });
                        }

                        await _activityRepository.BulkInsertActivitiesAsync(activities);
                    }
                }

                _logger.LogInformation("Event {EventId} updated with timeline by {OrganizerId}", eventId, organizerId);

                return await GetEventDetailAsync(eventId, organizerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event with details");
                throw;
            }
        }

        /// <summary>
        /// Check if user can edit event (must be organizer)
        /// </summary>
        public async Task<bool> CanUserEditEventAsync(Guid eventId, Guid userId)
        {
            var @event = await _eventRepository.GetEventByIdAsync(eventId);
            return @event != null && @event.OrganizerId == userId;
        }

        /// <summary>
        /// Update event budget and recalculate remaining budget
        /// </summary>
        public async Task UpdateEventBudgetAsync(Guid eventId)
        {
            try
            {
                var @event = await _eventRepository.GetEventByIdAsync(eventId);
                if (@event == null) return;

                var totalSponsorship = await _proposalRepository.GetTotalApprovedSponsorshipAsync(eventId);

                // Update event (Note: Cần thêm columns này vào Event model)
                // event.TotalSponsorship = totalSponsorship;
                // event.RemainingBudget = event.TotalBudget - totalSponsorship;

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
                ActivityName = activity.ActivityName,
                ActivityDescription = activity.ActivityDescription,
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
                catch { }
            }

            // Parse benefits
            List<string> benefits = null;
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