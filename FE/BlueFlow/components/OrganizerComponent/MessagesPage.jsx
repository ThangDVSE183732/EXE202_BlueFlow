import React, { useState } from 'react';
import MessageContent from './MessageContent';
import MessagesList from './MessagesList';

const MessagesPage = ({ showToast }) => {
  const [selectedChat, setSelectedChat] = useState({
    name: 'Event Tech',
    partnerId: '4fc2996d-3e88-45c7-9b09-9585fc5e4435' // Default chat
  });

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
          showToast={showToast}
        />
      </div>
      
      {/* Messages Sidebar */}
      <MessagesList 
        onSelectChat={handleSelectChat}
        showToast={showToast}
      />
    </div>
  );
};

export default MessagesPage;