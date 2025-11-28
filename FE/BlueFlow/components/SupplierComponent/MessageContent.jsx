import React, { useState, useEffect, useRef } from 'react';
import toast from 'react-hot-toast';
import { Send, Paperclip, Search, MoreHorizontal } from 'lucide-react';
import { messageService } from '../../services/messageService';
import signalRService from '../../services/signalRService';
import EqualizerLoader from '../EqualizerLoader';

const MessageContent = ({ selectedChat = 'Event Tech', partnerId }) => {
  const [newMessage, setNewMessage] = useState('');
  const [messages, setMessages] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [isPartnerTyping, setIsPartnerTyping] = useState(false);
  const typingTimeoutRef = useRef(null);

  // partnerId được truyền từ props (từ MessagesPage)

  // Load messages từ API
  useEffect(() => {
    if (!partnerId) {
      setLoading(false);
      return;
    }

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
            formattedTime: msg.formattedTime,
            isOwn: msg.senderId !== partnerId, // Message is own if sender is NOT the partner
            isRead: msg.isRead
          }));
          setMessages(formattedMessages);
          
          // Mark conversation as read
          await messageService.markConversationAsRead(partnerId);
          
          // Trigger conversationUpdated event to refresh chat list and unread count
          console.log('✅ Marked conversation as read, triggering refresh...');
          if (signalRService.isConnectionActive()) {
            // Manually trigger refresh by emitting event to ourselves
            signalRService.connection.invoke('OnConversationRead', partnerId).catch(err => {
              console.log('SignalR OnConversationRead invoke failed (expected if not supported):', err);
            });
          }
        }
      } catch (err) {
        console.error('Error loading messages:', err);
        setError('Failed to load messages');
        toast.error('Không thể tải tin nhắn. Vui lòng thử lại.');
      } finally {
        setLoading(false);
      }
    };

    fetchMessages();
  }, [partnerId, selectedChat]);

  // Setup SignalR connection
  useEffect(() => {
    if (!partnerId) return;

    let isSubscribed = true;

    const initSignalR = async () => {
      try {
        // Start connection if not already started
        if (!signalRService.isConnectionActive()) {
          await signalRService.startConnection();
        }

        // Listen for real-time messages from backend
        signalRService.onReceiveMessage((message) => {
          if (!isSubscribed) return;
          
          console.log('📨 Real-time message received:', message);
          
          // Add new message to the list if it's from the current partner
          if (message.senderId === partnerId || message.receiverId === partnerId) {
            setMessages(prev => {
              // Avoid duplicates
              if (prev.some(msg => msg.id === message.id)) {
                return prev;
              }
              
              return [...prev, {
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
              }];
            });
          }
        });
        
        // Listen for typing indicator - Backend sends senderId (string)
        signalRService.onUserTyping((senderId) => {
          if (!isSubscribed) return;
          
          // Check if the typing user is our chat partner
          if (senderId === partnerId) {
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

        // Listen for stop typing
        signalRService.onUserStoppedTyping((senderId) => {
          if (!isSubscribed) return;
          
          if (senderId === partnerId) {
            setIsPartnerTyping(false);
            if (typingTimeoutRef.current) {
              clearTimeout(typingTimeoutRef.current);
            }
          }
        });

      } catch (err) {
        console.error('SignalR initialization error:', err);
      }
    };

    initSignalR();

    // Cleanup
    return () => {
      isSubscribed = false;
      // SignalR event names are camelCase
      signalRService.off('receiveMessage');
      signalRService.off('userTyping');
      signalRService.off('userStoppedTyping');
      
      if (typingTimeoutRef.current) {
        clearTimeout(typingTimeoutRef.current);
      }
    };
  }, [partnerId, selectedChat]);

  const handleSendMessage = async () => {
    if (newMessage.trim()) {
      const messageContent = newMessage.trim();
      setNewMessage(''); // Clear input immediately
      
      const optimisticMessageId = Date.now(); // Store ID outside try-catch
      
      try {
        // Add optimistic message first
        const optimisticMessage = {
          id: optimisticMessageId,
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
        
        // Backend không có SendMessage qua SignalR - chỉ dùng REST API
        const messageData = {
          receiverId: partnerId,
          content: messageContent,
          partnershipId: null,
          messageType: "Text",
          attachmentUrl: null,
          attachmentName: null
        };
        await messageService.sendMessage(messageData);
        
        // Stop typing indicator after sending
        if (signalRService.isConnectionActive()) {
          await signalRService.stopTypingIndicator(partnerId);
        }
        
      } catch (err) {
        console.error('Error sending message:', err);
        setError('Failed to send message');
        toast.error('Không thể gửi tin nhắn. Vui lòng thử lại.');
        
        // Remove optimistic message on error
        setMessages(prev => prev.filter(msg => msg.id !== optimisticMessageId));
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

  // Get initials from name
  const getInitials = (name) => {
    if (!name) return 'U';
    const parts = name.trim().split(' ');
    if (parts.length >= 2) {
      return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
    }
    return name.substring(0, 2).toUpperCase();
  };

  return (
    <div className="flex-1 flex flex-col bg-white h-full overflow-hidden">
      {/* Chat Header */}
      <div className="border-b border-gray-200 px-5 py-4 flex-shrink-0 bg-white">
        <div className="flex items-center justify-between">
          <div className="flex items-center space-x-3 min-w-0 flex-1">
            <div className="relative">
              <div className="w-11 h-11 bg-blue-500 rounded-full flex items-center justify-center flex-shrink-0">
                <span className="text-white font-semibold text-sm">{getInitials(selectedChat)}</span>
              </div>
              <div className="absolute -bottom-0.5 -right-0.5 w-3.5 h-3.5 bg-green-500 border-2 border-white rounded-full"></div>
            </div>
            <div className="min-w-0 flex-1">
              <h2 className="text-base font-semibold text-gray-900 truncate text-left" title={selectedChat}>{selectedChat}</h2>
              <p className="text-xs text-gray-500 text-left">Đang hoạt động</p>
            </div>
          </div>
          <div className="flex items-center space-x-3">
            <button className="text-sm py-2 px-4 bg-green-500 text-white rounded-full hover:bg-green-600 transition-colors">
              Đồng ý hợp tác
            </button>
            <button className="text-gray-500 hover:text-gray-700 p-2 rounded-lg hover:bg-gray-100 transition-colors">
              <MoreHorizontal size={20} />
            </button>
          </div>
        </div>
      </div>

      {/* Messages Area */}
      <div className="flex-1 overflow-y-auto px-5 py-4 space-y-3 bg-white" style={{ scrollbarWidth: 'thin' }}>
        {loading ? (
          <div className="flex items-center justify-center h-full">
            <EqualizerLoader message="Đang tải tin nhắn..." />
          </div>
        ) : error ? (
          <div className="flex items-center justify-center h-full">
            <div className="text-red-500 bg-red-50 px-4 py-2 rounded-lg">{error}</div>
          </div>
        ) : messages.length === 0 ? (
          <div className="flex items-center justify-center h-full">
            <div className="text-center">
              <div className="text-gray-400 text-base mb-1">Chưa có tin nhắn nào</div>
              <div className="text-gray-400 text-sm">Bắt đầu cuộc trò chuyện bằng cách gửi tin nhắn đầu tiên</div>
            </div>
          </div>
        ) : (
          messages.map((message) => (
            <div key={message.id} className="flex flex-col">
              <div className={`flex ${message.isOwn ? 'justify-end' : 'justify-start'}`}>
                <div className={`flex items-end gap-2 max-w-[75%] sm:max-w-[65%] ${message.isOwn ? 'flex-row-reverse' : ''}`}>
                  {/* Avatar - Only show for partner messages */}
                  {!message.isOwn && (
                    <div className="w-8 h-8 bg-blue-500 rounded-full flex items-center justify-center flex-shrink-0 mb-1">
                      <span className="text-white font-semibold text-xs">{getInitials(selectedChat)}</span>
                    </div>
                  )}
                  
                  {/* Message Bubble */}
                  <div className={`relative ${message.isOwn ? 'order-2' : ''}`}>
                    <div
                      className={`px-4 py-2.5 rounded-2xl max-w-full ${
                        message.isOwn
                          ? 'bg-blue-500 text-white rounded-tr-sm'
                          : 'bg-white text-gray-800 border border-gray-200 rounded-tl-sm'
                      }`}
                    >
                      <p className={`text-sm leading-relaxed break-words whitespace-pre-wrap ${
                        message.isOwn ? 'text-white' : 'text-gray-800'
                      }`}>
                        {message.content}
                      </p>
                      {message.timestamp && (
                        <p className={`text-[10px] mt-1 text-right ${message.isOwn ? 'text-blue-100' : 'text-gray-400'}`}>
                          {message.formattedTime || message.timestamp}
                        </p>
                      )}
                    </div>
                  </div>

                  {/* Avatar - Only show for own messages */}
                  {message.isOwn && (
                    <div className="w-8 h-8 bg-pink-500 rounded-full flex items-center justify-center flex-shrink-0 mb-1">
                      <span className="text-white font-semibold text-[10px]">Bạn</span>
                    </div>
                  )}
                </div>
              </div>
            </div>
          ))
        )}
        
        {/* Typing Indicator */}
        {isPartnerTyping && (
          <div className="flex justify-start">
            <div className="flex items-end gap-2 max-w-[65%]">
              <div className="w-8 h-8 bg-blue-500 rounded-full flex items-center justify-center flex-shrink-0 mb-1">
                <span className="text-white font-semibold text-xs">{getInitials(selectedChat)}</span>
              </div>
              <div className="relative">
                <div className="px-4 py-2.5 rounded-2xl rounded-tl-sm bg-white border border-gray-200">
                  <div className="flex items-center space-x-1.5">
                    <div className="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style={{ animationDelay: '0ms' }}></div>
                    <div className="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style={{ animationDelay: '150ms' }}></div>
                    <div className="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style={{ animationDelay: '300ms' }}></div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        )}
      </div>

      {/* Message Input */}
      <div className="border-t border-gray-200 px-5 py-4 bg-white flex-shrink-0">
        <div className="flex items-center gap-3">
          <button className="text-gray-500 hover:text-gray-700 p-2 rounded-lg hover:bg-gray-100 transition-colors flex-shrink-0">
            <Paperclip size={20} />
          </button>
          <div className="flex-1 relative">
            <input
              type="text"
              value={newMessage}
              onChange={handleInputChange}
              onKeyPress={handleKeyPress}
              placeholder="Nhập tin nhắn..."
              className="w-full px-4 py-2.5 bg-gray-50 border-0 rounded-full text-sm focus:ring-2 focus:ring-blue-500 focus:bg-white outline-none transition-all"
            />
          </div>
          <button
            onClick={handleSendMessage}
            disabled={!newMessage.trim()}
            className={`p-2.5 rounded-full transition-colors flex-shrink-0 ${
              newMessage.trim()
                ? 'bg-blue-500 hover:bg-blue-600 text-white'
                : 'bg-gray-300 text-gray-500 cursor-not-allowed'
            }`}
          >
            <Send size={20} />
          </button>
        </div>
      </div>

      <style>{`
        /* Custom scrollbar */
        .overflow-y-auto::-webkit-scrollbar {
          width: 6px;
        }
        .overflow-y-auto::-webkit-scrollbar-track {
          background: transparent;
        }
        .overflow-y-auto::-webkit-scrollbar-thumb {
          background: #cbd5e1;
          border-radius: 3px;
        }
        .overflow-y-auto::-webkit-scrollbar-thumb:hover {
          background: #94a3b8;
        }
      `}</style>
    </div>
  );
};

export default MessageContent;
