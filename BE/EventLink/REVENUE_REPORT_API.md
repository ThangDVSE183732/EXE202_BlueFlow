# Revenue Report API Documentation

## Overview
API endpoint để lấy báo cáo doanh thu cho Admin Dashboard. Endpoint này chỉ có thể truy cập bởi user có role Admin.

## Endpoint

### GET /api/Payment/revenue-report

**Authorization Required:** Yes (Admin role only)

**Query Parameters:**
- `year` (optional): Năm cần lấy báo cáo. Mặc định là năm hiện tại.

**Request Example:**
```http
GET /api/Payment/revenue-report?year=2024
Authorization: Bearer {your-jwt-token}
```

**Response Success (200 OK):**
```json
{
  "success": true,
  "message": "Revenue report retrieved successfully",
  "data": {
    "totalRevenue": 2456789,
    "growth": 12.5,
    "totalTransactions": 1245,
    "averageOrder": 1973,
    "monthlyData": [
      {
        "month": "Jan",
        "revenue": 180000,
        "transactions": 120
      },
      // ... 11 more months
    ],
    "topRevenueSources": [
      {
        "source": "Premium Subscription",
        "revenue": 1250000,
        "percentage": 50.8
      },
      // ... more sources
    ],
    "recentTransactions": [
      {
        "id": "guid-here",
        "date": "2024-12-01T10:30:00Z",
        "customer": "John Doe",
        "amount": 15000,
        "status": "Completed",
        "paymentType": "Premium Subscription"
      },
      // ... up to 10 recent transactions
    ]
  }
}
```

**Response Error (401 Unauthorized):**
```json
{
  "success": false,
  "message": "User not authenticated"
}
```

**Response Error (403 Forbidden):**
```json
{
  "success": false,
  "message": "Access denied. Admin role required."
}
```

**Response Error (500 Internal Server Error):**
```json
{
  "success": false,
  "message": "Internal server error",
  "error": "Error details here"
}
```

## Data Fields Explained

### RevenueReportResponse
- **totalRevenue**: Tổng doanh thu trong năm (chỉ tính các payment có status = "Completed")
- **growth**: Phần trăm tăng trưởng so với năm trước
- **totalTransactions**: Tổng số giao dịch thành công
- **averageOrder**: Giá trị trung bình mỗi đơn hàng

### MonthlyRevenueData
- **month**: Tên tháng (Jan, Feb, Mar, ...)
- **revenue**: Doanh thu trong tháng đó
- **transactions**: Số lượng giao dịch trong tháng

### RevenueSourceData
- **source**: Nguồn doanh thu (dựa trên Payment.PaymentType)
- **revenue**: Doanh thu từ nguồn này
- **percentage**: Phần trăm đóng góp vào tổng doanh thu

### RecentTransactionData
- **id**: ID của payment
- **date**: Ngày thanh toán hoặc ngày tạo
- **customer**: Tên hoặc email của khách hàng
- **amount**: Số tiền
- **status**: Trạng thái thanh toán
- **paymentType**: Loại thanh toán

## Implementation Details

### Backend Files Created/Modified:

1. **RevenueReportResponse.cs** (NEW)
   - Location: `Eventlink_Services/Response/RevenueReportResponse.cs`
   - Contains DTOs for revenue report data

2. **IPaymentRepository.cs** (MODIFIED)
   - Added methods:
     - `GetAllPaymentsAsync()`
     - `GetPaymentsByDateRangeAsync()`
     - `GetCompletedPaymentsAsync()`

3. **PaymentRepository.cs** (MODIFIED)
   - Implemented new repository methods

4. **IPaymentService.cs** (MODIFIED)
   - Added: `GetRevenueReportAsync(int? year)`

5. **PaymentService.cs** (MODIFIED)
   - Implemented revenue report business logic
   - Calculates totals, growth, monthly breakdown, etc.

6. **PaymentController.cs** (MODIFIED)
   - Added endpoint: `GET /api/Payment/revenue-report`
   - Requires Admin role authorization

### Frontend Files Created/Modified:

1. **paymentService.js** (MODIFIED)
   - Added: `getRevenueReport(year)` method

2. **RevenueReport.jsx** (MODIFIED)
   - Integrated real API instead of mock data
   - Added loading and error states
   - Added year selector and refresh button
   - Uses `useEffect` to fetch data on mount and year change

## Usage in Frontend

```javascript
import paymentService from '../../services/paymentService';

// Fetch revenue report for current year
const response = await paymentService.getRevenueReport();

// Fetch revenue report for specific year
const response2024 = await paymentService.getRevenueReport(2024);

if (response.success) {
  const data = response.data;
  console.log('Total Revenue:', data.totalRevenue);
  console.log('Growth:', data.growth);
  // ... use data
}
```

## Features

✅ **Real-time Data**: Fetches actual payment data from database
✅ **Year Selection**: Users can view reports for different years
✅ **Growth Calculation**: Automatic comparison with previous year
✅ **Monthly Breakdown**: Revenue and transaction count per month
✅ **Revenue Sources**: Grouped by payment type with percentages
✅ **Recent Transactions**: Shows last 10 completed transactions
✅ **Loading States**: Shows loading spinner while fetching
✅ **Error Handling**: Displays error messages and retry option
✅ **Authorization**: Admin-only access with JWT validation

## Testing

To test the API:

1. Login as Admin user to get JWT token
2. Use Postman or similar tool:
   ```
   GET https://your-api-url/api/Payment/revenue-report?year=2024
   Authorization: Bearer YOUR_JWT_TOKEN
   ```
3. Or use the frontend RevenueReport component directly

## Notes

- Revenue calculations only include payments with `Status = "Completed"`
- Growth percentage is calculated comparing current year with previous year
- If no previous year data exists, growth will be 0
- Monthly data always returns 12 months (Jan-Dec), even if some months have 0 revenue
- Recent transactions are limited to 10 most recent entries
- All amounts are in VND (Vietnamese Dong)
