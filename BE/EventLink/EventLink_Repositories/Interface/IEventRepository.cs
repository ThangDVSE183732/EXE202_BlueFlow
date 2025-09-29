using EventLink_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLink_Repositories.Interface
{
    public interface IEventRepository : IGenericRepository<Event>
    {
        Task<List<Event>> GetAllEventsAsync();
        Task<Event> GetEventByIdAsync(Guid id);
        Task<List<Event>> GetEventsByOrganizerIdAsync(Guid organizerId);
        Task<List<Event>> GetActiveEventsAsync();
        Task<List<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<Event>> GetEventsByLocationAsync(string location);
        Task<List<Event>> GetEventsByTypeAsync(string eventType);
        Task<List<Event>> SearchEvents(string name, string location, DateTime? startDate, DateTime? endDate, string eventType);
    }
}
