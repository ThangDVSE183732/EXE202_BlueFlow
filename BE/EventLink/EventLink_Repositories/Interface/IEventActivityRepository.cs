using EventLink_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventLink_Repositories.Interface
{
    /// <summary>
    /// Repository interface for EventActivity entity
    /// Manages CRUD operations and business queries for event timeline activities
    /// </summary>
    public interface IEventActivityRepository : IGenericRepository<EventActivity>
    {
        /// <summary>
        /// Get all activities for a specific event, ordered by DisplayOrder and StartTime
        /// </summary>
        /// <param name="eventId">The event ID</param>
        /// <returns>List of activities sorted by display order and time</returns>
        Task<List<EventActivity>> GetActivitiesByEventIdAsync(Guid eventId);

        /// <summary>
        /// Get activities within a specific time range for an event
        /// </summary>
        /// <param name="eventId">The event ID</param>
        /// <param name="startTime">Start time range</param>
        /// <param name="endTime">End time range</param>
        /// <returns>List of activities within the time range</returns>
        Task<List<EventActivity>> GetActivitiesByTimeRangeAsync(Guid eventId, TimeSpan startTime, TimeSpan endTime);

        /// <summary>
        /// Check if a time slot is available (no overlapping activities)
        /// </summary>
        /// <param name="eventId">The event ID</param>
        /// <param name="startTime">Proposed start time</param>
        /// <param name="endTime">Proposed end time</param>
        /// <param name="excludeActivityId">Optional activity ID to exclude from check (for updates)</param>
        /// <returns>True if time slot is available, false if there's a conflict</returns>
        Task<bool> IsTimeSlotAvailableAsync(Guid eventId, TimeSpan startTime, TimeSpan endTime, Guid? excludeActivityId = null);

        /// <summary>
        /// Delete all activities for a specific event
        /// Used when replacing entire timeline or deleting an event
        /// </summary>
        /// <param name="eventId">The event ID</param>
        Task DeleteActivitiesByEventIdAsync(Guid eventId);

        /// <summary>
        /// Bulk insert multiple activities at once
        /// More efficient than inserting one by one
        /// </summary>
        /// <param name="activities">List of activities to insert</param>
        Task BulkInsertActivitiesAsync(List<EventActivity> activities);

        /// <summary>
        /// Get activity count for an event
        /// </summary>
        /// <param name="eventId">The event ID</param>
        /// <returns>Number of activities</returns>
        Task<int> GetActivityCountAsync(Guid eventId);

        /// <summary>
        /// Check if activity exists and belongs to event
        /// </summary>
        /// <param name="activityId">Activity ID</param>
        /// <param name="eventId">Event ID</param>
        /// <returns>True if activity exists and belongs to event</returns>
        Task<bool> ActivityBelongsToEventAsync(Guid activityId, Guid eventId);
    }
}