using Eventlink_Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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

        public PartnershipsController(IPartnershipService partnershipService)
        {
            _partnershipService = partnershipService;
        }

        [HttpPost]
        //[Authorize(Roles = "Organizer")]
        public async Task<IActionResult> CreatePartnership([FromBody] CreatePartnershipRequest request)
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

        [HttpPut("{id}/status")]
        //[Authorize(Roles = "Partner")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdatePartnershipStatusRequest request)
        {
            var result = await _partnershipService.UpdateStatusAsync(id, request.Status, request.OrganizerResponse);

            return Ok(new
            {
                success = true,
                message = $"Partnership {request.Status.ToLower()} successfully.",
                data = result
            });
        }
    }
}
