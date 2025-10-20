using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EventLink_Repositories.Models;
using Eventlink_Services.Response;
using Eventlink_Services.Interface;
using static Eventlink_Services.Request.EventProposalRequest;

namespace EventLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventProposalsController : ControllerBase
    {
        private readonly IEventProposalService _eventProposalService;

        public EventProposalsController(IEventProposalService eventProposalService)
        {
            _eventProposalService = eventProposalService;
        }

        // GET: api/EventProposals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventProposalResponse>>> GetEventProposals()
        {
            var eventProposals = await _eventProposalService.GetAllEventProposalsAsync();

            return Ok(new
            {
                success = true,
                message = "Event proposals retrieved successfully",
                data = eventProposals,
                count = eventProposals.Count
            });
        }

        // GET: api/EventProposals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EventProposalResponse>> GetEventProposal(Guid id)
        {
            var eventProposal = await _eventProposalService.GetEventProposalByIdAsync(id);

            return Ok(new
            {
                success = true,
                message = "Event proposal retrieved successfully",
                data = eventProposal
            });
        }

        // PUT: api/EventProposals/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEventProposal(Guid id, UpdateEventProposalRequest eventProposal)
        {
            var existingProposal = await _eventProposalService.GetEventProposalByIdAsync(id);

            if (existingProposal == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Event proposal not found"
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid data",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            var updatedProposal = await _eventProposalService.UpdateEventProposalAsync(id, eventProposal);

            return Ok(new
            {
                success = true,
                message = "Event proposal updated successfully",
                data = updatedProposal
            });
        }

        // POST: api/EventProposals
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EventProposal>> PostEventProposal(CreateEventProposalRequest eventProposal)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid data",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            var createdProposal = await _eventProposalService.CreateEventProposalAsync(eventProposal);

            return Ok(new
            {
                success = true,
                message = "Event proposal created successfully",
                data = createdProposal
            });
        }

        // DELETE: api/EventProposals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEventProposal(Guid id)
        {
            var existingProposal = await _eventProposalService.GetEventProposalByIdAsync(id);

            if (existingProposal == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Event proposal not found"
                });
            }

            var result = await _eventProposalService.DeleteEventProposalAsync(id);

            return Ok(new
            {
                success = result,
                message = result ? "Event proposal deleted successfully" : "Failed to delete event proposal"
            });
        }

        private async Task<bool> EventProposalExists(Guid id)
        {
            return await _eventProposalService.GetEventProposalByIdAsync(id) != null;
        }
    }
}
