import React from 'react';


export default function SegmentedControl({ options = [], value, onChange, className = '' }) {
  return (
    <div
      className={[
        'inline-flex items-center rounded-full border border-gray-200 bg-white p-1 shadow-sm',
        className,
      ].join(' ')}
      role="tablist"
      aria-label="Sections"
    >
      {options.map((opt) => {
        const selected = value === opt.value;
        return (
          <button
            key={opt.value}
            type="button"
            role="tab"
            aria-selected={selected}
            onClick={() => onChange && onChange(opt.value)}
            className={[
              'relative rounded-full px-4 py-1.5 text-sm transition-colors focus:outline-none',
              'focus-visible:ring-2 focus-visible:ring-black',
              selected
                ? 'bg-black text-white font-semibold shadow'
                : 'text-gray-600 hover:text-black',
            ].join(' ')}
          >
            {opt.label}
          </button>
        );
      })}
    </div>
  );
}
