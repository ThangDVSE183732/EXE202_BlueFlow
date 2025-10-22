import React, { useState, useEffect, useRef } from 'react';
import { Send, Paperclip, Search, MoreHorizontal } from 'lucide-react';
import { messageService } from '../../services/messageService';
import signalRService from '../../services/signalRService';

const MessageContent = ({ selectedChat = 'Event Tech' }) => {
  const [newMessage, setNewMessage] = useState('');
  const [messages, setMessages] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [isPartnerTyping, setIsPartnerTyping] = useState(false);
  const typingTimeoutRef = useRef(null);

  // Hardcode partnerId theo yêu cầu
  const partnerId = '4fc2996d-3e88-45c7-9b09-9585fc5e4435';

  // Load messages từ API
  useEffect(() => {
    const fetchMessages = async () => {
      try {
        setLoading(true);
        setError(null);
        const response = await messageService.getConversation(partnerId);
        
        if (response.success && response.data) {
          // Transform API data to component format
          const formattedMessages = response.data.map((msg) => ({
            id: msg.id,
            sender: msg.senderId === partnerId ? selectedChat : 'You',
            content: msg.content,
            timestamp: new Date(msg.sentAt).toLocaleTimeString('en-US', { 
              hour: 'numeric', 
              minute: '2-digit',
              hour12: true 
            }),
            isOwn: msg.senderId !== partnerId, // Message is own if sender is NOT the partner
            isRead: msg.isRead
          }));
          setMessages(formattedMessages);
        }
      } catch (err) {
        console.error('Error loading messages:', err);
        setError('Failed to load messages');
      } finally {
        setLoading(false);
      }
    };

    fetchMessages();
  }, [partnerId, selectedChat]);

  // Setup SignalR connection
  useEffect(() => {
    let isSubscribed = true;

    const initSignalR = async () => {
      try {
        // Start connection if not already started
        if (!signalRService.isConnectionActive()) {
          await signalRService.startConnection();
        }

        // Join this conversation room
        await signalRService.joinConversation(partnerId);

        // Listen for new messages
        signalRService.onReceiveMessage((message) => {
          if (!isSubscribed) return;
          
          console.log('New message received:', message);
          
          // Add new message to the list
          setMessages(prev => [...prev, {
            id: message.id || Date.now(),
            sender: message.senderId === partnerId ? selectedChat : 'You',
            content: message.content,
            timestamp: new Date(message.sentAt || new Date()).toLocaleTimeString('en-US', { 
              hour: 'numeric', 
              minute: '2-digit',
              hour12: true 
            }),
            isOwn: message.senderId !== partnerId,
            isRead: message.isRead || false
          }]);
        });

        // Listen for typing indicator
        signalRService.onUserTyping((data) => {
          if (!isSubscribed) return;
          
          if (data.userId === partnerId) {
            setIsPartnerTyping(true);
            
            // Clear existing timeout
            if (typingTimeoutRef.current) {
              clearTimeout(typingTimeoutRef.current);
            }
            
            // Hide typing indicator after 3 seconds
            typingTimeoutRef.current = setTimeout(() => {
              setIsPartnerTyping(false);
            }, 3000);
          }
        });

        // Listen for message read
        signalRService.onMessageRead((data) => {
          if (!isSubscribed) return;
          
          console.log('Messages marked as read:', data);
          
          // Update all messages as read
          setMessages(prev => prev.map(msg => ({
            ...msg,
            isRead: true
          })));
        });

      } catch (err) {
        console.error('SignalR initialization error:', err);
      }
    };

    initSignalR();

    // Cleanup
    return () => {
      isSubscribed = false;
      signalRService.leaveConversation(partnerId);
      signalRService.off('ReceiveMessage');
      signalRService.off('UserTyping');
      signalRService.off('MessageRead');
      
      if (typingTimeoutRef.current) {
        clearTimeout(typingTimeoutRef.current);
      }
    };
  }, [partnerId, selectedChat]);

  const handleSendMessage = async () => {
    if (newMessage.trim()) {
      const messageContent = newMessage.trim();
      setNewMessage(''); // Clear input immediately
      
      try {
        // Add optimistic message first
        const optimisticMessage = {
          id: Date.now(),
          sender: 'You',
          content: messageContent,
          timestamp: new Date().toLocaleTimeString('en-US', { 
            hour: 'numeric', 
            minute: '2-digit',
            hour12: true 
          }),
          isOwn: true,
          isRead: false
        };
        
        setMessages(prev => [...prev, optimisticMessage]);
        
        // Send via SignalR if connected, otherwise fallback to REST API
        if (signalRService.isConnectionActive()) {
          await signalRService.sendMessage(partnerId, messageContent, null);
        } else {
          // Fallback to REST API
          const messageData = {
            receiverId: partnerId,
            content: messageContent,
            partnershipId: null,
            messageType: "Text",
            attachmentUrl: null,
            attachmentName: null
          };
          await messageService.sendMessage(messageData);
        }
        
      } catch (err) {
        console.error('Error sending message:', err);
        setError('Failed to send message');
        // Remove optimistic message on error
        setMessages(prev => prev.filter(msg => msg.id !== optimisticMessage.id));
      }
    }
  };

  // Handle typing indicator
  const handleInputChange = (e) => {
    setNewMessage(e.target.value);
    
    // Send typing indicator via SignalR
    if (signalRService.isConnectionActive() && e.target.value.trim()) {
      signalRService.sendTypingIndicator(partnerId);
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
          <button className="text-sm py-1 px-2 bg-green-500 text-white rounded-2xl hover:bg-green-600">Agree to collaborate</button>

            <button className="text-gray-400 hover:text-gray-600 p-2">
              <MoreHorizontal size={20} />
            </button>
          </div>
        </div>
      </div>

      {/* Messages Area */}
      <div className="flex-1 overflow-y-auto p-4 space-y-3 bg-gray-50">
        {loading ? (
          <div className="flex items-center justify-center h-full">
            <div className="text-gray-500">Loading messages...</div>
          </div>
        ) : error ? (
          <div className="flex items-center justify-center h-full">
            <div className="text-red-500">{error}</div>
          </div>
        ) : messages.length === 0 ? (
          <div className="flex items-center justify-center h-full">
            <div className="text-gray-400">No messages yet</div>
          </div>
        ) : (
          messages.map((message) => (
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
                    <p className="text-xs leading-relaxed mb-1 text-left">{message.content}</p>
                    {message.timestamp && (
                      <p className={`text-[9px] mt-1 text-right ${message.isOwn ? 'text-gray-200' : 'text-gray-400'}`}>
                        {/* {message.timestamp} */}
                        20:31
                      </p>
                    )}
                  </div>
                </div>
              </div>
            </div>
          </div>
          ))
        )}
        
        {/* Typing Indicator */}
        {isPartnerTyping && (
          <div className="flex items-center space-x-2 py-2">
            <div className="flex space-x-1">
              <div className="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style={{ animationDelay: '0ms' }}></div>
              <div className="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style={{ animationDelay: '150ms' }}></div>
              <div className="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style={{ animationDelay: '300ms' }}></div>
            </div>
            <span className="text-xs text-gray-500">{selectedChat} is typing...</span>
          </div>
        )}
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
              onChange={handleInputChange}
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
