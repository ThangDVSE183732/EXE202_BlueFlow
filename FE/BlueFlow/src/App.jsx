import "./App.css";
import Homepage from "../pages/Homepage";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import Login from "../pages/Login";
import SignUp from "../pages/SignUp";
import UnauthorizedPage from "../pages/UnauthorizedPage";
import { AuthProvider } from "../contexts/AuthContext";
import { ProtectedRoute, RoleProtectedRoute, RoleBasedRedirect, GuestOnlyRoute } from "../components/ProtectedRoute";
import VerifyOtpCode from "../pages/VerifyCode";
import SetNewPassword from "../pages/SetPassword";
import PasswordChangedSuccessfully from "../pages/PasswordChanged";
import ForgotPassword from "../pages/ForgotPassword";
import OrganizerPage from "../pages/Organizer/OrganizerPage";
import SponsorPage from "../pages/Sponsor/SponsorPage";
import SupplierPage from "../pages/Supplier/SupplierPage";
import Pricing from "../pages/Pricing";
import ProgressBar from "../components/ProgressBar";
import { Toaster } from 'react-hot-toast';
import PaymentSuccessful from "../components/PaymentSuccessfull";
import PaymentFailed from "../components/PaymentFailed";

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <ProgressBar />
        <Toaster 
          position="top-right"
          reverseOrder={false}
          toastOptions={{
            duration: 3000,
            style: {
              background: '#363636',
              color: '#fff',
            },
            success: {
              duration: 3000,
              iconTheme: {
                primary: '#00A6F4',
                secondary: '#fff',
              },
            },
            error: {
              duration: 4000,
              iconTheme: {
                primary: '#ef4444',
                secondary: '#fff',
              },
            },
          }}
        />
        <Routes>
          {/* Homepage */}
          <Route index element={<Homepage />} />
          
          {/* Public Authentication routes */}
          <Route path="/login" element={
            <GuestOnlyRoute>
              <Login />
            </GuestOnlyRoute>
          } />
          <Route path="/signup" element={
            <GuestOnlyRoute>
              <SignUp />
            </GuestOnlyRoute>
          } />
          <Route path="/forgot-password" element={<ForgotPassword />} />
          <Route path="/verify-code" element={<VerifyOtpCode />} />
          <Route path="/set-password" element={<SetNewPassword />} />
          <Route path="/password-changed" element={<PasswordChangedSuccessfully />} />
          <Route path="/unauthorized" element={<UnauthorizedPage />} />
          
          {/* Payment routes */}
          <Route path="/pricing" element={<Pricing />} />
          <Route path="/success" element={<PaymentSuccessful />} />
          <Route path="/failed" element={<PaymentFailed />} />
          
          {/* Protected Role-based dashboard routes */}
          <Route 
            path="/organizer" 
            element={
              <RoleProtectedRoute allowedRoles={['ORGANIZER']}>
                <OrganizerPage />
              </RoleProtectedRoute>
            } 
          />
          
          <Route 
            path="/sponsor" 
            element={
              <RoleProtectedRoute allowedRoles={['SPONSOR']}>
                <SponsorPage />
              </RoleProtectedRoute>
            } 
          />
          
          <Route 
            path="/supplier" 
            element={
              <RoleProtectedRoute allowedRoles={['SUPPLIER']}>
                <SupplierPage />
              </RoleProtectedRoute>
            } 
          />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
