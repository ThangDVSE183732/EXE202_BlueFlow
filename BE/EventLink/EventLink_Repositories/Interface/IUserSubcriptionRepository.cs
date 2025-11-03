using EventLink_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventLink_Repositories.Interface
{
    public interface IUserSubscriptionRepository : IGenericRepository<UserSubscription>
    {
        /// <summary>
        /// Get active subscription for a user
        /// </summary>
        Task<UserSubscription> GetActiveSubscriptionByUserIdAsync(Guid userId);

        /// <summary>
        /// Get all subscriptions for a user
        /// </summary>
        Task<List<UserSubscription>> GetSubscriptionsByUserIdAsync(Guid userId);

        /// <summary>
        /// Get subscription with plan details
        /// </summary>
        Task<UserSubscription> GetSubscriptionWithPlanAsync(Guid subscriptionId);

        /// <summary>
        /// Check if user has active subscription
        /// </summary>
        Task<bool> HasActiveSubscriptionAsync(Guid userId);

        /// <summary>
        /// Get expiring subscriptions (within X days)
        /// </summary>
        Task<List<UserSubscription>> GetExpiringSubscriptionsAsync(int daysBeforeExpiry);

        /// <summary>
        /// Deactivate expired subscriptions
        /// </summary>
        Task DeactivateExpiredSubscriptionsAsync();

        /// <summary>
        /// Create or renew subscription
        /// </summary>
        Task<UserSubscription> CreateOrRenewSubscriptionAsync(Guid userId, Guid planId, bool isRenewal = false);
    }
}