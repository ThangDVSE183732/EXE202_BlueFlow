using EventLink_Repositories.Models;
using Eventlink_Services.Request;
using Eventlink_Services.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Eventlink_Services.Request.EventRequest;

namespace Eventlink_Services.Interface
{
    public interface IEventService
    {
        // ✅ EXISTING METHODS - KEEP
        Task<List<EventResponse>> GetAllEventsAsync();
        Task<EventResponse> GetEventByIdAsync(Guid id);
        Task<Event> GetEventById(Guid id);
        Task<List<EventResponse>> GetEventsByOrganizerIdAsync(Guid organizerId);
        Task<List<EventResponse>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<EventResponse>> GetEventsByLocationAsync(string location);
        Task<List<EventResponse>> GetEventsByTypeAsync(string eventType);
        Task<List<EventResponse>> SearchEvents(string name, string location, DateTime? startDate, DateTime? endDate, string eventType);
        Task Create(Guid userId, CreateEventRequest request);
        Task Update(Guid id, UpdateEventRequest request);
        void Remove(Event @event);
        Task UpdateStatus(Guid id, string status);

        // ✅ NEW METHODS FOR ENHANCED FUNCTIONALITY
        /// <summary>
        /// Get complete event details with timeline and proposals
        /// </summary>
        Task<EventDetailDto> GetEventDetailAsync(Guid eventId, Guid? currentUserId = null);

        /// <summary>
        /// Check if user can edit event (must be organizer)
        /// </summary>
        Task<bool> CanUserEditEventAsync(Guid eventId, Guid userId);

        /// <summary>
        /// Update event budget and recalculate remaining budget
        /// </summary>
        Task UpdateEventBudgetAsync(Guid eventId);

        // ❌ REMOVED: CreateEventWithDetailsAsync (duplicate, use separate APIs)
        // ❌ REMOVED: UpdateEventWithDetailsAsync (duplicate, use separate APIs)
    }

    public interface IEventActivityService
    {
        /// <summary>
        /// Get all activities for an event
        /// </summary>
        Task<List<EventActivityDto>> GetActivitiesByEventIdAsync(Guid eventId);

        /// <summary>
        /// Get single activity
        /// </summary>
        Task<EventActivityDto> GetActivityByIdAsync(Guid activityId);

        /// <summary>
        /// Create new activity (Organizer only)
        /// </summary>
        Task<EventActivityDto> CreateActivityAsync(Guid eventId, Guid organizerId, EventActivityRequest request);

        /// <summary>
        /// Update activity (Organizer only)
        /// </summary>
        Task<EventActivityDto> UpdateActivityAsync(Guid activityId, Guid organizerId, EventActivityRequest request);

        /// <summary>
        /// Delete activity (Organizer only)
        /// </summary>
        Task<bool> DeleteActivityAsync(Guid activityId, Guid organizerId);

        /// <summary>
        /// Bulk update activities for an event
        /// </summary>
        Task<List<EventActivityDto>> BulkUpdateActivitiesAsync(Guid eventId, Guid organizerId, List<EventActivityRequest> activities);
    }
}