using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventLink_Repositories.DBContext;
using EventLink_Repositories.Models;
using Eventlink_Services.Interface;
using Eventlink_Services.Response;
using static Eventlink_Services.Request.EventRequest;

namespace EventLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        // GET: api/Events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventResponse>>> GetEvents()
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(new
            {
                success = true,
                message = "Events retrieved successfully",
                data = events,
                count = events.Count
            });
        }

        // GET: api/Events/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EventResponse>> GetEvent(Guid id)
        {
            var @event = await _eventService.GetEventByIdAsync(id);

            if (@event == null)
            {
                return NotFound();
            }

            return @event;
        }

        // PUT: api/Events/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvent(Guid id, UpdateEventRequest @event)
        {
            var existingEvent = await _eventService.GetEventById(id);
            if (existingEvent == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Event not found"
                });
            }
            _eventService.Update(id, @event);

            return NoContent();
        }

        // POST: api/Events
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EventResponse>> PostEvent(CreateEventRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userIdClaim = User.FindFirst("userId").Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "User not authenticated"
                });
            }
            if(request.EndDate < request.EventDate)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "End date cannot be earlier than start date"
                });
            }

            if(request.EventDate < DateTime.UtcNow)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Event date cannot be in the past"
                });
            }
            await _eventService.Create(userId, request);
            return Ok(new
            {
                success = true,
                message = "Event created successfully",
                data = request
            });
        }

        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            var @event = await _eventService.GetEventById(id);
            if (@event == null)
            {
                return NotFound();
            }

            _eventService.Remove(@event);

            return Ok(new
            {
                success = true,
                message = "Event deleted successfully"
            });
        }

        private bool EventExists(Guid id)
        {
            return _eventService.GetEventByIdAsync(id) != null;
        }
    }
}
