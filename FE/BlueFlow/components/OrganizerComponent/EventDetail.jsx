import React from 'react';
import { Calendar, MapPin, Users, Building2, ChevronDown, Search, Info, User, Mail, Phone, Globe } from 'lucide-react';
import EventTimeline from './EventTimeline';

const EventDetail = () => {
  const eventData = {
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
    ]
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
            <div>
              <h1 className="text-xl font-bold text-gray-900">{eventData.name}</h1>
            </div>
            <div className="flex items-center space-x-1 ml-2">
              <span className="text-yellow-500 text-sm">★</span>
              <span className="font-semibold text-gray-900 text-sm">{eventData.rating}</span>
              <span className="text-gray-400 text-xs">| {eventData.reviews} reviews</span>
            </div>
          </div>
          <button className="px-4 py-1.5 bg-blue-500 hover:bg-blue-600 text-white text-sm font-medium rounded-lg flex items-center space-x-1 transition-colors">
            <span>{eventData.status}</span>
            <ChevronDown size={14} />
          </button>
        </div>

        {/* Event Info Cards */}
        <div className="grid grid-cols-4 gap-3">
          <div className="bg-white border border-gray-200 rounded-lg p-3 shadow-sm">
            <div className="flex items-center space-x-2 text-blue-500 mb-1.5">
              <Calendar size={16} />
              <span className="text-xs font-medium">Date</span>
            </div>
            <p className="text-gray-900 font-semibold text-sm">{eventData.date}</p>
          </div>
          
          <div className="bg-white border border-gray-200 rounded-lg p-3 shadow-sm">
            <div className="flex items-center space-x-2 text-blue-500 mb-1.5">
              <MapPin size={16} />
              <span className="text-xs font-medium">Location</span>
            </div>
            <p className="text-gray-900 font-semibold text-sm">{eventData.location}</p>
          </div>
          
          <div className="bg-white border border-gray-200 rounded-lg p-3 shadow-sm">
            <div className="flex items-center space-x-2 text-blue-500 mb-1.5">
              <Users size={16} />
              <span className="text-xs font-medium">Attendees</span>
            </div>
            <p className="text-gray-900 font-semibold text-sm">{eventData.attendees}</p>
          </div>
          
          <div className="bg-white border border-gray-200 rounded-lg p-3 shadow-sm">
            <div className="flex items-center space-x-2 text-blue-500 mb-1.5">
              <Building2 size={16} />
              <span className="text-xs font-medium">Industry</span>
            </div>
            <p className="text-gray-900 font-semibold text-sm">{eventData.industry}</p>
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div className="relative bg-white rounded-2xl p-6 shadow-sm border mt-10 border-gray-200">
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
          <p className="text-gray-600 text-xs leading-relaxed pl-4">
            {eventData.about}
          </p>
        </div>


        {/* What to Expect */}
        <div className="mb-4 ">
          <h2 className="text-sm font-bold text-gray-900 mb-3 text-left">What to Expect:</h2>
          <div className="grid grid-cols-3 gap-x-1 gap-y-1.5 pl-15">
            {eventData.whatToExpect.map((item, index) => (
              <div key={index} className="flex items-center space-x-1.5">
                <span className="w-1 h-1 bg-blue-500 rounded-full flex-shrink-0"></span>
                <span className="text-gray-700 text-xs">{item}</span>
              </div>
            ))}
          </div>
        </div>

        {/* Target Audience */}
        <div className="mb-7 text-left">
          <h2 className="text-sm font-bold text-gray-900 mb-2">Target Audience:</h2>
          <div className="grid grid-cols-2 gap-x-1 gap-y-1.5 pl-15">
            {eventData.targetAudience.map((item, index) => (
              <div key={index} className="flex items-center space-x-1.5">
                <span className="w-1 h-1 bg-blue-500 rounded-full flex-shrink-0"></span>
                <span className="text-gray-700 text-xs">{item}</span>
              </div>
            ))}
          </div>
        </div>

        {/* Tags */}
        <div className="flex flex-wrap gap-2">
          {eventData.tags.map((tag, index) => (
            <span 
              key={index}
              className="px-3 py-1 bg-blue-50 text-blue-500 text-xs font-medium rounded-full"
            >
              {tag}
            </span>
          ))}
          <button className="px-3 py-1 bg-gray-100 text-gray-600 text-xs font-medium rounded-full hover:bg-gray-200 transition-colors">
            ...
          </button>
        </div>
      </div>

      {/* Timeline Section */}
      <div className="mt-4">
        <EventTimeline />
      </div>

      {/* Sponsorship Budget and Event Information Section */}
      <div className="mt-4 grid grid-cols-1 lg:grid-cols-3 gap-3">
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
            <div className="grid grid-cols-2  ">
              {/* Event Manager */}
              <div className="flex justify-center items-center space-x-2">
                <div className="w-8 h-8 bg-blue-50 rounded-lg flex items-center justify-center flex-shrink-0">
                  <User size={14} className="text-blue-500" />
                </div>
                <div>
                  <p className="text-xs font-semibold text-gray-900">Event Manager</p>
                  <p className="text-xs text-gray-600">Jessica Miller</p>
                </div>
              </div>

              {/* Email */}
              <div className="flex justify-center items-center space-x-2">
                <div className="w-8 h-8 bg-blue-50 rounded-lg flex items-center justify-center flex-shrink-0">
                  <Mail size={14} className="text-blue-500" />
                </div>
                <div>
                  <p className="text-xs font-semibold text-gray-900">Email</p>
                  <p className="text-xs text-gray-600">events@techpro.com</p>
                </div>
              </div>
            </div>

            {/* Second Row: Phone and Website */}
            <div className="grid grid-cols-2">
              {/* Phone */}
              <div className="flex justify-center items-center pl-4 space-x-2">
                <div className="w-8 h-8 bg-blue-50 rounded-lg flex items-center justify-center flex-shrink-0">
                  <Phone size={14} className="text-blue-500" />
                </div>
                <div>
                  <p className="text-xs font-semibold text-gray-900">Phone</p>
                  <p className="text-xs text-gray-600">+1 (555) 123-4567</p>
                </div>
              </div>

              {/* Website */}
              <div className="flex justify-center items-center space-x-2">
                <div className="w-8 h-8 bg-blue-50 rounded-lg flex items-center justify-center flex-shrink-0">
                  <Globe size={14} className="text-blue-500" />
                </div>
                <div>
                  <p className="text-xs font-semibold text-gray-900">Website</p>
                  <p className="text-xs text-gray-600">techsummit2024.com</p>
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
