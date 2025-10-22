using Eventlink_Services.Interface;
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
        //private readonly IUserProfileService _userProfileService;
        private readonly OpenAIService _openAIService;

        //public PartnershipsController(IPartnershipService partnershipService, OpenAIService openAIService, IUserProfileService userProfileService)
        //{
        //    _partnershipService = partnershipService;
        //    _openAIService = openAIService;
        //    _userProfileService = userProfileService;
        //}

        public PartnershipsController(IPartnershipService partnershipService, OpenAIService openAIService)
        {
            _partnershipService = partnershipService;
            _openAIService = openAIService;
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
    }
}
