using EventLink_Repositories.DBContext;
using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventLink_Repositories.Repository
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        private readonly EventLinkDBContext _context;

        public PaymentRepository(EventLinkDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Payment> GetPaymentByOrderCodeAsync(string orderCode)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Subscription)
                    .ThenInclude(s => s.Plan)
                .FirstOrDefaultAsync(p => p.PayOsorderId == orderCode);
        }

        public async Task<Payment> GetPaymentByTransactionIdAsync(string transactionId)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Subscription)
                    .ThenInclude(s => s.Plan)
                .FirstOrDefaultAsync(p => p.PayOstransactionId == transactionId);
        }

        public async Task<List<Payment>> GetPaymentsByUserIdAsync(Guid userId)
        {
            return await _context.Payments
                .Include(p => p.Subscription)
                    .ThenInclude(s => s.Plan)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetSuccessfulPaymentsByUserIdAsync(Guid userId)
        {
            return await _context.Payments
                .Include(p => p.Subscription)
                    .ThenInclude(s => s.Plan)
                .Where(p => p.UserId == userId && p.Status == "Completed")
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetPendingPaymentsByUserIdAsync(Guid userId)
        {
            return await _context.Payments
                .Include(p => p.Subscription)
                    .ThenInclude(s => s.Plan)
                .Where(p => p.UserId == userId && p.Status == "Pending")
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Payment> GetPaymentWithDetailsAsync(Guid paymentId)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Subscription)
                    .ThenInclude(s => s.Plan)
                .FirstOrDefaultAsync(p => p.Id == paymentId);
        }

        public async Task<bool> OrderCodeExistsAsync(string orderCode)
        {
            return await _context.Payments
                .AnyAsync(p => p.PayOsorderId == orderCode);
        }

        public async Task UpdatePaymentStatusAsync(Guid paymentId, string status, string transactionId = null, string gatewayResponse = null)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment != null)
            {
                payment.Status = status;
                payment.UpdatedAt = DateTime.UtcNow;

                if (status == "Completed")
                {
                    payment.PaymentDate = DateTime.UtcNow;
                }

                if (!string.IsNullOrEmpty(transactionId))
                {
                    payment.PayOstransactionId = transactionId;
                }

                if (!string.IsNullOrEmpty(gatewayResponse))
                {
                    payment.PaymentGatewayResponse = gatewayResponse;
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}