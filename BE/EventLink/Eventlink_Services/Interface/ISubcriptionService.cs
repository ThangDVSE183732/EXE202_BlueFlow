using Eventlink_Services.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eventlink_Services.Interface
{
    /// <summary>
    /// Service for subscription management
    /// </summary>
    public interface ISubscriptionService
    {
        /// <summary>
        /// Get premium status for user
        /// </summary>
        Task<PremiumStatusResponse> GetPremiumStatusAsync(Guid userId);

        /// <summary>
        /// Check if user has active premium
        /// </summary>
        Task<bool> HasActivePremiumAsync(Guid userId);

        /// <summary>
        /// Get available subscription plans
        /// </summary>
        Task<List<SubscriptionPlanDto>> GetAvailablePlansAsync(string userRole = null);

        /// <summary>
        /// Deactivate expired subscriptions (scheduled job)
        /// </summary>
        Task DeactivateExpiredSubscriptionsAsync();

        /// <summary>
        /// Send expiry reminders (scheduled job)
        /// </summary>
        Task SendExpiryRemindersAsync(int daysBeforeExpiry = 7);
    }

    /// <summary>
    /// DTO for subscription plan
    /// </summary>
    public class SubscriptionPlanDto
    {
        public Guid Id { get; set; }
        public string PlanName { get; set; }
        public string PlanType { get; set; }
        public string TargetRole { get; set; }
        public decimal MonthlyPrice { get; set; }
        public decimal? YearlyPrice { get; set; }
        public int? MaxEvents { get; set; }
        public int? MaxPartnerships { get; set; }
        public int? MaxMessages { get; set; }
        public List<string> Features { get; set; }
        public bool IsActive { get; set; }
    }
}