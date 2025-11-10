using System;

namespace Eventlink_Services.Response
{
    /// <summary>
    /// Response after creating payment link
    /// </summary>
    public class CreatePaymentResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string CheckoutUrl { get; set; } // URL to redirect user to PayOS
        public long OrderCode { get; set; }
        public string QrCodeUrl { get; set; } // QR code for payment (optional)
    }

    /// <summary>
    /// Response for payment verification
    /// </summary>
    public class VerifyPaymentResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public PaymentDetailsDto PaymentDetails { get; set; }
    }

    public class PaymentDetailsDto
    {
        public Guid PaymentId { get; set; }
        public long OrderCode { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string Description { get; set; }
        public SubscriptionDetailsDto Subscription { get; set; }
    }

    public class SubscriptionDetailsDto
    {
        public Guid SubscriptionId { get; set; }
        public string PlanName { get; set; }
        public string PlanType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Response for premium status check
    /// </summary>
    public class PremiumStatusResponse
    {
        public bool IsPremium { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int DaysRemaining { get; set; }
        public string PlanType { get; set; }
        public bool AutoRenew { get; set; }
        public ActiveSubscriptionDto ActiveSubscription { get; set; }
    }

    public class ActiveSubscriptionDto
    {
        public Guid SubscriptionId { get; set; }
        public string PlanName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal MonthlyPrice { get; set; }
    }

    /// <summary>
    /// Response for payment history
    /// </summary>
    public class PaymentHistoryResponse
    {
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string PaymentType { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string Description { get; set; }
        public long OrderCode { get; set; }
        
        // ✅ NEW: Track payment creation and updates
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}