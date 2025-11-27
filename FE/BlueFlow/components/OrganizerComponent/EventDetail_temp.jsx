import React, { useState, useEffect } from 'react';
import { Calendar, MapPin, Users, Building2, ChevronDown, Search, Info, User, Mail, Phone, Globe, Pencil, Check, X, ArrowLeft, Upload } from 'lucide-react';
import toast from 'react-hot-toast';
import EventTimeline from './EventTimeline';
import { useEvent } from '../../hooks/useEvent';
import { usePartnership } from '../../hooks/usePartnership';
import { useAuth } from '../../contexts/AuthContext';
import eventService from '../../services/eventService';
import Loading from '../Loading';

const EventDetail = ({ eventId, onBack }) => {
  const { getActivities } = useEvent();
  const { createPartnership, updatePartnershipStatus, saving: savingPartnership } = usePartnership();
  const { user } = useAuth();
  const [isEditing, setIsEditing] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState('Technology'); // UI only
  const [timelineRows, setTimelineRows] = useState([]);
  const [originalTimelineRows, setOriginalTimelineRows] = useState([]); // Store original for comparison
  const [timeSlots, setTimeSlots] = useState(null); // Dynamic time slots from activities
  const [loadingTimeline, setLoadingTimeline] = useState(true);
  const [loadingEvent, setLoadingEvent] = useState(true);
  const [partnershipId, setPartnershipId] = useState(null); // Store partnership ID for status updates
  
  // Using same schema as EventCreate for API consistency
  const [editedData, setEditedData] = useState({
    title: 'TechEvents',
    description: 'Join us for the most anticipated technology conference of 2024! The Tech Innovation Summit connects industry leaders and startups to explore AI, blockchain, and future technologies',
    shortDescription: 'Jessica Miller',
    eventDate: 'March 15-17, 2024',
    endDate: 'March 17, 2024',
    location: 'San Francisco Convention Center',
    venueDetails: 'events@techpro.com',
    totalBudget: 50000000,
    expectedAttendees: '2,500+ Professional',
    category: 'Technology',
    eventType: 'Technology & Innovation',
    targetAudience: '',
    requiredServices: '+1 (555) 123-4567',
    sponsorshipNeeds: 'techsummit2024.com',
    specialRequirements: '',
    isPublic: false,
    eventHighlights: [
      '50+ Expert Speakers',
      'Interactive Workshops',
      'Networking Opportunities',
      'Startup Showcase',
      'Exhibition Hall'
    ],
    tags: [
      'Artificial Intelligence',
      'Machine Learning',
      'Blockchain',
      'Startups',
      'Innovation',
      'Networking'
    ],
    targetAudienceList: [
      'Tech executives & software developers',
      'Entrepreneurs & investors',
      'Fintech, healthcare, e-commerce leaders',
      'Future-focused innovators & students'
    ]
  });

  const [coverImageUrl, setCoverImageUrl] = useState('');
  const [coverImageFile, setCoverImageFile] = useState(null);
  
  // For display only - not part of API request
  const [displayInfo] = useState({
    rating: 4.8,
    reviews: 52,
    status: 'Ongoing'
  });

  // Helper functions for time conversion
  const convertToTimeSpan = (time12) => {
    if (!time12) return '08:00';
    const match = time12.match(/(\d{1,2}):(\d{2})\s*(AM|PM)/i);
    if (!match) return '08:00';
    
    let hours = parseInt(match[1]);
    const minutes = parseInt(match[2]);
    const period = match[3].toUpperCase();
    
    if (period === 'PM' && hours !== 12) hours += 12;
    if (period === 'AM' && hours === 12) hours = 0;
    
    return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}`;
  };
  
  const calculateEndTime = (time12, durationSlots) => {
    const startTimeSpan = convertToTimeSpan(time12);
    const [hours, minutes] = startTimeSpan.split(':').map(n => parseInt(n));
    const durationMinutes = durationSlots * 30;
    const totalMinutes = hours * 60 + minutes + durationMinutes;
    const endHours = Math.floor(totalMinutes / 60);
    const endMinutes = totalMinutes % 60;
    return `${endHours.toString().padStart(2, '0')}:${endMinutes.toString().padStart(2, '0')}`;
  };

  const handleCoverImageChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      // Validate file size (max 5MB)
      if (file.size > 5 * 1024 * 1024) {
        toast.error('Kích thước file không được vượt quá 5MB');
        return;
      }

      // Validate file type
      if (!file.type.startsWith('image/')) {
        toast.error('Vui lòng chọn file hình ảnh');
        return;
      }

      setCoverImageFile(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setCoverImageUrl(reader.result);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleSave = async () => {
    console.log('═══════════════════════════════════════════════════');
    console.log('💾 SAVE BUTTON - UPDATE EVENT INFO ONLY');
    console.log('═══════════════════════════════════════════════════');
    
    try {
      // Create FormData to match backend expectations
      const formDataToSend = new FormData();
      
      // Append all fields with PascalCase names (matching backend C# model)
      formDataToSend.append('Title', editedData.title);
      formDataToSend.append('Description', editedData.description || '');
      formDataToSend.append('ShortDescription', editedData.shortDescription || '');
      formDataToSend.append('EventDate', editedData.eventDate);
      formDataToSend.append('EndDate', editedData.endDate || editedData.eventDate);
      formDataToSend.append('Location', editedData.location);
      formDataToSend.append('VenueDetails', editedData.venueDetails || '');
      formDataToSend.append('TotalBudget', parseFloat(editedData.totalBudget) || 0);
      formDataToSend.append('ExpectedAttendees', parseInt(editedData.expectedAttendees) || 0);
      formDataToSend.append('Category', editedData.category || '');
      formDataToSend.append('EventType', editedData.eventType || '');
      formDataToSend.append('TargetAudience', editedData.targetAudience || '');
      formDataToSend.append('RequiredServices', editedData.requiredServices || '');
      formDataToSend.append('SponsorshipNeeds', editedData.sponsorshipNeeds || '');
      formDataToSend.append('SpecialRequirements', editedData.specialRequirements || '');
      formDataToSend.append('IsPublic', editedData.isPublic);
      
      // Convert arrays to comma-separated strings
      const eventHighlightsArray = editedData.eventHighlights.filter(item => item.trim());
      const tagsArray = editedData.tags.filter(tag => tag.trim());
      const targetAudienceArray = editedData.targetAudienceList.filter(item => item.trim());
      
      formDataToSend.append('EventHighlights', eventHighlightsArray.join(','));
      formDataToSend.append('Tags', tagsArray.join(','));
      formDataToSend.append('TargetAudienceList', targetAudienceArray.join(','));
      
      // Upload cover image file if changed
      if (coverImageFile) {
        formDataToSend.append('CoverImage', coverImageFile);
      } else {
        formDataToSend.append('CoverImage', coverImageUrl || '');
      }
      
      formDataToSend.append('ViewCount', 0);
      formDataToSend.append('InterestedCount', 0);
      
      console.log('� Sending FormData to API');
      console.log('🔄 Calling eventService.updateEvent...');
      
      const eventResult = await eventService.updateEvent(eventId, formDataToSend);
      
      console.log('📥 Event update result:', eventResult);
      console.log('  ├─ success:', eventResult.success);
      console.log('  ├─ message:', eventResult.message);
      console.log('  ├─ data:', eventResult.data);
      console.log('  └─ status:', eventResult.status);
      
      if (!eventResult.success) {
        console.error('❌ Event update FAILED!');
        console.error('   Error message:', eventResult.message);
        console.error('   Full response:', JSON.stringify(eventResult, null, 2));
        
        toast.error(eventResult.message || 'Không thể cập nhật event');
        return;
      }
      
      console.log('✅ Event updated successfully!');
      
      console.log('\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━');
      console.log('📦 STEP 2: UPDATE ACTIVITIES');
      console.log('━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━');
      
      // 2. Update Activities in timeline
      if (timelineRows && timelineRows.length > 0) {
        console.log(`📋 Processing ${timelineRows.length} timeline rows...`);
        let activityCount = 0;
        
        for (let rowIndex = 0; rowIndex < timelineRows.length; rowIndex++) {
          const row = timelineRows[rowIndex];
          console.log(`\n─────────────────────────────────────────────────`);
          console.log(`📌 Row ${rowIndex + 1}/${timelineRows.length}: "${row.title}"`);
          console.log(`   Events in row: ${row.events?.length || 0}`);
          
          if (row.events && row.events.length > 0) {
            for (let eventIndex = 0; eventIndex < row.events.length; eventIndex++) {
              const event = row.events[eventIndex];
              console.log(`\n  ┌─ Activity ${eventIndex + 1}/${row.events.length}`);
              console.log(`  │  ID: ${event.id}`);
              console.log(`  │  Description: ${event.description}`);
              console.log(`  │  Time: ${event.time}`);
              console.log(`  │  Duration: ${event.duration} slots`);
              
              const startTime = convertToTimeSpan(event.time);
              const endTime = calculateEndTime(event.time, event.duration || 1);
              
              console.log(`  │  Converted startTime: ${startTime}`);
              console.log(`  │  Calculated endTime: ${endTime}`);
              
              const activityData = {
                activityName: event.description || 'Untitled Activity',
                activityDescription: row.title || 'Untitled Row',
                startTime: startTime,
                endTime: endTime,
                location: event.location || editedData.location,
                speakers: event.speakers || '',
                activityType: row.title || 'general',
                maxParticipants: event.maxParticipants || 100000,
                isPublic: event.isPublic !== undefined ? event.isPublic : false,
                displayOrder: activityCount
              };
              
              console.log(`  │  Activity data:`, JSON.stringify(activityData, null, 2));
              console.log(`  └─ Calling updateActivity(${eventId}, ${event.id}, ...)...`);
              
              const activityResult = await eventService.updateActivity(eventId, event.id, activityData);
              
              console.log(`  📥 Result:`, activityResult);
              console.log(`     ├─ success: ${activityResult.success}`);
              console.log(`     ├─ message: ${activityResult.message}`);
              console.log(`     └─ status: ${activityResult.status}`);
              
              if (activityResult.success) {
                activityCount++;
                console.log(`  ✅ Activity updated! (${activityCount} total)`);
              } else {
                console.error(`  ❌ Activity update FAILED!`);
                console.error(`     Error: ${activityResult.message}`);
                console.error(`     Full response:`, JSON.stringify(activityResult, null, 2));
              }
            }
          }
        }
        
        console.log(`\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`);
        console.log(`✅ Activities Update Complete!`);
        console.log(`   Total activities updated: ${activityCount}`);
        console.log(`━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`);
      } else {
        console.log('⚠️  No timeline rows to update');
      }
      
      // Success
      console.log('\n═══════════════════════════════════════════════════');
      console.log('✅ HANDLE SAVE - SUCCESS!');
      console.log('═══════════════════════════════════════════════════');
      
      toast.success('Đã cập nhật event thành công!');
      
      setIsEditing(false);
      
    } catch (error) {
      console.log('\n═══════════════════════════════════════════════════');
      console.error('❌❌❌ HANDLE SAVE - ERROR!!! ❌❌❌');
      console.log('═══════════════════════════════════════════════════');
      console.error('Error object:', error);
      console.error('Error name:', error.name);
      console.error('Error message:', error.message);
      console.error('Error stack:', error.stack);
      
      if (error.response) {
        console.error('API Response Error:');
        console.error('  Status:', error.response.status);
        console.error('  Data:', error.response.data);
        console.error('  Headers:', error.response.headers);
      }
      
      toast.error(error.message || 'Có lỗi xảy ra khi lưu thay đổi');
    }
  };

  // Timeline Done button handler: Update and Delete activities
  const handleTimelineDone = async (updatedRows) => {
    console.log('═══════════════════════════════════════════════════');
    console.log('✅ TIMELINE DONE - UPDATE/DELETE ACTIVITIES');
    console.log('═══════════════════════════════════════════════════');
    console.log('📌 Original rows:', originalTimelineRows);
    console.log('📌 Updated rows:', updatedRows);
    
    try {
      // Step 1: Find all activity IDs in original rows
      const originalActivityIds = new Set();
      originalTimelineRows.forEach(row => {
        if (row.events && row.events.length > 0) {
          row.events.forEach(event => {
            if (event.id) {
              originalActivityIds.add(event.id);
            }
          });
        }
      });
      
      // Step 2: Find all activity IDs in updated rows
      const updatedActivityIds = new Set();
      if (updatedRows && updatedRows.length > 0) {
        updatedRows.forEach(row => {
          if (row.events && row.events.length > 0) {
            row.events.forEach(event => {
              if (event.id) {
                updatedActivityIds.add(event.id);
              }
            });
          }
        });
      }
      
      // Step 3: Find deleted activities (in original but not in updated)
      const deletedActivityIds = [...originalActivityIds].filter(id => !updatedActivityIds.has(id));
      
      console.log('🔍 Original activity IDs:', [...originalActivityIds]);
      console.log('🔍 Updated activity IDs:', [...updatedActivityIds]);
      console.log('🗑️ Deleted activity IDs:', deletedActivityIds);
      
      let updateCount = 0;
      let deleteCount = 0;
      
      // Step 4: Delete removed activities
      if (deletedActivityIds.length > 0) {
        console.log(`\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`);
        console.log(`🗑️ DELETING ${deletedActivityIds.length} ACTIVITIES`);
        console.log(`━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`);
        
        for (const activityId of deletedActivityIds) {
          console.log(`  ┌─ Deleting activity ${activityId}`);
          
          const deleteResult = await eventService.deleteActivity(eventId, activityId);
          
          if (deleteResult.success) {
            deleteCount++;
            console.log(`  ✅ Deleted (${deleteCount} total)`);
          } else {
            console.error(`  ❌ Delete failed:`, deleteResult.message);
          }
        }
      }
      
      // Step 5: Create NEW activities (not in originalActivityIds)
      let createCount = 0;
      if (updatedRows && updatedRows.length > 0) {
        console.log(`\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`);
        console.log(`🆕 CREATING NEW ACTIVITIES`);
        console.log(`━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`);
        
        for (let rowIndex = 0; rowIndex < updatedRows.length; rowIndex++) {
          const row = updatedRows[rowIndex];
          
          if (row.events && row.events.length > 0) {
            for (const event of row.events) {
              // Check if this is a NEW activity (ID not in originalActivityIds)
              if (!originalActivityIds.has(event.id)) {
                const startTime = convertToTimeSpan(event.time);
                const endTime = calculateEndTime(event.time, event.duration || 1);
                
                const activityData = {
                  activityName: event.description || 'Untitled Activity',
                  activityDescription: row.title || 'Untitled Row',
                  startTime: startTime,
                  endTime: endTime,
                  location: event.location || editedData.location,
                  speakers: event.speakers || '',
                  activityType: row.title || 'general',
                  maxParticipants: event.maxParticipants || 100000,
                  isPublic: event.isPublic !== undefined ? event.isPublic : false,
                  displayOrder: createCount
                };
                
                console.log(`  ┌─ Creating NEW activity`);
                console.log(`  │  Temp ID: ${event.id}`);
                console.log(`  │  ${event.time} - ${event.description}`);
                console.log(`  └─ Data:`, activityData);
                
                const createResult = await eventService.createActivity(eventId, activityData);
                
                if (createResult.success) {
                  createCount++;
                  console.log(`  ✅ Created (${createCount} total)`);
                } else {
                  console.error(`  ❌ Create failed:`, createResult.message);
                }
              }
            }
          }
        }
      }
      
      // Step 6: Update EXISTING activities
      if (updatedRows && updatedRows.length > 0) {
        console.log(`\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`);
        console.log(`🔄 UPDATING EXISTING ACTIVITIES`);
        console.log(`━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`);
        
        for (let rowIndex = 0; rowIndex < updatedRows.length; rowIndex++) {
          const row = updatedRows[rowIndex];
          console.log(`\n📌 Row ${rowIndex + 1}: "${row.title}"`);
          
          if (row.events && row.events.length > 0) {
            for (const event of row.events) {
              // Only UPDATE if this is an EXISTING activity (ID in originalActivityIds)
              if (originalActivityIds.has(event.id)) {
                const startTime = convertToTimeSpan(event.time);
                const endTime = calculateEndTime(event.time, event.duration || 1);
                
                const activityData = {
                  activityName: event.description || 'Untitled Activity',
                  activityDescription: row.title || 'Untitled Row',
                  startTime: startTime,
                  endTime: endTime,
                  location: event.location || editedData.location,
                  speakers: event.speakers || '',
                  activityType: row.title || 'general',
                  maxParticipants: event.maxParticipants || 100000,
                  isPublic: event.isPublic !== undefined ? event.isPublic : false,
                  displayOrder: updateCount
                };
                
                console.log(`  ┌─ Updating activity ${event.id}`);
                console.log(`  │  ${event.time} - ${event.description}`);
                console.log(`  └─ Data:`, activityData);
                
                const activityResult = await eventService.updateActivity(eventId, event.id, activityData);
                
                if (activityResult.success) {
                  updateCount++;
                  console.log(`  ✅ Updated (${updateCount} total)`);
                } else {
                  console.error(`  ❌ Update failed:`, activityResult.message);
                }
              }
            }
          }
        }
      }
      
      console.log(`\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`);
      console.log(`✅ SUMMARY`);
      console.log(`   - Created: ${createCount} activities`);
      console.log(`   - Updated: ${updateCount} activities`);
      console.log(`   - Deleted: ${deleteCount} activities`);
      console.log(`━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`);
      
      toast.success(`Đã tạo ${createCount}, cập nhật ${updateCount} và xóa ${deleteCount} hoạt động`);
      
      // Update local state
      setTimelineRows(updatedRows);
      setOriginalTimelineRows(JSON.parse(JSON.stringify(updatedRows))); // Update original for next comparison
      
    } catch (error) {
      console.error('❌ Timeline update error:', error);
      toast.error('Có lỗi xảy ra khi cập nhật timeline');
    }
  };

  const handleCancel = () => {
    // Reset to original data if needed
    setIsEditing(false);
  };

  // Handle Public/Private change and create/update partnership
  const handlePublicPrivateChange = async (newValue) => {
    const isPublic = newValue === 'Public';
    
    if (!user) {
      toast.error('Vui lòng đăng nhập để thực hiện thao tác này');
      return;
    }
    
    if (isPublic) {
      try {
        if (editedData.isFeatured === false) {
          const partnershipData = {
            eventId: eventId,
            partnerId: user.id,
            partnerType: user.role,
            initialMessage: null,
            proposedBudget: editedData.totalBudget || 0,
            serviceDescription: editedData.description || '',
            preferredContactMethod: null,
            organizerContactInfo: editedData.requiredServices || '',
            startDate: null,
            deadlineDate: null,
            partnershipImageFile: coverImageUrl
          };
          
          const partnershipResult = await createPartnership(partnershipData);
          
          if (partnershipResult.success) {
            if (partnershipResult.data?.id) {
              setPartnershipId(partnershipResult.data.id);
            }
            
            const statusResult = await eventService.updateEventStatus(eventId);
            
            if (statusResult.success) {
              setEditedData(prev => ({ ...prev, isFeatured: true }));
              toast.success('Đã tạo partnership và cập nhật trạng thái event thành công!');
            } else {
              toast.warning('Partnership đã tạo nhưng không thể cập nhật trạng thái event');
            }
          } else {
            toast.error('Không thể tạo partnership');
          }
          
        } else if (editedData.isFeatured === true) {
          const partnershipResult = await updatePartnershipStatus(eventId);
          
          if (partnershipResult.success) {
            const visibilityResult = await eventService.updateEventVisibility(eventId);
            
            if (visibilityResult.success) {
              toast.success('Đã cập nhật partnership status và event visibility thành công!');
            } else {
              toast.warning('Partnership đã cập nhật nhưng không thể cập nhật event visibility');
            }
          } else {
            toast.error('Không thể cập nhật trạng thái partnership');
          }
        }
        
      } catch (error) {
        console.error('Error during public process:', error);
        toast.error('Có lỗi xảy ra khi xử lý');
      }
    } 
    else if (!isPublic) {
      try {
        if (editedData.isFeatured === true) {
          const partnershipResult = await updatePartnershipStatus(eventId);
          
          if (partnershipResult.success) {
            const visibilityResult = await eventService.updateEventVisibility(eventId);
            
            if (visibilityResult.success) {
              toast.success('Đã cập nhật partnership status và event visibility thành công!');
            } else {
              toast.warning('Partnership đã cập nhật nhưng không thể cập nhật event visibility');
            }
          } else {
            toast.error('Không thể cập nhật trạng thái partnership');
          }
          
        } else {
          const visibilityResult = await eventService.updateEventVisibility(eventId);
          
          if (visibilityResult.success) {
            toast.success('Đã cập nhật trạng thái hiển thị event thành công!');
          } else {
            toast.error('Không thể cập nhật trạng thái hiển thị event');
          }
        }
        
      } catch (error) {
        console.error('Error during private process:', error);
        toast.error('Có lỗi xảy ra khi xử lý');
      }
    }
  };
      ...prev,
      [field]: prev[field].filter((_, i) => i !== index)
    }));
  };

  // Load event data from API when component mounts
  useEffect(() => {
    const loadEvent = async () => {
      if (!eventId) return;

      setLoadingEvent(true);
      try {
        const result = await eventService.getEvent(eventId);
        
        if (result.success && result.data) {
          const event = result.data;
          
          // Helper to format date from API (removes time part)
          const formatDateOnly = (dateString) => {
            if (!dateString) return '';
            // If it's ISO format like "2025-11-12T17:00:00", extract date part
            return dateString.split('T')[0];
          };
          
          // Map API data to form fields
          setEditedData({
            title: event.title || '',
            description: event.description || '',
            shortDescription: event.shortDescription || '',
            eventDate: formatDateOnly(event.eventDate),
            endDate: formatDateOnly(event.endDate),
            location: event.location || '',
            venueDetails: event.venueDetails || '',
            totalBudget: event.totalBudget || 0,
            expectedAttendees: event.expectedAttendees || 0,
            category: event.category || '',
            eventType: event.eventType || '',
            targetAudience: event.targetAudience || '',
            requiredServices: event.requiredServices || '',
            sponsorshipNeeds: event.sponsorshipNeeds || '',
            specialRequirements: event.specialRequirements || '',
            isPublic: event.isPublic !== undefined ? event.isPublic : false,
            isFeatured: event.isFeatured !== undefined ? event.isFeatured : false,
            // Ensure arrays are properly initialized (same as EventCreate)
            eventHighlights: Array.isArray(event.eventHighlights) ? event.eventHighlights : [],
            tags: Array.isArray(event.tags) ? event.tags : [],
            targetAudienceList: Array.isArray(event.targetAudienceList) ? event.targetAudienceList : []
          });
          
          // Load cover image if available
          if (event.coverImageUrl) {
            setCoverImageUrl(event.coverImageUrl);
          }
          
          setSelectedCategory(event.category || 'Technology');
        } else {
          toast.error('Không thể tải thông tin event');
        }
      } catch (error) {
        console.error('Load event error:', error);
      } finally {
        setLoadingEvent(false);
      }
    };

    loadEvent();
  }, [eventId]);

  // Load timeline from API when component mounts
  useEffect(() => {
    const loadTimeline = async () => {
      if (!eventId) return;

      setLoadingTimeline(true);
      const result = await getActivities(eventId);

      console.log('=================================');
      console.log('📡 ACTIVITIES - API RESPONSE');
      console.log('=================================');
      console.log('🔹 Full result:', JSON.stringify(result, null, 2));
      console.log('🔹 result.success:', result.success);
      console.log('🔹 result.data:', result.data);
      console.log('🔹 result.data type:', typeof result.data);
      console.log('🔹 Is result.data an array?:', Array.isArray(result.data));
      
      // Backend returns: { success: true, data: { data: [...], count: 4 } }
      // So we need to access result.data.data to get the actual array
      const actualData = result.data?.data || result.data;
      console.log('🔹 actualData:', actualData);
      console.log('🔹 Is actualData an array?:', Array.isArray(actualData));
      console.log('=================================');

      if (result.success && actualData) {
        // Ensure actualData is an array before sorting
        const activitiesData = Array.isArray(actualData) ? actualData : [];
        console.log('✅ Activities array length:', activitiesData.length);
        
        // Generate dynamic time slots from activities
        const generateTimeSlots = (activities) => {
          if (!activities || activities.length === 0) return null;
          
          const allTimes = new Set();
          
          activities.forEach(activity => {
            if (activity.startTime) {
              const [hours, minutes] = activity.startTime.split(':').map(n => parseInt(n));
              allTimes.add(hours * 60 + minutes);
            }
            if (activity.endTime) {
              const [hours, minutes] = activity.endTime.split(':').map(n => parseInt(n));
              allTimes.add(hours * 60 + minutes);
            }
          });
          
          if (allTimes.size === 0) return null;
          
          const minTime = Math.min(...Array.from(allTimes));
          const maxTime = Math.max(...Array.from(allTimes));
          
          // Round down to nearest 30 minutes for start
          const startMinutes = Math.floor(minTime / 30) * 30;
          // Round up to nearest 30 minutes for end
          const endMinutes = Math.ceil(maxTime / 30) * 30;
          
          const slots = [];
          for (let m = startMinutes; m <= endMinutes; m += 30) {
            const hours = Math.floor(m / 60);
            const minutes = m % 60;
            const period = hours >= 12 ? 'PM' : 'AM';
            const hour12 = hours === 0 ? 12 : hours > 12 ? hours - 12 : hours;
            slots.push(`${hour12}:${minutes.toString().padStart(2, '0')}\n${period}`);
          }
          
          console.log('🕒 Generated time slots:', slots);
          return slots;
        };
        
        const generatedTimeSlots = generateTimeSlots(activitiesData);
        setTimeSlots(generatedTimeSlots);
        
        // Helper function to convert 24-hour time to 12-hour format with AM/PM (exact conversion)
        const convertTo12Hour = (time24) => {
          if (!time24) return '8:30 AM';
          
          // Parse "08:00" or "08:00:00" format
          const [hours, minutes] = time24.split(':').map(n => parseInt(n));
          const period = hours >= 12 ? 'PM' : 'AM';
          const hour12 = hours === 0 ? 12 : hours > 12 ? hours - 12 : hours;
          const minuteStr = minutes.toString().padStart(2, '0');
          
          return `${hour12}:${minuteStr} ${period}`;
        };
        
        // Helper function to calculate duration in time slots (each slot = 30 minutes)
        const calculateDuration = (startTime, endTime) => {
          if (!startTime || !endTime) return 1;
          
          const [startHours, startMinutes] = startTime.split(':').map(n => parseInt(n));
          const [endHours, endMinutes] = endTime.split(':').map(n => parseInt(n));
          
          const startTotalMinutes = startHours * 60 + startMinutes;
          const endTotalMinutes = endHours * 60 + endMinutes;
          
          const durationMinutes = endTotalMinutes - startTotalMinutes;
          const durationSlots = durationMinutes / 30; // Each slot is 30 minutes
          
          return Math.max(0.5, durationSlots);
        };
        
        // Reconstruct timeline from activities
        // Sort by displayOrder to maintain the order
        const activities = activitiesData.sort((a, b) => (a.displayOrder || 0) - (b.displayOrder || 0));
        
        console.log('📋 Processing activities:', activities);
        
        // Group activities by activityType (row title)
        // In EventCreate: activityType = row.title (e.g., "HELLO", "RICE", "MEAT", "CORN")
        const rowMap = new Map();
        const reconstructedRows = [];

        activities.forEach(activity => {
          console.log('========================================');
          console.log('🔍 Processing Activity:', activity.activityName);
          console.log('  📅 activityType (row title):', activity.activityType);
          console.log('  ⏰ startTime (24h):', activity.startTime);
          console.log('  ⏰ endTime (24h):', activity.endTime);
          
          // activityType is the row title (e.g., "HELLO", "RICE")
          const rowTitle = activity.activityType || activity.activityDescription || 'Untitled';
          console.log('  📌 Row title will be:', rowTitle);
          
          // Create row if not exists
          if (!rowMap.has(rowTitle)) {
            const row = {
              id: `row-${rowTitle}-${Date.now()}`,
              title: rowTitle,
              events: []
            };
            rowMap.set(rowTitle, row);
            reconstructedRows.push(row);
          }
          
          // Convert API data to EventTimeline format
          const startTime12 = convertTo12Hour(activity.startTime);
          const duration = calculateDuration(activity.startTime, activity.endTime);
          
          console.log(`  ✅ Converted to EventTimeline format:`);
          console.log(`     - Original startTime: ${activity.startTime}`);
          console.log(`     - Converted to: ${startTime12}`);
          console.log(`     - Original endTime: ${activity.endTime}`);
          console.log(`     - Calculated duration: ${duration} slots (${duration * 30} minutes)`);
          console.log(`     - Event description: "${activity.activityName || 'New Event'}"`);
          console.log('========================================');
          
          // Add event to row (EventTimeline format)
          rowMap.get(rowTitle).events.push({
            id: activity.id,
            time: startTime12,  // EventTimeline uses "time" not "startTime"
            description: activity.activityName || 'New Event',  // EventTimeline uses "description" for event title
            duration: duration  // EventTimeline uses duration in slots
          });
        });

        setTimelineRows(reconstructedRows);
        setOriginalTimelineRows(JSON.parse(JSON.stringify(reconstructedRows))); // Deep copy for comparison
      }

      setLoadingTimeline(false);
    };

    loadTimeline();
  }, [eventId, getActivities]);

  if (loadingEvent) {
    return <Loading message="Đang tải thông tin event..." />;
  }

  return (
    <div className="min-h-screen  p-6 max-w-full overflow-hidden">
      {/* Back Button */}
      {onBack && (
        <button
          onClick={onBack}
          className="mb-4 flex items-center space-x-2 text-gray-600 hover:text-gray-900 transition-colors"
        >
          <ArrowLeft className="w-5 h-5" />
          <span className="font-medium">Quay lại</span>
        </button>
      )}
      
      {/* Event Header */}
      <div className="mb-4">
        {/* Top Row - Logo, Name, Rating, Button */}
        <div className="flex items-center justify-between mb-4">
          <div className="flex items-center space-x-3">
            <div className="relative group">
              <div className="w-10 h-10 bg-black rounded-lg flex items-center justify-center overflow-hidden cursor-pointer">
                {coverImageUrl ? (
                  <img src={coverImageUrl} alt="Cover" className="w-full h-full object-cover" />
                ) : (
                  <span className="text-white font-bold text-lg">M</span>
                )}
              </div>
              {isEditing && (
                <>
                  <input
                    type="file"
                    accept="image/*"
                    onChange={handleCoverImageChange}
                    className="hidden"
                    id="cover-upload"
                  />
                  <label
                    htmlFor="cover-upload"
                    className="absolute inset-0 flex items-center justify-center bg-black bg-opacity-50 opacity-0 group-hover:opacity-100 transition-opacity cursor-pointer rounded-lg"
                  >
                    <Upload size={14} className="text-white" />
                  </label>
                </>
              )}
            </div>
            <div className="max-w-xs">
              {isEditing ? (
                <input
                  type="text"
                  value={editedData.title}
                  onChange={(e) => handleInputChange('title', e.target.value)}
                  className="text-xl font-bold text-gray-900 border-b-2 border-blue-500 focus:outline-none bg-transparent w-full"
                  title={editedData.title}
                />
              ) : (
                <h1 className="text-xl font-bold text-gray-900 truncate" title={editedData.title}>{editedData.title}</h1>
              )}
            </div>
            <div className="flex items-center space-x-1 ml-2">
              <span className="text-yellow-500 text-sm">★</span>
              <span className="font-semibold text-gray-900 text-sm">{displayInfo.rating}</span>
              <span className="text-gray-400 text-xs">|</span>
              <span className="text-gray-400 text-xs">{displayInfo.reviews}</span>
              <span className="text-gray-400 text-xs">reviews</span>
            </div>
          </div>
          <div className="flex items-center space-x-2">
            {isEditing ? (
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
            ) : (
              <button className="px-4 py-1.5 bg-blue-500 hover:bg-blue-600 text-white text-sm font-medium rounded-lg flex items-center space-x-1 transition-colors">
                <span>{displayInfo.status}</span>
                <ChevronDown size={14} />
              </button>
            )}
            
            <select
              value={editedData.isPublic ? 'Public' : 'Private'}
              onChange={(e) => handlePublicPrivateChange(e.target.value)}
              disabled={savingPartnership}
              className="px-4 py-1.5 bg-green-500 text-white text-sm font-medium rounded-lg focus:outline-none focus:ring-2 focus:ring-green-600 cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <option value="Private">Private</option>
              <option value="Public">Public</option>
            </select>
          </div>
        </div>

        {/* Event Info Cards */}
        <div className="grid grid-cols-4 gap-3">
          <div className="bg-white border shadow-xl border-gray-200 rounded-lg p-3 shadow-sm">
            <div className="flex items-center space-x-2 text-blue-500 mb-1.5">
              <Calendar size={16} />
              <span className="text-xs font-medium">Date</span>
            </div>
            {isEditing ? (
              <input
                type="text"
                value={editedData.eventDate}
                onChange={(e) => handleInputChange('eventDate', e.target.value)}
                className="text-gray-900 font-semibold text-sm w-full border-b border-blue-500 focus:outline-none bg-transparent"
                placeholder="March 15-17, 2024"
                title={editedData.eventDate}
              />
            ) : (
              <p className="text-gray-900 font-semibold text-sm truncate" title={editedData.eventDate}>{editedData.eventDate}</p>
            )}
          </div>
          
          <div className="bg-white border shadow-xl border-gray-200 rounded-lg p-3 shadow-sm">
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
          
          <div className="bg-white border shadow-xl border-gray-200 rounded-lg p-3 shadow-sm">
            <div className="flex items-center space-x-2 text-blue-500 mb-1.5">
              <Users size={16} />
              <span className="text-xs font-medium">Attendees</span>
            </div>
            {isEditing ? (
              <input
                type="text"
                value={editedData.expectedAttendees}
                onChange={(e) => handleInputChange('expectedAttendees', e.target.value)}
                className="text-gray-900 font-semibold text-sm w-full border-b border-blue-500 focus:outline-none bg-transparent"
                placeholder="2,500+ Professional"
                title={editedData.expectedAttendees}
              />
            ) : (
              <p className="text-gray-900 font-semibold text-sm truncate" title={editedData.expectedAttendees}>{editedData.expectedAttendees}</p>
            )}
          </div>
          
          <div className="bg-white border shadow-xl border-gray-200 rounded-lg p-3 shadow-sm">
            <div className="flex items-center space-x-2 text-blue-500 mb-1.5">
              <Building2 size={16} />
              <span className="text-xs font-medium">Industry</span>
            </div>
            {isEditing ? (
              <input
                type="text"
                value={editedData.eventType}
                onChange={(e) => handleInputChange('eventType', e.target.value)}
                className="text-gray-900 font-semibold text-sm w-full border-b border-blue-500 focus:outline-none bg-transparent"
                placeholder="Technology & Innovation"
                title={editedData.eventType}
              />
            ) : (
              <p className="text-gray-900 font-semibold text-sm truncate" title={editedData.eventType}>{editedData.eventType}</p>
            )}
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div className="relative bg-white rounded-2xl p-6 shadow-xl border mt-10 border-gray-200">
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
              value={editedData.description}
              onChange={(e) => handleInputChange('description', e.target.value)}
              className="w-full text-gray-600 text-xs leading-relaxed pl-4 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-blue-500 resize-none whitespace-pre-wrap break-words"
              rows={4}
            />
          ) : (
            <p className="text-gray-600 text-xs leading-relaxed pl-4 break-words whitespace-pre-wrap">
              {editedData.description}
            </p>
          )}
        </div>


        {/* What to Expect */}
        <div className="mb-4 ">
          <h2 className="text-sm font-bold text-gray-900 mb-3 text-left">What to Expect:</h2>
          <div className="grid grid-cols-3 gap-x-3 gap-y-1.5 pl-15">
            {editedData.eventHighlights && editedData.eventHighlights.length > 0 ? (
              editedData.eventHighlights.map((item, index) => (
                <div key={index} className="flex items-start space-x-1.5 min-w-0 max-w-full">
                  <span className="w-1 h-1 bg-blue-500 rounded-full flex-shrink-0 mt-1"></span>
                  {isEditing ? (
                    <div className="flex items-center space-x-1 flex-1 min-w-0">
                      <input
                        type="text"
                        value={item}
                        onChange={(e) => handleArrayItemChange('eventHighlights', index, e.target.value)}
                        className="text-gray-700 text-xs border-b border-gray-300 focus:border-blue-500 focus:outline-none flex-1 min-w-0"
                      />
                      <button
                        onClick={() => removeArrayItem('eventHighlights', index)}
                        className="text-red-500 hover:text-red-700 flex-shrink-0"
                      >
                        <X size={12} />
                      </button>
                    </div>
                  ) : (
                    <span className="text-gray-700 text-xs truncate block" title={item}>{item}</span>
                  )}
                </div>
              ))
            ) : (
              !isEditing && <p className="text-gray-400 text-xs col-span-3 italic">No highlights available</p>
            )}
            {isEditing && (
              <button
                onClick={() => addArrayItem('eventHighlights')}
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
            {editedData.targetAudienceList && editedData.targetAudienceList.length > 0 ? (
              editedData.targetAudienceList.map((item, index) => (
                <div key={index} className="flex items-start space-x-1.5 min-w-0 max-w-full">
                  <span className="w-1 h-1 bg-blue-500 rounded-full flex-shrink-0 mt-1"></span>
                  {isEditing ? (
                    <div className="flex items-center space-x-1 flex-1 min-w-0">
                      <input
                        type="text"
                        value={item}
                        onChange={(e) => handleArrayItemChange('targetAudienceList', index, e.target.value)}
                        className="text-gray-700 text-xs border-b border-gray-300 focus:border-blue-500 focus:outline-none flex-1 min-w-0"
                      />
                      <button
                        onClick={() => removeArrayItem('targetAudienceList', index)}
                        className="text-red-500 hover:text-red-700 flex-shrink-0"
                      >
                        <X size={12} />
                      </button>
                    </div>
                  ) : (
                    <span className="text-gray-700 text-xs truncate block" title={item}>{item}</span>
                  )}
                </div>
              ))
            ) : (
              !isEditing && <p className="text-gray-400 text-xs col-span-2 italic">No target audience specified</p>
            )}
            {isEditing && (
              <button
                onClick={() => addArrayItem('targetAudienceList')}
                className="text-blue-500 text-xs hover:text-blue-700 flex items-center space-x-1"
              >
                <span>+ Add item</span>
              </button>
            )}
          </div>
        </div>

        {/* Tags */}
        <div className="flex flex-wrap gap-2">
          {editedData.tags && editedData.tags.length > 0 ? (
            editedData.tags.map((tag, index) => (
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
            ))
          ) : (
            !isEditing && <p className="text-gray-400 text-xs italic">No tags available</p>
          )}
          {isEditing ? (
            <button
              onClick={() => addArrayItem('tags')}
              className="px-3 py-1 bg-gray-100 text-gray-600 text-xs font-medium rounded-full hover:bg-gray-200 transition-colors flex-shrink-0"
            >
              + Add tag
            </button>
          ) : (
            editedData.tags && editedData.tags.length > 0 && (
              <button className="px-3 py-1 bg-gray-100 text-gray-600 text-xs font-medium rounded-full hover:bg-gray-200 transition-colors flex-shrink-0">
                ...
              </button>
            )
          )}
        </div>
      </div>

      <h2 className="text-xl ml-2 mt-6 font-semibold text-gray-900 text-left">Timeline</h2>

      {/* Timeline Section */}
      <div className="mt-3  shadow-xl">
        {loadingTimeline ? (
          <Loading message="Đang tải timeline..." />
        ) : timelineRows.length > 0 ? (
          <EventTimeline 
            initialRows={timelineRows}
            initialTimeSlots={timeSlots}
            isCreateMode={false}
            eventId={eventId}
            onDone={handleTimelineDone}
          />
        ) : (
          <div className="bg-white rounded-lg p-8 text-center">
            <p className="text-gray-500">Chưa có timeline cho sự kiện này</p>
          </div>
        )}
      </div>

      {/* Sponsorship Budget and Event Information Section */}
      <div className="mt-5 grid grid-cols-1 lg:grid-cols-3 gap-3">
        {/* Sponsorship Budget Card */}
        <div className="bg-white rounded-xl p-4 shadow-xl border border-gray-200">
          <div className="flex items-center space-x-2 mb-3">
            <div className="w-8 h-8 bg-blue-500 rounded-lg flex items-center justify-center">
              <Search size={16} className="text-white" />
            </div>
            <h3 className="text-sm font-semibold text-gray-900">Sponsorship Budget</h3>
          </div>
          
          <div className="mb-3">
            {isEditing ? (
              <input
                type="number"
                value={editedData.totalBudget}
                onChange={(e) => handleInputChange('totalBudget', e.target.value)}
                placeholder="0"
                className="text-xl font-bold text-blue-500 border-b-2 border-blue-500 focus:outline-none bg-transparent w-full"
              />
            ) : (
              <p className="text-xl font-bold text-blue-500">{editedData.totalBudget.toLocaleString()} VND</p>
            )}
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
        <div className="bg-white shadow-xl rounded-xl p-4  border border-gray-200 lg:col-span-2">
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
                      value={editedData.shortDescription}
                      onChange={(e) => handleInputChange('shortDescription', e.target.value)}
                      className="text-xs text-gray-600 w-full border-b border-blue-500 focus:outline-none bg-transparent"
                      placeholder="Event Manager"
                      title={editedData.shortDescription}
                    />
                  ) : (
                    <p className="text-xs text-gray-600 truncate" title={editedData.shortDescription}>{editedData.shortDescription}</p>
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
                      type="text"
                      value={editedData.venueDetails}
                      onChange={(e) => handleInputChange('venueDetails', e.target.value)}
                      className="text-xs text-gray-600 w-full border-b border-blue-500 focus:outline-none bg-transparent"
                      placeholder="Email"
                      title={editedData.venueDetails}
                    />
                  ) : (
                    <p className="text-xs text-gray-600 truncate" title={editedData.venueDetails}>{editedData.venueDetails}</p>
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
                      value={editedData.requiredServices}
                      onChange={(e) => handleInputChange('requiredServices', e.target.value)}
                      className="text-xs text-gray-600 w-full border-b border-blue-500 focus:outline-none bg-transparent"
                      placeholder="Phone"
                      title={editedData.requiredServices}
                    />
                  ) : (
                    <p className="text-xs text-gray-600 truncate" title={editedData.requiredServices}>{editedData.requiredServices}</p>
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
                      value={editedData.sponsorshipNeeds}
                      onChange={(e) => handleInputChange('sponsorshipNeeds', e.target.value)}
                      className="text-xs text-gray-600 w-full border-b border-blue-500 focus:outline-none bg-transparent"
                      placeholder="Website"
                      title={editedData.sponsorshipNeeds}
                    />
                  ) : (
                    <p className="text-xs text-gray-600 truncate" title={editedData.sponsorshipNeeds}>{editedData.sponsorshipNeeds}</p>
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
