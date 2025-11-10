import "./App.css";
import Homepage from "../pages/Homepage";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import Login from "../pages/Login";
import SignUp from "../pages/SignUp";
import UnauthorizedPage from "../pages/UnauthorizedPage";
import { AuthProvider } from "../contexts/AuthContext";
import { ProtectedRoute, RoleProtectedRoute, RoleBasedRedirect } from "../components/ProtectedRoute";
import VerifyOtpCode from "../pages/VerifyCode";
import SetNewPassword from "../pages/SetPassword";
import PasswordChangedSuccessfully from "../pages/PasswordChanged";
import ForgotPassword from "../pages/ForgotPassword";
import OrganizerPage from "../pages/Organizer/OrganizerPage";
import SponsorPage from "../pages/Sponsor/SponsorPage";
import SupplierPage from "../pages/Supplier/SupplierPage";
import ProgressBar from "../components/ProgressBar";
import { Toaster } from 'react-hot-toast';

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
          {/* Public routes */}
          <Route path="/login" element={<Login />} />
          <Route path="/signup" element={<SignUp />} />
          <Route path="/unauthorized" element={<UnauthorizedPage />} />
          <Route path="/forgot-password" element={<ForgotPassword />} />
          <Route path="/verify-code" element={<VerifyOtpCode />} />
          <Route path="/set-password" element={<SetNewPassword />} />
          <Route path="/password-changed" element={<PasswordChangedSuccessfully />} />
          <Route path="/organizer" element={<OrganizerPage />} />
          <Route path="/sponsor" element={<SponsorPage />} />
          <Route path="/supplier" element={<SupplierPage />} />


          <Route index element={<Homepage />} />
          
          {/* Root redirect based on role */}
          {/* <Route index element={<RoleBasedRedirect />} /> */}
          
          {/* Protected routes by role */}
          {/* <Route 
            path="/organizer" 
            element={
              <RoleProtectedRoute allowedRoles={['ORGANIZER']}>
                <OrganizerPage />
              </RoleProtectedRoute>
            } 
          />
           */}
          {/* <Route 
            path="/sponsor" 
            element={
              <RoleProtectedRoute allowedRoles={['SPONSOR']}>
                <SponsorPage />
              </RoleProtectedRoute>
            } 
          /> */}
          
          {/* <Route 
            path="/supplier" 
            element={
              <RoleProtectedRoute allowedRoles={['SUPPLIER']}>
                <SupplierPage />
              </RoleProtectedRoute>
            } 
          /> */}
          
          {/* <Route 
            path="/admin" 
            element={
              <RoleProtectedRoute allowedRoles={['ADMIN']}>
                <AdminPage />
              </RoleProtectedRoute>
            } 
          /> */}
          
          {/* General protected route */}
          {/* <Route 
            path="/dashboard" 
            element={
              <ProtectedRoute>
                <Homepage />
              </ProtectedRoute>
            } 
          /> */}

        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
