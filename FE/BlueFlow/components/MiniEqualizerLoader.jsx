import React from 'react';

function MiniEqualizerLoader({ color = "#ffffff" }) {
  return (
    <div className="flex items-center justify-center space-x-0.5 h-4">
      <div 
        className="w-0.5 rounded-full animate-equalizer-1"
        style={{ 
          backgroundColor: color,
          height: '0.5rem'
        }}
      />
      <div 
        className="w-0.5 rounded-full animate-equalizer-2"
        style={{ 
          backgroundColor: color,
          height: '0.5rem'
        }}
      />
      <div 
        className="w-0.5 rounded-full animate-equalizer-3"
        style={{ 
          backgroundColor: color,
          height: '0.5rem'
        }}
      />
    </div>
  );
}

export default MiniEqualizerLoader;
