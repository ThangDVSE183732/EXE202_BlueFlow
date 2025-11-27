import { useState, useCallback } from 'react';
import paymentService from '../services/paymentService';
import { useToast } from './useToast';

export const usePayment = () => {
  const [loading, setLoading] = useState(false);
  const [paymentHistory, setPaymentHistory] = useState([]);
  const [subscriptionPlans, setSubscriptionPlans] = useState([]);
  const [premiumStatus, setPremiumStatus] = useState(null);
  const [currentPayment, setCurrentPayment] = useState(null);
  const { showToast } = useToast();

  // Tạo thanh toán premium
  const createPremiumPayment = useCallback(async (paymentData) => {
    setLoading(true);
    try {
      const result = await paymentService.createPremiumPayment(paymentData);
      return result;
    } catch (error) {
      showToast(error.message || 'Tạo thanh toán thất bại!', 'error');
      throw error;
    } finally {
      setLoading(false);
    }
  }, [showToast]);

  // Lấy lịch sử thanh toán
  const fetchPaymentHistory = useCallback(async () => {
    setLoading(true);
    try {
      const data = await paymentService.getPaymentHistory();
      setPaymentHistory(data);
      return data;
    } catch (error) {
      showToast(error.message || 'Lấy lịch sử thanh toán thất bại!', 'error');
      throw error;
    } finally {
      setLoading(false);
    }
  }, [showToast]);

  // Hủy thanh toán
  const cancelPayment = useCallback(async (paymentId) => {
    setLoading(true);
    try {
      const result = await paymentService.cancelPayment(paymentId);
      showToast('Hủy thanh toán thành công!', 'success');
      // Refresh payment history
      await fetchPaymentHistory();
      return result;
    } catch (error) {
      showToast(error.message || 'Hủy thanh toán thất bại!', 'error');
      throw error;
    } finally {
      setLoading(false);
    }
  }, [showToast, fetchPaymentHistory]);

  // Lấy thông tin thanh toán theo ID
  const getPaymentById = useCallback(async (id) => {
    setLoading(true);
    try {
      const data = await paymentService.getPaymentById(id);
      setCurrentPayment(data);
      return data;
    } catch (error) {
      showToast(error.message || 'Lấy thông tin thanh toán thất bại!', 'error');
      throw error;
    } finally {
      setLoading(false);
    }
  }, [showToast]);

  // Lấy danh sách gói subscription
  const fetchSubscriptionPlans = useCallback(async () => {
    setLoading(true);
    try {
      const data = await paymentService.getSubscriptionPlans();
      setSubscriptionPlans(data);
      return data;
    } catch (error) {
      showToast(error.message || 'Lấy danh sách gói thất bại!', 'error');
      throw error;
    } finally {
      setLoading(false);
    }
  }, [showToast]);

  // Xác minh thanh toán
  const verifyPayment = useCallback(async (params) => {
    setLoading(true);
    try {
      const result = await paymentService.verifyPayment(params);
      return result;
    } finally {
      setLoading(false);
    }
  }, []);

  // Kiểm tra trạng thái premium
  const fetchPremiumStatus = useCallback(async () => {
    setLoading(true);
    try {
      const data = await paymentService.getPremiumStatus();
      setPremiumStatus(data);
      return data;
    } catch (error) {
      showToast(error.message || 'Lấy trạng thái premium thất bại!', 'error');
      throw error;
    } finally {
      setLoading(false);
    }
  }, [showToast]);

  // Kiểm tra xem user có premium không
  const checkPremium = useCallback(async () => {
    try {
      const result = await paymentService.checkPremium();
      return result;
    } catch (error) {
      console.error('Check premium error:', error);
      return false;
    }
  }, []);

  // Xử lý webhook (thường không gọi từ frontend, nhưng để sẵn)
  const handleWebhook = useCallback(async (webhookData) => {
    try {
      const result = await paymentService.handleWebhook(webhookData);
      return result;
    } catch (error) {
      console.error('Webhook error:', error);
      throw error;
    }
  }, []);

  return {
    loading,
    paymentHistory,
    subscriptionPlans,
    premiumStatus,
    currentPayment,
    createPremiumPayment,
    fetchPaymentHistory,
    cancelPayment,
    getPaymentById,
    fetchSubscriptionPlans,
    verifyPayment,
    fetchPremiumStatus,
    checkPremium,
    handleWebhook,
  };
};
