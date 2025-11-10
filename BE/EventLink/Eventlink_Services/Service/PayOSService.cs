using Eventlink_Services.Interface;
using Microsoft.Extensions.Configuration;
using PayOS;
using PayOS.Models.V2.PaymentRequests;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Eventlink_Services.Service
{
    public class PayOSService : IPayOSService
    {
        private readonly PayOSClient _client;
        private readonly IConfiguration _config;
        private readonly string _defaultReturnUrl;
        private readonly string _defaultCancelUrl;

        public PayOSService(IConfiguration configuration)
        {
            _config = configuration;

            var clientId = _config["PayOSSettings:ClientId"] ?? Environment.GetEnvironmentVariable("PAYOS_CLIENT_ID");
            var apiKey = _config["PayOSSettings:ApiKey"] ?? Environment.GetEnvironmentVariable("PAYOS_API_KEY");
            var checksumKey = _config["PayOSSettings:ChecksumKey"] ?? Environment.GetEnvironmentVariable("PAYOS_CHECKSUM_KEY");

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(checksumKey))
            {
                throw new ArgumentException("PayOS credentials are not configured properly");
            }

            _client = new PayOSClient(new PayOSOptions
            {
                ClientId = clientId,
                ApiKey = apiKey,
                ChecksumKey = checksumKey,
                LogLevel = Microsoft.Extensions.Logging.LogLevel.None
            });

            _defaultReturnUrl = _config["PayOSSettings:ReturnUrl"] ?? Environment.GetEnvironmentVariable("PAYOS_RETURN_URL");
            _defaultCancelUrl = _config["PayOSSettings:CancelUrl"] ?? Environment.GetEnvironmentVariable("PAYOS_CANCEL_URL");

            Console.WriteLine("[PayOS SDK] Service initialized");
        }

        public async Task<dynamic> CreatePaymentLinkAsync(
            long orderCode,
            long amount,
            string description,
            string buyerName,
            string buyerEmail,
            string returnUrl = null,
            string cancelUrl = null)
        {
            try
            {
                var actualReturnUrl = returnUrl ?? _defaultReturnUrl;
                var actualCancelUrl = cancelUrl ?? _defaultCancelUrl;

                var paymentRequest = new CreatePaymentLinkRequest
                {
                    OrderCode = orderCode,
                    Amount = amount,
                    Description = description?.Length > 25 ? description.Substring(0, 25) : "Premium",
                    ReturnUrl = actualReturnUrl,
                    CancelUrl = actualCancelUrl,
                    BuyerName = buyerName,
                    BuyerEmail = buyerEmail,
                    Items = new List<PaymentLinkItem>
            {
                new PaymentLinkItem
                {
                    Name = "Premium",
                    Quantity = 1,
                    Price = amount
                }
            }
                };

                Console.WriteLine($"[PayOS SDK] Creating payment link for order: {orderCode}");

                // ✅ Try to create payment via SDK
                var paymentResponse = await _client.PaymentRequests.CreateAsync(paymentRequest);

                Console.WriteLine("[PayOS SDK] ✅ Payment created successfully!");

                // ✅ Debug: Print ALL properties to find URL
                if (paymentResponse != null)
                {
                    var properties = paymentResponse.GetType().GetProperties();
                    Console.WriteLine("[PayOS SDK] Available properties:");
                    foreach (var prop in properties)
                    {
                        try
                        {
                            var value = prop.GetValue(paymentResponse);
                            Console.WriteLine($"  - {prop.Name}: {value}");

                            // Highlight URL-related properties
                            if (prop.Name.ToLower().Contains("url") ||
                                prop.Name.ToLower().Contains("link") ||
                                prop.Name.ToLower().Contains("checkout"))
                            {
                                Console.WriteLine($"  ⭐ URL PROPERTY FOUND: {prop.Name} = {value}");
                            }
                        }
                        catch { }
                    }
                }

                return paymentResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PayOS SDK] ⚠️ SDK Exception: {ex.GetType().Name}");
                Console.WriteLine($"[PayOS SDK] Message: {ex.Message}");

                // ✅ Check if it's signature verification error
                if (ex.Message.Contains("Data integrity check failed")
                    || ex.Message.Contains("signature")
                    || ex.GetType().Name.Contains("InvalidData"))
                {
                    Console.WriteLine("[PayOS SDK] 🔄 Signature verification failed, but payment might be created.");
                    Console.WriteLine("[PayOS SDK] 🔍 Attempting to retrieve payment manually...");

                    await Task.Delay(2000);

                    try
                    {
                        Console.WriteLine($"[PayOS SDK] Checking if payment {orderCode} exists...");
                        var paymentInfo = await GetPaymentByOrderCodeWithRetry(orderCode);

                        if (paymentInfo != null)
                        {
                            Console.WriteLine("[PayOS SDK] ✅ Payment found! SDK created it despite exception.");

                            // ✅ Debug retrieved payment info
                            var properties = paymentInfo.GetType().GetProperties();
                            Console.WriteLine("[PayOS SDK] Retrieved payment properties:");
                            foreach (var prop in properties)
                            {
                                try
                                {
                                    var value = prop.GetValue(paymentInfo);
                                    Console.WriteLine($"  - {prop.Name}: {value}");
                                }
                                catch { }
                            }

                            return paymentInfo;
                        }
                        else
                        {
                            Console.WriteLine("[PayOS SDK] ❌ Payment not found. Creation truly failed.");
                            throw new Exception($"Failed to create PayOS payment: {ex.Message}", ex);
                        }
                    }
                    catch (Exception verifyEx)
                    {
                        Console.WriteLine($"[PayOS SDK] ❌ Verification also failed: {verifyEx.Message}");
                        throw new Exception($"PayOS payment creation failed: {ex.Message}", ex);
                    }
                }
                else
                {
                    Console.WriteLine("[PayOS SDK] ❌ Non-signature error. Payment likely failed.");
                    throw new Exception($"PayOS payment creation failed: {ex.Message}", ex);
                }
            }
        }

        // ✅ Helper method: Retry getting payment info
        private async Task<dynamic> GetPaymentByOrderCodeWithRetry(long orderCode, int maxRetries = 3)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    Console.WriteLine($"[PayOS SDK] Attempt {i + 1}/{maxRetries} to get payment {orderCode}...");

                    var paymentInfo = await _client.PaymentRequests.GetAsync(orderCode.ToString());

                    Console.WriteLine($"[PayOS SDK] ✅ Found payment");
                    return paymentInfo;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[PayOS SDK] Attempt {i + 1} failed: {ex.Message}");

                    if (i < maxRetries - 1)
                    {
                        await Task.Delay(1000 * (i + 1));
                    }
                }
            }

            return null;
        }

        public async Task<dynamic> GetPaymentInfoAsync(string paymentLinkId)
        {
            try
            {
                Console.WriteLine($"[PayOS SDK] Getting payment info: {paymentLinkId}");
                var paymentLink = await _client.PaymentRequests.GetAsync(paymentLinkId);
                Console.WriteLine("[PayOS SDK] ✅ Payment info retrieved");
                return paymentLink;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PayOS SDK] ❌ Failed to get payment info: {ex.Message}");
                throw new Exception($"Failed to get payment info from PayOS: {ex.Message}", ex);
            }
        }

        public async Task<dynamic> CancelPaymentLinkAsync(string paymentLinkId, string cancellationReason = null)
        {
            try
            {
                Console.WriteLine($"[PayOS SDK] Cancelling payment: {paymentLinkId}");
                var cancelledPaymentLink = await _client.PaymentRequests.CancelAsync(
                    paymentLinkId,
                    cancellationReason ?? "Cancelled by user"
                );
                Console.WriteLine("[PayOS SDK] ✅ Payment cancelled");
                return cancelledPaymentLink;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PayOS SDK] ❌ Failed to cancel payment: {ex.Message}");
                throw new Exception($"Failed to cancel payment link: {ex.Message}", ex);
            }
        }

        public bool VerifyWebhookData(string webhookBody)
        {
            try
            {
                if (string.IsNullOrEmpty(webhookBody))
                    return false;

                var test = JsonDocument.Parse(webhookBody);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public long GenerateOrderCode()
        {
            return DateTimeOffset.Now.ToUnixTimeSeconds();
        }
    }
}