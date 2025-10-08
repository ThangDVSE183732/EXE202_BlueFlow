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
using static Eventlink_Services.Request.PartnershipRequest;

namespace EventLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnershipsController : ControllerBase
    {
        private readonly IPartnershipService _partnershipService;

        public PartnershipsController(IPartnershipService partnershipService)
        {
            _partnershipService = partnershipService;
        }

        // GET: api/Partnerships/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Partnership>> GetPartnership(Guid id)
        {
            var partnership = await _partnershipService.GetPartnershipByIdAsync(id);

            if (partnership == null)
            {
                return NotFound();
            }

            return partnership;
        }

        // PUT: api/Partnerships/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPartnership(Guid id, UpdatePartnershipRequest request)
        {
            var existingPartnership = await _partnershipService.GetPartnershipByIdAsync(id);
            if (existingPartnership == null)
            {
                return NotFound();
            }

            await _partnershipService.UpdatePartnershipAsync(id, request);

            return Ok(new
            {
                success = true,
                message = "Partnership updated successfully"
            });
        }

        // POST: api/Partnerships
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Partnership>> PostPartnership(CreatePartnershipRequest request)
        {
            if(ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            await _partnershipService.CreatePartnershipAsync(request);
            return Ok(new
            {
                success = true,
                message = "Partnership created successfully"
            });
        }

        // DELETE: api/Partnerships/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePartnership(Guid id)
        {
            var partnership = await _partnershipService.GetPartnershipByIdAsync(id);
            if (partnership == null)
            {
                return NotFound();
            }

            _partnershipService.DeletePartnershipAsync(id);

            return Ok(new
            {
                success = true,
                message = "Partnership deleted successfully"
            });
        }

        private bool PartnershipExists(Guid id)
        {
            return _partnershipService.GetPartnershipByIdAsync(id) != null;
        }
    }
}
