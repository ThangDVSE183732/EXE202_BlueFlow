using EventLink_Repositories.Interface;
using Eventlink_Services.Interface;
using Eventlink_Services.Response;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Eventlink_Services.Service
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUserSubscriptionRepository _subscriptionRepo;
        private readonly IUserRepository _userRepo;
        private readonly IGenericRepository<EventLink_Repositories.Models.SubscriptionPlan> _planRepo;
        private readonly IEmailService _emailService;
        private readonly ILogger<SubscriptionService> _logger;

        public SubscriptionService(
            IUserSubscriptionRepository subscriptionRepo,
            IUserRepository userRepo,
            IGenericRepository<EventLink_Repositories.Models.SubscriptionPlan> planRepo,
            IEmailService emailService,
            ILogger<SubscriptionService> logger)
        {
            _subscriptionRepo = subscriptionRepo;
            _userRepo = userRepo;
            _planRepo = planRepo;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<PremiumStatusResponse> GetPremiumStatusAsync(Guid userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            var activeSubscription = await _subscriptionRepo.GetActiveSubscriptionByUserIdAsync(userId);

            var response = new PremiumStatusResponse
            {
                IsPremium = user.IsPremium ?? false,
                ExpiryDate = user.PremiumExpiryDate,
                DaysRemaining = 0,
                PlanType = null,
                AutoRenew = false,
                ActiveSubscription = null
            };

            if (activeSubscription != null && user.PremiumExpiryDate.HasValue)
            {
                var daysRemaining = (user.PremiumExpiryDate.Value - DateTime.UtcNow).Days;
                response.DaysRemaining = Math.Max(0, daysRemaining);
                response.PlanType = activeSubscription.Plan?.PlanType;
                response.AutoRenew = activeSubscription.AutoRenew ?? false;
                response.ActiveSubscription = new ActiveSubscriptionDto
                {
                    SubscriptionId = activeSubscription.Id,
                    PlanName = activeSubscription.Plan?.PlanName,
                    StartDate = activeSubscription.StartDate,
                    EndDate = activeSubscription.EndDate ?? DateTime.MinValue,
                    MonthlyPrice = activeSubscription.Plan?.MonthlyPrice ?? 0
                };
            }

            return response;
        }

        public async Task<bool> HasActivePremiumAsync(Guid userId)
        {
            return await _subscriptionRepo.HasActiveSubscriptionAsync(userId);
        }

        public async Task<List<SubscriptionPlanDto>> GetAvailablePlansAsync(string userRole = null)
        {
            var plans = await _planRepo.GetAllAsync();

            var filteredPlans = plans
                .Where(p => p.IsActive == true)
                .Where(p => string.IsNullOrEmpty(userRole) ||
                           string.IsNullOrEmpty(p.TargetRole) ||
                           p.TargetRole == userRole);

            return filteredPlans.Select(p => new SubscriptionPlanDto
            {
                Id = p.Id,
                PlanName = p.PlanName,
                PlanType = p.PlanType,
                TargetRole = p.TargetRole,
                MonthlyPrice = p.MonthlyPrice,
                YearlyPrice = p.YearlyPrice,
                MaxEvents = p.MaxEvents,
                MaxPartnerships = p.MaxPartnerships,
                MaxMessages = p.MaxMessages,
                Features = ParseFeatures(p.Features),
                IsActive = p.IsActive ?? false
            }).ToList();
        }

        public async Task DeactivateExpiredSubscriptionsAsync()
        {
            try
            {
                await _subscriptionRepo.DeactivateExpiredSubscriptionsAsync();
                _logger.LogInformation("Expired subscriptions deactivated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deactivate expired subscriptions");
                throw;
            }
        }

        public async Task SendExpiryRemindersAsync(int daysBeforeExpiry = 7)
        {
            try
            {
                var expiringSubscriptions = await _subscriptionRepo.GetExpiringSubscriptionsAsync(daysBeforeExpiry);

                foreach (var subscription in expiringSubscriptions)
                {
                    if (subscription.User != null)
                    {
                        var daysRemaining = (subscription.EndDate.Value - DateTime.UtcNow).Days;

                        var subject = "EventLink Premium - Subscription Expiring Soon";
                        var body = $@"
                            <html>
                            <body>
                                <h2>Your EventLink Premium subscription is expiring soon!</h2>
                                <p>Hi {subscription.User.FullName},</p>
                                <p>Your <strong>{subscription.Plan?.PlanName}</strong> subscription will expire in <strong>{daysRemaining} days</strong> on {subscription.EndDate:yyyy-MM-dd}.</p>
                                <p>Don't lose access to your premium features! Renew now to continue enjoying:</p>
                                <ul>
                                    <li>Unlimited events</li>
                                    <li>Priority support</li>
                                    <li>Advanced analytics</li>
                                    <li>And more...</li>
                                </ul>
                                <p><a href='https://eventlink.com/premium/renew'>Renew Now</a></p>
                                <p>Best regards,<br/>EventLink Team</p>
                            </body>
                            </html>
                        ";

                        await _emailService.SendEmailAsync(subscription.User.Email, subject, body);
                        _logger.LogInformation($"Expiry reminder sent to user {subscription.UserId}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send expiry reminders");
                throw;
            }
        }

        private List<string> ParseFeatures(string featuresJson)
        {
            if (string.IsNullOrEmpty(featuresJson))
            {
                return new List<string>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<string>>(featuresJson) ?? new List<string>();
            }
            catch
            {
                return new List<string> { featuresJson };
            }
        }
    }
}