using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using Eventlink_Services.Interface;
using Eventlink_Services.Request;
using Eventlink_Services.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PayOS.Models.V2.PaymentRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Eventlink_Services.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IUserSubscriptionRepository _subscriptionRepo;
        private readonly IUserRepository _userRepo;
        private readonly IGenericRepository<SubscriptionPlan> _planRepo;
        private readonly IPayOSService _payOSService;
        private readonly IConfiguration _config;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IPaymentRepository paymentRepo,
            IUserSubscriptionRepository subscriptionRepo,
            IUserRepository userRepo,
            IGenericRepository<SubscriptionPlan> planRepo,
            IPayOSService payOSService,
            IConfiguration config,
            ILogger<PaymentService> logger)
        {
            _paymentRepo = paymentRepo;
            _subscriptionRepo = subscriptionRepo;
            _userRepo = userRepo;
            _planRepo = planRepo;
            _payOSService = payOSService;
            _config = config;
            _logger = logger;
        }

        public async Task<CreatePaymentResponse> CreatePremiumPaymentAsync(Guid userId, CreatePremiumPaymentRequest request)
        {
            try
            {
                var user = await _userRepo.GetByIdAsync(userId);
                if (user == null)
                {
                    throw new KeyNotFoundException("User not found");
                }

                long amount;
                string description;

                if (request.PlanType.ToLower() == "monthly")
                {
                    amount = long.Parse(_config["PremiumPricing:MonthlyPrice"] ?? "99000");
                    description = "Premium Monthly";
                }
                else if (request.PlanType.ToLower() == "yearly")
                {
                    amount = long.Parse(_config["PremiumPricing:YearlyPrice"] ?? "990000");
                    description = "Premium Yearly";
                }
                else
                {
                    throw new ArgumentException("Invalid plan type. Must be 'monthly' or 'yearly'");
                }

                var orderCode = _payOSService.GenerateOrderCode();

                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    SubscriptionId = null,
                    PaymentType = "Premium Subscription",
                    Amount = amount,
                    Currency = "VND",
                    PayOsorderId = orderCode.ToString(),
                    PayOstransactionId = null,
                    PaymentMethod = "PayOS",
                    Status = "Pending",
                    Description = description,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _paymentRepo.AddAsync(payment);

                _logger.LogInformation($"Payment record created for user {userId}, orderCode: {orderCode}");

                var paymentResult = await _payOSService.CreatePaymentLinkAsync(
                    orderCode: orderCode,
                    amount: amount,
                    description: description,
                    buyerName: user.FullName ?? "Customer",
                    buyerEmail: user.Email,
                    returnUrl: request.ReturnUrl,
                    cancelUrl: request.CancelUrl
                );

                // ✅ Extract properties safely with comprehensive property name checking
                string paymentLinkId = null;
                string checkoutUrl = null;
                string qrCodeUrl = null;

                try
                {
                    if (paymentResult != null)
                    {
                        var resultType = paymentResult.GetType();
                        _logger.LogInformation($"PayOS response type: {resultType.Name}");

                        // ✅ Extract PaymentLinkId
                        paymentLinkId = TryGetProperty(paymentResult, "PaymentLinkId")
                                        ?? TryGetProperty(paymentResult, "Id")
                                        ?? TryGetProperty(paymentResult, "id")
                                        ?? TryGetProperty(paymentResult, "paymentLinkId")
                                        ?? orderCode.ToString();

                        // ✅ CRITICAL: Try ALL possible URL property names
                        checkoutUrl = TryGetProperty(paymentResult, "CheckoutUrl")
                                      ?? TryGetProperty(paymentResult, "checkoutUrl")
                                      ?? TryGetProperty(paymentResult, "PaymentUrl")
                                      ?? TryGetProperty(paymentResult, "paymentUrl")
                                      ?? TryGetProperty(paymentResult, "Url")
                                      ?? TryGetProperty(paymentResult, "url")
                                      ?? TryGetProperty(paymentResult, "Link")
                                      ?? TryGetProperty(paymentResult, "link");

                        // ✅ If still no URL, try to get it via GetAsync
                        if (string.IsNullOrEmpty(checkoutUrl) && !string.IsNullOrEmpty(paymentLinkId))
                        {
                            _logger.LogInformation($"No CheckoutUrl found. Attempting GetAsync({paymentLinkId})...");

                            try
                            {
                                await Task.Delay(1000);
                                var fullInfo = await _payOSService.GetPaymentInfoAsync(paymentLinkId);

                                if (fullInfo != null)
                                {
                                    checkoutUrl = TryGetProperty(fullInfo, "CheckoutUrl")
                                                  ?? TryGetProperty(fullInfo, "checkoutUrl")
                                                  ?? TryGetProperty(fullInfo, "PaymentUrl")
                                                  ?? TryGetProperty(fullInfo, "Url");

                                    _logger.LogInformation($"Retrieved via GetAsync: {checkoutUrl}");
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning($"GetAsync failed: {ex.Message}");
                            }
                        }

                        // ✅ Final fallback: Use PaymentLinkId in URL (NOT orderCode!)
                        if (string.IsNullOrEmpty(checkoutUrl))
                        {
                            checkoutUrl = $"https://pay.payos.vn/web/{paymentLinkId}";
                            _logger.LogInformation($"Using fallback URL with PaymentLinkId: {checkoutUrl}");
                        }

                        // ✅ Extract QR Code
                        qrCodeUrl = TryGetProperty(paymentResult, "QrCode")
                                    ?? TryGetProperty(paymentResult, "qrCode")
                                    ?? TryGetProperty(paymentResult, "QRCode")
                                    ?? TryGetProperty(paymentResult, "QrCodeUrl")
                                    ?? TryGetProperty(paymentResult, "qrCodeUrl")
                                    ?? $"https://img.vietqr.io/image/{paymentLinkId}";

                        _logger.LogInformation($"Final values - PaymentLinkId: {paymentLinkId}, CheckoutUrl: {checkoutUrl}");
                    }
                    else
                    {
                        throw new Exception("PayOS returned null response");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error extracting PayOS response properties");
                    throw new Exception($"Failed to process PayOS response: {ex.Message}", ex);
                }

                payment.PayOstransactionId = paymentLinkId;
                payment.UpdatedAt = DateTime.UtcNow;
                _paymentRepo.Update(payment);

                _logger.LogInformation($"Payment link created successfully. OrderCode: {orderCode}, PaymentLinkId: {paymentLinkId}");

                return new CreatePaymentResponse
                {
                    Success = true,
                    Message = "Payment link created successfully",
                    CheckoutUrl = checkoutUrl,
                    OrderCode = orderCode,
                    QrCodeUrl = qrCodeUrl
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to create payment for user {userId}");
                return new CreatePaymentResponse
                {
                    Success = false,
                    Message = $"Failed to create payment: {ex.Message}"
                };
            }
        }

        // ✅ Helper method at bottom of class
        private string TryGetProperty(dynamic obj, string propertyName)
        {
            try
            {
                if (obj == null) return null;

                var type = obj.GetType();
                var property = type.GetProperty(propertyName);

                if (property != null)
                {
                    var value = property.GetValue(obj);
                    return value?.ToString();
                }

                return null;
            }
            catch
            {
                return null;
            }
        }


        public async Task<bool> HandlePaymentWebhookAsync(PayOSWebhookRequest webhookData)
        {
            try
            {
                _logger.LogInformation($"[Webhook Handler] Processing webhook for order: {webhookData.Data?.OrderCode}");

                if (!webhookData.Success || webhookData.Data == null)
                {
                    _logger.LogWarning($"[Webhook Handler] Unsuccessful webhook: {webhookData.Desc}");
                    return false;
                }

                var orderCode = webhookData.Data.OrderCode;
                var payment = await _paymentRepo.GetPaymentByOrderCodeAsync(orderCode.ToString());

                if (payment == null)
                {
                    _logger.LogWarning($"[Webhook Handler] Payment not found for orderCode: {orderCode}");
                    return false;
                }

                _logger.LogInformation($"[Webhook Handler] Found payment: {payment.Id}, Status: {payment.Status}");

                // ✅ Check if already processed
                if (payment.Status == "Completed")
                {
                    _logger.LogInformation($"[Webhook Handler] Payment already completed: {payment.Id}");
                    return true;
                }

                // ✅ Check webhook code
                if (webhookData.Data.Code == "00") // Success code
                {
                    _logger.LogInformation($"[Webhook Handler] ✅ Payment successful! Activating premium...");

                    await ProcessSuccessfulPaymentFromWebhook(payment, webhookData);

                    _logger.LogInformation($"[Webhook Handler] ✅ Payment {orderCode} processed successfully");
                    return true;
                }
                else
                {
                    _logger.LogWarning($"[Webhook Handler] Payment failed with code: {webhookData.Data.Code}");

                    await _paymentRepo.UpdatePaymentStatusAsync(
                        payment.Id,
                        "Failed",
                        webhookData.Data.Reference,
                        System.Text.Json.JsonSerializer.Serialize(webhookData)
                    );

                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Webhook Handler] Exception occurred");
                return false;
            }
        }

        // ✅ Process successful payment from webhook data
        private async Task ProcessSuccessfulPaymentFromWebhook(Payment payment, PayOSWebhookRequest webhookData)
        {
            if (payment.Status == "Completed")
            {
                return;
            }

            _logger.LogInformation($"[Process Payment] Processing payment {payment.Id} from webhook");

            // ✅ Determine plan type
            string planType = payment.Amount >= 500000 ? "yearly" : "monthly";

            var plans = await _planRepo.GetAllAsync();
            var plan = plans.FirstOrDefault(p => p.PlanType.ToLower() == planType && p.IsActive == true);

            if (plan == null)
            {
                _logger.LogWarning($"[Process Payment] No active plan found for type: {planType}");
                return;
            }

            _logger.LogInformation($"[Process Payment] Found plan: {plan.PlanName} (ID: {plan.Id})");

            // ✅ Create or renew subscription
            var subscription = await _subscriptionRepo.CreateOrRenewSubscriptionAsync(
                payment.UserId,
                plan.Id,
                isRenewal: false
            );

            _logger.LogInformation($"[Process Payment] ✅ Subscription created: {subscription.Id}");

            // ✅ Update payment
            payment.SubscriptionId = subscription.Id;

            await _paymentRepo.UpdatePaymentStatusAsync(
                payment.Id,
                "Completed",
                webhookData.Data.Reference,
                System.Text.Json.JsonSerializer.Serialize(webhookData)
            );

            _logger.LogInformation($"[Process Payment] ✅ Payment {payment.Id} completed. Subscription {subscription.Id} activated.");
        }

        private async Task ProcessSuccessfulPaymentAsync(Payment payment, dynamic paymentInfo)
        {
            if (payment.Status == "Completed")
            {
                return;
            }

            _logger.LogInformation($"[Process Payment] Processing payment {payment.Id}");

            // ✅ Determine plan type
            string planType = payment.Amount >= 500000 ? "yearly" : "monthly";

            var plans = await _planRepo.GetAllAsync();
            var plan = plans.FirstOrDefault(p => p.PlanType.ToLower() == planType && p.IsActive == true);

            if (plan == null)
            {
                _logger.LogWarning($"[Process Payment] No active plan found for type: {planType}");
                return;
            }

            _logger.LogInformation($"[Process Payment] Found plan: {plan.PlanName} (ID: {plan.Id})");

            // ✅ Create or renew subscription
            var subscription = await _subscriptionRepo.CreateOrRenewSubscriptionAsync(
                payment.UserId,
                plan.Id,
                isRenewal: false
            );

            _logger.LogInformation($"[Process Payment] ✅ Subscription created: {subscription.Id}");

            // ✅ Extract transaction ID
            string transactionId = null;
            try
            {
                var transactions = TryGetProperty(paymentInfo, "Transactions");
                if (transactions != null)
                {
                    // Try to get first transaction reference
                    var transactionsEnumerable = transactions as IEnumerable<dynamic>;
                    if (transactionsEnumerable != null && transactionsEnumerable.Any())
                    {
                        var firstTransaction = transactionsEnumerable.First();
                        transactionId = TryGetProperty(firstTransaction, "Reference");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failed to extract transaction ID: {ex.Message}");
            }

            // ✅ Update payment
            payment.SubscriptionId = subscription.Id;

            await _paymentRepo.UpdatePaymentStatusAsync(
                payment.Id,
                "Completed",
                transactionId,
                System.Text.Json.JsonSerializer.Serialize(paymentInfo)
            );

            _logger.LogInformation($"[Process Payment] ✅ Payment {payment.Id} completed. Subscription {subscription.Id} activated.");
        }

        // ✅ Verify webhook signature
        public bool VerifyWebhookSignature(string webhookBody, string signature)
        {
            try
            {
                var isValid = _payOSService.VerifyWebhookData(webhookBody);
                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Webhook verification failed: {ex.Message}");
                return false;
            }
        }

        public async Task<List<PaymentHistoryResponse>> GetPaymentHistoryAsync(Guid userId)
        {
            var payments = await _paymentRepo.GetPaymentsByUserIdAsync(userId);

            return payments.Select(p => new PaymentHistoryResponse
            {
                PaymentId = p.Id,
                Amount = p.Amount,
                Currency = p.Currency,
                Status = p.Status,
                PaymentType = p.PaymentType,
                PaymentDate = p.PaymentDate,
                Description = p.Description,
                OrderCode = long.Parse(p.PayOsorderId ?? "0"),
                // ✅ NEW: Include CreatedAt and UpdatedAt
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList();
        }

        public async Task<bool> CancelPaymentAsync(Guid userId, long orderCode)
        {
            try
            {
                var payment = await _paymentRepo.GetPaymentByOrderCodeAsync(orderCode.ToString());

                if (payment == null || payment.UserId != userId)
                {
                    return false;
                }

                if (payment.Status != "Pending")
                {
                    return false;
                }

                // ✅ FIX: Get paymentLinkId from payment
                var paymentLinkId = payment.PayOstransactionId;
                if (string.IsNullOrEmpty(paymentLinkId))
                {
                    return false;
                }

                await _payOSService.CancelPaymentLinkAsync(paymentLinkId);
                await _paymentRepo.UpdatePaymentStatusAsync(payment.Id, "Cancelled");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to cancel payment {orderCode}");
                return false;
            }
        }

        public async Task<PaymentDetailsDto> GetPaymentDetailsAsync(Guid paymentId)
        {
            var payment = await _paymentRepo.GetPaymentWithDetailsAsync(paymentId);

            if (payment == null)
            {
                throw new KeyNotFoundException("Payment not found");
            }

            return MapToPaymentDetailsDto(payment); // ✅ No await here
        }


        private PaymentDetailsDto MapToPaymentDetailsDto(Payment payment)
        {
            var dto = new PaymentDetailsDto
            {
                PaymentId = payment.Id,
                OrderCode = long.Parse(payment.PayOsorderId),
                TransactionId = payment.PayOstransactionId,
                Amount = payment.Amount,
                Currency = payment.Currency,
                Status = payment.Status,
                PaymentDate = payment.PaymentDate ?? payment.CreatedAt,
                Description = payment.Description
            };

            if (payment.Subscription != null)
            {
                dto.Subscription = new SubscriptionDetailsDto
                {
                    SubscriptionId = payment.Subscription.Id,
                    PlanName = payment.Subscription.Plan?.PlanName,
                    PlanType = payment.Subscription.Plan?.PlanType,
                    StartDate = payment.Subscription.StartDate,
                    EndDate = payment.Subscription.EndDate ?? DateTime.MinValue,
                    IsActive = payment.Subscription.IsActive ?? false
                };
            }

            return dto;
        }

        public async Task<VerifyPaymentResponse> VerifyPaymentAsync(long orderCode)
        {
            try
            {
                _logger.LogInformation($"[Verify Payment] Verifying orderCode: {orderCode}");

                var payment = await _paymentRepo.GetPaymentByOrderCodeAsync(orderCode.ToString());
                if (payment == null)
                {
                    return new VerifyPaymentResponse
                    {
                        Success = false,
                        Message = "Payment not found"
                    };
                }

                _logger.LogInformation($"[Verify Payment] Found payment: {payment.Id}, Status: {payment.Status}");

                // ✅ If already completed, return success
                if (payment.Status == "Completed")
                {
                    return new VerifyPaymentResponse
                    {
                        Success = true,
                        Message = "Payment already verified and completed",
                        PaymentDetails = MapToPaymentDetailsDto(payment)
                    };
                }

                // ✅ Get payment info from PayOS
                var paymentLinkId = payment.PayOstransactionId;
                if (string.IsNullOrEmpty(paymentLinkId))
                {
                    return new VerifyPaymentResponse
                    {
                        Success = false,
                        Message = "Payment link ID not found"
                    };
                }

                _logger.LogInformation($"[Verify Payment] Checking PayOS status for: {paymentLinkId}");

                var paymentInfo = await _payOSService.GetPaymentInfoAsync(paymentLinkId);

                if (paymentInfo == null)
                {
                    return new VerifyPaymentResponse
                    {
                        Success = false,
                        Message = "Could not retrieve payment info from PayOS"
                    };
                }

                // ✅ Extract status
                var status = TryGetProperty(paymentInfo, "Status") ?? TryGetProperty(paymentInfo, "status");
                _logger.LogInformation($"[Verify Payment] PayOS status: {status}");

                // ✅ Check if paid
                if (status?.ToUpper() == "PAID")
                {
                    _logger.LogInformation($"[Verify Payment] ✅ Payment is PAID. Processing...");

                    await ProcessSuccessfulPaymentAsync(payment, paymentInfo);

                    return new VerifyPaymentResponse
                    {
                        Success = true,
                        Message = "Payment verified successfully",
                        PaymentDetails = MapToPaymentDetailsDto(payment)
                    };
                }
                else if (status?.ToUpper() == "CANCELLED")
                {
                    await _paymentRepo.UpdatePaymentStatusAsync(payment.Id, "Cancelled");

                    return new VerifyPaymentResponse
                    {
                        Success = false,
                        Message = "Payment was cancelled"
                    };
                }
                else
                {
                    return new VerifyPaymentResponse
                    {
                        Success = false,
                        Message = $"Payment status: {status}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to verify payment {orderCode}");
                return new VerifyPaymentResponse
                {
                    Success = false,
                    Message = $"Verification failed: {ex.Message}"
                };
            }
        }
    }
}