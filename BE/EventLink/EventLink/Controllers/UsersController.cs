using EventLink_Repositories.DBContext;
using EventLink_Repositories.Models;
using Eventlink_Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Eventlink_Services.Request.FindPartnerRequest;
using static Eventlink_Services.Request.PartnershipRequest;

namespace EventLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public UsersController(IUserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        [HttpGet("suppliers")]
        public async Task<ActionResult<IEnumerable<User>>> FindSupplier(string? category, string? location)
        {
            var users = await _userService.GetSuppliersAsync(category, location);
            return Ok(new
            {
                success = true,
                message = "Suppliers retrieved successfully",
                data = users,
                count = users.Count
            });
        }

        [HttpGet("organizers")]
        public async Task<ActionResult<IEnumerable<User>>> FindOrganizer(decimal? budget, string? targetAudience, string? packageType)
        {
            var users = await _userService.GetOrganizersAsync(budget, targetAudience, packageType);
            return Ok(new
            {
                success = true,
                message = "Organizers retrieved successfully",
                data = users,
                count = users.Count
            });
        }

        //[HttpPost("request")]
        //public async Task RequestPartnership([FromBody] PartnershipSuggestionRequest request)
        //{
        //    var subject = "Partnership Request";
            
        //    var targetUser = await _userService.GetUserByEmailAsync(request.Email);
        //    if (targetUser == null)
        //    {
        //        throw new Exception("User with the provided email does not exist.");
        //    }
        //    await _emailService.SendEmailAsync(targetUser.Email, subject, request.Message);
        //}
    }
}
