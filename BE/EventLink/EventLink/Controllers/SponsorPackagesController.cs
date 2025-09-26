using Microsoft.AspNetCore.Mvc;
using EventLink_Repositories.Models;
using Eventlink_Services.Interface;
using Eventlink_Services.Response;
using static Eventlink_Services.Request.SponsorPackageRequest;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace EventLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SponsorPackagesController : ControllerBase
    {
        private readonly ISponsorPackageService _sponsorPackageService;

        public SponsorPackagesController(ISponsorPackageService sponsorPackageService)
        {
            _sponsorPackageService = sponsorPackageService;
        }

        // GET: api/SponsorPackages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SponsorPackageResponse>>> GetSponsorPackages()
        {
            var packages = await _sponsorPackageService.GetAllSponsorPackagesAsync();
            var packagesResponse = packages.Select(sp => new SponsorPackageResponse
            {
                SponsorId = sp.SponsorId,
                PackageName = sp.PackageName,
                PackageType = sp.PackageType,
                Budget = sp.Budget,
                BudgetRange = sp.BudgetRange,
                SponsorshipBenefits = sp.SponsorshipBenefits,
                TargetAudience = sp.TargetAudience,
                PreferredEventTypes = sp.PreferredEventTypes,
                BrandGuidelines = sp.BrandGuidelines,
                LogoUrl = sp.LogoUrl,
                BrandAssets = sp.BrandAssets,
                IsActive = sp.IsActive,
                CreatedAt = sp.CreatedAt,
                UpdatedAt = sp.UpdatedAt
            }).ToList();

            return Ok(new
            {
                success = true,
                message = "Sponsor packages retrieved successfully",
                data = packagesResponse,
                count = packagesResponse.Count
            });
        }

        // GET: api/SponsorPackages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SponsorPackageResponse>> GetSponsorPackage(Guid id)
        {
            var sponsorPackage = await _sponsorPackageService.GetSponsorPackageByIdAsync(id);

            if (sponsorPackage == null)
            {
                return NotFound();
            }

            var response = new SponsorPackageResponse
            {
                SponsorId = sponsorPackage.Id,
                PackageName = sponsorPackage.PackageName,
                PackageType = sponsorPackage.PackageType,
                Budget = sponsorPackage.Budget,
                BudgetRange = sponsorPackage.BudgetRange,
                SponsorshipBenefits = sponsorPackage.SponsorshipBenefits,
                TargetAudience = sponsorPackage.TargetAudience,
                PreferredEventTypes = sponsorPackage.PreferredEventTypes,
                BrandGuidelines = sponsorPackage.BrandGuidelines,
                LogoUrl = sponsorPackage.LogoUrl,
                BrandAssets = sponsorPackage.BrandAssets,
                IsActive = sponsorPackage.IsActive,
                CreatedAt = sponsorPackage.CreatedAt,
                UpdatedAt = sponsorPackage.UpdatedAt
            };

            return Ok(new
            {
                success = true,
                message = "Sponsor package retrieved successfully",
                data = response,
                count = 1
            });
        }

        [HttpGet("sponsor/{sponsorId}")]
        public async Task<ActionResult> GetSponsorPackageBySponsorId(Guid sponsorId)
        {
            var sponsorPackages = await _sponsorPackageService.GetSponsorPackageBySponsorIdAsync(sponsorId);

            if (sponsorPackages == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Not Found"
                });
            }

            var response = sponsorPackages.Select(sp => new SponsorPackageResponse
            {
                SponsorId = sp.SponsorId,
                PackageName = sp.PackageName,
                PackageType = sp.PackageType,
                Budget = sp.Budget,
                BudgetRange = sp.BudgetRange,
                SponsorshipBenefits = sp.SponsorshipBenefits,
                TargetAudience = sp.TargetAudience,
                PreferredEventTypes = sp.PreferredEventTypes,
                BrandGuidelines = sp.BrandGuidelines,
                LogoUrl = sp.LogoUrl,
                BrandAssets = sp.BrandAssets,
                IsActive = sp.IsActive,
                CreatedAt = sp.CreatedAt,
                UpdatedAt = sp.UpdatedAt
            }).ToList();

            return Ok(new
            {
                success = true,
                message = "Sponsor package retrieved successfully",
                data = response,
                count = 1
            });
        }

        [HttpGet("sponsor/active")]
        public async Task<ActionResult> GetActiveSponsorPackages()
        {
            var sponsorPackages = await _sponsorPackageService.GetActiveSponsorPackagesAsync();
            if (sponsorPackages == null || !sponsorPackages.Any())
            {
                return NotFound(new
                {
                    success = false,
                    message = "No active sponsor packages found."
                });
            }
            var response = sponsorPackages.Select(sp => new SponsorPackageResponse
            {
                SponsorId = sp.SponsorId,
                PackageName = sp.PackageName,
                PackageType = sp.PackageType,
                Budget = sp.Budget,
                BudgetRange = sp.BudgetRange,
                SponsorshipBenefits = sp.SponsorshipBenefits,
                TargetAudience = sp.TargetAudience,
                PreferredEventTypes = sp.PreferredEventTypes,
                BrandGuidelines = sp.BrandGuidelines,
                LogoUrl = sp.LogoUrl,
                BrandAssets = sp.BrandAssets,
                IsActive = sp.IsActive,
                CreatedAt = sp.CreatedAt,
                UpdatedAt = sp.UpdatedAt
            }).ToList();
            return Ok(new
            {
                success = true,
                message = "Active sponsor packages retrieved successfully",
                data = response,
                count = response.Count
            });
        }

        [HttpGet("sponsor/{min}/{max}")]
        public async Task<ActionResult> GetSponsorPackageByBudgetRange(decimal min, decimal max)
        {
            var sponsorPackages = await _sponsorPackageService.GetSponsorPackageByBudgetRangeAsync(min, max);
            if (sponsorPackages == null || !sponsorPackages.Any())
            {
                return NotFound(new
                {
                    success = false,
                    message = "No sponsor packages found in the specified budget range."
                });
            }
            var response = sponsorPackages.Select(sp => new SponsorPackageResponse
            {
                SponsorId = sp.SponsorId,
                PackageName = sp.PackageName,
                PackageType = sp.PackageType,
                Budget = sp.Budget,
                BudgetRange = sp.BudgetRange,
                SponsorshipBenefits = sp.SponsorshipBenefits,
                TargetAudience = sp.TargetAudience,
                PreferredEventTypes = sp.PreferredEventTypes,
                BrandGuidelines = sp.BrandGuidelines,
                LogoUrl = sp.LogoUrl,
                BrandAssets = sp.BrandAssets,
                IsActive = sp.IsActive,
                CreatedAt = sp.CreatedAt,
                UpdatedAt = sp.UpdatedAt
            }).ToList();
            return Ok(new
            {
                success = true,
                message = "Sponsor packages retrieved successfully",
                data = response,
                count = response.Count
            });
        }
        //PUT: api/SponsorPackages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSponsorPackage(Guid id, [FromBody] SponsorPackageUpdateRequest request)
        {
            if(ModelState.IsValid == false)
            {
                var allErrors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed",
                    errors = allErrors
                });
            }
            var sponsorPackage = await _sponsorPackageService.GetSponsorPackageByIdAsync(id);

            if (sponsorPackage == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Failed to update sponsor package."
                });
            }

            sponsorPackage.PackageName = request.PackageName ?? sponsorPackage.PackageName;
            sponsorPackage.PackageType = request.PackageType ?? sponsorPackage.PackageType;
            sponsorPackage.Budget = request.Budget ?? sponsorPackage.Budget;
            sponsorPackage.BudgetRange = request.BudgetRange ?? sponsorPackage.BudgetRange;
            sponsorPackage.SponsorshipBenefits = request.SponsorshipBenefits ?? sponsorPackage.SponsorshipBenefits;
            sponsorPackage.TargetAudience = request.TargetAudience ?? sponsorPackage.TargetAudience;
            sponsorPackage.PreferredEventTypes = request.PreferredEventTypes ?? sponsorPackage.PreferredEventTypes;
            sponsorPackage.BrandGuidelines = request.BrandGuidelines ?? sponsorPackage.BrandGuidelines;
            sponsorPackage.LogoUrl = request.LogoUrl ?? sponsorPackage.LogoUrl;
            sponsorPackage.BrandAssets = request.BrandAssets ?? sponsorPackage.BrandAssets;
            sponsorPackage.IsActive = sponsorPackage.IsActive;
            sponsorPackage.UpdatedAt = DateTime.UtcNow;

            _sponsorPackageService.Update(sponsorPackage);

            return Ok(new
            {
                success = true,
                message = "Sponsor package updated successfully"
            });
        }



        //// POST: api/SponsorPackages
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SponsorPackageResponse>> PostSponsorPackage([FromBody] SponsorPackageAddRequest request)
        {
            if (!ModelState.IsValid)
            {
                var allErrors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new
                {
                    Message = "Validation failed",
                    Errors = allErrors
                });
            }
                var sponsorPackage = new SponsorPackage
            {
                Id = Guid.NewGuid(),
                SponsorId = request.SponsorId,
                PackageName = request.PackageName,
                PackageType = request.PackageType,
                Budget = request.Budget,
                BudgetRange = request.BudgetRange,
                SponsorshipBenefits = request.SponsorshipBenefits,
                TargetAudience = request.TargetAudience,
                PreferredEventTypes = request.PreferredEventTypes,
                BrandGuidelines = request.BrandGuidelines,
                LogoUrl = request.LogoUrl,
                BrandAssets = request.BrandAssets,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _sponsorPackageService.CreateAsync(sponsorPackage);

            var response = new SponsorPackageResponse
            {
                SponsorId = sponsorPackage.SponsorId,
                PackageName = sponsorPackage.PackageName,
                PackageType = sponsorPackage.PackageType,
                Budget = sponsorPackage.Budget,
                BudgetRange = sponsorPackage.BudgetRange,
                SponsorshipBenefits = sponsorPackage.SponsorshipBenefits,
                TargetAudience = sponsorPackage.TargetAudience,
                PreferredEventTypes = sponsorPackage.PreferredEventTypes,
                BrandGuidelines = sponsorPackage.BrandGuidelines,
                LogoUrl = sponsorPackage.LogoUrl,
                BrandAssets = sponsorPackage.BrandAssets,
                IsActive = sponsorPackage.IsActive,
                CreatedAt = sponsorPackage.CreatedAt,
                UpdatedAt = sponsorPackage.UpdatedAt
            };

            return CreatedAtAction(nameof(GetSponsorPackage), new { id = sponsorPackage.Id }, response);
        }


        //// DELETE: api/SponsorPackages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSponsorPackage(Guid id)
        {
            var sponsorPackage = await _sponsorPackageService.GetSponsorPackageByIdAsync(id);
            if (sponsorPackage == null)
            {
                return NotFound();
            }

            _sponsorPackageService.Update(sponsorPackage);

            return Ok(new
            {
                success = true,
                message = "Sponsor package deleted successfully"
            });
        }

        private bool SponsorPackageExists(Guid id)
        {
            return _sponsorPackageService.GetSponsorPackageByIdAsync(id) != null;
        }
    }
}
