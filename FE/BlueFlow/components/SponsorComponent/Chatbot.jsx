import React, { useState, useRef, useEffect } from 'react';
import toast from 'react-hot-toast';
import { Send, Sparkles, User, Bot, Loader2 } from 'lucide-react';
import { chatbotService } from '../../services/chatbotService';
import { parseChatbotResponse } from '../../utils/chatbotUtils';

const Chatbot = () => {
  const [messages, setMessages] = useState([
    {
      id: 1,
      sender: 'ai',
      text: "Xin chào! Tôi là trợ lý AI của EventLink. Tôi có thể giúp gì cho bạn hôm nay?"
    }
  ]);
  const [inputText, setInputText] = useState('');
  const [isTyping, setIsTyping] = useState(false);
  const [error, setError] = useState(null);
  const messagesEndRef = useRef(null);
  const messagesContainerRef = useRef(null);

  const scrollToBottom = () => {
    if (messagesEndRef.current) {
      messagesEndRef.current.scrollIntoView({ 
        behavior: 'smooth',
        block: 'end'
      });
    }
  };

  useEffect(() => {
    // Auto-scroll to bottom when new message arrives
    if (messagesContainerRef.current) {
      const { scrollTop, scrollHeight, clientHeight } = messagesContainerRef.current;
      const isNearBottom = scrollHeight - scrollTop - clientHeight < 150;
      
      if (isNearBottom || messages.length <= 2) {
        setTimeout(() => scrollToBottom(), 100);
      }
    }
  }, [messages, isTyping]);

  const handleSendMessage = async () => {
    if (inputText.trim() === '' || isTyping) return;

    const userInput = inputText.trim();
    
    // Add user message
    const userMessage = {
      id: Date.now(),
      sender: 'user',
      text: userInput
    };

    setMessages(prev => [...prev, userMessage]);
    setInputText('');
    setIsTyping(true);
    setError(null);

    try {
      // Call API
      const response = await chatbotService.sendPrompt({
        question: userInput
      });

      // Parse and format response
      const formattedAnswer = parseChatbotResponse(response);

      // Add AI response
      const aiMessage = {
        id: Date.now() + 1,
        sender: 'ai',
        text: formattedAnswer
      };
      
      setMessages(prev => [...prev, aiMessage]);
    } catch (err) {
      console.error('Error sending message:', err);
      setError('Failed to get response from AI');
      
      // Show error toast
      toast.error('Không thể nhận phản hồi từ trợ lý AI. Vui lòng thử lại.');
      
      // Add error message
      const errorMessage = {
        id: Date.now() + 1,
        sender: 'ai',
        text: "Xin lỗi, tôi gặp lỗi khi xử lý yêu cầu của bạn. Vui lòng thử lại sau."
      };
      setMessages(prev => [...prev, errorMessage]);
    } finally {
      setIsTyping(false);
    }
  };

  const handleKeyPress = (e) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSendMessage();
    }
  };

  return (
    <div className="h-screen bg-gradient-to-br from-gray-50 to-gray-100 mb-10 rounded-2xl shadow-2xl overflow-hidden border border-gray-200">
      <div className="bg-white w-full h-full flex flex-col">
        {/* Header */}
        <div className="flex items-center justify-between px-6 py-4 bg-gradient-to-r from-blue-600 via-blue-500 to-cyan-500 shadow-md">
          <div className="flex items-center space-x-3">
            <div className="relative">
              <div className="w-12 h-12 rounded-xl bg-white/20 backdrop-blur-sm flex items-center justify-center shadow-lg">
                <Sparkles className="w-6 h-6 text-white" />
              </div>
              <div className="absolute -bottom-1 -right-1 w-4 h-4 bg-green-400 border-2 border-white rounded-full"></div>
            </div>
            <div>
              <h2 className="text-xl font-bold text-white">Trợ lý AI EventLink</h2>
              <p className="text-xs text-blue-100">Luôn sẵn sàng hỗ trợ</p>
            </div>
          </div>
        </div>

        {/* Messages Area */}
        <div 
          ref={messagesContainerRef} 
          className="flex-1 overflow-y-auto px-4 sm:px-6 py-6 space-y-4 bg-gradient-to-b from-gray-50 to-white"
          style={{ scrollbarWidth: 'thin' }}
        >
          {messages.map((message, index) => (
            <div 
              key={message.id} 
              className={`flex ${message.sender === 'user' ? 'justify-end' : 'justify-start'} animate-fade-in`}
              style={{ animationDelay: `${index * 50}ms` }}
            >
              <div className={`flex items-end gap-2 max-w-[75%] sm:max-w-[65%] ${message.sender === 'user' ? 'flex-row-reverse' : ''}`}>
                {/* Avatar - Only show for AI messages */}
                {message.sender === 'ai' && (
                  <div className="w-8 h-8 rounded-full bg-gradient-to-br from-blue-500 to-cyan-500 flex items-center justify-center flex-shrink-0 shadow-md mb-1">
                    <Bot className="w-4 h-4 text-white" />
                  </div>
                )}
                
                {/* Message Bubble */}
                <div className={`relative group ${message.sender === 'user' ? 'order-2' : ''}`}>
                  <div className={`px-4 py-3 rounded-2xl shadow-sm ${
                    message.sender === 'user'
                      ? 'bg-gradient-to-br from-blue-500 to-blue-600 text-white rounded-tr-sm'
                      : 'bg-white text-gray-800 border border-gray-200 rounded-tl-sm shadow-md'
                  }`}>
                    <p className={`text-sm leading-relaxed break-words whitespace-pre-wrap ${
                      message.sender === 'user' ? 'text-white' : 'text-gray-800'
                    }`}>
                      {message.text}
                    </p>
                  </div>
                  {/* Tail */}
                  {message.sender === 'user' ? (
                    <div className="absolute right-0 bottom-0 w-0 h-0 border-l-[8px] border-l-transparent border-b-[8px] border-b-blue-600"></div>
                  ) : (
                    <div className="absolute left-0 bottom-0 w-0 h-0 border-r-[8px] border-r-transparent border-b-[8px] border-b-white"></div>
                  )}
                </div>

                {/* Avatar - Only show for user messages */}
                {message.sender === 'user' && (
                  <div className="w-8 h-8 rounded-full bg-gradient-to-br from-purple-500 to-pink-500 flex items-center justify-center flex-shrink-0 shadow-md mb-1">
                    <User className="w-4 h-4 text-white" />
                  </div>
                )}
              </div>
            </div>
          ))}

          {/* Typing Indicator */}
          {isTyping && (
            <div className="flex justify-start animate-fade-in">
              <div className="flex items-end gap-2 max-w-[65%]">
                <div className="w-8 h-8 rounded-full bg-gradient-to-br from-blue-500 to-cyan-500 flex items-center justify-center flex-shrink-0 shadow-md mb-1">
                  <Bot className="w-4 h-4 text-white" />
                </div>
                <div className="relative">
                  <div className="px-4 py-3 rounded-2xl rounded-tl-sm bg-white border border-gray-200 shadow-md">
                    <div className="flex items-center space-x-1.5">
                      <div className="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style={{ animationDelay: '0ms' }}></div>
                      <div className="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style={{ animationDelay: '150ms' }}></div>
                      <div className="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style={{ animationDelay: '300ms' }}></div>
                    </div>
                  </div>
                  <div className="absolute left-0 bottom-0 w-0 h-0 border-r-[8px] border-r-transparent border-b-[8px] border-b-white"></div>
                </div>
              </div>
            </div>
          )}
          
          <div ref={messagesEndRef} />
        </div>

        {/* Input Area */}
        <div className="px-4 sm:px-6 py-4 bg-white border-t border-gray-200 shadow-lg">
          <div className="flex items-end gap-3">
            <div className="flex-1 relative">
              <div className="relative bg-gray-50 rounded-2xl border-2 border-gray-200 focus-within:border-blue-500 transition-colors shadow-sm">
                <textarea
                  value={inputText}
                  onChange={(e) => {
                    setInputText(e.target.value);
                    // Auto resize
                    e.target.style.height = 'auto';
                    e.target.style.height = Math.min(e.target.scrollHeight, 120) + 'px';
                  }}
                  onKeyPress={handleKeyPress}
                  placeholder="Nhập tin nhắn của bạn..."
                  className="w-full bg-transparent text-gray-800 placeholder-gray-400 rounded-2xl px-4 py-3 pr-12 resize-none focus:outline-none overflow-hidden text-sm"
                  rows={1}
                  style={{
                    height: '48px',
                    minHeight: '48px',
                    maxHeight: '120px'
                  }}
                />
              </div>
              <button
                onClick={handleSendMessage}
                disabled={inputText.trim() === '' || isTyping}
                className={`absolute right-2 bottom-2 p-2.5 rounded-xl transition-all duration-200 ${
                  inputText.trim() && !isTyping
                    ? 'bg-gradient-to-br from-blue-500 to-blue-600 hover:from-blue-600 hover:to-blue-700 text-white shadow-md hover:shadow-lg transform hover:scale-105'
                    : 'bg-gray-300 text-gray-500 cursor-not-allowed'
                }`}
              >
                {isTyping ? (
                  <Loader2 className="w-5 h-5 animate-spin" />
                ) : (
                  <Send className="w-5 h-5" />
                )}
              </button>
            </div>
          </div>
          {error && (
            <div className="mt-2 text-xs text-red-500 flex items-center gap-1">
              <span>⚠️</span>
              <span>{error}</span>
            </div>
          )}
        </div>
      </div>

      <style>{`
        @keyframes fade-in {
          from {
            opacity: 0;
            transform: translateY(10px);
          }
          to {
            opacity: 1;
            transform: translateY(0);
          }
        }
        .animate-fade-in {
          animation: fade-in 0.3s ease-out forwards;
        }
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

export default Chatbot;