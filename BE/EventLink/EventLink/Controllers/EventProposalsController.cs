using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Eventlink_Services.Interface;
using Eventlink_Services.Request;
using Eventlink_Services.Response;
using static Eventlink_Services.Request.EventProposalRequest;

namespace EventLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventProposalsController : ControllerBase
    {
        private readonly IEventProposalService _eventProposalService;

        public EventProposalsController(IEventProposalService eventProposalService)
        {
            _eventProposalService = eventProposalService;
        }

        #region Existing Endpoints

        /// <summary>
        /// GET: api/EventProposals
        /// Get all proposals (Admin only)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventProposalResponse>>> GetEventProposals()
        {
            var eventProposals = await _eventProposalService.GetAllEventProposalsAsync();

            return Ok(new
            {
                success = true,
                message = "Event proposals retrieved successfully",
                data = eventProposals,
                count = eventProposals.Count
            });
        }

        /// <summary>
        /// GET: api/EventProposals/5
        /// Get single proposal (basic info)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<EventProposalResponse>> GetEventProposal(Guid id)
        {
            var eventProposal = await _eventProposalService.GetEventProposalByIdAsync(id);

            if (eventProposal == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Event proposal not found"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Event proposal retrieved successfully",
                data = eventProposal
            });
        }

        /// <summary>
        /// POST: api/EventProposals
        /// Create basic proposal (Legacy endpoint)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<EventProposalResponse>> PostEventProposal(
            CreateEventProposalRequest eventProposal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid data",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            var createdProposal = await _eventProposalService.CreateEventProposalAsync(eventProposal);

            return Ok(new
            {
                success = true,
                message = "Event proposal created successfully",
                data = createdProposal
            });
        }

        /// <summary>
        /// PUT: api/EventProposals/5
        /// Update basic proposal (Legacy endpoint)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEventProposal(Guid id, UpdateEventProposalRequest eventProposal)
        {
            var existingProposal = await _eventProposalService.GetEventProposalByIdAsync(id);

            if (existingProposal == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Event proposal not found"
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid data",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            var updatedProposal = await _eventProposalService.UpdateEventProposalAsync(id, eventProposal);

            return Ok(new
            {
                success = true,
                message = "Event proposal updated successfully",
                data = updatedProposal
            });
        }

        /// <summary>
        /// DELETE: api/EventProposals/5
        /// Delete proposal
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEventProposal(Guid id)
        {
            var existingProposal = await _eventProposalService.GetEventProposalByIdAsync(id);

            if (existingProposal == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Event proposal not found"
                });
            }

            var result = await _eventProposalService.DeleteEventProposalAsync(id);

            return Ok(new
            {
                success = result,
                message = result ? "Event proposal deleted successfully" : "Failed to delete event proposal"
            });
        }

        #endregion

        #region New Enhanced Endpoints

        /// <summary>
        /// GET: api/EventProposals/5/detail
        /// Get proposal with full details including user info
        /// </summary>
        [HttpGet("{id}/detail")]
        public async Task<ActionResult<EventProposalDetailDto>> GetProposalDetail(Guid id)
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

                var proposal = await _eventProposalService.GetProposalDetailAsync(id);

                // Check permission: only organizer or proposer can see details
                var canView = proposal.ProposedBy == userId.Value ||
                             await _eventProposalService.CanUserApproveProposalAsync(id, userId.Value);

                if (!canView)
                {
                    return Forbid("You don't have permission to view this proposal");
                }

                return Ok(new
                {
                    success = true,
                    message = "Proposal detail retrieved successfully",
                    data = proposal
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Proposal not found"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Error retrieving proposal: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// GET: api/EventProposals/my-proposals
        /// Get all proposals created by current user
        /// </summary>
        [HttpGet("my-proposals")]
        public async Task<ActionResult<IEnumerable<EventProposalDetailDto>>> GetMyProposals()
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

                var proposals = await _eventProposalService.GetProposalsByProposerIdAsync(userId.Value);

                return Ok(new
                {
                    success = true,
                    message = "Your proposals retrieved successfully",
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

        /// <summary>
        /// POST: api/EventProposals/sponsorship
        /// Create sponsorship proposal (Sponsor only)
        /// </summary>
        [HttpPost("sponsorship")]
        [Authorize(Policy = "SponsorOnly")]
        public async Task<ActionResult<EventProposalDetailDto>> CreateSponsorshipProposal(
            [FromBody] SponsorshipProposalRequest request)
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

                var proposal = await _eventProposalService.CreateSponsorshipProposalAsync(userId.Value, request);

                return CreatedAtAction(
                    nameof(GetProposalDetail),
                    new { id = proposal.Id },
                    new
                    {
                        success = true,
                        message = "Sponsorship proposal created successfully",
                        data = proposal
                    });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
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
                    message = $"Error creating proposal: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// PUT: api/EventProposals/5/sponsorship
        /// Update sponsorship proposal (Sponsor only, before approval)
        /// </summary>
        [HttpPut("{id}/sponsorship")]
        public async Task<ActionResult<EventProposalDetailDto>> UpdateSponsorshipProposal(
            Guid id,
            [FromBody] SponsorshipProposalRequest request)
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

                // Check permission
                var canEdit = await _eventProposalService.CanUserEditProposalAsync(id, userId.Value);
                if (!canEdit)
                {
                    return Forbid("You can only edit your own pending proposals");
                }

                var proposal = await _eventProposalService.UpdateSponsorshipProposalAsync(id, userId.Value, request);

                return Ok(new
                {
                    success = true,
                    message = "Sponsorship proposal updated successfully",
                    data = proposal
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Proposal not found"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Error updating proposal: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// PUT: api/EventProposals/5/status
        /// Approve or reject proposal (Organizer only)
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<ActionResult<EventProposalDetailDto>> UpdateProposalStatus(
            Guid id,
            [FromBody] UpdateProposalStatusRequest request)
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

                // Check permission
                var canApprove = await _eventProposalService.CanUserApproveProposalAsync(id, userId.Value);
                if (!canApprove)
                {
                    return Forbid("Only event organizer can approve/reject proposals");
                }

                var proposal = await _eventProposalService.UpdateProposalStatusAsync(id, userId.Value, request);

                return Ok(new
                {
                    success = true,
                    message = $"Proposal {request.Status.ToLower()} successfully",
                    data = proposal
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Proposal not found"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Error updating proposal status: {ex.Message}"
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