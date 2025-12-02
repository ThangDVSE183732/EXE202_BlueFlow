import React, { useState, useEffect } from 'react';
import { TrendingUp, TrendingDown, DollarSign, Users, ShoppingBag, Activity, Calendar, Download, Filter, ArrowUpRight, MoreVertical, RefreshCw } from 'lucide-react';
import paymentService from '../../services/paymentService';
import { authService } from '../../services/userService';

const RevenueReport = () => {
  const [selectedYear, setSelectedYear] = useState(new Date().getFullYear());
  const [revenueData, setRevenueData] = useState(null);
  const [usersData, setUsersData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Color mapping for different revenue sources
  const colorMap = {
    'Premium Subscription': 'from-blue-500 to-cyan-500',
    'Event Subscriptions': 'from-purple-500 to-pink-500',
    'Premium Features': 'from-orange-500 to-red-500',
    'Partnership Fees': 'from-green-500 to-emerald-500',
    'Other Services': 'from-yellow-500 to-amber-500',
  };

  // Fetch revenue report data
  const fetchRevenueData = async () => {
    try {
      setLoading(true);
      setError(null);
      
      // Fetch revenue data
      const response = await paymentService.getRevenueReport(selectedYear);
      
      if (response.success) {
        // Map revenue sources with colors
        const mappedSources = response.data.topRevenueSources.map(source => ({
          ...source,
          color: colorMap[source.source] || 'from-gray-500 to-slate-500'
        }));

        setRevenueData({
          ...response.data,
          topRevenueSources: mappedSources
        });
      } else {
        setError(response.message || 'Failed to fetch revenue data');
      }

      // Fetch users data
      const usersResponse = await authService.getAllUsers();
      if (usersResponse.success) {
        setUsersData(usersResponse.data);
      }
    } catch (err) {
      console.error('Error fetching data:', err);
      setError(err.message || 'An error occurred while fetching data');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchRevenueData();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [selectedYear]);

  const formatCurrency = (amount) => {
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND',
      minimumFractionDigits: 0,
    }).format(amount);
  };

  const formatNumber = (num) => {
    return new Intl.NumberFormat('vi-VN').format(num);
  };

  const formatDate = (dateString) => {
    const date = new Date(dateString);
    return date.toLocaleDateString('vi-VN');
  };

  if (loading) {
    return (
      <div className="h-screen bg-gray-50 px-8 flex items-center justify-center">
        <div className="text-center">
          <RefreshCw className="animate-spin text-blue-500 mx-auto mb-2" size={32} />
          <p className="text-sm text-gray-600">Loading revenue report...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="h-screen bg-gray-50 px-8 flex items-center justify-center">
        <div className="text-center">
          <p className="text-sm text-red-600 mb-2">Error: {error}</p>
          <button 
            onClick={fetchRevenueData}
            className="px-4 py-2 bg-blue-500 text-white rounded-md text-sm hover:bg-blue-600"
          >
            Retry
          </button>
        </div>
      </div>
    );
  }

  if (!revenueData) {
    return (
      <div className="h-screen bg-gray-50 px-8 flex items-center justify-center">
        <p className="text-sm text-gray-600">No data available</p>
      </div>
    );
  }

  const maxRevenue = Math.max(...revenueData.monthlyData.map(d => d.revenue), 1);

  return (
    <div className="h-screen bg-gray-50 px-8 ">
      <div className="max-w-[1200px] mx-auto flex flex-col gap-1.5 pt-2">
        {/* Header Section - Centered title */}
        <div className="flex items-center justify-between flex-shrink-0">
          <div className="flex-1"></div>
          <div className="text-center">
            <h1 className="text-base font-bold text-gray-900 mb-0.5">Revenue Report</h1>
            <p className="text-[10px] text-gray-500">Track and analyze your revenue performance</p>
          </div>
          <div className="flex-1 flex justify-end items-center gap-2">
            <select
              value={selectedYear}
              onChange={(e) => setSelectedYear(parseInt(e.target.value))}
              className="text-[10px] border border-gray-300 rounded px-2 py-1"
            >
              {[...Array(5)].map((_, i) => {
                const year = new Date().getFullYear() - i;
                return <option key={year} value={year}>{year}</option>;
              })}
            </select>
            <button 
              onClick={fetchRevenueData}
              className="p-1 hover:bg-gray-200 rounded"
              title="Refresh"
            >
              <RefreshCw size={12} />
            </button>
          </div>
        </div>

        {/* KPI Cards - Horizontal Layout */}
        <div className="grid grid-cols-4 gap-1.5 flex-shrink-0">
          {/* Total Revenue - Premium Card */}
          <div className="relative bg-gradient-to-br from-blue-600 via-blue-500 to-cyan-500 rounded-md shadow-lg p-2 text-white overflow-hidden">
            <div className="absolute top-0 right-0 w-12 h-12 bg-white/10 rounded-full -mr-6 -mt-6"></div>
            <div className="absolute bottom-0 left-0 w-10 h-10 bg-white/5 rounded-full -ml-5 -mb-5"></div>
            <div className="relative z-10">
              <div className="flex items-center justify-between mb-1">
                <div className="bg-white/20 backdrop-blur-md rounded p-1 shadow-sm">
                  <DollarSign size={12} />
                </div>
                <div className="flex items-center gap-0.5 bg-white/20 backdrop-blur-md px-1 py-0.5 rounded-full">
                  <TrendingUp size={8} />
                  <span className="text-[9px] font-semibold">{revenueData.growth}%</span>
                </div>
              </div>
              <h3 className="text-blue-100 text-[9px] font-medium mb-0.5">Total Revenue</h3>
              <p className="text-base font-bold mb-0.5">{formatCurrency(revenueData.totalRevenue)}</p>
              <p className="text-blue-100 text-[8px]">vs previous period</p>
            </div>
          </div>

          {/* Total Transactions */}
          <div className="bg-white rounded-md shadow-sm border border-gray-200 p-2 hover:shadow-md transition-shadow">
            <div className="flex items-center justify-between mb-1">
              <div className="bg-gradient-to-br from-green-500 to-emerald-500 rounded p-1 shadow-sm">
                <Users className="text-white" size={12} />
              </div>
              <div className="flex items-center gap-0.5 text-green-600 bg-green-50 px-1 py-0.5 rounded-full">
                <ArrowUpRight size={8} />
                <span className="text-[9px] font-semibold">8.2%</span>
              </div>
            </div>
            <h3 className="text-gray-500 text-[9px] font-medium mb-0.5">Total Transactions</h3>
            <p className="text-base font-bold text-gray-900 mb-0.5">{formatNumber(revenueData.totalTransactions)}</p>
            <p className="text-gray-400 text-[8px]">Active transactions</p>
          </div>

          {/* Average Order Value */}
          <div className="bg-white rounded-md shadow-sm border border-gray-200 p-2 hover:shadow-md transition-shadow">
            <div className="flex items-center justify-between mb-1">
              <div className="bg-gradient-to-br from-purple-500 to-pink-500 rounded p-1 shadow-sm">
                <ShoppingBag className="text-white" size={12} />
              </div>
              <div className="flex items-center gap-0.5 text-purple-600 bg-purple-50 px-1 py-0.5 rounded-full">
                <ArrowUpRight size={8} />
                <span className="text-[9px] font-semibold">5.1%</span>
              </div>
            </div>
            <h3 className="text-gray-500 text-[9px] font-medium mb-0.5">Avg Order Value</h3>
            <p className="text-base font-bold text-gray-900 mb-0.5">{formatCurrency(revenueData.averageOrder)}</p>
            <p className="text-gray-400 text-[8px]">Per transaction</p>
          </div>

          {/* Growth Rate */}
          <div className="bg-white rounded-md shadow-sm border border-gray-200 p-2 hover:shadow-md transition-shadow">
            <div className="flex items-center justify-between mb-1">
              <div className="bg-gradient-to-br from-orange-500 to-red-500 rounded p-1 shadow-sm">
                <Activity className="text-white" size={12} />
              </div>
              <div className="flex items-center gap-0.5 text-orange-600 bg-orange-50 px-1 py-0.5 rounded-full">
                <ArrowUpRight size={8} />
                <span className="text-[9px] font-semibold">12.5%</span>
              </div>
            </div>
            <h3 className="text-gray-500 text-[9px] font-medium mb-0.5">Growth Rate</h3>
            <p className="text-base font-bold text-gray-900 mb-0.5">{revenueData.growth}%</p>
            <p className="text-gray-400 text-[8px]">Month over month</p>
          </div>
        </div>

        {/* Main Content - All Transactions -> (Monthly Revenue + Revenue Sources) */}
        <div className="flex flex-col gap-1.5">
          {/* All Transactions (pic 3) */}
          <div className="bg-white rounded-md shadow-sm border border-gray-200 p-2 flex flex-col max-h-[200px]">
            <div className="flex items-center justify-between mb-1.5 flex-shrink-0">
              <h2 className="text-xs font-bold text-gray-900">All Transactions</h2>
              <span className="text-[9px] text-gray-500">
                {revenueData.recentTransactions.length} recent transactions
              </span>
            </div>
            <div className="overflow-auto flex-1">
              <table className="w-full text-[10px]">
                <thead className="sticky top-0 bg-white border-b border-gray-200">
                  <tr>
                    <th className="text-center py-1 px-1.5 font-semibold text-gray-600">Date</th>
                    <th className="text-center py-1 px-1.5 font-semibold text-gray-600">Customer</th>
                    <th className="text-center py-1 px-1.5 font-semibold text-gray-600">Email</th>
                    <th className="text-right py-1 px-1.5 font-semibold text-gray-600">Amount</th>
                    <th className="text-center py-1 px-1.5 font-semibold text-gray-600">Status</th>
                  </tr>
                </thead>
                <tbody>
                  {revenueData.recentTransactions.map((transaction) => (
                    <tr key={transaction.id} className="border-b border-gray-100 hover:bg-gray-50 transition-colors">
                      <td className="py-1 px-1.5 text-gray-700">{formatDate(transaction.date)}</td>
                      <td
                        className="py-1 px-1.5 text-gray-900 font-medium truncate max-w-[120px]"
                        title={transaction.customer}
                      >
                        {transaction.customer}
                      </td>
                      <td
                        className="py-1 px-1.5 text-gray-600 truncate max-w-[150px]"
                        title={transaction.email}
                      >
                        {transaction.email}
                      </td>
                      <td className="py-1 px-1.5 text-gray-900 font-semibold text-right">
                        {formatCurrency(transaction.amount)}
                      </td>
                      <td className="py-1 px-1.5 text-center">
                        <span
                          className={`inline-flex items-center px-1 py-0.5 rounded-full text-[9px] font-semibold ${
                            transaction.status === 'Completed'
                              ? 'bg-green-50 text-green-700'
                              : 'bg-yellow-50 text-yellow-700'
                          }`}
                        >
                          {transaction.status}
                        </span>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>

          {/* Monthly Revenue + Revenue Sources side-by-side (pic 2 layout) */}
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-1.5">
            {/* Monthly Revenue */}
            <div className="bg-white rounded-md shadow-sm border border-gray-200 p-2 flex flex-col">
              <div className="flex items-center justify-between mb-1.5 flex-shrink-0">
                <h2 className="text-xs font-bold text-gray-900">Monthly Revenue</h2>
                <div className="flex items-center gap-1">
                  <Calendar className="text-gray-400" size={10} />
                  <button className="text-gray-400 hover:text-gray-600">
                    <MoreVertical size={10} />
                  </button>
                </div>
              </div>
              <div className="space-y-1">
                {revenueData.monthlyData.map((data, index) => (
                  <div key={index} className="flex items-center gap-1.5">
                    <div className="w-7 text-[9px] font-semibold text-gray-600">{data.month}</div>
                    <div className="flex-1 relative group">
                    <div className="h-2.5 bg-gray-100 rounded overflow-hidden">
                        <div
                          className="h-full bg-gradient-to-r from-blue-500 via-blue-400 to-cyan-400 rounded transition-all duration-500 group-hover:from-blue-600 group-hover:via-blue-500 group-hover:to-cyan-500"
                          style={{ width: `${(data.revenue / maxRevenue) * 100}%` }}
                        />
                      </div>
                      <div className="absolute right-0.5 top-1/2 transform -translate-y-1/2 text-[8px] font-bold text-gray-700 bg-white/90 backdrop-blur-sm px-0.5 py-0.5 rounded shadow-sm">
                        {formatCurrency(data.revenue)}
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </div>

            {/* Revenue Sources */}
            <div className="bg-white rounded-md shadow-sm border border-gray-200 p-2 flex flex-col">
              <h2 className="text-xs font-bold text-gray-900 mb-1.5 flex-shrink-0">Revenue Sources</h2>
              <div className="space-y-1.5">
                {revenueData.topRevenueSources.map((source, index) => (
                  <div key={index} className="space-y-0.5">
                    <div className="flex items-center justify-between">
                      <span className="text-[10px] font-semibold text-gray-800">{source.source}</span>
                      <span className="text-[10px] font-bold text-gray-900">{formatCurrency(source.revenue)}</span>
                    </div>
                    <div className="h-1.5 bg-gray-100 rounded-full overflow-hidden">
                      <div
                        className={`h-full bg-gradient-to-r ${source.color} rounded-full transition-all duration-500 shadow-sm`}
                        style={{ width: `${source.percentage}%` }}
                      />
                    </div>
                    <div className="flex items-center justify-between">
                      <span className="text-[8px] text-gray-500">{source.percentage}% of total revenue</span>
                      <span className="text-[8px] font-semibold text-gray-600">{source.percentage}%</span>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>

        {/* All Users Table */}
        <div className="bg-white rounded-md shadow-sm border border-gray-200 p-2 flex flex-col max-h-[200px]">
          <div className="flex items-center justify-between mb-1.5 flex-shrink-0">
            <h2 className="text-xs font-bold text-gray-900">All Users</h2>
            <span className="text-[9px] text-gray-500">
              {usersData.length} total users
            </span>
          </div>
          <div className="overflow-auto flex-1">
            <table className="w-full text-[10px]">
              <thead className="sticky top-0 bg-white border-b border-gray-200">
                <tr>
                  <th className="text-left py-1 px-1.5 font-semibold text-gray-600">Full Name</th>
                  <th className="text-left py-1 px-1.5 font-semibold text-gray-600">Email</th>
                  <th className="text-center py-1 px-1.5 font-semibold text-gray-600">Phone</th>
                  <th className="text-center py-1 px-1.5 font-semibold text-gray-600">Role</th>
                  <th className="text-center py-1 px-1.5 font-semibold text-gray-600">Joined Date</th>
                  <th className="text-center py-1 px-1.5 font-semibold text-gray-600">Status</th>
                </tr>
              </thead>
              <tbody>
                {usersData.map((user) => (
                  <tr key={user.userId} className="border-b border-gray-100 hover:bg-gray-50 transition-colors">
                    <td
                      className="py-1 px-1.5 text-gray-900 font-medium truncate max-w-[150px]"
                      title={user.fullName}
                    >
                      {user.fullName}
                    </td>
                    <td
                      className="py-1 px-1.5 text-gray-600 truncate max-w-[180px]"
                      title={user.email}
                    >
                      {user.email}
                    </td>
                    <td className="py-1 px-1.5 text-gray-700 text-center">
                      {user.phoneNumber || 'N/A'}
                    </td>
                    <td className="py-1 px-1.5 text-center">
                      <span
                        className={`inline-flex items-center px-1.5 py-0.5 rounded-full text-[9px] font-semibold ${
                          user.role === 'Admin'
                            ? 'bg-red-50 text-red-700'
                            : user.role === 'Organizer'
                            ? 'bg-blue-50 text-blue-700'
                            : user.role === 'Sponsor'
                            ? 'bg-purple-50 text-purple-700'
                            : 'bg-green-50 text-green-700'
                        }`}
                      >
                        {user.role}
                      </span>
                    </td>
                    <td className="py-1 px-1.5 text-gray-700 text-center">
                      {formatDate(user.createdAt)}
                    </td>
                    <td className="py-1 px-1.5 text-center">
                      <span
                        className={`inline-flex items-center px-1 py-0.5 rounded-full text-[9px] font-semibold ${
                          user.isActive
                            ? 'bg-green-50 text-green-700'
                            : 'bg-gray-50 text-gray-700'
                        }`}
                      >
                        {user.isActive ? 'Active' : 'Inactive'}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <style>{`
        /* Custom scrollbar */
        [style*="scrollbarWidth"]::-webkit-scrollbar {
          width: 6px;
        }
        [style*="scrollbarWidth"]::-webkit-scrollbar-track {
          background: #f1f5f9;
          border-radius: 3px;
        }
        [style*="scrollbarWidth"]::-webkit-scrollbar-thumb {
          background: #cbd5e1;
          border-radius: 3px;
        }
        [style*="scrollbarWidth"]::-webkit-scrollbar-thumb:hover {
          background: #94a3b8;
        }
      `}</style>
    </div>
  );
};

export default RevenueReport;
