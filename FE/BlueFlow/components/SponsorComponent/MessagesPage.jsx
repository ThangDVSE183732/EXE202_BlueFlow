import React, { useState, useEffect, useCallback } from 'react';
import MessageContent from './MessageContent';
import MessagesList from './MessagesList';
import { MessageCircle, ArrowRight } from 'lucide-react';

const MessagesPage = ({ initialPartnerId = null, initialPartnerName = null, onUnreadCountChange }) => {
  const [selectedChat, setSelectedChat] = useState(null);
  const [partnerList, setPartnerList] = useState([]);

  // Update selected chat khi initialPartnerId thay đổi
  useEffect(() => {
    if (initialPartnerId) {
      setSelectedChat({
        name: initialPartnerName || 'Partner',
        partnerId: initialPartnerId
      });
    }
  }, [initialPartnerId, initialPartnerName]);

  // Auto-select first chat when partner list is loaded
  useEffect(() => {
    if (!selectedChat && partnerList.length > 0) {
      const firstChat = partnerList[0];
      setSelectedChat({
        name: firstChat.name,
        partnerId: firstChat.id
      });
    }
  }, [partnerList, selectedChat]);

  const handleSelectChat = (chat) => {
    setSelectedChat({
      name: chat.name,
      partnerId: chat.id // chat.id là partnerId từ MessagesList
    });
  };

  const handlePartnerListLoaded = useCallback((chats) => {
    setPartnerList(chats);
  }, []);

  return (
    <div className="flex h-screen bg-white mb-10 rounded-lg overflow-hidden shadow-sm border border-gray-200">
      {/* Main Content Area - Giữ nguyên tỉ lệ flex-1 */}
      <div className="flex-1 flex flex-col bg-white relative">
        {selectedChat ? (
          <MessageContent 
            selectedChat={selectedChat.name} 
            partnerId={selectedChat.partnerId}
          />
        ) : (
          <div className="flex-1 flex flex-col items-center justify-center h-full bg-gradient-to-br from-gray-50 to-white">
            <div className="text-center px-8 max-w-md">
              <div className="w-20 h-20 mx-auto mb-6 rounded-full bg-blue-50 flex items-center justify-center">
                <MessageCircle className="w-10 h-10 text-blue-500" />
              </div>
              <h3 className="text-xl font-semibold text-gray-900 mb-2">
                Chọn cuộc trò chuyện
              </h3>
              <p className="text-gray-500 text-sm leading-relaxed mb-6">
                Chọn một cuộc trò chuyện từ danh sách bên phải để bắt đầu nhắn tin
              </p>
              <div className="flex items-center justify-center gap-2 text-xs text-gray-400">
                <ArrowRight className="w-4 h-4" />
                <span>Chọn từ danh sách để bắt đầu</span>
              </div>
            </div>
          </div>
        )}
      </div>
      
      {/* Messages Sidebar - Giữ nguyên tỉ lệ w-70 */}
      <div className="w-70 flex-shrink-0 border-l border-gray-200">
        <MessagesList 
          onSelectChat={handleSelectChat}
          onPartnerListLoaded={handlePartnerListLoaded}
          onUnreadCountChange={onUnreadCountChange}
        />
      </div>
    </div>
  );
};

export default MessagesPage;
