import React, { useState } from 'react';
import { Edit, Plus, Upload } from 'lucide-react';

const BrandProfile = () => {
  const profileData = {
    companyName: 'TechCorp Solutions',
    tagline: 'Technology & Innovation Sponsor',
    location: 'Ho Chi Minh City, Vietnam',
    eventsSponsored: 45,
    activePartnerships: 12,
    satisfactionRate: 98,
    aboutUs: 'TechCorp Solutions is a leading technology company specializing in innovative software solutions and digital transformation services. We are passionate about supporting the tech community through strategic event sponsorships and partnerships.',
    mission: [
      'Expertise in digital transformation and software innovation',
      'Strong commitment to industry collaboration and ecosystem building',
      'Focused on fostering innovation within the Vietnamese tech community',
      'Proven record in successful partnerships and event sponsorships'
    ],
    companyInfo: {
      industry: 'Technology & Software',
      companySize: '500-1000 employees',
      founded: '2018',
      website: 'www.techcorp.vn',
      email: 'techcorpsolution@gmail.com',
      phone: '+84 949xxxxxx'
    }
  };


  const handleEdit = (section) => {
    console.log(`Editing ${section}`);
  };



  return (
    <div className="min-h-screen bg-white p-6 pt-1">
      {/* Header */}
      <div className="mb-8 text-left">
        <h1 className="text-2xl font-semibold text-blue-500 mb-2">Profile Management</h1>
        <p className="text-gray-500 text-sm">Create and manage your professional sponsor profile to attract event organizers</p>
              <div className="h-px w-full bg-gray-300 mx-1 mb-5 mt-2" />

      </div>

      <div className="bg-white border rounded-xl shadow-xl border-gray-300">
      {/* Company Banner */}
      <div className="bg-black rounded-2xl px-10 py-5 mb-3 relative overflow-hidden">
        {/* Published Button - Top Right */}
        <div className="absolute top-3 right-4 flex items-center space-x-2">
          <button
            className="px-3 py-1 rounded-full text-xs font-medium transition-all bg-blue-500 text-white hover:bg-blue-600"
          >
           Edit
          </button>
          <button className="w-6 h-6 flex items-center justify-center text-white hover:bg-white/10 rounded-full transition-colors">
            <Upload size={14} />
          </button>
        </div>

        {/* Logo and Company Info - Left */}
        <div className="flex items-center space-x-5 mb-6 mt-5">
          <div className="w-26 h-26 bg-white rounded-full flex items-center justify-center flex-shrink-0">
            <span className="text-2xl font-bold text-gray-800">TC</span>
          </div>
          <div className='text-left'>
            <h2 className="text-2xl font-bold text-blue-400 mb-0.5">{profileData.companyName}</h2>
            <p className="text-white text-sm mb-0.5">{profileData.tagline}</p>
            <p className="text-gray-400 text-xs">{profileData.location}</p>
          </div>
        </div>

        
      </div>

      {/* Main Content - Centered */}
      <div className="max-w-5xl space-y-4 px-6">
        {/* About Us Section */}
        <div className="bg-white p-5 ">
          <div className="flex justify-between items-center mb-3">
            <h3 className="text-lg font-bold text-gray-900">About Us</h3>
            <button
              onClick={() => handleEdit('about')}
              className="px-3 py-1 bg-blue-500 hover:bg-blue-600 text-white text-xs font-medium rounded-lg transition-colors"
            >
              Edit
            </button>
          </div>

          <p className="text-gray-600 leading-relaxed text-xs text-left">
            {profileData.aboutUs}
          </p>
        </div>

        {/* Our Mission Section */}
        <div className="bg-white  p-5 ">
          <div className="flex justify-between items-center mb-3">
            <h3 className="text-lg font-bold text-gray-900">Our mission:</h3>
            <button
              onClick={() => handleEdit('mission')}
              className="px-3 py-1 bg-blue-500 hover:bg-blue-600 text-white text-xs font-medium rounded-lg transition-colors"
            >
              Edit
            </button>
          </div>

          <ul className="space-y-1.5">
            {profileData.mission.map((item, index) => (
              <li key={index} className="flex items-start space-x-2">
                <span className="w-1 h-1 bg-gray-400 rounded-full mt-1.5 flex-shrink-0"></span>
                <span className="text-gray-600 text-xs">{item}</span>
              </li>
            ))}
          </ul>
        </div>

        {/* Company Information Section */}
        <div className="bg-white  p-5 ">
          <div className="flex justify-between items-center mb-3">
            <h3 className="text-lg font-bold text-gray-900">Company Information</h3>
            <button
              onClick={() => handleEdit('company')}
              className="px-3 py-1 bg-blue-500 hover:bg-blue-600 text-white text-xs font-medium rounded-lg transition-colors"
            >
              Edit
            </button>
          </div>

          <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
            <div>
              <label className="block text-xs font-medium text-gray-400 mb-0.5">Industry</label>
              <p className="text-gray-900 text-sm font-semibold">{profileData.companyInfo.industry}</p>
            </div>
            <div>
              <label className="block text-xs font-medium text-gray-400 mb-0.5">Company Size</label>
              <p className="text-gray-900 text-sm font-semibold">{profileData.companyInfo.companySize}</p>
            </div>
            <div>
              <label className="block text-xs font-medium text-gray-400 mb-0.5">Founded</label>
              <p className="text-gray-900 text-sm font-semibold">{profileData.companyInfo.founded}</p>
            </div>
            <div>
              <label className="block text-xs font-medium text-gray-400 mb-0.5">Website</label>
              <p className="text-blue-500 hover:text-blue-600 cursor-pointer text-sm font-semibold">{profileData.companyInfo.website}</p>
            </div>
            <div>
              <label className="block text-xs font-medium text-gray-400 mb-0.5">email</label>
              <p className="text-gray-900 text-sm font-semibold">{profileData.companyInfo.email}</p>
            </div>
            <div>
              <label className="block text-xs font-medium text-gray-400 mb-0.5">Company Size</label>
              <p className="text-gray-900 text-sm font-semibold">{profileData.companyInfo.phone}</p>
            </div>
          </div>
        </div>

        {/* Sponsorship Portfolio Section */}
        <div className="mt-5">
          <div className="flex justify-between items-center mb-3">
            <h2 className="text-lg font-bold text-gray-900">Recent Projects</h2>
            <button className="flex items-center space-x-1 px-3 py-1 bg-blue-500 hover:bg-blue-600 text-white text-xs font-medium rounded-lg transition-colors">
              <Plus size={14} />
              <span>Add Project</span>
            </button>
          </div>

          {/* Portfolio Grid */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-5 text-left">
            {/* Project Card 1 */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden hover:shadow-md transition-shadow">
              {/* Card Header with Gradient */}
              <div className="bg-gradient-to-br from-blue-400 to-blue-500 h-24 flex items-center justify-center">
                <h3 className="text-base font-bold text-white">Startup Ecosystem 2024</h3>
              </div>
              
              {/* Card Content */}
              <div className="p-2.5">
                <h4 className="text-xs font-bold text-gray-900 mb-0.5">Startup Ecosystem Forum 2024</h4>
                <p className="text-xs text-blue-500 font-medium mb-1">Organized by: Innovation Hub</p>
                <p className="text-xs text-gray-600 mb-1.5 leading-relaxed">
                  Gold sponsor supporting emerging startups and entrepreneurs, providing mentorship opportunities and networking platforms.
                </p>
                
                {/* Tags */}
                <div className="flex flex-wrap gap-1">
                  <span className="px-1.5 py-0.5 bg-blue-50 text-blue-600 text-xs font-medium rounded-full">Startup</span>
                  <span className="px-1.5 py-0.5 bg-blue-50 text-blue-600 text-xs font-medium rounded-full">Networking</span>
                  <span className="px-1.5 py-0.5 bg-blue-50 text-blue-600 text-xs font-medium rounded-full">Gold</span>
                </div>
              </div>
            </div>

            {/* Project Card 2 */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden hover:shadow-md transition-shadow">
              {/* Card Header with Gradient */}
              <div className="bg-gradient-to-br from-cyan-400 to-blue-500 h-24 flex items-center justify-center">
                <h3 className="text-base font-bold text-white">Digital Innovation Expo</h3>
              </div>
              
              {/* Card Content */}
              <div className="p-2.5">
                <h4 className="text-xs font-bold text-gray-900 mb-0.5">Digital Innovation Expo 2023</h4>
                <p className="text-xs text-blue-500 font-medium mb-1">Organized by: HCMC Technology Department</p>
                <p className="text-xs text-gray-600 mb-1.5 leading-relaxed">
                  Silver sponsor showcasing latest digital transformation solutions and connecting with government and enterprise clients.
                </p>
                
                {/* Tags */}
                <div className="flex flex-wrap gap-1">
                  <span className="px-1.5 py-0.5 bg-blue-50 text-blue-600 text-xs font-medium rounded-full">Digital</span>
                  <span className="px-1.5 py-0.5 bg-blue-50 text-blue-600 text-xs font-medium rounded-full">Exhibition</span>
                  <span className="px-1.5 py-0.5 bg-blue-50 text-blue-600 text-xs font-medium rounded-full">Silver</span>
                </div>
              </div>
            </div>
          </div>

          {/* More Link */}
          <div className="mt-3 text-left mb-4">
            <button className="text-blue-500 hover:text-blue-600 font-semibold text-xs ">
              More...
            </button>
          </div>
        </div>
      </div>
      </div>
    </div>
  );
};

export default BrandProfile;
