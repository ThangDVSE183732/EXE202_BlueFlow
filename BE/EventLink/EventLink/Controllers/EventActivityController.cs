using Eventlink_Services.Interface;
using Eventlink_Services.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventLink.Controllers
{
    /// <summary>
    /// Controller for individual activity CRUD operations
    /// Timeline bulk operations are in EventsController
    /// </summary>
    [Route("api/events/{eventId}/activities")]
    [ApiController]
    [Authorize(Policy = "OrganizerOnly")]
    public class EventActivityController : ControllerBase
    {
        private readonly IEventActivityService _activityService;
        private readonly IEventService _eventService;

        public EventActivityController(
            IEventActivityService activityService,
            IEventService eventService)
        {
            _activityService = activityService;
            _eventService = eventService;
        }

        /// <summary>
        /// GET: api/events/{eventId}/activities - Get all activities for event
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetActivities(Guid eventId)
        {
            try
            {
                var activities = await _activityService.GetActivitiesByEventIdAsync(eventId);
                return Ok(new
                {
                    success = true,
                    message = "Activities retrieved successfully",
                    data = activities,
                    count = activities.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Error retrieving activities: {ex.Message}" });
            }
        }

        /// <summary>
        /// GET: api/events/{eventId}/activities/{id} - Get single activity
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetActivity(Guid eventId, Guid id)
        {
            try
            {
                var activity = await _activityService.GetActivityByIdAsync(id);
                if (activity == null)
                {
                    return NotFound(new { success = false, message = "Activity not found" });
                }

                if (activity.EventId != eventId)
                {
                    return BadRequest(new { success = false, message = "Activity does not belong to this event" });
                }

                return Ok(new { success = true, message = "Activity retrieved successfully", data = activity });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Error retrieving activity: {ex.Message}" });
            }
        }

        /// <summary>
        /// POST: api/events/{eventId}/activities - Create new activity (JSON body)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> CreateActivity(Guid eventId, [FromBody] EventActivityRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                var canEdit = await _eventService.CanUserEditEventAsync(eventId, userId.Value);
                if (!canEdit)
                {
                    return Forbid("Only event organizer can create activities");
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

                var activity = await _activityService.CreateActivityAsync(eventId, userId.Value, request);

                return CreatedAtAction(
                    nameof(GetActivity),
                    new { eventId = eventId, id = activity.Id },
                    new
                    {
                        success = true,
                        message = "Activity created successfully",
                        data = activity
                    });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Error creating activity: {ex.Message}" });
            }
        }

        /// <summary>
        /// PUT: api/events/{eventId}/activities/{id} - Update activity (JSON body)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateActivity(Guid eventId, Guid id, [FromBody] EventActivityRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                var canEdit = await _eventService.CanUserEditEventAsync(eventId, userId.Value);
                if (!canEdit)
                {
                    return Forbid("Only event organizer can update activities");
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

                var activity = await _activityService.UpdateActivityAsync(id, userId.Value, request);

                if (activity.EventId != eventId)
                {
                    return BadRequest(new { success = false, message = "Activity does not belong to this event" });
                }

                return Ok(new
                {
                    success = true,
                    message = "Activity updated successfully",
                    data = activity
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Error updating activity: {ex.Message}" });
            }
        }

        /// <summary>
        /// DELETE: api/events/{eventId}/activities/{id} - Delete activity
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteActivity(Guid eventId, Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                var canEdit = await _eventService.CanUserEditEventAsync(eventId, userId.Value);
                if (!canEdit)
                {
                    return Forbid("Only event organizer can delete activities");
                }

                // Verify activity belongs to event
                var activity = await _activityService.GetActivityByIdAsync(id);
                if (activity == null)
                {
                    return NotFound(new { success = false, message = "Activity not found" });
                }

                if (activity.EventId != eventId)
                {
                    return BadRequest(new { success = false, message = "Activity does not belong to this event" });
                }

                await _activityService.DeleteActivityAsync(id, userId.Value);

                return Ok(new { success = true, message = "Activity deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Error deleting activity: {ex.Message}" });
            }
        }

        #region Helper Methods

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return Guid.TryParse(userIdClaim, out Guid userId) ? userId : null;
        }

        #endregion
    }
}