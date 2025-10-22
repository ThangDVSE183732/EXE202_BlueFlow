import React, { useState } from 'react';
import MessageContent from './MessageContent';
import MessagesList from './MessagesList';

const MessagesPage = () => {
  const [selectedChat, setSelectedChat] = useState('Event Tech');

  const handleSelectChat = (chat) => {
    setSelectedChat(chat.name);
  };

  return (
    <div className="flex h-screen bg-gray-100 mb-10 border border-gray-300 rounded-xl shadow-lg overflow-hidden">
      {/* Main Content Area */}
      <div className="flex-1">
        <MessageContent selectedChat={selectedChat} />
      </div>
      
      {/* Messages Sidebar */}
      <MessagesList onSelectChat={handleSelectChat} />
    </div>
  );
};

export default MessagesPage;