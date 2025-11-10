using System;
using System.ComponentModel.DataAnnotations;

namespace Eventlink_Services.Request
{
    /// <summary>
    /// Request to create premium subscription payment
    /// </summary>
    public class CreatePremiumPaymentRequest
    {
        [Required(ErrorMessage = "Plan type is required")]
        [RegularExpression("^(monthly|yearly)$", ErrorMessage = "Plan type must be 'monthly' or 'yearly'")]
        public string PlanType { get; set; } // "monthly" or "yearly"

        public string ReturnUrl { get; set; } // Optional: override default return URL

        public string CancelUrl { get; set; } // Optional: override default cancel URL
    }

    /// <summary>
    /// Request to verify payment after redirect from PayOS
    /// </summary>
    public class VerifyPaymentRequest
    {
        [Required]
        public long OrderCode { get; set; }
    }

    /// <summary>
    /// Webhook data from PayOS
    /// </summary>
    public class PayOSWebhookRequest
    {
        public string Code { get; set; }
        public string Desc { get; set; }
        public bool Success { get; set; }
        public PayOSWebhookData Data { get; set; }
        public string Signature { get; set; }
    }

    public class PayOSWebhookData
    {
        public long OrderCode { get; set; }
        public long Amount { get; set; }
        public string Description { get; set; }
        public string AccountNumber { get; set; }
        public string Reference { get; set; }
        public string TransactionDateTime { get; set; }
        public string Currency { get; set; }
        public string PaymentLinkId { get; set; }
        public string Code { get; set; }
        public string Desc { get; set; }
        public string CounterAccountBankId { get; set; }
        public string CounterAccountBankName { get; set; }
        public string CounterAccountName { get; set; }
        public string CounterAccountNumber { get; set; }
        public string VirtualAccountName { get; set; }
        public string VirtualAccountNumber { get; set; }
    }
}