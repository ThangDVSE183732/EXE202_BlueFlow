import { useState, useEffect, useRef } from 'react';
import { brandService } from '../services/brandService';
import { useAuth } from '../contexts/AuthContext';

export const useBrandProfile = (showToast = null, shouldFetch = true) => {
  const { user } = useAuth();
  const [loading, setLoading] = useState(true);
  const [brandProfileId, setBrandProfileId] = useState(null);
  const [error, setError] = useState(null);
  const isCreatingRef = useRef(false); // Flag to prevent duplicate creation

  // Dữ liệu mặc định
  const defaultData = {
    companyName: 'TechCorp Solutions',
    location: 'Ho Chi Minh City, Vietnam',
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
    },
    industries: [
      'Artificial Intelligence',
      'Machine Learning',
      'Blockchain',
      'Startups',
      'Innovation',
      'Networking'
    ]
  };

  const [brandData, setBrandData] = useState(defaultData);

  // Map API response to UI format
  const mapApiToUI = (apiData) => {
    console.log('🔄 Mapping API data:', apiData);
    console.log('🖼️ brandLogo from API:', apiData?.brandLogo);
    
    // Handle ourMission - có thể là array hoặc string
    let missionArray = defaultData.mission;
    if (apiData?.ourMission) {
      if (Array.isArray(apiData.ourMission)) {
        // Nếu là array, lấy trực tiếp
        missionArray = apiData.ourMission.map(m => m.trim()).filter(m => m);
      } else if (typeof apiData.ourMission === 'string') {
        // Nếu là string, split by semicolon
        missionArray = apiData.ourMission.split(';').map(m => m.trim()).filter(m => m);
      }
    }

    // Handle tags - có thể là array của strings hoặc array với 1 string dài
    let industriesArray = defaultData.industries;
    if (apiData?.tags) {
      if (Array.isArray(apiData.tags)) {
        if (apiData.tags.length === 1 && typeof apiData.tags[0] === 'string' && apiData.tags[0].includes(',')) {
          // Nếu là array với 1 phần tử chứa chuỗi dài có dấu phẩy, split nó
          industriesArray = apiData.tags[0].split(',').map(t => t.trim()).filter(t => t);
        } else {
          // Nếu là array bình thường
          industriesArray = apiData.tags.map(t => t.trim()).filter(t => t);
        }
      } else if (typeof apiData.tags === 'string') {
        // Nếu là string, split by comma
        industriesArray = apiData.tags.split(',').map(t => t.trim()).filter(t => t);
      }
    }
    
    return {
      id: apiData?.id,
      companyName: apiData?.brandName || defaultData.companyName,
      brandLogo: apiData?.brandLogo,
      tagline: apiData?.tags,
      location: apiData?.location || defaultData.location,
      aboutUs: apiData?.aboutUs || defaultData.aboutUs,
      mission: missionArray,
      isPublic: apiData?.isPublic || false,
      companyInfo: {
        industry: apiData?.industry || defaultData.companyInfo.industry,
        companySize: apiData?.companySize || defaultData.companyInfo.companySize,
        founded: apiData?.foundedYear || defaultData.companyInfo.founded,
        website: apiData?.website || defaultData.companyInfo.website,
        email: apiData?.email || defaultData.companyInfo.email,
        phone: apiData?.phoneNumber || defaultData.companyInfo.phone
      },
      industries: industriesArray
    };
  };

  // Map UI format to API request (FormData for file upload support)
  const mapUIToApiFormData = async (uiData, logoFile = null, isCreate = false) => {
    const formData = new FormData();
    
    console.log('🔍 Mapping UI data to FormData:', uiData);
    
    // Chuẩn bị data theo đúng thứ tự Swagger API (từ trên xuống dưới)
    const brandName = uiData.companyName || 'Default Company';
    const location = uiData.location || 'Vietnam';
    const aboutUs = uiData.aboutUs || 'About our company';
    const ourMission = Array.isArray(uiData.mission) ? uiData.mission.join('; ') : (uiData.mission || 'Our mission');
    const industry = uiData.companyInfo?.industry || 'Technology';
    const companySize = uiData.companyInfo?.companySize || '100';
    const foundedYear = uiData.companyInfo?.founded || '2020';
    const website = uiData.companyInfo?.website || 'https://example.com';
    const email = uiData.companyInfo?.email || 'info@example.com';
    const phoneNumber = uiData.companyInfo?.phone || '0000000000';
    const tags = Array.isArray(uiData.industries) ? uiData.industries.join(', ') : (uiData.industries || 'Technology');
    
    // Append theo đúng thứ tự từ Swagger: BrandName, Location, AboutUs, OurMission, Industry, CompanySize, FoundedYear, Website, Email, PhoneNumber, Tags, BrandLogo
    formData.append('BrandName', brandName);
    formData.append('Location', location);
    formData.append('AboutUs', aboutUs);
    formData.append('OurMission', ourMission);
    formData.append('Industry', industry);
    formData.append('CompanySize', companySize);
    formData.append('FoundedYear', foundedYear);
    formData.append('Website', website);
    formData.append('Email', email);
    formData.append('PhoneNumber', phoneNumber);
    formData.append('Tags', tags);
    
    // BrandLogo - logic khác nhau cho CREATE vs UPDATE
    if (logoFile) {
      // User upload logo mới
      formData.append('BrandLogo', logoFile);
      console.log('✅ Added new BrandLogo file:', logoFile.name);
    } else if (isCreate) {
      // CREATE mới: cần placeholder vì backend require BrandLogo
      const emptyImageBlob = await fetch('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==')
        .then(res => res.blob());
      const placeholderFile = new File([emptyImageBlob], 'placeholder.png', { type: 'image/png' });
      formData.append('BrandLogo', placeholderFile);
      console.log('✅ Added placeholder BrandLogo for CREATE');
    } else if (uiData.brandLogo) {
      // UPDATE với logo hiện có: download và gửi lại để giữ nguyên
      try {
        console.log('📥 Downloading existing logo to preserve it:', uiData.brandLogo);
        const logoResponse = await fetch(uiData.brandLogo);
        const logoBlob = await logoResponse.blob();
        const existingLogoFile = new File([logoBlob], 'existing-logo.png', { type: logoBlob.type || 'image/png' });
        formData.append('BrandLogo', existingLogoFile);
        console.log('✅ Re-uploading existing logo to preserve it');
      } catch (fetchError) {
        console.warn('⚠️ Failed to fetch existing logo, using placeholder:', fetchError);
        // Fallback: gửi placeholder
        const emptyImageBlob = await fetch('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==')
          .then(res => res.blob());
        const placeholderFile = new File([emptyImageBlob], 'keep-existing.png', { type: 'image/png' });
        formData.append('BrandLogo', placeholderFile);
      }
    } else {
      // Không có logo hiện có và không upload mới: gửi placeholder
      const emptyImageBlob = await fetch('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==')
        .then(res => res.blob());
      const placeholderFile = new File([emptyImageBlob], 'no-logo.png', { type: 'image/png' });
      formData.append('BrandLogo', placeholderFile);
      console.log('ℹ️ No existing logo, sending placeholder');
    }
    
    console.log('📋 FormData fields (in order):', {
      '1. BrandName': brandName,
      '2. Location': location,
      '3. AboutUs': aboutUs.substring(0, 50) + '...',
      '4. OurMission': ourMission.substring(0, 50) + '...',
      '5. Industry': industry,
      '6. CompanySize': companySize,
      '7. FoundedYear': foundedYear,
      '8. Website': website,
      '9. Email': email,
      '10. PhoneNumber': phoneNumber,
      '11. Tags': tags,
      '12. BrandLogo': logoFile ? logoFile.name : 'placeholder.png'
    });
    
    return formData;
  };

  // Fetch hoặc tạo brand profile
  useEffect(() => {
    const fetchOrCreateBrandProfile = async () => {
      console.log('🚀 useBrandProfile: Starting fetch/create process...');
      console.log('👤 Current user:', user);
      console.log('🆔 User ID:', user?.id);
      
      if (!user?.id) {
        console.log('⚠️ No user ID found, using default data');
        setLoading(false);
        return;
      }

      // If caller set shouldFetch=false, skip fetching (fetch on demand)
      if (!shouldFetch) {
        console.log('⏭️ shouldFetch is false, skipping fetch');
        setLoading(false);
        return;
      }

      try {
        setLoading(true);
        setError(null);
        console.log('🔍 Fetching brand profile for user:', user.id);

        // Thử lấy brand profile
        const response = await brandService.getBrandProfileByUserId(user.id);

        console.log('📥 getBrandProfileByUserId response:', response);
        console.log('📊 Response success:', response.success);
        console.log('📊 Response data:', response.data);

        // Nếu tìm thấy brand profile (success = true và có data)
        if (response.success && response.data) {
          // Backend trả về {success, message, data}, và service wrap lại
          // Nên phải lấy response.data.data
          const actualData = response.data.data || response.data;
          console.log('✅ Brand profile found:', actualData);
          console.log('✅ Brand profile ID:', actualData.id);
          console.log('✅ Brand Logo URL:', actualData.brandLogo);
          setBrandProfileId(actualData.id); // Sử dụng ID từ response
          const mappedData = mapApiToUI(actualData);
          console.log('✅ Mapped data:', mappedData);
          console.log('✅ Mapped brandLogo:', mappedData.brandLogo);
          setBrandData(mappedData);
          setLoading(false);
          return; // ✅ Dừng lại ở đây, không tạo mới
        }
        
        // Nếu không tìm thấy (success = false hoặc không có data), tạo mới
        console.log('❌ Brand profile not found (success=false or no data), will create new one');
        console.log('📝 Starting brand profile creation process...');
        
        // Check if already creating to prevent duplicate
        if (isCreatingRef.current) {
          console.log('⏭️ Already creating brand profile, skipping...');
          setLoading(false);
          return;
        }
        isCreatingRef.current = true;
        
        // Tạo brand profile mới
        // Sử dụng defaultData đầy đủ, chỉ override user-specific fields
        const createDataUI = {
          ...defaultData,
          companyName: user?.companyName || defaultData.companyName,
          companyInfo: {
            ...defaultData.companyInfo,
            email: user?.email || defaultData.companyInfo.email
          }
        };

        console.log('📝 Creating brand profile with data:', createDataUI);
        const formData = await mapUIToApiFormData(createDataUI, null, true); // isCreate = true
        const createResponse = await brandService.createBrandProfile(formData);

        console.log('📥 Create brand profile response:', createResponse);
        
        // Reset flag after creation attempt
        isCreatingRef.current = false;

        if (createResponse.success && createResponse.data) {
          console.log('✅ Brand profile created successfully');
          console.log('✅ Created Brand Profile ID:', createResponse.data.id);
          console.log('✅ Created response data:', createResponse.data);
          setBrandProfileId(createResponse.data.id);
          setBrandData(mapApiToUI(createResponse.data));
          
          // Show success toast
          if (showToast) {
            showToast({
              type: 'success',
              message: 'Đã tạo hồ sơ thương hiệu thành công',
              duration: 3000
            });
          }

          // Broadcast so discovery/UI can refresh
          try { window.dispatchEvent(new CustomEvent('brandProfile:updated', { detail: { userId: user?.id, brandProfileId: createResponse.data.id } })); } catch(err) { void err; }
        } else {
          console.log('⚠️ Create failed, response:', createResponse);
          console.log('⚠️ Using default data');
          setError('Failed to create brand profile: ' + (createResponse.message || 'Unknown error'));
          
          // Show error toast
          if (showToast) {
            showToast({
              type: 'error',
              message: createResponse.message || 'Không thể tạo hồ sơ thương hiệu',
              duration: 4000
            });
          }
        }
      } catch (err) {
        console.error('❌ Error in fetch/create process:', err);
        setError(err.message || 'Failed to fetch or create brand profile');
        isCreatingRef.current = false; // Reset flag on error
        
        // Show error toast
        if (showToast) {
          showToast({
            type: 'error',
            message: 'Đã xảy ra lỗi: ' + (err.message || 'Unknown error'),
            duration: 4000
          });
        }
      } finally {
        setLoading(false);
      }
    };

    fetchOrCreateBrandProfile();
    
    // Cleanup function
    return () => {
      // Reset flags when component unmounts or user changes
      isCreatingRef.current = false;
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [user?.id, shouldFetch]);

  // Parse backend validation errors
  const parseBackendError = (error) => {
    // Nếu có response.data.errors (ASP.NET Core validation errors)
    if (error.response?.data?.errors) {
      const errors = error.response.data.errors;
      const errorMessages = [];
      
      // Parse từng field error
      Object.keys(errors).forEach(field => {
        const messages = errors[field];
        if (Array.isArray(messages)) {
          messages.forEach(msg => {
            errorMessages.push(`${field}: ${msg}`);
          });
        }
      });

      return {
        title: error.response.data.title || 'Validation Error',
        status: error.response.data.status,
        errors: error.response.data.errors,
        errorMessages: errorMessages,
        traceId: error.response.data.traceId
      };
    }

    // Nếu có message thông thường
    return {
      title: 'Error',
      status: error.response?.status || 500,
      errors: {},
      errorMessages: [error.message || 'An error occurred'],
      traceId: null
    };
  };

  // Update brand profile
  const updateBrandProfile = async (updatedData, logoFile = null) => {
    console.log("🔍 Update brand profile called");
    console.log("👤 UserID:", user?.id);
    console.log("🆔 BrandProfileId:", brandProfileId);
    
    if (!brandProfileId) {
      console.error('❌ No brandProfileId available for update');
      console.log('⏳ Attempting to fetch brand profile first...');
      
      // Try to fetch brand profile if not loaded yet
      if (user?.id) {
        try {
          const response = await brandService.getBrandProfileByUserId(user.id);
          if (response.success && response.data) {
            const actualData = response.data.data || response.data;
            const fetchedId = actualData.id;
            console.log('✅ Found brandProfileId:', fetchedId);
            setBrandProfileId(fetchedId);
            
            // Now retry update with the fetched ID
            const formData = await mapUIToApiFormData(updatedData, logoFile);
            const updateResponse = await brandService.updateBrandProfile(fetchedId, formData);
            
            if (updateResponse.success) {
              console.log('✅ Brand profile updated successfully');
              setBrandData(mapApiToUI(updateResponse.data));
              
              if (showToast) {
                showToast({
                  type: 'success',
                  message: 'Cập nhật hồ sơ thương hiệu thành công',
                  duration: 3000
                });
              }
              
              return { success: true };
            }
          }
        } catch (fetchError) {
          console.error('❌ Failed to fetch brand profile for update:', fetchError);
        }
      }
      
      // If still no ID, throw error
      const errorDetail = {
        title: 'Brand Profile Not Found',
        status: 400,
        errors: { BrandProfileId: ['Brand profile is not loaded yet. Please wait or refresh the page.'] },
        errorMessages: ['Brand profile is not loaded yet. Please wait or refresh the page.']
      };
      
      if (showToast) {
        showToast({
          type: 'error',
          message: 'Vui lòng đợi tải hồ sơ hoặc tải lại trang',
          duration: 4000
        });
      }
      
      throw errorDetail;
    }

    try {
      console.log('💾 Saving brand profile changes...');
      console.log('📝 Updated data:', updatedData);

      // Backend PUT endpoint yêu cầu FormData giống như POST
      const formData = await mapUIToApiFormData(updatedData, logoFile);
      const response = await brandService.updateBrandProfile(brandProfileId, formData);

      if (response.success) {
        console.log('✅ Brand profile updated successfully');
        setBrandData(updatedData);
        
        // Show success toast
        if (showToast) {
          showToast({
            type: 'success',
            title: 'Đã lưu!',
            message: 'Cập nhật hồ sơ thương hiệu thành công',
            duration: 3000
          });
        }

        // Notify other parts of the app (discovery lists) to refresh
        try {
          window.dispatchEvent(new CustomEvent('brandProfile:updated', { detail: { userId: user?.id, brandProfileId } }));
        } catch (err) {
          void err; // ignore in environments without window
        }
        
        return { success: true };
      } else {
        console.error('❌ Failed to update:', response.message);
        const errorDetail = {
          title: 'Update Failed',
          status: 400,
          errors: { Update: [response.message || 'Update failed'] },
          errorMessages: [response.message || 'Update failed']
        };
        
        // Show error toast
        if (showToast) {
          showToast({
            type: 'error',
            title: 'Lỗi cập nhật!',
            message: response.message || 'Không thể cập nhật hồ sơ',
            duration: 4000
          });
        }
        
        throw errorDetail;
      }
    } catch (error) {
      console.error('❌ Error updating brand profile:', error);
      
      // Parse và throw error với format chuẩn
      const parsedError = parseBackendError(error);
      console.error('📋 Error details:', parsedError);
      
      // Show detailed error toast
      if (showToast) {
        showToast({
          type: 'error',
          title: 'Lỗi!',
          message: parsedError.errorMessages[0] || 'Đã xảy ra lỗi khi cập nhật',
          duration: 5000
        });
      }
      
      throw parsedError;
    }
  };

  // Refresh brand profile
  const refreshBrandProfile = async () => {
    if (!user?.id) return;

    try {
      setLoading(true);
      const response = await brandService.getBrandProfileByUserId(user.id);

      if (response.success && response.data) {
        setBrandData(mapApiToUI(response.data));
      }
    } catch (error) {
      console.error('Error refreshing brand profile:', error);
    } finally {
      setLoading(false);
    }
  };

  // Toggle brand profile status (Public/Private)
  const toggleBrandProfileStatus = async () => {
    if (!brandProfileId) {
      console.error('No brand profile ID found');
      if (showToast) {
        showToast({
          type: 'error',
          title: 'Lỗi!',
          message: 'Không tìm thấy ID hồ sơ thương hiệu',
          duration: 3000
        });
      }
      return { success: false, message: 'No brand profile ID' };
    }

    try {
      const response = await brandService.toggleBrandProfileStatus(brandProfileId);

      if (response.success) {
        // Update local state
        setBrandData(prev => ({
          ...prev,
          isPublic: !prev.isPublic
        }));

        // Success returned to caller; UI components should show user-facing notifications

        // Broadcast update so discovery lists refresh
        try { window.dispatchEvent(new CustomEvent('brandProfile:updated', { detail: { userId: user?.id, brandProfileId } })); } catch(err) { void err; }
        return { success: true, data: response.data };
      } else {
        if (showToast) {
          showToast({
            type: 'error',
            title: 'Lỗi!',
            message: response.message || 'Không thể thay đổi trạng thái',
            duration: 4000
          });
        }
        return response;
      }
    } catch (error) {
      console.error('Error toggling brand profile status:', error);
      if (showToast) {
        showToast({
          type: 'error',
          title: 'Lỗi!',
          message: 'Đã xảy ra lỗi khi thay đổi trạng thái',
          duration: 4000
        });
      }
      return { success: false, message: error.message };
    }
  };

  // Toggle brand profile all status (Public/Private + Partnership)
  const toggleBrandProfileAllStatus = async () => {
    if (!brandProfileId) {
      console.error('No brand profile ID found');
      if (showToast) {
        showToast({
          type: 'error',
          title: 'Lỗi!',
          message: 'Không tìm thấy ID hồ sơ thương hiệu',
          duration: 3000
        });
      }
      return { success: false, message: 'No brand profile ID' };
    }

    try {
      const response = await brandService.toggleBrandProfileAllStatus(brandProfileId);

      if (response.success) {
        // Update local state
        setBrandData(prev => ({
          ...prev,
          isPublic: !prev.isPublic
        }));

        // Success returned to caller; UI components should show user-facing notifications

        // Broadcast update so discovery lists refresh
        try { window.dispatchEvent(new CustomEvent('brandProfile:updated', { detail: { userId: user?.id, brandProfileId } })); } catch(err) { void err; }
        return { success: true, data: response.data };
      } else {
        if (showToast) {
          showToast({
            type: 'error',
            title: 'Lỗi!',
            message: response.message || 'Không thể thay đổi trạng thái',
            duration: 4000
          });
        }
        return response;
      }
    } catch (error) {
      console.error('Error toggling brand profile all status:', error);
      if (showToast) {
        showToast({
          type: 'error',
          title: 'Lỗi!',
          message: 'Đã xảy ra lỗi khi thay đổi trạng thái',
          duration: 4000
        });
      }
      return { success: false, message: error.message };
    }
  };

  return {
    brandData,
    setBrandData,
    loading,
    error,
    brandProfileId,
    updateBrandProfile,
    refreshBrandProfile,
    toggleBrandProfileStatus,
    toggleBrandProfileAllStatus
  };
};
