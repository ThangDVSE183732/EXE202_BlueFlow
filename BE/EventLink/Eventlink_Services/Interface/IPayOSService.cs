using System.Threading.Tasks;

namespace Eventlink_Services.Interface
{
    public interface IPayOSService
    {
        // ✅ Return dynamic để access any properties
        Task<dynamic> CreatePaymentLinkAsync(
            long orderCode,
            long amount,
            string description,
            string buyerName,
            string buyerEmail,
            string returnUrl = null,
            string cancelUrl = null);

        Task<dynamic> GetPaymentInfoAsync(string paymentLinkId);

        Task<dynamic> CancelPaymentLinkAsync(string paymentLinkId, string cancellationReason = null);

        bool VerifyWebhookData(string webhookBody);

        long GenerateOrderCode();
    }
}