import React, { useState } from 'react';
import RevenueReport from '../../components/AdminComponent/RevenueReport';
import Footer from '../../components/Footer';
import PageNav from '../../components/PageNav';

const AdminPage = () => {
  const [activeTab, setActiveTab] = useState('revenue');

  return (
    <div className="min-h-screen bg-gray-50">
      <PageNav />
      <div className="pt-5 pb-15">
        {activeTab === 'revenue' && <RevenueReport />}
      </div>
      <Footer />
    </div>
  );
};

export default AdminPage;

