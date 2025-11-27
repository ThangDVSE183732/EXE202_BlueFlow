using Eventlink_Services.Request;
using Eventlink_Services.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eventlink_Services.Interface
{
    /// <summary>
    /// Service for payment management
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// Create premium subscription payment
        /// </summary>
        Task<CreatePaymentResponse> CreatePremiumPaymentAsync(Guid userId, CreatePremiumPaymentRequest request);

        /// <summary>
        /// Verify payment after redirect from PayOS
        /// </summary>
        Task<VerifyPaymentResponse> VerifyPaymentAsync(long orderCode);

        /// <summary>
        /// Handle webhook from PayOS
        /// </summary>
        Task<bool> HandlePaymentWebhookAsync(PayOSWebhookRequest webhookData);

        /// <summary>
        /// Verify webhook signature
        /// </summary>
        bool VerifyWebhookSignature(string webhookBody, string signature);

        /// <summary>
        /// Get payment history for user
        /// </summary>
        Task<List<PaymentHistoryResponse>> GetPaymentHistoryAsync(Guid userId);

        /// <summary>
        /// Cancel pending payment
        /// </summary>
        Task<bool> CancelPaymentAsync(Guid userId, long orderCode);

        /// <summary>
        /// Get payment details
        /// </summary>
        Task<PaymentDetailsDto> GetPaymentDetailsAsync(Guid paymentId);
    }
}