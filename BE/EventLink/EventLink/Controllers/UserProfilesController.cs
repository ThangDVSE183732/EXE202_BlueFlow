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
    public class UserProfilesController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfilesController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        // GET: api/UserProfiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserProfile>>> GetUserProfiles()
        {
            return await _userProfileService.GetAllUserProfilesAsync();
        }

        // GET: api/UserProfiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserProfile>> GetUserProfile(Guid id)
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
        public async Task<IActionResult> PutUserProfile(Guid id, UserProfile userProfile)
        {
            var existingProfile = await _userProfileService.GetByUserIdAsync(id);
            if (existingProfile == null)
            {
                return NotFound();
            }
            return NoContent();
        }

        // POST: api/UserProfiles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserProfile>> PostUserProfile(UserProfile userProfile)
        {
            await _userProfileService.CreateAsync(userProfile);

            return CreatedAtAction("GetUserProfile", new { id = userProfile.Id }, userProfile);
        }

        // DELETE: api/UserProfiles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserProfile(Guid id)
        {
            var userProfile = await _userProfileService.GetByUserIdAsync(id);
            if (userProfile == null)
            {
                return NotFound();
            }

            _userProfileService.Remove(userProfile);

            return NoContent();
        }

        private bool UserProfileExists(Guid id)
        {
            return _userProfileService.UserProfileExistsAsync(id) != null;
        }
    }
}
