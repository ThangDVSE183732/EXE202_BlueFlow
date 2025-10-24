import React, { useState } from 'react';
import { Calendar, MapPin, Users, Building2, ChevronDown, Search, Info, User, Mail, Phone, Globe, Pencil, Check, X } from 'lucide-react';
import EventTimeline from './EventTimeline';

const EventDetail = () => {
  const [isEditing, setIsEditing] = useState(false);
  const [editedData, setEditedData] = useState({
    name: 'TechEvents',
    rating: 4.8,
    reviews: 52,
    status: 'Ongoing',
    date: 'March 15-17, 2024',
    location: 'San Francisco Convention Center',
    attendees: '2,500+ Professional',
    industry: 'Technology & Innovation',
    about: 'Join us for the most anticipated technology conference of 2024! The Tech Innovation Summit connects industry leaders and startups to explore AI, blockchain, and future technologies',
    whatToExpect: [
      '50+ Expert Speakers',
      'Interactive Workshops',
      'Networking Opportunities',
      'Startup Showcase',
      'Exhibition Hall'
    ],
    targetAudience: [
      'Tech executives & software developers',
      'Entrepreneurs & investors',
      'Fintech, healthcare, e-commerce leaders',
      'Future-focused innovators & students'
    ],
    tags: [
      'Artificial Intelligence',
      'Machine Learning',
      'Blockchain',
      'Startups',
      'Innovation',
      'Networking'
    ],
    eventManager: 'Jessica Miller',
    email: 'events@techpro.com',
    phone: '+1 (555) 123-4567',
    website: 'techsummit2024.com'
  });

  const handleSave = () => {
    // Save logic here (API call)
    console.log('Saving changes:', editedData);
    setIsEditing(false);
  };

  const handleCancel = () => {
    // Reset to original data if needed
    setIsEditing(false);
  };

  const handleInputChange = (field, value) => {
    setEditedData(prev => ({
      ...prev,
      [field]: value
    }));
  };

  const handleArrayItemChange = (field, index, value) => {
    setEditedData(prev => ({
      ...prev,
      [field]: prev[field].map((item, i) => i === index ? value : item)
    }));
  };

  const addArrayItem = (field) => {
    setEditedData(prev => ({
      ...prev,
      [field]: [...prev[field], '']
    }));
  };

  const removeArrayItem = (field, index) => {
    setEditedData(prev => ({
      ...prev,
      [field]: prev[field].filter((_, i) => i !== index)
    }));
  };

  return (
    <div className="min-h-screen bg-gray-50 p-6 max-w-full overflow-hidden">
      {/* Event Header */}
      <div className="mb-4">
        {/* Top Row - Logo, Name, Rating, Button */}
        <div className="flex items-center justify-between mb-4">
          <div className="flex items-center space-x-3">
            <div className="w-10 h-10 bg-black rounded-lg flex items-center justify-center">
              <span className="text-white font-bold text-lg">M</span>
            </div>
            <div className="max-w-xs">
              {isEditing ? (
                <input
                  type="text"
                  value={editedData.name}
                  onChange={(e) => handleInputChange('name', e.target.value)}
                  className="text-xl font-bold text-gray-900 border-b-2 border-blue-500 focus:outline-none bg-transparent w-full"
                  title={editedData.name}
                />
              ) : (
                <h1 className="text-xl font-bold text-gray-900 truncate" title={editedData.name}>{editedData.name}</h1>
              )}
            </div>
            <div className="flex items-center space-x-1 ml-2">
              <span className="text-yellow-500 text-sm">★</span>
              {isEditing ? (
                <input
                  type="number"
                  step="0.1"
                  min="0"
                  max="5"
                  value={editedData.rating}
                  onChange={(e) => handleInputChange('rating', parseFloat(e.target.value))}
                  className="font-semibold text-gray-900 text-sm w-12 border-b border-blue-500 focus:outline-none bg-transparent"
                />
              ) : (
                <span className="font-semibold text-gray-900 text-sm">{editedData.rating}</span>
              )}
              <span className="text-gray-400 text-xs">|</span>
              {isEditing ? (
                <input
                  type="number"
                  min="0"
                  value={editedData.reviews}
                  onChange={(e) => handleInputChange('reviews', parseInt(e.target.value))}
                  className="text-gray-400 text-xs w-12 border-b border-blue-500 focus:outline-none bg-transparent"
                />
              ) : (
                <span className="text-gray-400 text-xs">{editedData.reviews}</span>
              )}
              <span className="text-gray-400 text-xs">reviews</span>
            </div>
          </div>
          {isEditing ? (
            <select
              value={editedData.status}
              onChange={(e) => handleInputChange('status', e.target.value)}
              className="px-4 py-1.5 bg-blue-500 text-white text-sm font-medium rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-600"
            >
              <option value="Ongoing">Ongoing</option>
              <option value="Upcoming">Upcoming</option>
              <option value="Completed">Completed</option>
              <option value="Cancelled">Cancelled</option>
            </select>
          ) : (
            <button className="px-4 py-1.5 bg-blue-500 hover:bg-blue-600 text-white text-sm font-medium rounded-lg flex items-center space-x-1 transition-colors">
              <span>{editedData.status}</span>
              <ChevronDown size={14} />
            </button>
          )}
        </div>

        {/* Event Info Cards */}
        <div className="grid grid-cols-4 gap-3">
          <div className="bg-white border border-gray-200 rounded-lg p-3 shadow-sm">
            <div className="flex items-center space-x-2 text-blue-500 mb-1.5">
              <Calendar size={16} />
              <span className="text-xs font-medium">Date</span>
            </div>
            {isEditing ? (
              <input
                type="text"
                value={editedData.date}
                onChange={(e) => handleInputChange('date', e.target.value)}
                className="text-gray-900 font-semibold text-sm w-full border-b border-blue-500 focus:outline-none bg-transparent"
                placeholder="March 15-17, 2024"
                title={editedData.date}
              />
            ) : (
              <p className="text-gray-900 font-semibold text-sm truncate" title={editedData.date}>{editedData.date}</p>
            )}
          </div>
          
          <div className="bg-white border border-gray-200 rounded-lg p-3 shadow-sm">
            <div className="flex items-center space-x-2 text-blue-500 mb-1.5">
              <MapPin size={16} />
              <span className="text-xs font-medium">Location</span>
            </div>
            {isEditing ? (
              <input
                type="text"
                value={editedData.location}
                onChange={(e) => handleInputChange('location', e.target.value)}
                className="text-gray-900 font-semibold text-sm w-full border-b border-blue-500 focus:outline-none bg-transparent"
                placeholder="Location"
                title={editedData.location}
              />
            ) : (
              <p className="text-gray-900 font-semibold text-sm truncate" title={editedData.location}>{editedData.location}</p>
            )}
          </div>
          
          <div className="bg-white border border-gray-200 rounded-lg p-3 shadow-sm">
            <div className="flex items-center space-x-2 text-blue-500 mb-1.5">
              <Users size={16} />
              <span className="text-xs font-medium">Attendees</span>
            </div>
            {isEditing ? (
              <input
                type="text"
                value={editedData.attendees}
                onChange={(e) => handleInputChange('attendees', e.target.value)}
                className="text-gray-900 font-semibold text-sm w-full border-b border-blue-500 focus:outline-none bg-transparent"
                placeholder="2,500+ Professional"
                title={editedData.attendees}
              />
            ) : (
              <p className="text-gray-900 font-semibold text-sm truncate" title={editedData.attendees}>{editedData.attendees}</p>
            )}
          </div>
          
          <div className="bg-white border border-gray-200 rounded-lg p-3 shadow-sm">
            <div className="flex items-center space-x-2 text-blue-500 mb-1.5">
              <Building2 size={16} />
              <span className="text-xs font-medium">Industry</span>
            </div>
            {isEditing ? (
              <input
                type="text"
                value={editedData.industry}
                onChange={(e) => handleInputChange('industry', e.target.value)}
                className="text-gray-900 font-semibold text-sm w-full border-b border-blue-500 focus:outline-none bg-transparent"
                placeholder="Technology & Innovation"
                title={editedData.industry}
              />
            ) : (
              <p className="text-gray-900 font-semibold text-sm truncate" title={editedData.industry}>{editedData.industry}</p>
            )}
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div className="relative bg-white rounded-2xl p-6 shadow-sm border mt-10 border-gray-200">
        {/* Edit/Save/Cancel Buttons - Top Right Corner */}
        <div className="absolute top-4 right-4 flex items-center space-x-2">
          {isEditing ? (
            <>
              <button 
                className="p-2 hover:bg-green-50 rounded-lg transition-colors group"
                onClick={handleSave}
                title="Save changes"
              >
                <Check size={18} className="text-green-600 group-hover:text-green-700" />
              </button>
              <button 
                className="p-2 hover:bg-red-50 rounded-lg transition-colors group"
                onClick={handleCancel}
                title="Cancel editing"
              >
                <X size={18} className="text-red-600 group-hover:text-red-700" />
              </button>
            </>
          ) : (
            <button 
              className="p-2 hover:bg-gray-100 rounded-lg transition-colors group"
              onClick={() => setIsEditing(true)}
              title="Edit event details"
            >
              <Pencil size={18} className="text-gray-600 group-hover:text-blue-500" />
            </button>
          )}
        </div>

        {/* Tab Navigation - Inside Card */}
        <div className="flex justify-center absolute -top-4 left-1/2 transform -translate-x-1/2">
          <button className="px-6 py-1 bg-blue-400 text-white text-lg font-semibold rounded-full hover:bg-blue-600 transition-colors shadow-md">
            Overview
          </button>
        </div>

        {/* 3 dots decoration */}
        <div className="flex justify-center space-x-1 mb-4 mt-3">
          <span className="w-0.5 h-0.5 bg-gray-400 rounded-full"></span>
          <span className="w-0.5 h-0.5 bg-gray-400 rounded-full"></span>
          <span className="w-0.5 h-0.5 bg-gray-400 rounded-full"></span>
        </div>

        {/* About this event */}
        <div className="mb-4 text-left">
          <h2 className="text-sm font-bold text-gray-900 mb-2">About this event:</h2>
          {isEditing ? (
            <textarea
              value={editedData.about}
              onChange={(e) => handleInputChange('about', e.target.value)}
              className="w-full text-gray-600 text-xs leading-relaxed pl-4 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-blue-500 resize-none whitespace-pre-wrap break-words"
              rows={4}
            />
          ) : (
            <p className="text-gray-600 text-xs leading-relaxed pl-4 break-words whitespace-pre-wrap">
              {editedData.about}
            </p>
          )}
        </div>


        {/* What to Expect */}
        <div className="mb-4 ">
          <h2 className="text-sm font-bold text-gray-900 mb-3 text-left">What to Expect:</h2>
          <div className="grid grid-cols-3 gap-x-3 gap-y-1.5 pl-15">
            {editedData.whatToExpect.map((item, index) => (
              <div key={index} className="flex items-start space-x-1.5 min-w-0 max-w-full">
                <span className="w-1 h-1 bg-blue-500 rounded-full flex-shrink-0 mt-1"></span>
                {isEditing ? (
                  <div className="flex items-center space-x-1 flex-1 min-w-0">
                    <input
                      type="text"
                      value={item}
                      onChange={(e) => handleArrayItemChange('whatToExpect', index, e.target.value)}
                      className="text-gray-700 text-xs border-b border-gray-300 focus:border-blue-500 focus:outline-none flex-1 min-w-0"
                    />
                    <button
                      onClick={() => removeArrayItem('whatToExpect', index)}
                      className="text-red-500 hover:text-red-700 flex-shrink-0"
                    >
                      <X size={12} />
                    </button>
                  </div>
                ) : (
                  <span className="text-gray-700 text-xs truncate block" title={item}>{item}</span>
                )}
              </div>
            ))}
            {isEditing && (
              <button
                onClick={() => addArrayItem('whatToExpect')}
                className="text-blue-500 text-xs hover:text-blue-700 flex items-center space-x-1"
              >
                <span>+ Add item</span>
              </button>
            )}
          </div>
        </div>

        {/* Target Audience */}
        <div className="mb-7 text-left">
          <h2 className="text-sm font-bold text-gray-900 mb-2">Target Audience:</h2>
          <div className="grid grid-cols-2 gap-x-3 gap-y-1.5 pl-15">
            {editedData.targetAudience.map((item, index) => (
              <div key={index} className="flex items-start space-x-1.5 min-w-0 max-w-full">
                <span className="w-1 h-1 bg-blue-500 rounded-full flex-shrink-0 mt-1"></span>
                {isEditing ? (
                  <div className="flex items-center space-x-1 flex-1 min-w-0">
                    <input
                      type="text"
                      value={item}
                      onChange={(e) => handleArrayItemChange('targetAudience', index, e.target.value)}
                      className="text-gray-700 text-xs border-b border-gray-300 focus:border-blue-500 focus:outline-none flex-1 min-w-0"
                    />
                    <button
                      onClick={() => removeArrayItem('targetAudience', index)}
                      className="text-red-500 hover:text-red-700 flex-shrink-0"
                    >
                      <X size={12} />
                    </button>
                  </div>
                ) : (
                  <span className="text-gray-700 text-xs truncate block" title={item}>{item}</span>
                )}
              </div>
            ))}
            {isEditing && (
              <button
                onClick={() => addArrayItem('targetAudience')}
                className="text-blue-500 text-xs hover:text-blue-700 flex items-center space-x-1"
              >
                <span>+ Add item</span>
              </button>
            )}
          </div>
        </div>

        {/* Tags */}
        <div className="flex flex-wrap gap-2">
          {editedData.tags.map((tag, index) => (
            <div key={index} className="relative group max-w-[10rem]">
              {isEditing ? (
                <div className="flex items-center space-x-1 px-3 py-1 bg-blue-50 rounded-full">
                  <input
                    type="text"
                    value={tag}
                    onChange={(e) => handleArrayItemChange('tags', index, e.target.value)}
                    className="bg-transparent text-blue-500 text-xs font-medium focus:outline-none w-24"
                  />
                  <button
                    onClick={() => removeArrayItem('tags', index)}
                    className="text-red-500 hover:text-red-700 flex-shrink-0"
                  >
                    <X size={12} />
                  </button>
                </div>
              ) : (
                <span 
                  className="px-3 py-1 bg-blue-50 text-blue-500 text-xs font-medium rounded-full inline-block truncate max-w-full" 
                  title={tag}
                >
                  {tag}
                </span>
              )}
            </div>
          ))}
          {isEditing ? (
            <button
              onClick={() => addArrayItem('tags')}
              className="px-3 py-1 bg-gray-100 text-gray-600 text-xs font-medium rounded-full hover:bg-gray-200 transition-colors flex-shrink-0"
            >
              + Add tag
            </button>
          ) : (
            <button className="px-3 py-1 bg-gray-100 text-gray-600 text-xs font-medium rounded-full hover:bg-gray-200 transition-colors flex-shrink-0">
              ...
            </button>
          )}
        </div>
      </div>
      
      <h2 className="text-xl ml-2 mt-6 font-semibold text-gray-900 text-left">Timeline</h2>

      {/* Timeline Section */}
      <div className="mt-4">
        <EventTimeline />
      </div>

      {/* Sponsorship Budget and Event Information Section */}
      <div className="mt-5 grid grid-cols-1 lg:grid-cols-3 gap-3">
        {/* Sponsorship Budget Card */}
        <div className="bg-white rounded-xl p-4 shadow-sm border border-gray-200">
          <div className="flex items-center space-x-2 mb-3">
            <div className="w-8 h-8 bg-blue-500 rounded-lg flex items-center justify-center">
              <Search size={16} className="text-white" />
            </div>
            <h3 className="text-sm font-semibold text-gray-900">Sponsorship Budget</h3>
          </div>
          
          <div className="mb-3">
            <p className="text-xl font-bold text-blue-500">7.000.000 VND</p>
          </div>
          
          <div className="flex  gap-1.5">
            <span className="px-1 py-1 bg-blue-50 text-blue-500 text-xs font-medium rounded-md">
              Premium Sponsor
            </span>
            <span className="px-1 py-1 bg-blue-50 text-blue-500 text-xs font-medium rounded-md">
              Exhibition Space
            </span>
          </div>
        </div>

        {/* Event Information Card */}
        <div className="bg-white rounded-xl p-4 shadow-sm border border-gray-200 lg:col-span-2">
          <div className="flex items-center space-x-2 mb-4">
            <div className="w-8 h-8 bg-blue-500 rounded-lg flex items-center justify-center">
              <Info size={16} className="text-white" />
            </div>
            <h3 className="text-sm font-semibold text-gray-900">Event Information</h3>
          </div>
          
          <div className="space-y-3 mr-3">
            {/* First Row: Event Manager and Email */}
            <div className="grid grid-cols-2">
              {/* Event Manager */}
              <div className="flex justify-center items-center space-x-2 min-w-0">
                <div className="w-8 h-8 bg-blue-50 rounded-lg flex items-center justify-center flex-shrink-0">
                  <User size={14} className="text-blue-500" />
                </div>
                <div className="min-w-0 flex-1">
                  <p className="text-xs font-semibold text-gray-900">Event Manager</p>
                  {isEditing ? (
                    <input
                      type="text"
                      value={editedData.eventManager}
                      onChange={(e) => handleInputChange('eventManager', e.target.value)}
                      className="text-xs text-gray-600 w-full border-b border-blue-500 focus:outline-none bg-transparent"
                      placeholder="Event Manager"
                      title={editedData.eventManager}
                    />
                  ) : (
                    <p className="text-xs text-gray-600 truncate" title={editedData.eventManager}>{editedData.eventManager}</p>
                  )}
                </div>
              </div>

              {/* Email */}
              <div className="flex justify-center items-center space-x-2 min-w-0">
                <div className="w-8 h-8 bg-blue-50 rounded-lg flex items-center justify-center flex-shrink-0">
                  <Mail size={14} className="text-blue-500" />
                </div>
                <div className="min-w-0 flex-1">
                  <p className="text-xs font-semibold text-gray-900">Email</p>
                  {isEditing ? (
                    <input
                      type="email"
                      value={editedData.email}
                      onChange={(e) => handleInputChange('email', e.target.value)}
                      className="text-xs text-gray-600 w-full border-b border-blue-500 focus:outline-none bg-transparent"
                      placeholder="Email"
                      title={editedData.email}
                    />
                  ) : (
                    <p className="text-xs text-gray-600 truncate" title={editedData.email}>{editedData.email}</p>
                  )}
                </div>
              </div>
            </div>

            {/* Second Row: Phone and Website */}
            <div className="grid grid-cols-2">
              {/* Phone */}
              <div className="flex justify-center items-center  space-x-2 min-w-0">
                <div className="w-8 h-8 bg-blue-50 rounded-lg flex items-center justify-center flex-shrink-0">
                  <Phone size={14} className="text-blue-500" />
                </div>
                <div className="min-w-0 flex-1">
                  <p className="text-xs font-semibold text-gray-900">Phone</p>
                  {isEditing ? (
                    <input
                      type="tel"
                      value={editedData.phone}
                      onChange={(e) => handleInputChange('phone', e.target.value)}
                      className="text-xs text-gray-600 w-full border-b border-blue-500 focus:outline-none bg-transparent"
                      placeholder="Phone"
                      title={editedData.phone}
                    />
                  ) : (
                    <p className="text-xs text-gray-600 truncate" title={editedData.phone}>{editedData.phone}</p>
                  )}
                </div>
              </div>

              {/* Website */}
              <div className="flex justify-center items-center space-x-2 min-w-0">
                <div className="w-8 h-8 bg-blue-50 rounded-lg flex items-center justify-center flex-shrink-0">
                  <Globe size={14} className="text-blue-500" />
                </div>
                <div className="min-w-0 flex-1">
                  <p className="text-xs font-semibold text-gray-900">Website</p>
                  {isEditing ? (
                    <input
                      type="url"
                      value={editedData.website}
                      onChange={(e) => handleInputChange('website', e.target.value)}
                      className="text-xs text-gray-600 w-full border-b border-blue-500 focus:outline-none bg-transparent"
                      placeholder="Website"
                      title={editedData.website}
                    />
                  ) : (
                    <p className="text-xs text-gray-600 truncate" title={editedData.website}>{editedData.website}</p>
                  )}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default EventDetail;
