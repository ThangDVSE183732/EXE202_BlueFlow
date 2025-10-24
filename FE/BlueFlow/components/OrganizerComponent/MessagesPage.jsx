import React, { useState, useEffect } from 'react';
import MessageContent from './MessageContent';
import MessagesList from './MessagesList';

const MessagesPage = ({ initialPartnerId = null, initialPartnerName = null }) => {
  const [selectedChat, setSelectedChat] = useState({
    name: initialPartnerName || 'Event Tech',
    partnerId: initialPartnerId || '4fc2996d-3e88-45c7-9b09-9585fc5e4435' // Default chat hoặc từ props
  });

  // Update selected chat khi initialPartnerId thay đổi
  useEffect(() => {
    if (initialPartnerId) {
      setSelectedChat({
        name: initialPartnerName || 'Partner',
        partnerId: initialPartnerId
      });
    }
  }, [initialPartnerId, initialPartnerName]);

  const handleSelectChat = (chat) => {
    setSelectedChat({
      name: chat.name,
      partnerId: chat.id // chat.id là partnerId từ MessagesList
    });
  };

  return (
    <div className="flex h-screen bg-gray-100 mb-10 border border-gray-300 rounded-xl shadow-lg overflow-hidden">
      {/* Main Content Area */}
      <div className="flex-1">
        <MessageContent 
          selectedChat={selectedChat.name} 
          partnerId={selectedChat.partnerId}
        />
      </div>
      
      {/* Messages Sidebar */}
      <MessagesList onSelectChat={handleSelectChat} />
    </div>
  );
};

export default MessagesPage;