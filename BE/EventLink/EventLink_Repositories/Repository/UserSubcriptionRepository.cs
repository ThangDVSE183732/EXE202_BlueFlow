using EventLink_Repositories.DBContext;
using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventLink_Repositories.Repository
{
    public class UserSubscriptionRepository : GenericRepository<UserSubscription>, IUserSubscriptionRepository
    {
        private readonly EventLinkDBContext _context;

        public UserSubscriptionRepository(EventLinkDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<UserSubscription> GetActiveSubscriptionByUserIdAsync(Guid userId)
        {
            return await _context.UserSubscriptions
                .Include(s => s.Plan)
                .Include(s => s.User)
                .Where(s => s.UserId == userId && s.IsActive == true)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefaultAsync();
        }

        public async Task<List<UserSubscription>> GetSubscriptionsByUserIdAsync(Guid userId)
        {
            return await _context.UserSubscriptions
                .Include(s => s.Plan)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<UserSubscription> GetSubscriptionWithPlanAsync(Guid subscriptionId)
        {
            return await _context.UserSubscriptions
                .Include(s => s.Plan)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == subscriptionId);
        }

        public async Task<bool> HasActiveSubscriptionAsync(Guid userId)
        {
            return await _context.UserSubscriptions
                .AnyAsync(s => s.UserId == userId &&
                              s.IsActive == true &&
                              s.EndDate > DateTime.UtcNow);
        }

        public async Task<List<UserSubscription>> GetExpiringSubscriptionsAsync(int daysBeforeExpiry)
        {
            var targetDate = DateTime.UtcNow.AddDays(daysBeforeExpiry);

            return await _context.UserSubscriptions
                .Include(s => s.User)
                .Include(s => s.Plan)
                .Where(s => s.IsActive == true &&
                           s.EndDate.HasValue &&
                           s.EndDate.Value <= targetDate &&
                           s.EndDate.Value > DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task DeactivateExpiredSubscriptionsAsync()
        {
            var expiredSubscriptions = await _context.UserSubscriptions
                .Where(s => s.IsActive == true &&
                           s.EndDate.HasValue &&
                           s.EndDate.Value <= DateTime.UtcNow)
                .ToListAsync();

            foreach (var subscription in expiredSubscriptions)
            {
                subscription.IsActive = false;

                // Also update user's premium status
                var user = await _context.Users.FindAsync(subscription.UserId);
                if (user != null)
                {
                    user.IsPremium = false;
                    user.PremiumExpiryDate = null;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<UserSubscription> CreateOrRenewSubscriptionAsync(Guid userId, Guid planId, bool isRenewal = false)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(planId);
            if (plan == null)
            {
                throw new KeyNotFoundException("Subscription plan not found");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            // Deactivate existing subscriptions
            var existingSubscriptions = await _context.UserSubscriptions
                .Where(s => s.UserId == userId && s.IsActive == true)
                .ToListAsync();

            foreach (var sub in existingSubscriptions)
            {
                sub.IsActive = false;
            }

            // Calculate dates
            var startDate = DateTime.UtcNow;
            DateTime endDate;

            if (plan.PlanType.ToLower() == "monthly")
            {
                endDate = startDate.AddMonths(1);
            }
            else if (plan.PlanType.ToLower() == "yearly")
            {
                endDate = startDate.AddYears(1);
            }
            else
            {
                endDate = startDate.AddMonths(1); // Default to monthly
            }

            // Create new subscription
            var newSubscription = new UserSubscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PlanId = planId,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = true,
                AutoRenew = true,
                PaymentMethod = "PayOS",
                CreatedAt = DateTime.UtcNow
            };

            _context.UserSubscriptions.Add(newSubscription);

            // Update user premium status
            user.IsPremium = true;
            user.PremiumExpiryDate = endDate;

            await _context.SaveChangesAsync();

            return newSubscription;
        }
    }
}