using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Eventlink_Services.Interface;
using Eventlink_Services.Response;
using static Eventlink_Services.Request.BrandProfileRequest;

namespace EventLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BrandProfilesController : ControllerBase
    {
        private readonly IBrandProfileService _brandProfileService;

        public BrandProfilesController(IBrandProfileService brandProfileService)
        {
            _brandProfileService = brandProfileService;
        }

        // GET: api/BrandProfiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandProfileResponse>>> GetBrandProfiles()
        {
            return await _brandProfileService.GetAllAsync();
        }

        // GET: api/BrandProfiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BrandProfileResponse>> GetBrandProfile(Guid id)
        {
            var brandProfile = await _brandProfileService.GetByIdAsync(id);

            if (brandProfile == null)
            {
                return NotFound();
            }

            return brandProfile;
        }

        [HttpGet("brand_profile_by_userid/{userId}")]
        public async Task<ActionResult<BrandProfileResponse>> GetBrandProfileByUserId(Guid userId)
        {
            var brandProfile = await _brandProfileService.GetByUserIdAsync(userId);
            if (brandProfile == null)
            {
                return NotFound();
            }
            return brandProfile;
        }

        // PUT: api/BrandProfiles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBrandProfile(Guid id, UpdateBrandProfileRequest request)
        {
            var existingProfile = await _brandProfileService.GetByUserIdAsync(id);
            if (existingProfile == null)
            {
                return NotFound();
            }

            await _brandProfileService.UpdateAsync(id, request);

            return NoContent();
        }

        // POST: api/BrandProfiles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostBrandProfile(CreateBrandProfileRequest request)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized();
            }

            await _brandProfileService.AddAsync(userId, request);

            return Ok(new
            {
                success = true,
                data = request
            });
        }

        // DELETE: api/BrandProfiles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrandProfile(Guid id)
        {
            var brandProfile = await _brandProfileService.GetByIdAsync(id);
            if (brandProfile == null)
            {
                return NotFound();
            }

            await _brandProfileService.DeleteAsync(id);

            return Ok(new
            {
                success = true,
                data = brandProfile
            });
        }

        private bool BrandProfileExists(Guid id)
        {
            return _brandProfileService.GetByIdAsync(id) != null;
        }
    }
}
