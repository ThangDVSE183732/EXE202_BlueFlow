using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using Eventlink_Services.Interface;
using Eventlink_Services.Request;
using Eventlink_Services.Response;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eventlink_Services.Service
{
    public class EventActivityService : IEventActivityService
    {
        private readonly IEventActivityRepository _activityRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<EventActivityService> _logger;

        public EventActivityService(
            IEventActivityRepository activityRepository,
            IEventRepository eventRepository,
            ILogger<EventActivityService> logger)
        {
            _activityRepository = activityRepository;
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task<List<EventActivityDto>> GetActivitiesByEventIdAsync(Guid eventId)
        {
            try
            {
                var activities = await _activityRepository.GetActivitiesByEventIdAsync(eventId);
                return activities.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting activities for event {EventId}", eventId);
                throw;
            }
        }

        public async Task<EventActivityDto> GetActivityByIdAsync(Guid activityId)
        {
            try
            {
                var activity = await _activityRepository.GetByIdAsync(activityId);
                if (activity == null) return null;
                return MapToDto(activity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting activity {ActivityId}", activityId);
                throw;
            }
        }

        public async Task<EventActivityDto> CreateActivityAsync(Guid eventId, Guid organizerId, EventActivityRequest request)
        {
            try
            {
                // Verify event exists and user is organizer
                var eventEntity = await _eventRepository.GetEventByIdAsync(eventId);
                if (eventEntity == null)
                    throw new KeyNotFoundException("Event not found");

                if (eventEntity.OrganizerId != organizerId)
                    throw new InvalidOperationException("Only event organizer can create activities");

                // Parse time
                if (!TimeSpan.TryParse(request.StartTime, out TimeSpan startTime))
                    throw new InvalidOperationException("Invalid start time format");

                if (!TimeSpan.TryParse(request.EndTime, out TimeSpan endTime))
                    throw new InvalidOperationException("Invalid end time format");

                if (endTime <= startTime)
                    throw new InvalidOperationException("End time must be after start time");

                // ?? OPTIONAL: Check time slot availability
                // Comment out if you want to allow overlapping activities
                var isAvailable = await _activityRepository.IsTimeSlotAvailableAsync(eventId, startTime, endTime);
                if (!isAvailable)
                {
                    _logger.LogWarning("Time slot conflict detected for event {EventId} between {StartTime} and {EndTime}", 
                        eventId, startTime, endTime);
                    // ? Changed from throw to just warning - allow overlap
                    // throw new InvalidOperationException("Time slot is not available");
                }

                // Create activity
                var activity = new EventActivity
                {
                    Id = Guid.NewGuid(),
                    EventId = eventId,
                    ActivityName = request.ActivityName,
                    ActivityDescription = request.ActivityDescription,
                    StartTime = startTime,
                    EndTime = endTime,
                    Location = request.Location,
                    Speakers = request.Speakers,
                    ActivityType = request.ActivityType,
                    MaxParticipants = request.MaxParticipants,
                    CurrentParticipants = 0,
                    IsPublic = request.IsPublic ?? true,
                    DisplayOrder = request.DisplayOrder ?? 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _activityRepository.AddAsync(activity);

                _logger.LogInformation("Activity {ActivityId} created for event {EventId}", activity.Id, eventId);

                return MapToDto(activity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating activity for event {EventId}", eventId);
                throw;
            }
        }

        public async Task<EventActivityDto> UpdateActivityAsync(Guid activityId, Guid organizerId, EventActivityRequest request)
        {
            try
            {
                var activity = await _activityRepository.GetByIdAsync(activityId);
                if (activity == null)
                    throw new KeyNotFoundException("Activity not found");

                // Verify user is organizer
                var eventEntity = await _eventRepository.GetEventByIdAsync(activity.EventId);
                if (eventEntity == null)
                    throw new KeyNotFoundException("Event not found");

                if (eventEntity.OrganizerId != organizerId)
                    throw new InvalidOperationException("Only event organizer can update activities");

                // Parse time
                if (!TimeSpan.TryParse(request.StartTime, out TimeSpan startTime))
                    throw new InvalidOperationException("Invalid start time format");

                if (!TimeSpan.TryParse(request.EndTime, out TimeSpan endTime))
                    throw new InvalidOperationException("Invalid end time format");

                if (endTime <= startTime)
                    throw new InvalidOperationException("End time must be after start time");

                // ?? OPTIONAL: Check time slot availability (exclude current activity)
                // Comment out if you want to allow overlapping activities
                var isAvailable = await _activityRepository.IsTimeSlotAvailableAsync(
                    activity.EventId, startTime, endTime, activityId);
                if (!isAvailable)
                {
                    _logger.LogWarning("Time slot conflict detected for event {EventId} between {StartTime} and {EndTime}", 
                        activity.EventId, startTime, endTime);
                    // ? Changed from throw to just warning - allow overlap
                    // throw new InvalidOperationException("Time slot is not available");
                }

                // Update activity
                activity.ActivityName = request.ActivityName;
                activity.ActivityDescription = request.ActivityDescription;
                activity.StartTime = startTime;
                activity.EndTime = endTime;
                activity.Location = request.Location;
                activity.Speakers = request.Speakers;
                activity.ActivityType = request.ActivityType;
                activity.MaxParticipants = request.MaxParticipants;
                activity.IsPublic = request.IsPublic ?? activity.IsPublic;
                activity.DisplayOrder = request.DisplayOrder ?? activity.DisplayOrder;
                activity.UpdatedAt = DateTime.UtcNow;

                _activityRepository.Update(activity);

                _logger.LogInformation("Activity {ActivityId} updated", activityId);

                return MapToDto(activity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating activity {ActivityId}", activityId);
                throw;
            }
        }

        public async Task<bool> DeleteActivityAsync(Guid activityId, Guid organizerId)
        {
            try
            {
                var activity = await _activityRepository.GetByIdAsync(activityId);
                if (activity == null)
                    throw new KeyNotFoundException("Activity not found");

                // Verify user is organizer
                var eventEntity = await _eventRepository.GetEventByIdAsync(activity.EventId);
                if (eventEntity == null)
                    throw new KeyNotFoundException("Event not found");

                if (eventEntity.OrganizerId != organizerId)
                    throw new InvalidOperationException("Only event organizer can delete activities");

                _activityRepository.Remove(activity);

                _logger.LogInformation("Activity {ActivityId} deleted", activityId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting activity {ActivityId}", activityId);
                throw;
            }
        }

        public async Task<List<EventActivityDto>> BulkUpdateActivitiesAsync(
            Guid eventId, Guid organizerId, List<EventActivityRequest> activities)
        {
            try
            {
                // Verify event exists and user is organizer
                var eventEntity = await _eventRepository.GetEventByIdAsync(eventId);
                if (eventEntity == null)
                    throw new KeyNotFoundException("Event not found");

                if (eventEntity.OrganizerId != organizerId)
                    throw new InvalidOperationException("Only event organizer can update activities");

                // Delete existing activities
                await _activityRepository.DeleteActivitiesByEventIdAsync(eventId);

                // Create new activities
                var newActivities = new List<EventActivity>();
                foreach (var request in activities)
                {
                    if (!TimeSpan.TryParse(request.StartTime, out TimeSpan startTime))
                        throw new InvalidOperationException($"Invalid start time format: {request.StartTime}");

                    if (!TimeSpan.TryParse(request.EndTime, out TimeSpan endTime))
                        throw new InvalidOperationException($"Invalid end time format: {request.EndTime}");

                    if (endTime <= startTime)
                        throw new InvalidOperationException($"End time must be after start time for {request.ActivityName}");

                    var activity = new EventActivity
                    {
                        Id = Guid.NewGuid(),
                        EventId = eventId,
                        ActivityName = request.ActivityName,
                        ActivityDescription = request.ActivityDescription,
                        StartTime = startTime,
                        EndTime = endTime,
                        Location = request.Location,
                        Speakers = request.Speakers,
                        ActivityType = request.ActivityType,
                        MaxParticipants = request.MaxParticipants,
                        CurrentParticipants = 0,
                        IsPublic = request.IsPublic ?? true,
                        DisplayOrder = request.DisplayOrder ?? 0,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    newActivities.Add(activity);
                }

                await _activityRepository.BulkInsertActivitiesAsync(newActivities);

                _logger.LogInformation("Bulk updated {Count} activities for event {EventId}", 
                    newActivities.Count, eventId);

                return newActivities.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk updating activities for event {EventId}", eventId);
                throw;
            }
        }

        #region Helper Methods

        private EventActivityDto MapToDto(EventActivity activity)
        {
            return new EventActivityDto
            {
                Id = activity.Id,
                EventId = activity.EventId,
                ActivityName = activity.ActivityName ?? string.Empty,
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

        #endregion
    }
}
