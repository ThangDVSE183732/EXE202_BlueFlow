using Eventlink_Services.Interface;
using Eventlink_Services.Request;
using Eventlink_Services.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EventLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            IPaymentService paymentService,
            ISubscriptionService subscriptionService,
            ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        /// <summary>
        /// POST: api/Payment/create-premium-payment
        /// </summary>
        [HttpPost("create-premium-payment")]
        public async Task<IActionResult> CreatePremiumPayment([FromBody] CreatePremiumPaymentRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid request",
                        errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

                var result = await _paymentService.CreatePremiumPaymentAsync(userId.Value, request);

                // ✅ Check result.Success and return appropriate status code
                if (!result.Success)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result.Message
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = result.Message,
                    data = new
                    {
                        checkoutUrl = result.CheckoutUrl,
                        orderCode = result.OrderCode,
                        qrCodeUrl = result.QrCodeUrl
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating premium payment");
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }


        /// <summary>
        /// POST: api/Payment/webhook
        /// PayOS sẽ gọi endpoint này khi thanh toán thành công
        /// </summary>
        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentWebhook()
        {
            try
            {
                // ✅ Read raw body
                using var reader = new StreamReader(Request.Body);
                var webhookBody = await reader.ReadToEndAsync();

                _logger.LogInformation($"[Webhook] Received webhook: {webhookBody}");

                if (string.IsNullOrEmpty(webhookBody))
                {
                    _logger.LogWarning("[Webhook] Empty webhook body");
                    return BadRequest(new { success = false, message = "Empty webhook body" });
                }

                // ✅ Parse webhook data
                var webhookData = System.Text.Json.JsonSerializer.Deserialize<PayOSWebhookRequest>(webhookBody);

                if (webhookData == null)
                {
                    _logger.LogWarning("[Webhook] Failed to parse webhook data");
                    return BadRequest(new { success = false, message = "Invalid webhook data" });
                }

                _logger.LogInformation($"[Webhook] OrderCode: {webhookData.Data?.OrderCode}, Code: {webhookData.Data?.Code}");

                // ✅ Verify webhook signature (optional but recommended)
                var signature = Request.Headers["x-signature"].FirstOrDefault();
                if (!string.IsNullOrEmpty(signature))
                {
                    var isValid = _paymentService.VerifyWebhookSignature(webhookBody, signature);
                    if (!isValid)
                    {
                        _logger.LogWarning("[Webhook] Invalid signature");
                        return BadRequest(new { success = false, message = "Invalid signature" });
                    }
                }

                // ✅ Process payment webhook
                var result = await _paymentService.HandlePaymentWebhookAsync(webhookData);

                if (result)
                {
                    _logger.LogInformation("[Webhook] ✅ Processed successfully");
                    return Ok(new { success = true, message = "Webhook processed successfully" });
                }

                _logger.LogWarning("[Webhook] ❌ Processing failed");
                return BadRequest(new { success = false, message = "Webhook processing failed" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Webhook] Exception occurred");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// GET: api/Payment/history
        /// </summary>
        [HttpGet("history")]
        public async Task<ActionResult<List<PaymentHistoryResponse>>> GetPaymentHistory()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                var history = await _paymentService.GetPaymentHistoryAsync(userId.Value);

                return Ok(new
                {
                    success = true,
                    message = "Payment history retrieved successfully",
                    data = history,
                    count = history.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment history");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// POST: api/Payment/cancel
        /// </summary>
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelPayment([FromQuery] long orderCode)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                var result = await _paymentService.CancelPaymentAsync(userId.Value, orderCode);

                if (result)
                {
                    return Ok(new { success = true, message = "Payment cancelled successfully" });
                }

                return BadRequest(new { success = false, message = "Failed to cancel payment" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling payment {orderCode}");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// GET: api/Payment/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDetailsDto>> GetPaymentDetails(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                var details = await _paymentService.GetPaymentDetailsAsync(id);

                if (details == null)
                {
                    return NotFound(new { success = false, message = "Payment not found" });
                }

                return Ok(new
                {
                    success = true,
                    message = "Payment details retrieved successfully",
                    data = details
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Payment not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving payment details {id}");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }


        /// <summary>
        /// GET: api/Payment/subscription-plans
        /// </summary>
        [HttpGet("subscription-plans")]
        [AllowAnonymous]
        public async Task<ActionResult<List<SubscriptionPlanDto>>> GetSubscriptionPlans([FromQuery] string role = null)
        {
            try
            {
                var plans = await _subscriptionService.GetAvailablePlansAsync(role);

                return Ok(new
                {
                    success = true,
                    message = "Subscription plans retrieved successfully",
                    data = plans,
                    count = plans.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plans");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return Guid.TryParse(userIdClaim, out Guid userId) ? userId : null;
        }

        /// <summary>
        /// GET: api/Payment/verify?orderCode={orderCode}
        /// Kiểm tra trạng thái thanh toán và kích hoạt premium nếu chưa
        /// </summary>
        [HttpGet("verify")]
        public async Task<IActionResult> VerifyPayment([FromQuery] long orderCode)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                if (orderCode <= 0)
                {
                    return BadRequest(new { success = false, message = "Invalid order code" });
                }

                _logger.LogInformation($"[Verify] User {userId} verifying payment {orderCode}");

                var result = await _paymentService.VerifyPaymentAsync(orderCode);

                if (result.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = result.Message,
                        data = result.PaymentDetails
                    });
                }

                return BadRequest(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error verifying payment {orderCode}");
                return StatusCode(500, new { success = false, message = "Verification failed" });
            }
        }

        /// <summary>
        /// GET: api/Payment/premium-status
        /// Kiểm tra user có premium không
        /// </summary>
        [HttpGet("premium-status")]
        public async Task<IActionResult> GetPremiumStatus()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                var status = await _subscriptionService.GetPremiumStatusAsync(userId.Value);

                return Ok(new
                {
                    success = true,
                    message = "Premium status retrieved successfully",
                    data = status
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "User not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving premium status");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// GET: api/Payment/check-premium
        /// Simple check: user có premium không (true/false)
        /// </summary>
        [HttpGet("check-premium")]
        public async Task<IActionResult> CheckPremium()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                var hasPremium = await _subscriptionService.HasActivePremiumAsync(userId.Value);

                return Ok(new
                {
                    success = true,
                    message = hasPremium ? "User has active premium" : "User does not have premium",
                    data = new { hasPremium }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking premium status");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
    }
}