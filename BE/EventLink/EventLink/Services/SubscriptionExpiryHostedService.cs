using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Eventlink_Services.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventLink.Services
{
    /// <summary>
    /// Background service to automatically deactivate expired subscriptions
    /// and send expiry reminder emails
    /// </summary>
    public class SubscriptionExpiryHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SubscriptionExpiryHostedService> _logger;
        
        // ? Check interval: 1 hour (configurable via appsettings.json)
        private readonly TimeSpan _checkInterval;
        
        // ? Reminder: 7 days before expiry (configurable via appsettings.json)
        private readonly int _reminderDays;

        public SubscriptionExpiryHostedService(
            IServiceProvider serviceProvider,
            ILogger<SubscriptionExpiryHostedService> logger,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            
            // ? Read configuration with defaults
            var hours = configuration.GetValue<int>("SubscriptionSettings:CheckIntervalHours", 1);
            _checkInterval = TimeSpan.FromHours(hours);
            
            _reminderDays = configuration.GetValue<int>("SubscriptionSettings:ReminderDaysBeforeExpiry", 7);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("? Subscription Expiry Service started at {Time}", DateTime.UtcNow);
            _logger.LogInformation("?? Check interval: {Hours} hour(s), Reminder days: {Days}", 
                _checkInterval.TotalHours, _reminderDays);

            // ? Wait 10 seconds before first run to let app fully start
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessSubscriptionsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "? Error in Subscription Expiry Service");
                }

                // ? Wait before next check
                _logger.LogInformation("? Next subscription check in {Hours} hour(s) at {NextRun}", 
                    _checkInterval.TotalHours, DateTime.UtcNow.Add(_checkInterval));
                
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("?? Subscription Expiry Service stopped at {Time}", DateTime.UtcNow);
        }

        private async Task ProcessSubscriptionsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();

            _logger.LogInformation("?? Checking subscriptions at {Time}", DateTime.UtcNow);

            var totalProcessed = 0;
            var totalErrors = 0;

            // ? 1. Deactivate expired subscriptions
            try
            {
                await subscriptionService.DeactivateExpiredSubscriptionsAsync();
                _logger.LogInformation("? Expired subscriptions deactivated successfully");
                totalProcessed++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Failed to deactivate expired subscriptions");
                totalErrors++;
            }

            // ? 2. Send expiry reminders (7 days before expiry by default)
            try
            {
                await subscriptionService.SendExpiryRemindersAsync(_reminderDays);
                _logger.LogInformation("? Expiry reminders sent successfully");
                totalProcessed++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Failed to send expiry reminders");
                totalErrors++;
            }

            _logger.LogInformation("?? Subscription check completed: {Processed} tasks processed, {Errors} errors", 
                totalProcessed, totalErrors);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("?? Subscription Expiry Service is stopping...");
            await base.StopAsync(cancellationToken);
        }
    }
}
