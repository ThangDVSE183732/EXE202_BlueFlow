import React, { useState } from 'react';
import toast from 'react-hot-toast';
import { Calendar, MapPin, Users, Building2, Info, User, X, ArrowLeft, Upload } from 'lucide-react';
import EventTimeline from './EventTimeline';
import { useEvent } from '../../hooks/useEvent';
import EqualizerLoader from '../EqualizerLoader';

const EventCreate = ({ onBack }) => {
  const { createEvent, createActivity, saving: isSaving } = useEvent();
  
  const [formData, setFormData] = useState({
    title: '',
    description: '',
    shortDescription: '',
    eventDate: '',
    endDate: '',
    location: '',
    venueDetails: '',
    totalBudget: 0,
    expectedAttendees: 0,
    category: '',
    eventType: '',
    targetAudience: '',
    requiredServices: '',
    sponsorshipNeeds: '',
    specialRequirements: '',
    eventHighlights: [''],
    tags: [''],
    targetAudienceList: ['']
  });

  const [coverImageUrl, setCoverImageUrl] = useState(''); // Preview URL
  const [coverImageFile, setCoverImageFile] = useState(null); // File to upload
  const [selectedCategory, setSelectedCategory] = useState(''); // UI only, not sent to API

  // Timeline state - Initialize with one sample row
  const [timelineRows, setTimelineRows] = useState([
    {
      id: Date.now(),
      title: 'New Event Title',
      events: []
    }
  ]);

  const handleSave = async () => {
    console.log('🔵 handleSave called');
    console.log('📋 formData:', formData);
    
    // Validate required fields
    if (!formData.title || !formData.eventDate || !formData.location) {
      console.log('❌ Validation failed:', {
        hasTitle: !!formData.title,
        hasEventDate: !!formData.eventDate,
        hasLocation: !!formData.location
      });
      
      toast.error('Vui lòng điền đầy đủ thông tin bắt buộc (Tên, Ngày bắt đầu, Địa điểm)');
      return;
    }
    
    console.log('✅ Validation passed');

    // Format date to ISO 8601 DateTime for backend
    const formatDateTime = (date) => {
      if (!date) return null;
      // date input: "2025-11-05" -> ISO 8601: "2025-11-05T00:00:00.000Z"
      const dateObj = new Date(date + 'T00:00:00');
      return dateObj.toISOString();
    };

    // Create FormData for file upload
    const formDataToSend = new FormData();
    
    // Append all fields
    formDataToSend.append('Title', formData.title);
    formDataToSend.append('Description', formData.description || '');
    formDataToSend.append('ShortDescription', formData.shortDescription || '');
    formDataToSend.append('EventDate', formatDateTime(formData.eventDate));
    formDataToSend.append('EndDate', formatDateTime(formData.endDate) || formatDateTime(formData.eventDate));
    formDataToSend.append('Location', formData.location);
    formDataToSend.append('VenueDetails', formData.venueDetails || '');
    formDataToSend.append('TotalBudget', parseFloat(formData.totalBudget) || 0);
    formDataToSend.append('ExpectedAttendees', parseInt(formData.expectedAttendees) || 0);
    formDataToSend.append('Category', formData.category || 'Other');
    formDataToSend.append('EventType', formData.eventType || '');
    formDataToSend.append('TargetAudience', formData.targetAudience || '');
    formDataToSend.append('RequiredServices', formData.requiredServices || '');
    formDataToSend.append('SponsorshipNeeds', formData.sponsorshipNeeds || '');
    formDataToSend.append('SpecialRequirements', formData.specialRequirements || '');
    
    // Append arrays as comma-separated strings
    const eventHighlightsArray = formData.eventHighlights.filter(item => item.trim());
    const tagsArray = formData.tags.filter(tag => tag.trim());
    const targetAudienceArray = formData.targetAudienceList.filter(item => item.trim());
    
    formDataToSend.append('EventHighlights', eventHighlightsArray.join(','));
    formDataToSend.append('Tags', tagsArray.join(','));
    formDataToSend.append('TargetAudienceList', targetAudienceArray.join(','));
    
    // Append file if exists
    if (coverImageFile) {
      formDataToSend.append('CoverImage', coverImageFile);
    }
    
    formDataToSend.append('ViewCount', 0);
    formDataToSend.append('InterestedCount', 0);

    console.log('📤 Sending FormData to API');

    const result = await createEvent(formDataToSend);
    
    console.log('📥 API Response:', result);

    if (result.success) {
      // Get eventId from response (API returns data.id)
      const eventId = result.data?.id;

      if (!eventId) {
        console.error('No eventId returned from createEvent API');
        toast('Tạo event thành công nhưng không nhận được eventId', { icon: '⚠️' });
        return;
      }

      // Create activities for timeline if eventId exists
      if (timelineRows && timelineRows.length > 0) {
        let activityCount = 0;
        
        // Loop through each timeline row with index
        for (let rowIndex = 0; rowIndex < timelineRows.length; rowIndex++) {
          const row = timelineRows[rowIndex];
          
          // Loop through each event box in the row
          if (row.events && row.events.length > 0) {
            for (const event of row.events) {
              // Helper function to convert 12-hour time to TimeSpan format (HH:mm)
              const convertToTimeSpan = (time12) => {
                if (!time12) return '08:00';
                
                // Parse "8:30 AM" or "2:00 PM" format
                const match = time12.match(/(\d{1,2}):(\d{2})\s*(AM|PM)/i);
                if (!match) return '08:00';
                
                let hours = parseInt(match[1]);
                const minutes = parseInt(match[2]);
                const period = match[3].toUpperCase();
                
                // Convert to 24-hour format
                if (period === 'PM' && hours !== 12) {
                  hours += 12;
                } else if (period === 'AM' && hours === 12) {
                  hours = 0;
                }
                
                // Format as HH:mm (NOT HH:mm:ss)
                const hoursStr = hours.toString().padStart(2, '0');
                const minutesStr = minutes.toString().padStart(2, '0');
                return `${hoursStr}:${minutesStr}`;
              };
              
              // Calculate endTime from startTime and duration
              const calculateEndTime = (time12, durationSlots) => {
                const startTimeSpan = convertToTimeSpan(time12);
                const [hours, minutes] = startTimeSpan.split(':').map(n => parseInt(n));
                
                // Each slot is 30 minutes
                const durationMinutes = durationSlots * 30;
                const totalMinutes = hours * 60 + minutes + durationMinutes;
                
                const endHours = Math.floor(totalMinutes / 60);
                const endMinutes = totalMinutes % 60;
                
                const endHoursStr = endHours.toString().padStart(2, '0');
                const endMinutesStr = endMinutes.toString().padStart(2, '0');
                return `${endHoursStr}:${endMinutesStr}`;
              };
              
              // event from EventTimeline has: time (e.g., "8:30 AM"), duration (e.g., 1.5)
              const startTime = convertToTimeSpan(event.time);
              const endTime = calculateEndTime(event.time, event.duration || 1);
              
              // Prepare activity data according to API schema
              // Each event box = 1 activity
              const activityData = {
                activityName: event.description || 'Untitled Activity',  // EventTimeline uses "description" for event title
                activityDescription: row.title || 'Untitled Row',  // Store row title here
                startTime: startTime,  // TimeSpan format: "HH:mm:ss"
                endTime: endTime,      // TimeSpan format: "HH:mm:ss"
                location: event.location || formData.location,
                speakers: event.speakers || '',
                activityType: row.title || 'general',  // Use row title for grouping
                maxParticipants: event.maxParticipants || 100000,
                isPublic: event.isPublic !== undefined ? event.isPublic : false,
                displayOrder: activityCount
              };

              // Create activity
              const activityResult = await createActivity(eventId, activityData);
              
              if (activityResult.success) {
                activityCount++;
              }
            }
          }
        }

        // Show summary toast
        if (activityCount > 0) {
          toast.success(`Đã tạo ${activityCount} hoạt động cho sự kiện`);
        }
      }

      // Don't navigate back immediately
      // Let EventManagement handle the navigation after fetching the new event
      if (onBack) onBack(eventId);
    }
  };

  const handleCancel = () => {
    if (onBack) onBack();
  };

  const handleInputChange = (field, value) => {
    setFormData(prev => ({
      ...prev,
      [field]: value
    }));
  };

  const handleArrayItemChange = (field, index, value) => {
    setFormData(prev => ({
      ...prev,
      [field]: prev[field].map((item, i) => i === index ? value : item)
    }));
  };

  const addArrayItem = (field) => {
    setFormData(prev => ({
      ...prev,
      [field]: [...prev[field], '']
    }));
  };

  const removeArrayItem = (field, index) => {
    setFormData(prev => ({
      ...prev,
      [field]: prev[field].filter((_, i) => i !== index)
    }));
  };

  // Timeline callback
  const handleTimelineChange = (updatedRows) => {
    setTimelineRows(updatedRows);
  };

  return (
    <div className="min-h-screen p-6 max-w-full overflow-hidden relative">
      {/* Loading Overlay */}
      {isSaving && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-8">
            <EqualizerLoader message="Đang tạo sự kiện và hoạt động..." />
          </div>
        </div>
      )}

      {/* Back Button */}
      <button
        onClick={handleCancel}
        className="mb-4 flex items-center space-x-2 text-gray-600 hover:text-blue-500 transition-colors"
        disabled={isSaving}
      >
        <ArrowLeft size={20} />
        <span className="font-medium">Quay lại</span>
      </button>

      {/* Event Header */}
      <div className="mb-4">
        {/* Top Row - Logo, Name, Rating, Button */}
        <div className="flex items-center justify-between mb-4">
          <div className="flex items-center space-x-3">
            <label 
              className="w-10 h-10 bg-black rounded-lg flex items-center justify-center cursor-pointer hover:bg-gray-800 transition-colors overflow-hidden group relative" 
              title="Click để upload logo"
            >
              {coverImageUrl ? (
                <>
                  <img 
                    src={coverImageUrl} 
                    alt="Event Logo" 
                    className="w-full h-full object-cover"
                  />
                  <div className="absolute inset-0 bg-black bg-opacity-50 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center">
                    <Upload size={16} className="text-white" />
                  </div>
                </>
              ) : (
                <>
                  <span className="text-white font-bold text-lg group-hover:hidden">N</span>
                  <Upload size={16} className="text-white hidden group-hover:block" />
                </>
              )}
              <input
                type="file"
                accept="image/*"
                onChange={(e) => {
                  const file = e.target.files[0];
                  if (file) {
                    // Kiểm tra size file (max 5MB)
                    if (file.size > 5 * 1024 * 1024) {
                      toast.error('File quá lớn! Vui lòng chọn ảnh dưới 5MB');
                      return;
                    }
                    
                    // Lưu file để upload
                    setCoverImageFile(file);
                    
                    // Tạo URL preview từ file (chỉ để hiển thị)
                    const previewUrl = URL.createObjectURL(file);
                    setCoverImageUrl(previewUrl);
                    
                    toast.success('Đã chọn ảnh thành công!');
                  }
                }}
                className="hidden"
              />
            </label>
            <div className="max-w-xs">
              <input
                type="text"
                value={formData.title}
                onChange={(e) => handleInputChange('title', e.target.value)}
                placeholder="Tên sự kiện *"
                className="text-xl font-bold text-gray-900 border-b-2 border-blue-500 focus:outline-none bg-transparent w-full"
                required
              />
            </div>
            <div className="flex items-center space-x-1 ml-2">
              <span className="text-yellow-500 text-sm">★</span>
              <input
                type="number"
                step="0.1"
                min="0"
                max="5"
                placeholder="0.0"
                className="font-semibold text-gray-900 text-sm w-12 border-b border-blue-500 focus:outline-none bg-transparent"
              />
              <span className="text-gray-400 text-xs">|</span>
              <input
                type="number"
                min="0"
                placeholder="0"
                className="text-gray-400 text-xs w-12 border-b border-blue-500 focus:outline-none bg-transparent"
              />
              <span className="text-gray-400 text-xs">reviews</span>
            </div>
          </div>
          <select
            value={selectedCategory}
            onChange={(e) => setSelectedCategory(e.target.value)}
            className="px-4 py-1.5 bg-blue-500 text-white text-sm font-medium rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-600"
          >
            <option value="">Chọn danh mục</option>
            <option value="Technology">Technology</option>
            <option value="Business">Business</option>
            <option value="Education">Education</option>
            <option value="Entertainment">Entertainment</option>
          </select>
        </div>

        {/* Event Info Cards */}
        <div className="grid grid-cols-4 gap-3">
          <div className="bg-white border border-gray-200 rounded-lg p-3 shadow-sm">
            <div className="flex items-center space-x-2 text-blue-500 mb-1.5">
              <Calendar size={16} />
              <span className="text-xs font-medium">Start Date *</span>
            </div>
            <input
              type="date"
              value={formData.eventDate}
              onChange={(e) => handleInputChange('eventDate', e.target.value)}
              className="text-gray-900 font-semibold text-sm w-full border-b border-blue-500 focus:outline-none bg-transparent"
              required
            />
          </div>
          
          <div className="bg-white border border-gray-200 rounded-lg p-3 shadow-sm">
            <div className="flex items-center space-x-2 text-blue-500 mb-1.5">
              <MapPin size={16} />
              <span className="text-xs font-medium">Location *</span>
            </div>
            <input
              type="text"
              value={formData.location}
              onChange={(e) => handleInputChange('location', e.target.value)}
              placeholder="Convention Center"
              className="text-gray-900 font-semibold text-sm w-full border-b border-blue-500 focus:outline-none bg-transparent"
              required
            />
          </div>
          
          <div className="bg-white border border-gray-200 rounded-lg p-3 shadow-sm">
            <div className="flex items-center space-x-2 text-blue-500 mb-1.5">
              <Users size={16} />
              <span className="text-xs font-medium">Attendees</span>
            </div>
            <input
              type="text"
              value={formData.expectedAttendees}
              onChange={(e) => handleInputChange('expectedAttendees', e.target.value)}
              placeholder="2,500+ Professional"
              className="text-gray-900 font-semibold text-sm w-full border-b border-blue-500 focus:outline-none bg-transparent"
            />
          </div>
          
          <div className="bg-white border border-gray-200 rounded-lg p-3 shadow-sm">
            <div className="flex items-center space-x-2 text-blue-500 mb-1.5">
              <Building2 size={16} />
              <span className="text-xs font-medium">Industry</span>
            </div>
            <input
              type="text"
              value={formData.eventType}
              onChange={(e) => handleInputChange('eventType', e.target.value)}
              placeholder="Technology & Innovation"
              className="text-gray-900 font-semibold text-sm w-full border-b border-blue-500 focus:outline-none bg-transparent"
            />
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div className="relative bg-white rounded-2xl p-6 shadow-xl border mt-10 border-gray-200">
        {/* Save/Cancel Buttons - Top Right Corner */}
        <div className="absolute top-4 right-4 flex items-center space-x-2">
          <button 
            className="px-4 py-2 bg-green-500 hover:bg-green-600 text-white text-sm font-medium rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            onClick={handleSave}
            disabled={isSaving}
          >
            {isSaving ? 'Đang lưu...' : 'Lưu sự kiện'}
          </button>
          <button 
            className="px-4 py-2 bg-gray-200 hover:bg-gray-300 text-gray-700 text-sm font-medium rounded-lg transition-colors"
            onClick={handleCancel}
          >
            Hủy
          </button>
        </div>

        {/* Tab Navigation - Inside Card */}
        <div className="flex justify-center absolute -top-4 left-1/2 transform -translate-x-1/2">
          <button className="px-6 py-1 bg-blue-400 text-white text-lg font-semibold rounded-full shadow-md">
            Tạo sự kiện mới
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
          <textarea
            value={formData.description}
            onChange={(e) => handleInputChange('description', e.target.value)}
            placeholder="Mô tả về sự kiện của bạn..."
            className="w-full text-gray-600 text-xs leading-relaxed pl-4 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-blue-500 resize-none"
            rows={4}
          />
        </div>

        {/* What to Expect */}
        <div className="mb-4">
          <h2 className="text-sm font-bold text-gray-900 mb-3 text-left">What to Expect:</h2>
          <div className="grid grid-cols-3 gap-x-3 gap-y-1.5 pl-15">
            {formData.eventHighlights.map((item, index) => (
              <div key={index} className="flex items-start space-x-1.5 min-w-0 max-w-full">
                <span className="w-1 h-1 bg-blue-500 rounded-full flex-shrink-0 mt-1"></span>
                <div className="flex items-center space-x-1 flex-1 min-w-0">
                  <input
                    type="text"
                    value={item}
                    onChange={(e) => handleArrayItemChange('eventHighlights', index, e.target.value)}
                    placeholder="Nhập nội dung..."
                    className="text-gray-700 text-xs border-b border-gray-300 focus:border-blue-500 focus:outline-none flex-1 min-w-0"
                  />
                  <button
                    onClick={() => removeArrayItem('eventHighlights', index)}
                    className="text-red-500 hover:text-red-700 flex-shrink-0"
                  >
                    <X size={12} />
                  </button>
                </div>
              </div>
            ))}
            <button
              onClick={() => addArrayItem('eventHighlights')}
              className="text-blue-500 text-xs hover:text-blue-700 flex items-center space-x-1"
            >
              <span>+ Thêm mục</span>
            </button>
          </div>
        </div>

        {/* Target Audience */}
        <div className="mb-7 text-left">
          <h2 className="text-sm font-bold text-gray-900 mb-2">Target Audience:</h2>
          <div className="grid grid-cols-2 gap-x-3 gap-y-1.5 pl-15">
            {formData.targetAudienceList.map((item, index) => (
              <div key={index} className="flex items-start space-x-1.5 min-w-0 max-w-full">
                <span className="w-1 h-1 bg-blue-500 rounded-full flex-shrink-0 mt-1"></span>
                <div className="flex items-center space-x-1 flex-1 min-w-0">
                  <input
                    type="text"
                    value={item}
                    onChange={(e) => handleArrayItemChange('targetAudienceList', index, e.target.value)}
                    placeholder="Nhập đối tượng..."
                    className="text-gray-700 text-xs border-b border-gray-300 focus:border-blue-500 focus:outline-none flex-1 min-w-0"
                  />
                  <button
                    onClick={() => removeArrayItem('targetAudienceList', index)}
                    className="text-red-500 hover:text-red-700 flex-shrink-0"
                  >
                    <X size={12} />
                  </button>
                </div>
              </div>
            ))}
            <button
              onClick={() => addArrayItem('targetAudienceList')}
              className="text-blue-500 text-xs hover:text-blue-700 flex items-center space-x-1"
            >
              <span>+ Thêm mục</span>
            </button>
          </div>
        </div>

        {/* Tags */}
        <div className="flex flex-wrap gap-2">
          {formData.tags.map((tag, index) => (
            <div key={index} className="flex items-center space-x-1 px-3 py-1 bg-blue-50 rounded-full">
              <input
                type="text"
                value={tag}
                onChange={(e) => handleArrayItemChange('tags', index, e.target.value)}
                placeholder="Tag..."
                className="bg-transparent text-blue-500 text-xs font-medium focus:outline-none w-24"
              />
              <button
                onClick={() => removeArrayItem('tags', index)}
                className="text-red-500 hover:text-red-700 flex-shrink-0"
              >
                <X size={12} />
              </button>
            </div>
          ))}
          <button
            onClick={() => addArrayItem('tags')}
            className="px-3 py-1 bg-gray-100 text-gray-600 text-xs font-medium rounded-full hover:bg-gray-200 transition-colors flex-shrink-0"
          >
            + Thêm tag
          </button>
        </div>
      </div>

      <h2 className="text-xl ml-2 mt-6 mb-3 font-semibold text-gray-900">Timeline</h2>

      {/* Timeline Section */}
      <div className="mt-3 shadow-xl">
        <EventTimeline 
          initialRows={timelineRows}
          isCreateMode={true}
          onChange={handleTimelineChange}
        />
      </div>

      {/* Sponsorship Budget and Event Information Section */}
      <div className="mt-5 grid grid-cols-1 lg:grid-cols-3 gap-3">
        {/* Sponsorship Budget Card */}
        <div className="bg-white rounded-xl p-4 shadow-xl border border-gray-200">
          <div className="flex items-center space-x-2 mb-3">
            <div className="w-8 h-8 bg-blue-500 rounded-lg flex items-center justify-center">
              <Info size={16} className="text-white" />
            </div>
            <h3 className="text-sm font-semibold text-gray-900">Sponsorship Budget</h3>
          </div>
          
          <div className="mb-3">
            <input
              type="number"
              value={formData.totalBudget}
              onChange={(e) => handleInputChange('totalBudget', e.target.value)}
              placeholder="0"
              className="text-xl font-bold text-blue-500 border-b-2 border-blue-500 focus:outline-none bg-transparent w-full"
            />
            <span className="text-sm text-gray-500 ml-1">VND</span>
          </div>
        </div>

        {/* Event Information Card */}
        <div className="bg-white shadow-xl rounded-xl p-4 border border-gray-200 lg:col-span-2">
          <div className="flex items-center space-x-2 mb-4">
            <div className="w-8 h-8 bg-blue-500 rounded-lg flex items-center justify-center">
              <Info size={16} className="text-white" />
            </div>
            <h3 className="text-sm font-semibold text-gray-900">Event Information</h3>
          </div>
          
          <div className="space-y-3 mr-3">
            {/* First Row: Event Manager and Email */}
            <div className="grid grid-cols-2 gap-4">
              <div className="flex items-center space-x-2 min-w-0">
                <div className="w-8 h-8 bg-blue-50 rounded-lg flex items-center justify-center flex-shrink-0">
                  <User size={14} className="text-blue-500" />
                </div>
                <div className="min-w-0 flex-1">
                  <p className="text-xs font-semibold text-gray-900">Event Manager</p>
                  <input
                    type="text"
                    value={formData.shortDescription}
                    onChange={(e) => handleInputChange('shortDescription', e.target.value)}
                    placeholder="Tên người quản lý"
                    className="text-xs text-gray-600 w-full border-b border-blue-500 focus:outline-none bg-transparent"
                  />
                </div>
              </div>

              <div className="flex items-center space-x-2 min-w-0">
                <div className="w-8 h-8 bg-blue-50 rounded-lg flex items-center justify-center flex-shrink-0">
                  <MapPin size={14} className="text-blue-500" />
                </div>
                <div className="min-w-0 flex-1">
                  <p className="text-xs font-semibold text-gray-900">Email</p>
                  <input
                    type="text"
                    value={formData.venueDetails}
                    onChange={(e) => handleInputChange('venueDetails', e.target.value)}
                    placeholder="email@example.com"
                    className="text-xs text-gray-600 w-full border-b border-blue-500 focus:outline-none bg-transparent"
                  />
                </div>
              </div>
            </div>

            {/* Second Row: Phone and Website */}
            <div className="grid grid-cols-2 gap-4">
              <div className="flex items-center space-x-2 min-w-0">
                <div className="w-8 h-8 bg-blue-50 rounded-lg flex items-center justify-center flex-shrink-0">
                  <Info size={14} className="text-blue-500" />
                </div>
                <div className="min-w-0 flex-1">
                  <p className="text-xs font-semibold text-gray-900">Phone</p>
                  <input
                    type="tel"
                    value={formData.requiredServices}
                    onChange={(e) => handleInputChange('requiredServices', e.target.value)}
                    placeholder="+84 123 456 789"
                    className="text-xs text-gray-600 w-full border-b border-blue-500 focus:outline-none bg-transparent"
                  />
                </div>
              </div>

              <div className="flex items-center space-x-2 min-w-0">
                <div className="w-8 h-8 bg-blue-50 rounded-lg flex items-center justify-center flex-shrink-0">
                  <Building2 size={14} className="text-blue-500" />
                </div>
                <div className="min-w-0 flex-1">
                  <p className="text-xs font-semibold text-gray-900">Website</p>
                  <input
                    type="url"
                    value={formData.sponsorshipNeeds}
                    onChange={(e) => handleInputChange('sponsorshipNeeds', e.target.value)}
                    placeholder="example.com"
                    className="text-xs text-gray-600 w-full border-b border-blue-500 focus:outline-none bg-transparent"
                  />
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default EventCreate;
