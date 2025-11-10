using Eventlink_Services.Request;
using Eventlink_Services.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Interface
{
    public interface IEventActivityService
    {
        /// <summary>
        /// Get all activities for an event
        /// </summary>
        Task<List<EventActivityDto>> GetActivitiesByEventIdAsync(Guid eventId);

        /// <summary>
        /// Get single activity
        /// </summary>
        Task<EventActivityDto> GetActivityByIdAsync(Guid activityId);

        /// <summary>
        /// Create new activity (Organizer only)
        /// </summary>
        Task<EventActivityDto> CreateActivityAsync(Guid eventId, Guid organizerId, EventActivityRequest request);

        /// <summary>
        /// Update activity (Organizer only)
        /// </summary>
        Task<EventActivityDto> UpdateActivityAsync(Guid activityId, Guid organizerId, EventActivityRequest request);

        /// <summary>
        /// Delete activity (Organizer only)
        /// </summary>
        Task<bool> DeleteActivityAsync(Guid activityId, Guid organizerId);

        /// <summary>
        /// Bulk update activities for an event
        /// </summary>
        Task<List<EventActivityDto>> BulkUpdateActivitiesAsync(Guid eventId, Guid organizerId, List<EventActivityRequest> activities);
    }
}
