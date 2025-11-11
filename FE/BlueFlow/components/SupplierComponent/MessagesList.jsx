import React, { useState, useEffect } from 'react';
import toast from 'react-hot-toast';
import { Edit, Search, Pin } from 'lucide-react';
import { messageService } from '../../services/messageService';
import signalRService from '../../services/signalRService';
import EqualizerLoader from '../EqualizerLoader';

const MessagesList = ({ onSelectChat, onPartnerListLoaded }) => {
  const [searchTerm, setSearchTerm] = useState('');
  const [allChats, setAllChats] = useState([]);
  const [pinnedChatIds, setPinnedChatIds] = useState(() => {
    // Load pinned chat IDs from localStorage
    const saved = localStorage.getItem('pinnedChats_supplier');
    return saved ? JSON.parse(saved) : [];
  });
  const [onlineUsers, setOnlineUsers] = useState(new Set());
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Save pinned chat IDs to localStorage whenever it changes
  useEffect(() => {
    localStorage.setItem('pinnedChats_supplier', JSON.stringify(pinnedChatIds));
  }, [pinnedChatIds]);

  // Toggle pin/unpin chat
  const togglePinChat = (e, chat) => {
    e.stopPropagation(); // Prevent triggering onSelectChat
    
    const isPinned = pinnedChatIds.includes(chat.id);
    
    if (isPinned) {
      // Unpin
      setPinnedChatIds(pinnedChatIds.filter(id => id !== chat.id));
      toast.success(`Đã bỏ ghim "${chat.name}"`);
    } else {
      // Pin
      setPinnedChatIds([...pinnedChatIds, chat.id]);
      toast.success(`Đã ghim "${chat.name}"`);
    }
  };

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

  // Setup SignalR for online/offline status (backend không có ReceiveMessage broadcast)
  useEffect(() => {
    let isSubscribed = true;

    const initSignalR = async () => {
      try {
        // Start connection if not already started
        if (!signalRService.isConnectionActive()) {
          await signalRService.startConnection();
        }

        // Listen for conversation updates
        signalRService.onConversationUpdated((senderId) => {
          if (!isSubscribed) return;
          console.log('🔄 Conversation updated from:', senderId);
          
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
              setAllChats(formattedChats);
            }
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

  // Separate pinned and unpinned chats
  const pinnedChats = filteredAllChats.filter(chat => pinnedChatIds.includes(chat.id));
  const unpinnedChats = filteredAllChats.filter(chat => !pinnedChatIds.includes(chat.id));

  const handleChatClick = (chat) => {
    // Update chat to mark as read immediately in UI
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
  };

  const renderChatItem = (chat, showNotification = true) => {
    const isPinned = pinnedChatIds.includes(chat.id);
    
    return (
      <div
        key={chat.id}
        onClick={() => handleChatClick(chat)}
        className="flex items-center space-x-3 p-3 hover:bg-gray-50 cursor-pointer rounded-lg transition-colors"
      >
        <div className="relative flex-shrink-0">
          <div className="w-10 h-10 bg-gray-300 rounded-full flex items-center justify-center">
            <span className="text-sm font-medium text-gray-600">
              {chat.name.charAt(0)}
            </span>
          </div>
          {/* Online/Offline indicator */}
          <div className={`absolute -bottom-1 -right-1 w-3 h-3 border-2 border-white rounded-full ${
            onlineUsers.has(chat.id) ? 'bg-green-500' : 'bg-gray-400'
          }`}></div>
          {/* Unread count badge */}
          {showNotification && chat.hasNotification && chat.isRead === false && (
            <div className="absolute -top-1 -right-1 w-4 h-4 bg-red-500 rounded-full flex items-center justify-center">
              <span className="text-xs text-white font-bold">{chat.unreadCount}</span>
            </div>
          )}
        </div>
        <div className="flex-1 min-w-0">
          <div className="flex items-center justify-between">
            <h4 className="text-sm font-medium text-gray-900 truncate">
              {chat.name}
            </h4>
            <button
              onClick={(e) => togglePinChat(e, chat)}
              className={`p-1 hover:bg-gray-200 rounded transition-colors ${
                isPinned ? 'text-blue-500' : 'text-gray-400'
              }`}
              title={isPinned ? 'Unpin chat' : 'Pin chat'}
            >
              <Pin size={14} className={isPinned ? 'fill-current' : ''} />
            </button>
          </div>
          <p className="text-sm text-left text-gray-500 truncate mt-1">
            {chat.message}
          </p>
        </div>
      </div>
    );
  };

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

        {/* Pinned Users - Only show if there are pinned chats */}
        {pinnedChats.length > 0 && (
          <div className="flex space-x-3 mb-4">
            {pinnedChats.slice(0, 4).map((chat) => (
              <div 
                key={chat.id} 
                className="relative cursor-pointer hover:opacity-80 transition-opacity"
                onClick={(e) => togglePinChat(e, chat)}
                title={`Click to unpin ${chat.name}`}
              >
                <div className="w-12 h-12 bg-gray-300 rounded-full flex items-center justify-center">
                  <span className="text-sm font-medium text-gray-600">
                    {chat.name.charAt(0)}
                  </span>
                </div>
                <div className={`absolute -bottom-1 -right-1 w-4 h-4 border-2 border-white rounded-full ${
                  onlineUsers.has(chat.id) ? 'bg-green-500' : 'bg-gray-400'
                }`}></div>
              </div>
            ))}
          </div>
        )}
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
        {/* Pinned Messages - Only show when more than 4 pinned chats */}
        {pinnedChats.length > 4 && (
          <div className="px-4 py-2">
            <div className="flex items-center space-x-2 text-xs text-gray-500 mb-3">
              <div className="w-4 h-4 rounded bg-gray-200 flex items-center justify-center">
                <span className="text-xs">📌</span>
              </div>
              <span className="font-medium">Pinned Messages ({pinnedChats.length})</span>
            </div>
            <div className="space-y-1">
              {pinnedChats.map(chat => renderChatItem(chat))}
            </div>
          </div>
        )}

        {/* All Chats Section */}
        <div className="px-4 py-4">
          <div className="flex items-center space-x-2 text-xs text-gray-500 mb-3">
            <div className="w-4 h-4 rounded bg-gray-200 flex items-center justify-center">
              <span className="text-xs">💬</span>
            </div>
            <span className="font-medium">All Chats</span>
          </div>
          
          {loading ? (
            <div className="py-8">
              <EqualizerLoader message="Đang tải danh sách chat..." />
            </div>
          ) : error ? (
            <div className="text-center text-red-500 py-4">{error}</div>
          ) : unpinnedChats.length === 0 && pinnedChats.length === 0 ? (
            <div className="text-center text-gray-400 py-4">No chats found</div>
          ) : (
            unpinnedChats.map(chat => renderChatItem(chat))
          )}
        </div>
      </div>
    </div>
  );
};

export default MessagesList;