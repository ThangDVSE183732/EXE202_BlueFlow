using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Eventlink_Services.Interface;
using Eventlink_Services.Request;
using Eventlink_Services.Response;
using static Eventlink_Services.Request.EventRequest;

namespace EventLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        // ❌ REMOVED: IEventActivityService _eventActivityService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
            // ❌ REMOVED: _eventActivityService = eventActivityService;
        }

        #region Basic CRUD

        /// <summary>
        /// GET: api/Events - Get all events (Public)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
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

        /// <summary>
        /// GET: api/Events/{id} - Get single event (Public)
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<EventResponse>> GetEvent(Guid id)
        {
            var @event = await _eventService.GetEventByIdAsync(id);
            if (@event == null)
            {
                return NotFound(new { success = false, message = "Event not found" });
            }

            return Ok(new { success = true, message = "Event retrieved successfully", data = @event });
        }

        /// <summary>
        /// GET: api/Events/{id}/detail - Get complete event with timeline & proposals (Public)
        /// ✅ This returns EventDetailDto with timeline included!
        /// </summary>
        [HttpGet("{id}/detail")]
        [AllowAnonymous]
        public async Task<ActionResult<EventDetailDto>> GetEventDetail(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var eventDetail = await _eventService.GetEventDetailAsync(id, userId);

                return Ok(new
                {
                    success = true,
                    message = "Event detail retrieved successfully",
                    data = eventDetail
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Event not found" });
            }
        }

        /// <summary>
        /// POST: api/Events - Create event (JSON body) - Organizer only
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "OrganizerOnly")]
        public async Task<ActionResult> PostEvent([FromBody] CreateEventRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Validation failed",
                        errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                // Validate dates
                if (request.EndDate < request.EventDate)
                {
                    return BadRequest(new { success = false, message = "End date cannot be earlier than start date" });
                }

                if (request.EventDate < DateTime.UtcNow)
                {
                    return BadRequest(new { success = false, message = "Event date cannot be in the past" });
                }

                await _eventService.Create(userId.Value, request);

                return Ok(new
                {
                    success = true,
                    message = "Event created successfully",
                    data = request
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Error creating event: {ex.Message}" });
            }
        }

        /// <summary>
        /// PUT: api/Events/{id} - Update event (JSON body) - Organizer only
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvent(Guid id, [FromBody] UpdateEventRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                var canEdit = await _eventService.CanUserEditEventAsync(id, userId.Value);
                if (!canEdit)
                {
                    return Forbid("Only event organizer can edit this event");
                }

                var existingEvent = await _eventService.GetEventById(id);
                if (existingEvent == null)
                {
                    return NotFound(new { success = false, message = "Event not found" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Validation failed",
                        errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

                await _eventService.Update(id, request);

                return Ok(new { success = true, message = "Event updated successfully" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Event not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Error updating event: {ex.Message}" });
            }
        }

        /// <summary>
        /// DELETE: api/Events/{id} - Delete event - Organizer only
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                var canEdit = await _eventService.CanUserEditEventAsync(id, userId.Value);
                if (!canEdit)
                {
                    return Forbid("Only event organizer can delete this event");
                }

                var existingEvent = await _eventService.GetEventById(id);
                if (existingEvent == null)
                {
                    return NotFound(new { success = false, message = "Event not found" });
                }

                _eventService.Remove(existingEvent);

                return Ok(new { success = true, message = "Event deleted successfully" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Event not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Error deleting event: {ex.Message}" });
            }
        }

        #endregion

        #region Timeline Management
        // ⚠️ TEMPORARILY COMMENTED OUT - Implement EventActivityService later
        /*
        /// <summary>
        /// POST: api/Events/{id}/timeline/initialize - Create initial activities (JSON body)
        /// Separate API to initialize timeline after event creation
        /// </summary>
        [HttpPost("{id}/timeline/initialize")]
        [Authorize(Policy = "OrganizerOnly")]
        public async Task<ActionResult> InitializeTimeline(Guid id, [FromBody] InitializeTimelineRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                var canEdit = await _eventService.CanUserEditEventAsync(id, userId.Value);
                if (!canEdit)
                {
                    return Forbid("Only event organizer can edit timeline");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Validation failed",
                        errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

                var activities = await _eventActivityService.BulkUpdateActivitiesAsync(
                    id, userId.Value, request.Activities);

                return Ok(new
                {
                    success = true,
                    message = $"{activities.Count} activities initialized successfully",
                    data = activities
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Error initializing timeline: {ex.Message}" });
            }
        }

        /// <summary>
        /// PUT: api/Events/{id}/timeline - Replace entire timeline (JSON body)
        /// </summary>
        [HttpPut("{id}/timeline")]
        [Authorize(Policy = "OrganizerOnly")]
        public async Task<ActionResult> ReplaceTimeline(Guid id, [FromBody] ReplaceTimelineRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                var canEdit = await _eventService.CanUserEditEventAsync(id, userId.Value);
                if (!canEdit)
                {
                    return Forbid("Only event organizer can edit timeline");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Validation failed",
                        errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

                var activities = await _eventActivityService.BulkUpdateActivitiesAsync(
                    id, userId.Value, request.Activities);

                return Ok(new
                {
                    success = true,
                    message = "Timeline replaced successfully",
                    data = activities
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Error replacing timeline: {ex.Message}" });
            }
        }
        */
        #endregion

        #region Helper Methods

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return Guid.TryParse(userIdClaim, out Guid userId) ? userId : null;
        }

        #endregion
    }

    /// <summary>
    /// Request DTOs for timeline management (for future use)
    /// </summary>
    public class InitializeTimelineRequest
    {
        public List<EventActivityRequest> Activities { get; set; }
    }

    public class ReplaceTimelineRequest
    {
        public List<EventActivityRequest> Activities { get; set; }
    }
}