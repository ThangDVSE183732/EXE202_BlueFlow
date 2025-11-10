import { useEffect, useState } from 'react';
import { usePayment } from '../hooks/usePayment';

const PaymentHistory = () => {
  const { fetchPaymentHistory, loading } = usePayment();
  const [payments, setPayments] = useState([]);

  useEffect(() => {
    loadPaymentHistory();
  }, []);

  const loadPaymentHistory = async () => {
    try {
      const data = await fetchPaymentHistory();
      setPayments(data?.data || []);
    } catch (error) {
      console.error('Error loading payment history:', error);
    }
  };

  const formatDate = (dateString) => {
    if (!dateString) return 'N/A';
    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    return `${day}/${month}/${year} ${hours}:${minutes}`;
  };

  const formatCurrency = (amount) => {
    if (!amount) return '0 VND';
    return new Intl.NumberFormat('vi-VN').format(amount) + ' VND';
  };

  const getStatusBadge = (status) => {
    const statusMap = {
      'PAID': { text: 'HOÀN THÀNH', color: 'bg-green-500' },
      'SUCCESS': { text: 'HOÀN THÀNH', color: 'bg-green-500' },
      'COMPLETED': { text: 'HOÀN THÀNH', color: 'bg-green-500' },
      'PENDING': { text: 'CHỜ XỬ LÝ', color: 'bg-yellow-500' },
      'FAILED': { text: 'ĐÃ HỦY', color: 'bg-red-500' },
      'CANCELLED': { text: 'ĐÃ HỦY', color: 'bg-red-500' }
    };

    const statusInfo = statusMap[status?.toUpperCase()] || { text: status || 'N/A', color: 'bg-gray-400' };

    return (
      <span className={`${statusInfo.color} text-white px-4 py-1.5 rounded-md text-xs font-semibold inline-block`}>
        {statusInfo.text}
      </span>
    );
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-[400px]">
        <div className="text-gray-500">Đang tải...</div>
      </div>
    );
  }

  return (
    <div className="w-full bg-white min-h-screen">
      {/* Header Section */}
      <div className="space-y-1 text-left">
            <h1 className="text-2xl font-semibold text-sky-500">Lịch sử giao dịch</h1>
            <h className="text-sm text-gray-400">Xem chi tiết các giao dịch với cập nhật trạng thái tức thời.</h>
            </div>
      <div className="h-px w-full bg-gray-300 mx-1 mb-3 mt-6" />    
      <div className="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden">
        {/* Header */}
        <div className="grid grid-cols-5 gap-4 px-6 py-4 bg-gray-50 border-b border-gray-200">
          <div className="font-semibold text-gray-700 text-sm">MÃ ĐƠN HÀNG</div>
          <div className="font-semibold text-gray-700 text-sm">DỊCH VỤ</div>
          <div className="font-semibold text-gray-700 text-sm">SỐ TIỀN</div>
          <div className="font-semibold text-gray-700 text-sm">TRẠNG THÁI</div>
          <div className="font-semibold text-gray-700 text-sm">THỜI GIAN</div>
        </div>

        {/* Body */}
        <div className="divide-y divide-gray-200 max-h-[400px] overflow-y-auto">
          {payments.length === 0 ? (
            <div className="px-6 py-12 text-center text-gray-500">
              Chưa có giao dịch nào
            </div>
          ) : (
            payments.map((payment, index) => (
              <div
                key={payment.id || index}
                className="grid grid-cols-5 gap-4 px-6 py-5 hover:bg-gray-50 transition-colors"
              >
                {/* Mã đơn hàng */}
                <div className="text-gray-900 font-medium text-sm">
                  {payment.orderCode || payment.id || 'N/A'}
                </div>

                {/* Dịch vụ */}
                <div className="text-gray-600 text-sm">
                  {payment.description || payment.planType || 'Premium Plan'}
                  
                </div>

                {/* Số tiền */}
                <div className="text-gray-900 font-medium text-sm">
                  {formatCurrency(payment.amount)}
                </div>

                {/* Trạng thái */}
                <div>
                  {getStatusBadge(payment.status)}
                </div>

                {/* Thời gian */}
                <div className="text-gray-600 text-sm">
                  <div>{formatDate(payment.paymentDate || payment.updatedAt || payment.createdAt)}</div>
                </div>
              </div>
            ))
          )}
        </div>
      </div>
    </div>
  );
};

export default PaymentHistory;
