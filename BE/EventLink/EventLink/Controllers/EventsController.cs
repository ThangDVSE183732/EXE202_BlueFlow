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
        private readonly IEventProposalService _eventProposalService;

        public EventsController(
            IEventService eventService,
            IEventProposalService eventProposalService)
        {
            _eventService = eventService;
            _eventProposalService = eventProposalService;
        }

        #region Existing Endpoints

        /// <summary>
        /// GET: api/Events
        /// Get all events (public)
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
        /// GET: api/Events/5
        /// Get single event (basic info)
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<EventResponse>> GetEvent(Guid id)
        {
            var @event = await _eventService.GetEventByIdAsync(id);

            if (@event == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Event not found"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Event retrieved successfully",
                data = @event
            });
        }

        /// <summary>
        /// POST: api/Events
        /// Create event (Organizer only)
        /// </summary>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Policy = "OrganizerOnly")]
        public async Task<ActionResult<EventResponse>> PostEvent([FromForm] CreateEventRequest request)
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
                    return Unauthorized(new
                    {
                        success = false,
                        message = "User not authenticated"
                    });
                }

                // Validate dates
                if (request.EndDate < request.EventDate)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "End date cannot be earlier than start date"
                    });
                }

                if (request.EventDate < DateTime.UtcNow)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Event date cannot be in the past"
                    });
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
                return BadRequest(new
                {
                    success = false,
                    message = $"Error creating event: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// PUT: api/Events/5
        /// Update event (Organizer only)
        /// </summary>
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PutEvent(Guid id, [FromForm] UpdateEventRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "User not authenticated"
                    });
                }

                // Check permission
                var canEdit = await _eventService.CanUserEditEventAsync(id, userId.Value);
                if (!canEdit)
                {
                    return Forbid("Only event organizer can edit this event");
                }

                var existingEvent = await _eventService.GetEventById(id);
                if (existingEvent == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Event not found"
                    });
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

                return Ok(new
                {
                    success = true,
                    message = "Event updated successfully"
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Event not found"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Error updating event: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// PUT: api/Events/5/status
        /// Update event status (Organizer only)
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "User not authenticated"
                    });
                }

                var canEdit = await _eventService.CanUserEditEventAsync(id, userId.Value);
                if (!canEdit)
                {
                    return Forbid("Only event organizer can update status");
                }

                var existingEvent = await _eventService.GetEventById(id);
                if (existingEvent == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Event not found"
                    });
                }

                await _eventService.UpdateStatus(id, status);

                return Ok(new
                {
                    success = true,
                    message = "Event status updated successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Error updating status: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// DELETE: api/Events/5
        /// Delete event (Organizer only)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "User not authenticated"
                    });
                }

                var canEdit = await _eventService.CanUserEditEventAsync(id, userId.Value);
                if (!canEdit)
                {
                    return Forbid("Only event organizer can delete this event");
                }

                var @event = await _eventService.GetEventById(id);
                if (@event == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Event not found"
                    });
                }

                _eventService.Remove(@event);

                return Ok(new
                {
                    success = true,
                    message = "Event deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Error deleting event: {ex.Message}"
                });
            }
        }

        #endregion

        #region New Enhanced Endpoints

        /// <summary>
        /// GET: api/Events/5/detail
        /// Get complete event details with timeline and proposals
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
                return NotFound(new
                {
                    success = false,
                    message = "Event not found"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Error retrieving event detail: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// POST: api/Events/with-timeline
        /// Create event with initial timeline activities (Organizer only)
        /// </summary>
        [HttpPost("with-timeline")]
        [Consumes("multipart/form-data")]
        [Authorize(Policy = "OrganizerOnly")]
        public async Task<ActionResult<EventDetailDto>> CreateEventWithTimeline(
            [FromForm] CreateEventWithDetailsRequest request)
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
                    return Unauthorized(new
                    {
                        success = false,
                        message = "User not authenticated"
                    });
                }

                // Validate dates
                if (request.EndDate < request.EventDate)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "End date cannot be earlier than start date"
                    });
                }

                var eventDetail = await _eventService.CreateEventWithDetailsAsync(userId.Value, request);

                return CreatedAtAction(
                    nameof(GetEventDetail),
                    new { id = eventDetail.Id },
                    new
                    {
                        success = true,
                        message = "Event with timeline created successfully",
                        data = eventDetail
                    });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Error creating event: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// PUT: api/Events/5/with-timeline
        /// Update event and replace timeline activities (Organizer only)
        /// </summary>
        [HttpPut("{id}/with-timeline")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<EventDetailDto>> UpdateEventWithTimeline(
            Guid id,
            [FromForm] UpdateEventWithDetailsRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "User not authenticated"
                    });
                }

                var canEdit = await _eventService.CanUserEditEventAsync(id, userId.Value);
                if (!canEdit)
                {
                    return Forbid("Only event organizer can edit this event");
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

                var eventDetail = await _eventService.UpdateEventWithDetailsAsync(id, userId.Value, request);

                return Ok(new
                {
                    success = true,
                    message = "Event with timeline updated successfully",
                    data = eventDetail
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Event not found"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Error updating event: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// GET: api/Events/5/proposals
        /// Get all proposals for an event
        /// Organizer sees all, others see only own proposals
        /// </summary>
        [HttpGet("{id}/proposals")]
        public async Task<ActionResult<IEnumerable<EventProposalDetailDto>>> GetEventProposals(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "User not authenticated"
                    });
                }

                var proposals = await _eventProposalService.GetProposalsByEventIdAsync(id);

                // Check if user is organizer
                var isOrganizer = await _eventService.CanUserEditEventAsync(id, userId.Value);

                // Filter proposals if not organizer
                if (!isOrganizer)
                {
                    proposals = proposals.Where(p => p.ProposedBy == userId.Value).ToList();
                }

                return Ok(new
                {
                    success = true,
                    message = "Proposals retrieved successfully",
                    data = proposals,
                    count = proposals.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Error retrieving proposals: {ex.Message}"
                });
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