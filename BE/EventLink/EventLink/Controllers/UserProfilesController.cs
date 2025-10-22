using EventLink_Repositories.DBContext;
using EventLink_Repositories.Models;
using Eventlink_Services.Interface;
using Eventlink_Services.Request;
using Eventlink_Services.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserProfilesController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfilesController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        // GET: api/UserProfiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserProfileResponse>>> GetUserProfiles()
        {
            return await _userProfileService.GetAllUserProfilesAsync();
        }

        // GET: api/UserProfiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserProfileResponse>> GetUserProfile(Guid id)
        {
            var userProfile = await _userProfileService.GetByUserIdAsync(id);

            if (userProfile == null)
            {
                return NotFound();
            }

            return userProfile;
        }

        // PUT: api/UserProfiles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserProfile(Guid id, UpdateUserProfileRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _userProfileService.Update(id, request);
            return Ok(new
            {
                success = true,
                message = "User profile updated successfully"
            });
        }

        // POST: api/UserProfiles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserProfileResponse>> PostUserProfile(CreateUserProfileRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                return Unauthorized();

            await _userProfileService.CreateAsync(userId, request);

            return Ok(new
            {
                success = true,
                message = "Sponsor packages retrieved successfully",
                data = request
            });
        }

        // DELETE: api/UserProfiles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserProfile(Guid id)
        {
            var userProfile = await _userProfileService.GetByUserId(id);
            if (userProfile == null)
            {
                return NotFound();
            }
            await _userProfileService.Remove(userProfile);
            return Ok(new
            {
                success = true,
                message = "User profile deleted successfully",
                data = userProfile
            });
        }

        private bool UserProfileExists(Guid id)
        {
            return _userProfileService.UserProfileExistsAsync(id) != null;
        }
    }
}
