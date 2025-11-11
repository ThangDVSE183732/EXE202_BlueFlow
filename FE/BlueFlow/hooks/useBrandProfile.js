import { useState, useEffect } from 'react';
import { brandService } from '../services/brandService';
import { useAuth } from '../contexts/AuthContext';

export const useBrandProfile = (showToast = null) => {
  const { user } = useAuth();
  const [loading, setLoading] = useState(true);
  const [brandProfileId, setBrandProfileId] = useState(null);
  const [error, setError] = useState(null);

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
  const mapUIToApiFormData = async (uiData, logoFile = null) => {
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
    
    // BrandLogo cuối cùng
    if (logoFile) {
      formData.append('BrandLogo', logoFile);
      console.log('✅ Added BrandLogo:', logoFile.name);
    } else {
      // Tạo placeholder nhỏ nếu không có logo
      const emptyImageBlob = await fetch('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==')
        .then(res => res.blob());
      const placeholderFile = new File([emptyImageBlob], 'placeholder.png', { type: 'image/png' });
      formData.append('BrandLogo', placeholderFile);
      console.log('✅ Added placeholder BrandLogo');
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
      if (!user?.id) {
    console.log(user?.id);
        console.log('No user ID, using default data');
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

        // Nếu tìm thấy brand profile
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
        
        // Nếu backend trả về lỗi (404, 500, etc.) hoặc không có data
        console.log('❌ Brand profile not found or error occurred, will create new one');
        throw new Error(response.message || 'Brand profile not found');
      } catch (err) {
        console.log('❌ Brand profile fetch failed, creating new one...', err.message);

        // Tạo brand profile mới
        try {
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
          const formData = await mapUIToApiFormData(createDataUI);
          const createResponse = await brandService.createBrandProfile(formData);

          if (createResponse.success && createResponse.data) {
            console.log('✅ Brand profile created successfully');
            console.log('✅ Created Brand Profile ID:', createResponse.data.id);
            setBrandProfileId(createResponse.data.id); // ✅ SỬA: Dùng brandProfile.id từ response
            setBrandData(mapApiToUI(createResponse.data));
            
            // Show success toast
            if (showToast) {
              showToast({
                type: 'success',
                title: 'Thành công!',
                message: 'Đã tạo hồ sơ thương hiệu thành công',
                duration: 3000
              });
            }
          } else {
            console.log('⚠️ Create failed, using default data');
            setError('Failed to create brand profile');
            
            // Show error toast
            if (showToast) {
              showToast({
                type: 'error',
                title: 'Lỗi!',
                message: 'Không thể tạo hồ sơ thương hiệu',
                duration: 4000
              });
            }
          }
        } catch (createError) {
          console.error('❌ Error creating brand profile:', createError);
          
          // Parse error details
          const parsedError = parseBackendError(createError);
          console.error('📋 Create error details:', parsedError);
          
          // Set error message cho UI
          if (parsedError.errorMessages.length > 0) {
            setError(parsedError.errorMessages.join(', '));
            
            // Show detailed error toast
            if (showToast) {
              showToast({
                type: 'error',
                title: 'Lỗi tạo hồ sơ!',
                message: parsedError.errorMessages[0] || 'Không thể tạo hồ sơ thương hiệu',
                duration: 5000
              });
            }
          } else {
            setError('Failed to create brand profile');
            
            // Show generic error toast
            if (showToast) {
              showToast({
                type: 'error',
                title: 'Lỗi!',
                message: 'Đã xảy ra lỗi khi tạo hồ sơ',
                duration: 4000
              });
            }
          }
          
          // Giữ defaultData trong state
        }
      } finally {
        setLoading(false);
      }
    };

    fetchOrCreateBrandProfile();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [user?.id]);

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
    console.log("UserID: ", user?.id);
    if (!brandProfileId) {
      console.error('No user ID to update');
      const errorDetail = {
        title: 'No User ID',
        status: 400,
        errors: { UserId: ['UserId is missing'] },
        errorMessages: ['UserId is missing']
      };
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

        if (showToast) {
          showToast({
            type: 'success',
            title: 'Thành công!',
            message: `Đã chuyển sang ${!brandData.isPublic ? 'Public' : 'Private'}`,
            duration: 3000
          });
        }

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

        if (showToast) {
          showToast({
            type: 'success',
            title: 'Thành công!',
            message: `Đã chuyển sang ${!brandData.isPublic ? 'Public' : 'Private'} và cập nhật partnership`,
            duration: 3000
          });
        }

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
