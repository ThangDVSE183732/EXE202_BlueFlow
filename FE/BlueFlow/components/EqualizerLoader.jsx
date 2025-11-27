import React from 'react';

function EqualizerLoader({ color = "#00A6F4", message = "Đang tải..." }) {
  return (
    <div className="flex flex-col justify-center items-center h-96">
      <div className="flex items-center justify-center space-x-1.5 h-12">
        <div 
          className="w-1.5 rounded-full animate-equalizer-1"
          style={{ 
            backgroundColor: color,
            animationDelay: '0s'
          }}
        />
        <div 
          className="w-1.5 rounded-full animate-equalizer-2"
          style={{ 
            backgroundColor: color,
            animationDelay: '0.1s'
          }}
        />
        <div 
          className="w-1.5 rounded-full animate-equalizer-3"
          style={{ 
            backgroundColor: color,
            animationDelay: '0.2s'
          }}
        />
        <div 
          className="w-1.5 rounded-full animate-equalizer-4"
          style={{ 
            backgroundColor: color,
            animationDelay: '0.3s'
          }}
        />
        <div 
          className="w-1.5 rounded-full animate-equalizer-5"
          style={{ 
            backgroundColor: color,
            animationDelay: '0.4s'
          }}
        />
      </div>
      {message && (
        <p className="mt-6 text-gray-600 text-lg">{message}</p>
      )}
    </div>
  );
}

export default EqualizerLoader;
