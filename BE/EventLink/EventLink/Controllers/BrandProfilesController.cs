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
            var brandProfiles = await _brandProfileService.GetAllAsync();
            
            return Ok(new
            {
                success = true,
                message = "Brand profiles retrieved successfully",
                data = brandProfiles,
                count = brandProfiles.Count
            });
        }

        // GET: api/BrandProfiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BrandProfileResponse>> GetBrandProfile(Guid id)
        {
            var brandProfile = await _brandProfileService.GetByIdAsync(id);

            if (brandProfile == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Brand profile not found"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Brand profile retrieved successfully",
                data = brandProfile
            });
        }

        [HttpGet("brand_profile_by_userid/{userId}")]
        public async Task<ActionResult<BrandProfileResponse>> GetBrandProfileByUserId(Guid userId)
        {
            var brandProfile = await _brandProfileService.GetByUserIdAsync(userId);
            
            if (brandProfile == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Brand profile not found for this user"
                });
            }
            
            return Ok(new
            {
                success = true,
                message = "Brand profile retrieved successfully",
                data = brandProfile
            });
        }

        // PUT: api/BrandProfiles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBrandProfile(Guid id, UpdateBrandProfileRequest request)
        {
            // Service UpdateAsync giờ sẽ trả về true (nếu thành công) hoặc false (nếu not found)
            var success = await _brandProfileService.UpdateAsync(id, request);

            if (!success)
            {
                // Nếu service trả về false, có nghĩa là không tìm thấy BrandProfile
                return NotFound(new
                {
                    success = false,
                    message = "Brand profile not found with the specified ID."
                });
            }

            // Nếu thành công, trả về 204 NoContent
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

        /// <summary>
        /// PATCH: api/BrandProfiles/{id}/toggle-status
        /// Toggle BrandProfile visibility status (IsPublic) by BrandProfile ID
        /// </summary>
        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleBrandProfileStatus(Guid id)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;

                if (userIdClaim == null || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new 
                    { 
                        success = false, 
                        message = "User not authenticated" 
                    });
                }

                // ✅ Check if brand profile exists first
                var existingProfile = await _brandProfileService.GetByIdAsync(id);
                if (existingProfile == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Brand profile not found"
                    });
                }

                // ✅ Toggle status by brand profile ID
                var result = await _brandProfileService.ToggleBrandProfileStatusByIdAsync(id);

                return Ok(new
                {
                    success = true,
                    message = $"Brand profile visibility toggled to {(result.IsPublic == true ? "Public" : "Private")}",
                    data = new
                    {
                        brandProfileId = id,
                        isPublic = result.IsPublic,
                        previousStatus = !(result.IsPublic ?? false)
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new 
                { 
                    success = false, 
                    message = $"Error toggling brand profile status: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// PATCH: api/BrandProfiles/{id}/toggle-all-status
        /// Toggle both IsPublic and HasPartnership status by BrandProfile ID
        /// ✅ NEW: Toggle both visibility and partnership status at once
        /// </summary>
        [HttpPatch("{id}/toggle-all-status")]
        public async Task<IActionResult> ToggleBrandProfileAllStatus(Guid id)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;

                if (userIdClaim == null || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new 
                    { 
                        success = false, 
                        message = "User not authenticated" 
                    });
                }

                // ✅ Check if brand profile exists first
                var existingProfile = await _brandProfileService.GetByIdAsync(id);
                if (existingProfile == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Brand profile not found"
                    });
                }

                // ✅ Toggle both statuses by brand profile ID
                var result = await _brandProfileService.ToggleBrandProfileAllStatusAsync(id);

                return Ok(new
                {
                    success = true,
                    message = "Brand profile status toggled successfully",
                    data = new
                    {
                        brandProfileId = id,
                        isPublic = result.IsPublic,
                        hasPartnership = result.HasPartnership,
                        previousIsPublic = !(result.IsPublic ?? false),
                        previousHasPartnership = !(result.HasPartnership ?? false)
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new 
                { 
                    success = false, 
                    message = $"Error toggling brand profile status: {ex.Message}" 
                });
            }
        }

        private bool BrandProfileExists(Guid id)
        {
            return _brandProfileService.GetByIdAsync(id) != null;
        }
    }
}
