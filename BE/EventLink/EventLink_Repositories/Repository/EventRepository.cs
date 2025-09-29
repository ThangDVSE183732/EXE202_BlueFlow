using EventLink_Repositories.DBContext;
using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using EventLink_Repositories.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Interface
{
    public class EventRepository : GenericRepository<Event>, IEventRepository
    {
        private readonly EventLinkDBContext _context;
        public EventRepository(EventLinkDBContext context) : base(context)
        {
            _context = context;
        }
        public Task<List<Event>> GetActiveEventsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Event>> GetAllEventsAsync()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task<Event> GetEventByIdAsync(Guid id)
        {
            return await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
        }

        public Task<List<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task<List<Event>> GetEventsByLocationAsync(string location)
        {
            return _context.Events.Where(e => e.Location.Contains(location)).ToListAsync();
        }

        public async Task<List<Event>> GetEventsByOrganizerIdAsync(Guid organizerId)
        {
            return await _context.Events.Where(e => e.OrganizerId == organizerId).ToListAsync();
        }

        public async Task<List<Event>> GetEventsByTypeAsync(string eventType)
        {
            return await _context.Events.Where(e => e.EventType == eventType).ToListAsync();
        }

        public Task<List<Event>> SearchEvents(string title, string location, DateTime? startDate, DateTime? endDate, string eventType)
        {
            var query = _context.Events.AsQueryable();
            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(e => e.Title.Contains(title));
            }
            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(e => e.Location.Contains(location));
            }
            if (startDate.HasValue)
            {
                query = query.Where(e => e.EventDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(e => e.EventDate <= endDate.Value);
            }
            if (!string.IsNullOrEmpty(eventType))
            {
                query = query.Where(e => e.EventType == eventType);
            }
            return query.ToListAsync();
        }
    }
}
