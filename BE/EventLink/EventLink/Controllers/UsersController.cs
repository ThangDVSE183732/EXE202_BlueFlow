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

namespace EventLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
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
    }
}
