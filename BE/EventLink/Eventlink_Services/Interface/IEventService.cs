using EventLink_Repositories.Models;
using Eventlink_Services.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Eventlink_Services.Request.EventRequest;

namespace Eventlink_Services.Interface
{
    public interface IEventService
    {
        Task<List<EventResponse>> GetAllEventsAsync();
        Task<EventResponse> GetEventByIdAsync(Guid id);
        Task<Event> GetEventById(Guid id);
        Task<List<EventResponse>> GetEventsByOrganizerIdAsync(Guid organizerId);
        //Task<List<EventResponse>> GetActiveEventsAsync();
        Task<List<EventResponse>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<EventResponse>> GetEventsByLocationAsync(string location);
        Task<List<EventResponse>> GetEventsByTypeAsync(string eventType);
        Task<List<EventResponse>> SearchEvents(string name, string location, DateTime? startDate, DateTime? endDate, string eventType);
        Task Create(Guid userId, CreateEventRequest request);
        Task Update(Guid id, UpdateEventRequest request);
        void Remove(Event @event);
        Task UpdateStatus(Guid id, string status);
    }
}
