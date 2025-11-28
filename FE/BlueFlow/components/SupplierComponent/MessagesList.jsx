import React, { useState, useEffect } from 'react';
import toast from 'react-hot-toast';
import { Edit, Search } from 'lucide-react';
import { messageService } from '../../services/messageService';
import signalRService from '../../services/signalRService';
import EqualizerLoader from '../EqualizerLoader';

const MessagesList = ({ onSelectChat, onPartnerListLoaded }) => {
  const [searchTerm, setSearchTerm] = useState('');
  const [allChats, setAllChats] = useState([]);
  const [onlineUsers, setOnlineUsers] = useState(new Set());
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Load partner list chats from API
  useEffect(() => {
    const fetchPartnerListChat = async () => {
      try {
        setLoading(true);
        setError(null);
        const response = await messageService.getPartnerListChat();
        
        console.log('Partner list chat response:', response);
        
        if (response.success && response.data && Array.isArray(response.data)) {
          // Transform API data to component format
          const formattedChats = response.data.map((chat) => ({
            id: chat.partnerId,
            name: chat.partnerName || 'Unknown',
            message: chat.lastMessage?.content || 'No messages yet',
            time: chat.lastMessageTime ? new Date(chat.lastMessageTime).toLocaleTimeString('en-US', { 
              hour: 'numeric', 
              minute: '2-digit',
              hour12: false 
            }) : '',
            avatar: chat.partnerAvatar,
            hasNotification: (chat.unreadCount || 0) > 0,
            isRead: chat.lastMessage?.isRead,
            unreadCount: chat.unreadCount || 0,
            partnerRole: chat.partnerRole
          }));
          setAllChats(formattedChats);
          
          // Notify parent component that partner list is loaded
          if (onPartnerListLoaded && formattedChats.length > 0) {
            onPartnerListLoaded(formattedChats);
          }
        } else {
          setAllChats([]);
        }
      } catch (err) {
        console.error('Error loading partner list chat:', err);
        setError('Failed to load chats');
        toast.error('Không thể tải danh sách đối tác. Vui lòng thử lại.');
      } finally {
        setLoading(false);
      }
    };

    fetchPartnerListChat();
  }, [onPartnerListLoaded]);

  // Setup SignalR for conversation updates and online/offline status
  useEffect(() => {
    let isSubscribed = true;

    const initSignalR = async () => {
      try {
        // Start connection if not already started (OrganizerPage might have started it already)
        if (!signalRService.isConnectionActive()) {
          console.log('🚀 MessagesList: Initializing SignalR connection...');
          await signalRService.startConnection();
        } else {
          console.log('✅ MessagesList: SignalR already connected');
        }

        // Register event handlers for conversation updates in chat list
        console.log('📝 MessagesList: Registering conversationUpdated handler...');
        signalRService.onConversationUpdated((senderId) => {
          if (!isSubscribed) return;
          console.log('🔄 MessagesList: Conversation updated from:', senderId);
          
          // Refresh the partner list to get latest message
          messageService.getPartnerListChat().then(response => {
            if (response.success && response.data) {
              const formattedChats = response.data.map((chat) => ({
                id: chat.partnerId,
                name: chat.partnerName || 'Unknown',
                message: chat.lastMessage?.content || 'No messages yet',
                time: chat.lastMessageTime ? new Date(chat.lastMessageTime).toLocaleTimeString('en-US', { 
                  hour: 'numeric', 
                  minute: '2-digit',
                  hour12: false 
                }) : '',
                avatar: chat.partnerAvatar,
                hasNotification: (chat.unreadCount || 0) > 0,
                unreadCount: chat.unreadCount || 0,
                partnerRole: chat.partnerRole
              }));
              console.log('✅ Updated chats with unread counts:', formattedChats.map(c => ({ name: c.name, unread: c.unreadCount })));
              setAllChats(formattedChats);
            }
          }).catch(err => {
            console.error('❌ Error refreshing partner list:', err);
          });
        });
        
        signalRService.onUserOnline((userId) => {
          if (!isSubscribed) return;
          console.log('🟢 User online:', userId);
          setOnlineUsers(prev => new Set([...prev, userId]));
        });

        signalRService.onUserOffline((userId) => {
          if (!isSubscribed) return;
          console.log('🔴 User offline:', userId);
          setOnlineUsers(prev => {
            const newSet = new Set(prev);
            newSet.delete(userId);
            return newSet;
          });
        });

      } catch (err) {
        console.error('SignalR initialization error in MessagesList:', err);
      }
    };

    initSignalR();

    // Cleanup
    return () => {
      isSubscribed = false;
      // SignalR event names are camelCase
      signalRService.off('conversationUpdated');
      signalRService.off('userOnline');
      signalRService.off('userOffline');
    };
  }, []);

  const filteredAllChats = allChats.filter(chat =>
    chat.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleChatClick = async (chat) => {
    // Update chat to mark as read immediately in UI (optimistic update)
    setAllChats(prevChats => 
      prevChats.map(c => 
        c.id === chat.id 
          ? { ...c, isRead: true, hasNotification: false, unreadCount: 0 } 
          : c
      )
    );
    
    // Call parent handler
    if (onSelectChat) {
      onSelectChat(chat);
    }
    
    // Refresh chat list after a short delay to sync with server
    setTimeout(async () => {
      try {
        const response = await messageService.getPartnerListChat();
        if (response.success && response.data) {
          const formattedChats = response.data.map((chatItem) => ({
            id: chatItem.partnerId,
            name: chatItem.partnerName || 'Unknown',
            message: chatItem.lastMessage?.content || 'No messages yet',
            time: chatItem.lastMessageTime ? new Date(chatItem.lastMessageTime).toLocaleTimeString('en-US', { 
              hour: 'numeric', 
              minute: '2-digit',
              hour12: false 
            }) : '',
            avatar: chatItem.partnerAvatar,
            hasNotification: (chatItem.unreadCount || 0) > 0,
            unreadCount: chatItem.unreadCount || 0,
            partnerRole: chatItem.partnerRole
          }));
          setAllChats(formattedChats);
          console.log('🔄 Chat list refreshed after marking as read');
        }
      } catch (err) {
        console.error('Error refreshing chat list:', err);
      }
    }, 1000); // 1 second delay to allow backend to process
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

  const renderChatItem = (chat, showNotification = true) => {
    return (
      <div
        key={chat.id}
        onClick={() => handleChatClick(chat)}
        className="flex items-center gap-3 pr-5 py-3 hover:bg-gray-50 cursor-pointer transition-colors"
      >
        <div className="relative flex-shrink-0">
          <div className="w-11 h-11 bg-blue-500 rounded-full flex items-center justify-center">
            <span className="text-sm font-semibold text-white">
              {getInitials(chat.name)}
            </span>
          </div>
          {/* Online/Offline indicator */}
          <div className={`absolute -bottom-0.5 -right-0.5 w-3.5 h-3.5 border-2 border-white rounded-full ${
            onlineUsers.has(chat.id) ? 'bg-green-500' : 'bg-gray-400'
          }`}></div>
          {/* Unread count badge */}
          {showNotification && chat.hasNotification && chat.isRead === false && (
            <div className="absolute -top-1 -right-1 min-w-[18px] h-[18px] bg-red-500 rounded-full flex items-center justify-center px-1">
              <span className="text-[10px] text-white font-bold">{chat.unreadCount > 99 ? '99+' : chat.unreadCount}</span>
            </div>
          )}
        </div>
        <div className="flex-1 min-w-0">
          <div className="flex items-center justify-between mb-0.5">
            <h4 className={`text-sm font-medium truncate text-left ${
              chat.hasNotification && !chat.isRead ? 'text-gray-900 font-semibold' : 'text-gray-900'
            }`}>
              {chat.name}
            </h4>
            {chat.time && (
              <span className="text-xs text-gray-400 flex-shrink-0 ml-2">{chat.time}</span>
            )}
          </div>
          <p className={`text-sm text-left truncate ${
            chat.hasNotification && !chat.isRead 
              ? 'text-gray-900 font-medium' 
              : 'text-gray-500'
          }`}>
            {chat.message}
          </p>
        </div>
      </div>
    );
  };

  return (
    <div className="w-70 bg-white flex flex-col h-full overflow-hidden">
      {/* Header */}
      <div className="px-5 border-b border-gray-200 flex-shrink-0 bg-white" style={{ paddingTop: '21px', paddingBottom: '21px' }}>
        <div className="flex items-center justify-between">
          <h2 className="text-lg font-semibold text-gray-900">Tin nhắn</h2>
          <div className="flex items-center space-x-1">
            <button className="p-2 text-gray-500 hover:text-gray-700 rounded-lg hover:bg-gray-100 transition-colors">
              <Edit size={18} />
            </button>
            <button className="p-2 text-gray-500 hover:text-gray-700 rounded-lg hover:bg-gray-100 transition-colors">
              <Search size={18} />
            </button>
          </div>
        </div>
      </div>

      {/* Search */}
      <div className="px-5 pt-4 pb-4 flex-shrink-0">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" size={16} />
          <input
            type="text"
            placeholder="Tìm kiếm tin nhắn..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full pl-10 pr-4 py-2.5 bg-gray-50 border-0 rounded-lg text-sm focus:ring-2 focus:ring-blue-500 focus:bg-white outline-none transition-all"
          />
        </div>
      </div>

      {/* Messages List */}
      <div className="flex-1 overflow-y-auto" style={{ scrollbarWidth: 'thin' }}>
        <div className="pl-4 pr-5 py-3">
          <div className="flex items-center space-x-2 text-xs text-gray-500 mb-3">
            <span className="font-medium text-gray-600">Tất cả cuộc trò chuyện</span>
          </div>
          
          {loading ? (
            <div className="py-8">
              <EqualizerLoader message="Đang tải danh sách chat..." />
            </div>
          ) : error ? (
            <div className="text-center text-red-500 py-4 bg-red-50 rounded-lg mx-2">{error}</div>
          ) : filteredAllChats.length === 0 ? (
            <div className="text-center py-8">
              <div className="text-gray-400 text-sm mb-1">Chưa có cuộc trò chuyện nào</div>
              <div className="text-gray-400 text-xs">Bắt đầu trò chuyện với đối tác của bạn</div>
            </div>
          ) : (
            <div className="space-y-0">
              {filteredAllChats.map(chat => renderChatItem(chat))}
            </div>
          )}
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

export default MessagesList;
