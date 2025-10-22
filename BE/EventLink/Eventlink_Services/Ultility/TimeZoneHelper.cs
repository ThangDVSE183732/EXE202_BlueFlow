using System;

namespace Eventlink_Services.Ultility
{
    /// <summary>
    /// Helper class for timezone conversions
    /// </summary>
    public static class TimeZoneHelper
    {
        // Default timezone for the application (Vietnam)
        private static readonly string DefaultTimeZoneId = "SE Asia Standard Time"; // UTC+7

        /// <summary>
        /// Convert UTC time to local time (Vietnam timezone by default)
        /// </summary>
        public static DateTime ToLocalTime(DateTime utcTime, string? timeZoneId = null)
        {
            // Ensure the time is treated as UTC
            var utcDateTime = utcTime.Kind == DateTimeKind.Utc
                ? utcTime
                : DateTime.SpecifyKind(utcTime, DateTimeKind.Utc);

            // Get timezone info
            var timeZone = GetTimeZone(timeZoneId);

            // Convert to local time
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
        }

        /// <summary>
        /// Get current local time (Vietnam timezone by default)
        /// </summary>
        public static DateTime GetCurrentLocalTime(string? timeZoneId = null)
        {
            var timeZone = GetTimeZone(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
        }

        /// <summary>
        /// Get TimeZoneInfo from timezone ID
        /// </summary>
        private static TimeZoneInfo GetTimeZone(string? timeZoneId)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId ?? DefaultTimeZoneId);
            }
            catch
            {
                // Fallback to default timezone if specified timezone not found
                return TimeZoneInfo.FindSystemTimeZoneById(DefaultTimeZoneId);
            }
        }

        /// <summary>
        /// Common timezone IDs for reference
        /// </summary>
        public static class TimeZones
        {
            public const string Vietnam = "SE Asia Standard Time";           // UTC+7
            public const string Singapore = "Singapore Standard Time";        // UTC+8
            public const string Tokyo = "Tokyo Standard Time";                // UTC+9
            public const string Sydney = "AUS Eastern Standard Time";         // UTC+10/+11
            public const string NewYork = "Eastern Standard Time";            // UTC-5/-4
            public const string London = "GMT Standard Time";                 // UTC+0/+1
        }
    }
}