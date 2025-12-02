using EventLink_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventLink_Repositories.Interface
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        /// <summary>
        /// Get payment by OrderCode from PayOS
        /// </summary>
        Task<Payment> GetPaymentByOrderCodeAsync(string orderCode);

        /// <summary>
        /// Get payment by TransactionId from PayOS
        /// </summary>
        Task<Payment> GetPaymentByTransactionIdAsync(string transactionId);

        /// <summary>
        /// Get all payments for a user
        /// </summary>
        Task<List<Payment>> GetPaymentsByUserIdAsync(Guid userId);

        /// <summary>
        /// Get successful payments for a user
        /// </summary>
        Task<List<Payment>> GetSuccessfulPaymentsByUserIdAsync(Guid userId);

        /// <summary>
        /// Get pending payments for a user
        /// </summary>
        Task<List<Payment>> GetPendingPaymentsByUserIdAsync(Guid userId);

        /// <summary>
        /// Get payment by ID with related data
        /// </summary>
        Task<Payment> GetPaymentWithDetailsAsync(Guid paymentId);

        /// <summary>
        /// Check if order code already exists
        /// </summary>
        Task<bool> OrderCodeExistsAsync(string orderCode);

        /// <summary>
        /// Update payment status
        /// </summary>
        Task UpdatePaymentStatusAsync(Guid paymentId, string status, string transactionId = null, string gatewayResponse = null);

        /// <summary>
        /// Get all payments (for admin revenue report)
        /// </summary>
        Task<List<Payment>> GetAllPaymentsAsync();

        /// <summary>
        /// Get payments by date range
        /// </summary>
        Task<List<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Get completed payments for revenue calculation
        /// </summary>
        Task<List<Payment>> GetCompletedPaymentsAsync();
    }
}