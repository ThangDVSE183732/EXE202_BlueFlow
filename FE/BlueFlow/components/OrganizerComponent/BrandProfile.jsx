import React, { useState } from 'react';
import toast from 'react-hot-toast';
import { Edit, Plus, Upload, Check, X } from 'lucide-react';
import { useBrandProfile } from '../../hooks/useBrandProfile';
import Loading from '../Loading';

const BrandProfile = () => {
  const { brandData, setBrandData, loading, error, updateBrandProfile } = useBrandProfile();
  const [isEditing, setIsEditing] = useState(false);
  const [editedData, setEditedData] = useState(brandData);
  const [logoPreview, setLogoPreview] = useState(null);
  const [logoFile, setLogoFile] = useState(null);

  // Sync editedData với brandData khi brandData thay đổi
  React.useEffect(() => {
    setEditedData(brandData);
  }, [brandData]);

  // Cleanup preview URL on unmount
  React.useEffect(() => {
    return () => {
      if (logoPreview && logoPreview.startsWith('blob:')) {
        URL.revokeObjectURL(logoPreview);
      }
    };
  }, [logoPreview]);

  const handleLogoUpload = (e) => {
    const file = e.target.files[0];
    if (!file) return;

    // Validate file type
    if (!file.type.startsWith('image/')) {
      toast.error('Vui lòng chọn file ảnh');
      return;
    }

    // Validate file size (max 2MB)
    if (file.size > 2 * 1024 * 1024) {
      toast.error('Kích thước ảnh không được vượt quá 2MB');
      return;
    }

    // Create preview
    const previewUrl = URL.createObjectURL(file);
    setLogoPreview(previewUrl);
    setLogoFile(file);
  };

  const handleSave = async () => {
    try {
      await updateBrandProfile(editedData, logoFile);
      setIsEditing(false);
      // Clear logo file after save
      setLogoFile(null);
    } catch (error) {
      console.error('Failed to save:', error);
      
      // Hiển thị lỗi chi tiết từ backend
      if (error.errorMessages && error.errorMessages.length > 0) {
        alert(`Failed to save:\n\n${error.errorMessages.join('\n')}`);
      } else {
        alert('Failed to save changes. Please try again.');
      }
    }
  };

  const handleCancel = () => {
    setEditedData(brandData); // Reset về dữ liệu gốc
    setIsEditing(false);
  };

  const handleInputChange = (field, value) => {
    if (field.includes('.')) {
      const [parent, child] = field.split('.');
      setEditedData(prev => ({
        ...prev,
        [parent]: {
          ...prev[parent],
          [child]: value
        }
      }));
    } else {
      setEditedData(prev => ({
        ...prev,
        [field]: value
      }));
    }
  };

  const handleArrayItemChange = (index, value) => {
    setEditedData(prev => ({
      ...prev,
      mission: prev.mission.map((item, i) => i === index ? value : item)
    }));
  };

  const addMissionItem = () => {
    setEditedData(prev => ({
      ...prev,
      mission: [...prev.mission, '']
    }));
  };

  const removeMissionItem = (index) => {
    setEditedData(prev => ({
      ...prev,
      mission: prev.mission.filter((_, i) => i !== index)
    }));
  };

  if (loading) {
    return <Loading message="Đang tải thông tin thương hiệu..." />;
  }

  // Hiển thị error nếu có
  if (error) {
    console.warn('Brand profile error:', error);
  }

  return (
    <div className="min-h-screen bg-white p-6 pt-1 overflow-x-hidden">
      {/* Header */}
      <div className="mb-8 text-left">
        <h1 className="text-2xl font-semibold text-blue-500 mb-2">Brand Profile</h1>
        <p className="text-gray-500 text-sm">Create and manage your professional brand profile to attract partners</p>
              <div className="h-px w-full bg-gray-300 mx-1 mb-5 mt-2" />

      </div>

      <div className="bg-white border rounded-xl shadow-xl border-gray-300 overflow-hidden max-w-full">
      {/* Company Banner */}
      <div className="bg-black rounded-2xl px-10 py-5 mb-3 relative overflow-hidden min-w-0 max-w-full">
        {/* Edit/Save/Cancel Buttons - Top Right */}
        <div className="absolute top-3 right-4 flex items-center space-x-2 z-10">
          {isEditing ? (
            <>
              <button
                onClick={handleSave}
                className="px-3 py-1 rounded-full text-xs font-medium transition-all bg-green-500 text-white hover:bg-green-600 flex items-center space-x-1"
                title="Save changes"
              >
                <Check size={14} />
                <span>Save</span>
              </button>
              <button
                onClick={handleCancel}
                className="px-3 py-1 rounded-full text-xs font-medium transition-all bg-red-500 text-white hover:bg-red-600 flex items-center space-x-1"
                title="Cancel editing"
              >
                <X size={14} />
                <span>Cancel</span>
              </button>
            </>
          ) : (
            <button
              onClick={() => setIsEditing(true)}
              className="px-3 py-1 rounded-full text-xs font-medium transition-all bg-blue-500 text-white hover:bg-blue-600"
            >
              Edit
            </button>
          )}
          <label className="w-6 h-6 flex items-center justify-center text-white hover:bg-white/10 rounded-full transition-colors cursor-pointer" title="Upload logo">
            <Upload size={14} />
            <input
              type="file"
              accept="image/*"
              onChange={handleLogoUpload}
              className="hidden"
            />
          </label>
        </div>

        {/* Logo and Company Info - Left */}
        <div className="flex items-center space-x-5 mb-6 mt-5 min-w-0 w-full max-w-[calc(100%-8rem)]">
          <div className="w-26 h-26 bg-white rounded-full flex items-center justify-center flex-shrink-0 overflow-hidden">
            {(logoPreview || brandData.brandLogo) ? (
              <img 
                src={logoPreview || brandData.brandLogo} 
                alt="Company Logo" 
                className="w-full h-full object-cover"
              />
            ) : (
              <span className="text-2xl font-bold text-gray-800">
                {editedData.companyName ? editedData.companyName.substring(0, 2).toUpperCase() : 'TC'}
              </span>
            )}
          </div>
          <div className='text-left min-w-0 flex-1 overflow-hidden'>
            {isEditing ? (
              <input
                type="text"
                value={editedData.companyName}
                onChange={(e) => handleInputChange('companyName', e.target.value)}
                className="text-2xl font-bold text-blue-400 mb-0.5 bg-transparent border-b-2 border-blue-400 focus:outline-none w-full min-w-0"
                title={editedData.companyName}
              />
            ) : (
              <h2 className="text-2xl font-bold text-blue-400 mb-0.5 truncate min-w-0" title={editedData.companyName}>{editedData.companyName}</h2>
            )}
            {isEditing ? (
              <input
                type="text"
                value={editedData.companyInfo.industry}
                onChange={(e) => handleInputChange('tagline', e.target.value)}
                className="text-white text-sm mb-0.5 bg-transparent border-b border-white/50 focus:outline-none w-full min-w-0"
                title={editedData.companyInfo.industry}
              />
            ) : (
              <p className="text-white text-sm mb-0.5 truncate min-w-0" title={editedData.companyInfo.industry}>{editedData.companyInfo.industry}</p>
            )}
            {isEditing ? (
              <input
                type="text"
                value={editedData.location}
                onChange={(e) => handleInputChange('location', e.target.value)}
                className="text-gray-400 text-xs bg-transparent border-b border-gray-400/50 focus:outline-none w-full min-w-0"
                title={editedData.location}
              />
            ) : (
              <p className="text-gray-400 text-xs truncate min-w-0" title={editedData.location}>{editedData.location}</p>
            )}
          </div>
        </div>

        
      </div>

      {/* Main Content - Centered */}
      <div className="max-w-full space-y-4 px-6 min-w-0 w-full overflow-x-hidden">
        {/* About Us Section */}
        <div className="bg-white p-5 pb-0 min-w-0 overflow-hidden max-w-full">
          <div className="flex justify-between items-center mb-4">
            <h3 className="text-lg font-bold text-gray-900">About Us</h3>
           
          </div>

          {isEditing ? (
            <textarea
              value={editedData.aboutUs}
              onChange={(e) => handleInputChange('aboutUs', e.target.value)}
              className="w-full max-w-full text-gray-600 leading-relaxed text-xs text-left border border-gray-300 rounded-lg p-3 focus:outline-none focus:border-blue-500 resize-none break-all overflow-wrap-anywhere box-border"
              rows={4}
              style={{ wordBreak: 'break-all' }}
            />
          ) : (
            <p className="text-gray-600 leading-relaxed text-xs text-left break-all overflow-wrap-anywhere whitespace-pre-wrap overflow-hidden max-w-full" style={{ wordBreak: 'break-all' }}>
              {editedData.aboutUs}
            </p>
          )}
        </div>

        {/* Our Mission Section */}
        <div className="bg-white p-5 pt-0 pb-0 min-w-0 overflow-hidden max-w-full">
          <div className="flex justify-between items-center mb-4">
            <h3 className="text-lg font-bold text-gray-900">Our mission:</h3>
            
          </div>

          <ul className="space-y-1.5 min-w-0 w-full overflow-hidden max-w-full">
            {editedData.mission.map((item, index) => (
              <li key={index} className="flex items-start space-x-2 min-w-0 w-full overflow-hidden max-w-full pl-4">
                <span className="w-1 h-1 bg-gray-400 rounded-full mt-1.5 flex-shrink-0"></span>
                {isEditing ? (
                  <div className="flex items-center space-x-1 flex-1 min-w-0 overflow-hidden max-w-full">
                    <input
                      type="text"
                      value={item}
                      onChange={(e) => handleArrayItemChange(index, e.target.value)}
                      className="text-gray-600 text-xs flex-1 min-w-0 max-w-full border-b border-gray-300 focus:border-blue-500 focus:outline-none bg-transparent box-border"
                      title={item}
                      style={{ wordBreak: 'break-all' }}
                    />
                    <button
                      onClick={() => removeMissionItem(index)}
                      className="text-red-500 hover:text-red-700 flex-shrink-0"
                    >
                      <X size={12} />
                    </button>
                  </div>
                ) : (
                  <span className="text-gray-600 text-left text-xs min-w-0 flex-1 max-w-full block" title={item} style={{ wordBreak: 'break-all', overflowWrap: 'anywhere' }}>{item}</span>
                )}
              </li>
            ))}
            {isEditing && (
              <button
                onClick={addMissionItem}
                className="text-blue-500 text-xs hover:text-blue-700 flex items-center space-x-1 ml-3"
              >
                <Plus size={14} />
                <span>Add mission</span>
              </button>
            )}
          </ul>
        </div>

        {/* Company Information Section */}
        <div className="bg-white  p-5 pb-0 pt-0">
          <div className="flex justify-between items-center mb-4">
            <h3 className="text-lg font-bold text-gray-900">Company Information</h3>
           
          </div>

          <div className="grid grid-cols-2 md:grid-cols-3 gap-4 min-w-0">
            <div className="min-w-0">
              <label className="block text-xs font-medium text-gray-400 mb-0.5">Industry</label>
              {isEditing ? (
                <input
                  type="text"
                  value={editedData.companyInfo.industry}
                  onChange={(e) => handleInputChange('companyInfo.industry', e.target.value)}
                  className="text-gray-900 text-sm font-semibold w-full min-w-0 border-b border-blue-500 focus:outline-none bg-transparent"
                  title={editedData.companyInfo.industry}
                />
              ) : (
                <p className="text-gray-900 text-sm font-semibold truncate min-w-0" title={editedData.companyInfo.industry}>{editedData.companyInfo.industry}</p>
              )}
            </div>
            <div className="min-w-0">
              <label className="block text-xs font-medium text-gray-400 mb-0.5">Company Size</label>
              {isEditing ? (
                <input
                  type="text"
                  value={editedData.companyInfo.companySize}
                  onChange={(e) => handleInputChange('companyInfo.companySize', e.target.value)}
                  className="text-gray-900 text-sm font-semibold w-full min-w-0 border-b border-blue-500 focus:outline-none bg-transparent"
                  title={editedData.companyInfo.companySize}
                />
              ) : (
                <p className="text-gray-900 text-sm font-semibold truncate min-w-0" title={editedData.companyInfo.companySize}>{editedData.companyInfo.companySize}</p>
              )}
            </div>
            <div className="min-w-0">
              <label className="block text-xs font-medium text-gray-400 mb-0.5">Founded</label>
              {isEditing ? (
                <input
                  type="text"
                  value={editedData.companyInfo.founded}
                  onChange={(e) => handleInputChange('companyInfo.founded', e.target.value)}
                  className="text-gray-900 text-sm font-semibold w-full min-w-0 border-b border-blue-500 focus:outline-none bg-transparent"
                  title={editedData.companyInfo.founded}
                />
              ) : (
                <p className="text-gray-900 text-sm font-semibold truncate min-w-0" title={editedData.companyInfo.founded}>{editedData.companyInfo.founded}</p>
              )}
            </div>
            <div className="min-w-0">
              <label className="block text-xs font-medium text-gray-400 mb-0.5">Website</label>
              {isEditing ? (
                <input
                  type="url"
                  value={editedData.companyInfo.website}
                  onChange={(e) => handleInputChange('companyInfo.website', e.target.value)}
                  className="text-blue-500 text-sm font-semibold w-full min-w-0 border-b border-blue-500 focus:outline-none bg-transparent"
                  title={editedData.companyInfo.website}
                />
              ) : (
                <p className="text-blue-500 hover:text-blue-600 cursor-pointer text-sm font-semibold truncate min-w-0" title={editedData.companyInfo.website}>{editedData.companyInfo.website}</p>
              )}
            </div>
            <div className="min-w-0">
              <label className="block text-xs font-medium text-gray-400 mb-0.5">email</label>
              {isEditing ? (
                <input
                  type="email"
                  value={editedData.companyInfo.email}
                  onChange={(e) => handleInputChange('companyInfo.email', e.target.value)}
                  className="text-gray-900 text-sm font-semibold w-full min-w-0 border-b border-blue-500 focus:outline-none bg-transparent"
                  title={editedData.companyInfo.email}
                />
              ) : (
                <p className="text-gray-900 text-sm font-semibold truncate min-w-0" title={editedData.companyInfo.email}>{editedData.companyInfo.email}</p>
              )}
            </div>
            <div className="min-w-0">
              <label className="block text-xs font-medium text-gray-400 mb-0.5">Phone</label>
              {isEditing ? (
                <input
                  type="tel"
                  value={editedData.companyInfo.phone}
                  onChange={(e) => handleInputChange('companyInfo.phone', e.target.value)}
                  className="text-gray-900 text-sm font-semibold w-full min-w-0 border-b border-blue-500 focus:outline-none bg-transparent"
                  title={editedData.companyInfo.phone}
                />
              ) : (
                <p className="text-gray-900 text-sm font-semibold truncate min-w-0" title={editedData.companyInfo.phone}>{editedData.companyInfo.phone}</p>
              )}
            </div>
          </div>
        </div>

        {/* Industries/Tags Section */}
        <div className="bg-white p-5 min-w-0 overflow-hidden max-w-full mt-4">
          
          <div className="flex flex-wrap gap-2">
            {editedData.industries?.map((tag, index) => (
              <div key={index} className="relative group max-w-[10rem]">
                {isEditing ? (
                  <div className="flex items-center space-x-1 px-3 py-1 bg-blue-50 rounded-full">
                    <input
                      type="text"
                      value={tag}
                      onChange={(e) => {
                        const newIndustries = editedData.industries.map((item, i) => 
                          i === index ? e.target.value : item
                        );
                        setEditedData({ ...editedData, industries: newIndustries });
                      }}
                      className="bg-transparent text-blue-500 text-xs font-medium focus:outline-none w-24"
                    />
                    <button
                      onClick={() => {
                        const newIndustries = editedData.industries.filter((_, i) => i !== index);
                        setEditedData({ ...editedData, industries: newIndustries });
                      }}
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
                onClick={() => {
                  setEditedData({
                    ...editedData,
                    industries: [...(editedData.industries || []), '']
                  });
                }}
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
