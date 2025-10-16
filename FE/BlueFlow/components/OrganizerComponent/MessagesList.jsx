import React, { useState } from 'react';
import { Edit, Search } from 'lucide-react';

const MessagesList = ({ onSelectChat }) => {
  const [searchTerm, setSearchTerm] = useState('');

  const contacts = [
    {
      id: 1,
      name: 'George Lobko',
      message: 'Thanks for the quick response...',
      time: '10:28',
      avatar: '/api/placeholder/32/32',
      hasNotification: false
    },
    {
      id: 2,
      name: 'George Lobko',
      message: 'Thanks for the quick response...',
      time: '10:28',
      avatar: '/api/placeholder/32/32',
      hasNotification: true
    },
    {
      id: 3,
      name: 'George Lobko',
      message: 'Thanks for the quick response...',
      time: '10:28',
      avatar: '/api/placeholder/32/32',
      hasNotification: false
    }
  ];

  const allChats = [
    {
      id: 4,
      name: 'An Nguyen',
      message: 'Thanks for the quick...',
      time: '10:28',
      avatar: '/api/placeholder/32/32',
      hasNotification: false
    },
    {
      id: 5,
      name: 'George Lobko',
      message: 'Thanks for the quick response...',
      time: '10:28',
      avatar: '/api/placeholder/32/32',
      hasNotification: false
    },
    {
      id: 6,
      name: 'George Lobko',
      message: 'Thanks for the quick response...',
      time: '10:28',
      avatar: '/api/placeholder/32/32',
      hasNotification: true
    },
    {
      id: 7,
      name: 'George Lobko',
      message: 'Thanks for the quick response...',
      time: '10:28',
      avatar: '/api/placeholder/32/32',
      hasNotification: true
    },
    {
      id: 8,
      name: 'George Lobko',
      message: 'Thanks for the quick response...',
      time: '10:28',
      avatar: '/api/placeholder/32/32',
      hasNotification: false
    },
    {
      id: 9,
      name: 'George Lobko',
      message: 'Thanks for the quick response...',
      time: '10:28',
      avatar: '/api/placeholder/32/32',
      hasNotification: true
    }
  ];

  const filteredContacts = contacts.filter(contact =>
    contact.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const filteredAllChats = allChats.filter(chat =>
    chat.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const renderChatItem = (chat, showNotification = true) => (
    <div
      key={chat.id}
      onClick={() => onSelectChat && onSelectChat(chat)}
      className="flex items-center space-x-3 p-3 hover:bg-gray-50 cursor-pointer rounded-lg transition-colors"
    >
      <div className="relative flex-shrink-0">
        <div className="w-10 h-10 bg-gray-300 rounded-full flex items-center justify-center">
          <span className="text-sm font-medium text-gray-600">
            {chat.name.charAt(0)}
          </span>
        </div>
        {showNotification && chat.hasNotification && (
          <div className="absolute -top-1 -right-1 w-4 h-4 bg-red-500 rounded-full flex items-center justify-center">
            <span className="text-xs text-white font-bold">!</span>
          </div>
        )}
      </div>
      <div className="flex-1 min-w-0">
        <div className="flex items-center justify-between">
          <h4 className="text-sm font-medium text-gray-900 truncate">
            {chat.name}
          </h4>
          <span className="text-xs text-gray-500 flex-shrink-0">
            {chat.time}
          </span>
        </div>
        <p className="text-sm text-gray-500 truncate mt-1">
          {chat.message}
        </p>
      </div>
    </div>
  );

  return (
    <div className="w-70 bg-white border-l border-gray-200 flex flex-col h-full max-h-screen  overflow-hidden">
      {/* Header */}
      <div className="p-4 border-b border-gray-200 flex-shrink-0">
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-xl font-semibold text-gray-900">Messages</h2>
          <div className="flex items-center space-x-2">
            <button className="p-2 text-gray-400 hover:text-gray-600 rounded-lg hover:bg-gray-100">
              <Edit size={18} />
            </button>
            <button className="p-2 text-gray-400 hover:text-gray-600 rounded-lg hover:bg-gray-100">
              <Search size={18} />
            </button>
          </div>
        </div>

        {/* Online Users */}
        <div className="flex space-x-3 mb-4">
          {[1, 2, 3, 4].map((user, index) => (
            <div key={index} className="relative">
              <div className="w-12 h-12 bg-gray-300 rounded-full flex items-center justify-center">
                <span className="text-sm font-medium text-gray-600">U</span>
              </div>
              <div className="absolute -bottom-1 -right-1 w-4 h-4 bg-green-500 border-2 border-white rounded-full"></div>
            </div>
          ))}
        </div>
      </div>

      {/* Search */}
      <div className="p-4 flex-shrink-0">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" size={16} />
          <input
            type="text"
            placeholder="Search messages..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none"
          />
        </div>
      </div>

      {/* Messages List */}
      <div className="flex-1 overflow-y-auto">
        {/* Pinned Message */}
        <div className="px-4 py-2">
          <div className="flex items-center space-x-2 text-xs text-gray-500 mb-3">
            <div className="w-4 h-4 rounded bg-gray-200 flex items-center justify-center">
              <span className="text-xs">📌</span>
            </div>
            <span className="font-medium">Pinned Message</span>
          </div>
        </div>

        {/* Recent Contacts */}
        <div className="px-4">
          {filteredContacts.map(contact => renderChatItem(contact))}
        </div>

        {/* All Chats Section */}
        <div className="px-4 py-4">
          <div className="flex items-center space-x-2 text-xs text-gray-500 mb-3">
            <div className="w-4 h-4 rounded bg-gray-200 flex items-center justify-center">
              <span className="text-xs">💬</span>
            </div>
            <span className="font-medium">All Chats</span>
          </div>
          {filteredAllChats.map(chat => renderChatItem(chat))}
        </div>
      </div>
    </div>
  );
};

export default MessagesList;