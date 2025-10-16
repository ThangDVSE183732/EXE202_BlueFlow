import React, { useState } from 'react';
import { Send, Paperclip, Search, MoreHorizontal } from 'lucide-react';

const MessageContent = ({ selectedChat = 'Event Tech' }) => {
  const [newMessage, setNewMessage] = useState('');

  const messages = [
    {
      id: 1,
      sender: 'Event Tech',
      content: 'Hi there! I\'d be happy to assist you with that. Can you please confirm the email address associated with your account?',
      timestamp: '9:00 AM',
      isOwn: false
    },
    {
      id: 2,
      sender: 'You',
      content: 'My email is example@email.com',
      timestamp: '9:30 AM',
      isOwn: true
    },
    {
      id: 3,
      sender: 'You',
      content: 'Thanks I see your account! It looks like there might be a sync issue. Could you please try clearing your browser\'s cache and cookies?',
      timestamp: '',
      isOwn: true
    },
    {
      id: 4,
      sender: 'Event Tech',
      content: 'Okay, just cleared them. Trying to log in now... Yes! It worked! Thank you so much for your help!',
      timestamp: '10:22 AM',
      isOwn: false
    }
  ];

  const handleSendMessage = () => {
    if (newMessage.trim()) {
      console.log('Sending message:', newMessage);
      setNewMessage('');
    }
  };

  const handleKeyPress = (e) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSendMessage();
    }
  };

  return (
    <div className="flex-1 flex flex-col bg-white h-full max-h-screen  overflow-hidden shadow-xl">
      {/* Chat Header */}
      <div className="border-b border-gray-200 px-6 py-4 flex-shrink-0">
        <div className="flex items-center justify-between">
          <div className="flex items-center space-x-3">
            <div className="w-10 h-10 bg-blue-500 rounded-full flex items-center justify-center">
              <span className="text-white font-semibold text-sm">ET</span>
            </div>
            <div>
              <h2 className="text-lg font-semibold text-gray-900">{selectedChat}</h2>
            </div>
          </div>
          <div className="flex items-center space-x-4">
          <button className="text-sm text-blue-500 py-1 px-2 bg-green-500 text-white rounded-2xl">Agree to collaborate</button>

            <button className="text-gray-400 hover:text-gray-600 p-2">
              <MoreHorizontal size={20} />
            </button>
          </div>
        </div>
      </div>

      {/* Messages Area */}
      <div className="flex-1 overflow-y-auto p-4 space-y-3 bg-gray-50">
        {messages.map((message) => (
          <div key={message.id} className="flex flex-col">
            <div className={`flex ${message.isOwn ? 'justify-end' : 'justify-start'}`}>
              <div className={`flex items-start space-x-2 max-w-sm ${message.isOwn ? 'flex-row-reverse space-x-reverse' : ''}`}>
                {!message.isOwn && (
                  <div className="w-7 h-7 bg-blue-500 rounded-full flex items-center justify-center flex-shrink-0 mt-1">
                    <span className="text-white font-semibold text-xs">ET</span>
                  </div>
                )}
                <div className="flex flex-col">
                  <div
                    className={`px-3 py-2 rounded-xl max-w-xs ${
                      message.isOwn
                        ? 'bg-blue-500 text-white rounded-br-md'
                        : 'bg-white text-gray-900 border border-gray-200 rounded-bl-md'
                    }`}
                  >
                    <p className="text-xs leading-relaxed">{message.content}</p>
                  </div>
                </div>
              </div>
            </div>
            {message.timestamp && (
              <div className={`flex ${message.isOwn ? 'justify-end' : 'justify-start'} mt-1`}>
                <p className="text-xs text-gray-500 px-2">
                  {message.timestamp}
                </p>
              </div>
            )}
          </div>
        ))}
      </div>

      {/* Message Input */}
      <div className="border-t border-gray-200 px-6 py-4 bg-white flex-shrink-0">
        <div className="flex items-center space-x-3">
          <button className="text-gray-400 hover:text-gray-600 p-2">
            <Paperclip size={20} />
          </button>
          <div className="flex-1 relative">
            <input
              type="text"
              value={newMessage}
              onChange={(e) => setNewMessage(e.target.value)}
              onKeyPress={handleKeyPress}
              placeholder="Type a message..."
              className="w-full px-4 py-3 border border-gray-300 rounded-full text-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none bg-gray-50"
            />
          </div>
          <button
            onClick={handleSendMessage}
            disabled={!newMessage.trim()}
            className="text-blue-500 hover:text-blue-600 disabled:text-gray-400 p-2"
          >
            <Send size={20} />
          </button>
        </div>
      </div>
    </div>
  );
};

export default MessageContent;
