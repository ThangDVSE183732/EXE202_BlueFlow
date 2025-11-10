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
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EventLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
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


        [HttpGet("{id}/UserId")]
        [AllowAnonymous]
        public async Task<ActionResult<EventResponse>> GetEventByUserId(Guid id)
        {
            var @event = await _eventService.GetEventsByOrganizerIdAsync(id);
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
        /// POST: api/Events - Create event (FormData with file upload) - Organizer only
        /// ✅ Changed to accept FormData for CoverImage upload
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "OrganizerOnly")]
        public async Task<ActionResult> PostEvent([FromForm] CreateEventFormRequest formRequest)
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
                if (formRequest.EndDate < formRequest.EventDate)
                {
                    return BadRequest(new { success = false, message = "End date cannot be earlier than start date" });
                }

                if (formRequest.EventDate < DateTime.UtcNow)
                {
                    return BadRequest(new { success = false, message = "Event date cannot be in the past" });
                }

                // ✅ Create returns EventResponse with Id
                var createdEvent = await _eventService.CreateWithFormData(userId.Value, formRequest);

                return Ok(new
                {
                    success = true,
                    message = "Event created successfully",
                    data = createdEvent
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Error creating event: {ex.Message}" });
            }
        }

        /// <summary>
        /// PUT: api/Events/{id} - Update event (FormData with file upload) - Organizer only
        /// ✅ Changed to accept FormData for CoverImage upload
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvent(Guid id, [FromForm] UpdateEventFormRequest formRequest)
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

                await _eventService.UpdateWithFormData(id, formRequest);

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

        /// <summary>
        /// PATCH: api/Events/{id}/visibility - Toggle event visibility (isPublic) - Organizer only
        /// Automatically switches between Public and Private
        /// </summary>
        [HttpPatch("{id}/visibility")]
        public async Task<IActionResult> UpdateEventVisibility(Guid id)
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
                    return Forbid("Only event organizer can update event visibility");
                }

                var existingEvent = await _eventService.GetEventById(id);
                if (existingEvent == null)
                {
                    return NotFound(new { success = false, message = "Event not found" });
                }

                // ✅ Toggle: Nếu Public → Private, nếu Private → Public
                var newVisibility = !(existingEvent.IsPublic ?? true);
                
                await _eventService.UpdateEventVisibilityAsync(id, newVisibility);

                return Ok(new
                {
                    success = true,
                    message = $"Event visibility toggled to {(newVisibility ? "Public" : "Private")}",
                    data = new
                    {
                        eventId = id,
                        isPublic = newVisibility,
                        previousState = existingEvent.IsPublic ?? true
                    }
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Event not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Error updating event visibility: {ex.Message}" });
            }
        }



        
        /// <summary>
        /// PATCH: api/Events/{id}/status - Toggle both visibility AND featured status - Organizer only
        /// Automatically toggles BOTH IsPublic and IsFeatured at the same time
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateEventStatus(Guid id)
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
                    return Forbid("Only event organizer can update event status");
                }

                var existingEvent = await _eventService.GetEventById(id);
                if (existingEvent == null)
                {
                    return NotFound(new { success = false, message = "Event not found" });
                }

                // ✅ Toggle BOTH properties
                var newVisibility = !(existingEvent.IsPublic ?? true);
                var newFeaturedStatus = !(existingEvent.IsFeatured ?? false);

                // Update both
                await _eventService.UpdateEventVisibilityAsync(id, newVisibility);
                await _eventService.UpdateEventFeaturedAsync(id, newFeaturedStatus);

                return Ok(new
                {
                    success = true,
                    message = "Event status toggled successfully",
                    data = new
                    {
                        eventId = id,
                        isPublic = newVisibility,
                        isFeatured = newFeaturedStatus,
                        changes = new
                        {
                            previousVisibility = existingEvent.IsPublic ?? true,
                            previousFeatured = existingEvent.IsFeatured ?? false
                        }
                    }
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Event not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Error updating event status: {ex.Message}" });
            }
        }

        #endregion

        #region Helper Methods

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return Guid.TryParse(userIdClaim, out Guid userId) ? userId : null;
        }

        #endregion
    }
}