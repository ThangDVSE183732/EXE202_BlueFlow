using EventLink_Repositories.Models;
using Eventlink_Services.Interface;
using Eventlink_Services.Response;
using Eventlink_Services.Service;
using MailKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Eventlink_Services.Request.PartnershipRequest;

namespace EventLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PartnershipsController : ControllerBase
    {
        private readonly IPartnershipService _partnershipService;
        private readonly IUserProfileService _userProfileService;
        private readonly OpenAIService _openAIService;

        public PartnershipsController(IPartnershipService partnershipService, OpenAIService openAIService, IUserProfileService userProfileService)
        {
            _partnershipService = partnershipService;
            _openAIService = openAIService;
            _userProfileService = userProfileService;
        }

        /// <summary>
        /// GET: api/Partnerships
        /// Get all partnerships in the system with event and partner details
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PartnershipResponse>>> GetAllPartnerships()
        {
            var partnerships = await _partnershipService.GetAllPartnershipsAsync();

            return Ok(new
            {
                success = true,
                message = "All partnerships retrieved successfully",
                data = partnerships,
                count = partnerships.Count()
            });
        }

        /// <summary>
        /// POST: api/Partnerships
        /// Create partnership with file upload support (FormData)
        /// </summary>
        [HttpPost]
        //[Authorize(Roles = "Organizer")]
        public async Task<IActionResult> CreatePartnership([FromForm] CreatePartnershipFormRequest request)
        {
            var userId = Guid.Parse(User.FindFirst("UserId").Value);

            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _partnershipService.CreateAsync(userId, request);

            return Ok(new
            {
                success = true,
                message = "Partnership request sent successfully.",
                data = result
            });
        }

        /// <summary>
        /// PUT: api/Partnerships/{eventId}/toggle-status
        /// Toggle partnership status between Ongoing and Pending by EventId
        /// </summary>
        [HttpPut("{eventId}/toggle-status")]
        public async Task<IActionResult> TogglePartnershipStatus(Guid eventId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("UserId").Value);

                if (userId == Guid.Empty)
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                var result = await _partnershipService.TogglePartnershipStatusByEventAsync(eventId);

                return Ok(new
                {
                    success = true,
                    message = $"Partnership status toggled to {result.Status}",
                    data = new
                    {
                        partnershipId = result.Id,
                        eventId = result.EventId,
                        status = result.Status,
                        previousStatus = result.Status == PartnershipStatus.Ongoing 
                            ? PartnershipStatus.Pending 
                            : PartnershipStatus.Ongoing
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new 
                { 
                    success = false, 
                    message = $"Error toggling partnership status: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// PUT: api/Partnerships/partner/{partnerId}/toggle-status
        /// Toggle partnership status between Ongoing and Pending by PartnerId
        /// ✅ NEW: Toggle status for a specific partner's partnership
        /// </summary>
        [HttpPut("partner/{partnerId}/toggle-status")]
        public async Task<IActionResult> TogglePartnershipStatusByPartner(Guid partnerId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("UserId").Value);

                if (userId == Guid.Empty)
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                var result = await _partnershipService.TogglePartnershipStatusByPartnerAsync(partnerId);

                return Ok(new
                {
                    success = true,
                    message = $"Partnership status toggled to {result.Status}",
                    data = new
                    {
                        partnershipId = result.Id,
                        partnerId = result.PartnerId,
                        eventId = result.EventId,
                        status = result.Status,
                        previousStatus = result.Status == PartnershipStatus.Ongoing 
                            ? PartnershipStatus.Pending 
                            : PartnershipStatus.Ongoing
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new 
                { 
                    success = false, 
                    message = $"Error toggling partnership status: {ex.Message}" 
                });
            }
        }

        [HttpPut("{id}/status")]
        //[Authorize(Roles = "Partner")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdatePartnershipStatusRequest request)
        {
            var result = await _partnershipService.UpdateStatusAsync(id, request);

            return Ok(new
            {
                success = true,
                message = $"Partnership {request.Status.ToLower()} successfully.",
                data = result
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePartnership(Guid id, [FromBody] UpdatePartnershipRequest request)
        {
            await _partnershipService.UpdateAsync(id, request);

            return Ok(new
            {
                success = true,
                message = "Partnership updated successfully.",
            });
        }

        /// <summary>
        /// GET: api/Partnerships/{eventId}/partners
        /// Get all partnerships with event and partner details for a specific event
        /// </summary>
        [HttpGet("{eventId}/partners")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PartnershipResponse>>> GetPartnershipsByEventAsync(Guid eventId)
        {
            var partnerships = await _partnershipService.GetPartnershipsByEventAsync(eventId);

            return Ok(new
            {
                success = true,
                message = "Partnerships retrieved successfully",
                data = partnerships,
                count = partnerships.Count()
            });
        }

        

        
        
    }
}
