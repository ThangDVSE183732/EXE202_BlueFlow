using EventLink_Repositories.DBContext;
using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventLink_Repositories.Repository
{
    /// <summary>
    /// Repository implementation for EventActivity entity
    /// Handles all database operations for event timeline activities
    /// </summary>
    public class EventActivityRepository : GenericRepository<EventActivity>, IEventActivityRepository
    {
        private readonly EventLinkDBContext _context;
        private readonly ILogger<EventActivityRepository> _logger;

        public EventActivityRepository(
            EventLinkDBContext context,
            ILogger<EventActivityRepository> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all activities for a specific event, ordered by DisplayOrder and StartTime
        /// </summary>
        public async Task<List<EventActivity>> GetActivitiesByEventIdAsync(Guid eventId)
        {
            try
            {
                return await _context.EventActivities
                    .Where(a => a.EventId == eventId)
                    .OrderBy(a => a.DisplayOrder)
                    .ThenBy(a => a.StartTime)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting activities for event {EventId}", eventId);
                throw;
            }
        }

        /// <summary>
        /// Get activities within a specific time range
        /// </summary>
        public async Task<List<EventActivity>> GetActivitiesByTimeRangeAsync(
            Guid eventId,
            TimeSpan startTime,
            TimeSpan endTime)
        {
            try
            {
                return await _context.EventActivities
                    .Where(a => a.EventId == eventId &&
                               a.StartTime >= startTime &&
                               a.EndTime <= endTime)
                    .OrderBy(a => a.StartTime)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting activities in time range for event {EventId}", eventId);
                throw;
            }
        }

        /// <summary>
        /// Check if a time slot is available (no overlapping activities)
        /// </summary>
        public async Task<bool> IsTimeSlotAvailableAsync(
            Guid eventId,
            TimeSpan startTime,
            TimeSpan endTime,
            Guid? excludeActivityId = null)
        {
            try
            {
                var query = _context.EventActivities
                    .Where(a => a.EventId == eventId &&
                               (
                                   (a.StartTime < endTime && a.EndTime > startTime) ||
                                   (a.StartTime <= startTime && a.EndTime > startTime) ||
                                   (a.StartTime < endTime && a.EndTime >= endTime) ||
                                   (a.StartTime >= startTime && a.EndTime <= endTime)
                               ));

                if (excludeActivityId.HasValue)
                {
                    query = query.Where(a => a.Id != excludeActivityId.Value);
                }

                var hasConflict = await query.AnyAsync();

                if (hasConflict)
                {
                    _logger.LogWarning(
                        "Time slot conflict detected for event {EventId} between {StartTime} and {EndTime}",
                        eventId, startTime, endTime);
                }

                return !hasConflict;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking time slot availability for event {EventId}", eventId);
                throw;
            }
        }

        /// <summary>
        /// Delete all activities for a specific event
        /// </summary>
        public async Task DeleteActivitiesByEventIdAsync(Guid eventId)
        {
            try
            {
                var activities = await _context.EventActivities
                    .Where(a => a.EventId == eventId)
                    .ToListAsync();

                if (activities.Any())
                {
                    _context.EventActivities.RemoveRange(activities);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation(
                        "Deleted {Count} activities for event {EventId}",
                        activities.Count, eventId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting activities for event {EventId}", eventId);
                throw;
            }
        }

        /// <summary>
        /// Bulk insert multiple activities at once
        /// </summary>
        public async Task BulkInsertActivitiesAsync(List<EventActivity> activities)
        {
            try
            {
                if (activities == null || !activities.Any())
                {
                    _logger.LogWarning("Attempted to bulk insert null or empty activity list");
                    return;
                }

                foreach (var activity in activities)
                {
                    if (activity.Id == Guid.Empty)
                        activity.Id = Guid.NewGuid();

                    if (activity.CreatedAt == null || activity.CreatedAt == DateTime.MinValue)
                        activity.CreatedAt = DateTime.UtcNow;

                    if (activity.UpdatedAt == null || activity.UpdatedAt == DateTime.MinValue)
                        activity.UpdatedAt = DateTime.UtcNow;
                }

                await _context.EventActivities.AddRangeAsync(activities);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Bulk inserted {Count} activities for event {EventId}",
                    activities.Count, activities.First().EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk inserting activities");
                throw;
            }
        }

        /// <summary>
        /// Get activity count for an event
        /// </summary>
        public async Task<int> GetActivityCountAsync(Guid eventId)
        {
            try
            {
                return await _context.EventActivities
                    .CountAsync(a => a.EventId == eventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting activity count for event {EventId}", eventId);
                throw;
            }
        }

        /// <summary>
        /// Check if activity belongs to event
        /// </summary>
        public async Task<bool> ActivityBelongsToEventAsync(Guid activityId, Guid eventId)
        {
            try
            {
                return await _context.EventActivities
                    .AnyAsync(a => a.Id == activityId && a.EventId == eventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if activity {ActivityId} belongs to event {EventId}",
                    activityId, eventId);
                throw;
            }
        }

        /// <summary>
        /// Custom AddAsync (not overriding base)
        /// Adds timestamps and returns entity
        /// </summary>
        public new async Task<EventActivity> AddAsync(EventActivity activity)
        {
            if (activity.Id == Guid.Empty)
                activity.Id = Guid.NewGuid();

            if (activity.CreatedAt == null || activity.CreatedAt == DateTime.MinValue)
                activity.CreatedAt = DateTime.UtcNow;

            if (activity.UpdatedAt == null || activity.UpdatedAt == DateTime.MinValue)
                activity.UpdatedAt = DateTime.UtcNow;

            await base.AddAsync(activity); // base.AddAsync() returns void
            return activity;
        }

        /// <summary>
        /// Custom Update (not overriding base)
        /// </summary>
        public new void Update(EventActivity activity)
        {
            activity.UpdatedAt = DateTime.UtcNow;
            base.Update(activity);
        }

        /// <summary>
        /// Get activities by type for an event
        /// </summary>
        public async Task<List<EventActivity>> GetActivitiesByTypeAsync(Guid eventId, string activityType)
        {
            try
            {
                return await _context.EventActivities
                    .Where(a => a.EventId == eventId && a.ActivityType == activityType)
                    .OrderBy(a => a.StartTime)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting activities by type {Type} for event {EventId}",
                    activityType, eventId);
                throw;
            }
        }

        /// <summary>
        /// Get public activities only
        /// </summary>
        public async Task<List<EventActivity>> GetPublicActivitiesAsync(Guid eventId)
        {
            try
            {
                return await _context.EventActivities
                    .Where(a => a.EventId == eventId && a.IsPublic == true)
                    .OrderBy(a => a.DisplayOrder)
                    .ThenBy(a => a.StartTime)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting public activities for event {EventId}", eventId);
                throw;
            }
        }

        /// <summary>
        /// Update display order for multiple activities
        /// </summary>
        public async Task UpdateDisplayOrdersAsync(Dictionary<Guid, int> activityOrders)
        {
            try
            {
                foreach (var kvp in activityOrders)
                {
                    var activity = await _context.EventActivities
                        .FirstOrDefaultAsync(a => a.Id == kvp.Key);

                    if (activity != null)
                    {
                        activity.DisplayOrder = kvp.Value;
                        activity.UpdatedAt = DateTime.UtcNow;
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated display orders for {Count} activities", activityOrders.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating display orders");
                throw;
            }
        }

        /// <summary>
        /// Get activities with participant info (future feature)
        /// </summary>
        public async Task<List<EventActivity>> GetActivitiesWithParticipantsAsync(Guid eventId)
        {
            try
            {
                return await _context.EventActivities
                    .Where(a => a.EventId == eventId)
                    .OrderBy(a => a.DisplayOrder)
                    .ThenBy(a => a.StartTime)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting activities with participants for event {EventId}", eventId);
                throw;
            }
        }
    }
}
