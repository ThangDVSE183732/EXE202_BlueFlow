using System;
using System.Collections.Generic;

namespace Eventlink_Services.Response
{
    /// <summary>
    /// Revenue Report Response for Admin Dashboard
    /// </summary>
    public class RevenueReportResponse
    {
        public decimal TotalRevenue { get; set; }
        public decimal Growth { get; set; }
        public int TotalTransactions { get; set; }
        public decimal AverageOrder { get; set; }
        public List<MonthlyRevenueData> MonthlyData { get; set; }
        public List<RevenueSourceData> TopRevenueSources { get; set; }
        public List<RecentTransactionData> RecentTransactions { get; set; }
    }

    public class MonthlyRevenueData
    {
        public string Month { get; set; }
        public decimal Revenue { get; set; }
        public int Transactions { get; set; }
    }

    public class RevenueSourceData
    {
        public string Source { get; set; }
        public decimal Revenue { get; set; }
        public decimal Percentage { get; set; }
    }

    public class RecentTransactionData
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Customer { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string PaymentType { get; set; }
    }
}
