using EventLink_Repositories.Models;
using Eventlink_Services.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Eventlink_Services.Request.EventProposalRequest;

namespace Eventlink_Services.Interface
{
    public interface IEventProposalService
    {
        Task<List<EventProposalResponse>> GetAllEventProposalsAsync();
        Task<EventProposalResponse> GetEventProposalByIdAsync(Guid id);
        Task<EventProposalResponse> CreateEventProposalAsync(CreateEventProposalRequest request);
        Task<EventProposalResponse> UpdateEventProposalAsync(Guid id, UpdateEventProposalRequest request);
        Task<bool> DeleteEventProposalAsync(Guid id);
    }
}
