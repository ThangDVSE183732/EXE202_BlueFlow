import React, { useState, useEffect, useCallback } from 'react';
import MessageContent from './MessageContent';
import MessagesList from './MessagesList';

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
    <div className="flex h-screen bg-gray-100 mb-10 border border-gray-300 rounded-xl shadow-lg overflow-hidden">
      {/* Main Content Area */}
      <div className="flex-1">
        {selectedChat ? (
          <MessageContent 
            selectedChat={selectedChat.name} 
            partnerId={selectedChat.partnerId}
          />
        ) : (
          <div className="flex items-center justify-center h-full">
            <p className="text-gray-400">No messages yet</p>
          </div>
        )}
      </div>
      
      {/* Messages Sidebar */}
      <MessagesList 
        onSelectChat={handleSelectChat}
        onPartnerListLoaded={handlePartnerListLoaded}
        onUnreadCountChange={onUnreadCountChange}
      />
    </div>
  );
};

export default MessagesPage;