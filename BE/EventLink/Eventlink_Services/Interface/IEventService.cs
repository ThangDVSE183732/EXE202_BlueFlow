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
        
        // ✅ JSON methods (keep for backward compatibility)
        Task<EventResponse> Create(Guid userId, CreateEventRequest request);
        Task Update(Guid id, UpdateEventRequest request);
        
        // ✅ NEW: FormData methods with file upload
        Task<EventResponse> CreateWithFormData(Guid userId, CreateEventFormRequest formRequest);
        Task UpdateWithFormData(Guid id, UpdateEventFormRequest formRequest);
        
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

        /// <summary>
        /// Update event visibility (isPublic property only)
        /// </summary>
        Task UpdateEventVisibilityAsync(Guid eventId, bool isPublic);

        /// <summary>
        /// Update event featured status (isFeatured property only)
        /// </summary>
        Task UpdateEventFeaturedAsync(Guid eventId, bool isFeatured);
    }

    
}