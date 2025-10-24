import { useState, useEffect, useRef, useMemo } from 'react';
import { FunnelIcon, MagnifyingGlassIcon } from '@heroicons/react/24/outline';

function useOutside(ref, onClose, enabled) {
  useEffect(() => {
    if (!enabled) return;
    function handler(e) {
      if (ref.current && !ref.current.contains(e.target)) onClose();
    }
    window.addEventListener('mousedown', handler);
    return () => window.removeEventListener('mousedown', handler);
  }, [enabled, onClose]);
}

const PillButton = ({ active, label, onClick }) => (
  <button
    onClick={onClick}
    className={`relative px-4 h-10 rounded-xl border text-sm font-medium flex items-center gap-2
      ${active ? 'border-sky-500 bg-sky-50 text-sky-600' : 'border-sky-300 hover:border-sky-400 text-gray-700 bg-white'}
      transition`}
  >
    {label}
    <svg className={`w-3 h-3 transition ${active ? 'rotate-180 text-sky-500' : 'text-gray-500'}`} viewBox="0 0 20 20" fill="currentColor">
      <path d="M5.23 7.21a.75.75 0 011.06.02L10 10.17l3.71-2.94a.75.75 0 111.04 1.08l-4.23 3.35a.75.75 0 01-.94 0L5.21 8.29a.75.75 0 01.02-1.08z"/>
    </svg>
  </button>
);

const PopoverCard = ({ innerRef, children }) => (
  <div
    ref={innerRef}
    className="absolute top-full mt-2 z-40 w-56 rounded-xl border border-sky-200 bg-white shadow-lg p-3 animate-fadeIn"
  >
    {children}
  </div>
);

// Rating stars
const Stars = ({ value }) => {
  const full = Math.round(value);
  return (
    <div className="flex items-center gap-0.5">
      {Array.from({ length: 5 }).map((_, i) => (
        <svg key={i} className={`w-4 h-4 ${i < full ? 'text-yellow-400' : 'text-gray-300'}`} viewBox="0 0 20 20" fill="currentColor">
          <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.1 3.386a1 1 0 00.95.69h3.562c.969 0 1.371 1.24.588 1.81l-2.882 2.094a1 1 0 00-.364 1.118l1.1 3.386c.3.922-.755 1.688-1.54 1.118L10.95 14.95a1 1 0 00-1.175 0L6.535 16.53c-.785.57-1.84-.196-1.54-1.118l1.1-3.386a1 1 0 00-.364-1.118L2.85 8.813c-.783-.57-.38-1.81.588-1.81H7c.438 0 .824-.282.95-.69l1.1-3.386z"/>
        </svg>
      ))}
    </div>
  );
};

export default function PartnerFilters({ data, onFilter }) {
  const [open, setOpen] = useState(null);

  const [supplierType, setSupplierType] = useState('All');                // Supplier | Sponsor | All
  const [service, setService] = useState('All');                          // Equipment | Decoration | Media | Financial sponsor | All
//   const [priceRange, setPriceRange] = useState([50, 5000]);               // min max
  const [regionQuery, setRegionQuery] = useState('');
  const [region, setRegion] = useState('All');
  const [minRating, setMinRating] = useState(0);

  const rootRefs = {
    supplier: useRef(null),
    service: useRef(null),
    // price: useRef(null),
    region: useRef(null),
    rating: useRef(null),
  };

  useOutside(rootRefs.supplier, () => setOpen(o => o === 'supplier' ? null : o), open === 'supplier');
  useOutside(rootRefs.service, () => setOpen(o => o === 'service' ? null : o), open === 'service');
//   useOutside(rootRefs.price, () => setOpen(o => o === 'price' ? null : o), open === 'price');
  useOutside(rootRefs.region, () => setOpen(o => o === 'region' ? null : o), open === 'region');
  useOutside(rootRefs.rating, () => setOpen(o => o === 'rating' ? null : o), open === 'rating');

  // Regions & services derived from data
  const allRegions = useMemo(() => {
    const set = new Set();
    data.forEach(d => d.region && set.add(d.region));
    return Array.from(set).sort();
  }, [data]);

  const filteredRegions = regionQuery
    ? allRegions.filter(r => r.toLowerCase().includes(regionQuery.toLowerCase()))
    : allRegions;

  const lastResultRef = useRef([]);

  // Apply filters - BỎ onFilter khỏi dependency
  useEffect(() => {
    const res = data.filter(item => {
      if (supplierType !== 'All' && item.type !== supplierType) return false;
      if (service !== 'All' && item.service !== service) return false;
      if (region !== 'All' && item.region !== region) return false;
      if (minRating > 0 && (item.rating || 0) < minRating) return false;
      return true;
    });

    // Chỉ gọi onFilter nếu kết quả thực sự thay đổi
    const prev = lastResultRef.current;
    if (prev.length !== res.length || !prev.every((v, i) => v === res[i])) {
      lastResultRef.current = res;
      onFilter(res);
    }
  }, [data, supplierType, service, region, minRating]); // BỎ onFilter

  return (
    <div className="flex  flex-wrap items-start gap-3 mb-6">
      <div className="flex items-center pt-2 pl-2 gap-1 pr-3">
        <FunnelIcon className="w-5 h-5 text-sky-500" />
        <span className="text-sm font-medium text-sky-600">Filter</span>
      </div>

      {/* Supplier */}
      <div className="relative" ref={rootRefs.supplier}>
        <PillButton
          active={open === 'supplier' || supplierType !== 'All'}
          label={supplierType === 'All' ? 'All' : supplierType}
          onClick={() => setOpen(o => o === 'supplier' ? null : 'supplier')}
        />
        {open === 'supplier' && (
          <PopoverCard innerRef={rootRefs.supplier}>
            <ul className="space-y-1 text-sm">
              {['Supplier', 'Sponsor', 'All'].map(opt => (
                <li key={opt}>
                  <button
                    onClick={() => { setSupplierType(opt); setOpen(null); }}
                    className={`w-full text-left px-2 py-1 rounded-md hover:bg-sky-50 ${supplierType === opt ? 'bg-sky-100 text-sky-600' : 'text-gray-700'}`}
                  >{opt}</button>
                </li>
              ))}
            </ul>
          </PopoverCard>
        )}
      </div>

      {/* Field of service */}
      <div className="relative" ref={rootRefs.service}>
        <PillButton
          active={open === 'service' || service !== 'All'}
          label={service === 'All' ? 'Field of service' : service}
          onClick={() => setOpen(o => o === 'service' ? null : 'service')}
        />
        {open === 'service' && (
          <PopoverCard innerRef={rootRefs.service}>
            <ul className="space-y-1 text-sm max-h-52 overflow-auto pr-1">
              {['Equipment', 'Decoration', 'Media', 'Financial sponsor', 'All'].map(opt => (
                <li key={opt}>
                  <button
                    onClick={() => { setService(opt); setOpen(null); }}
                    className={`w-full text-left px-2 py-1 rounded-md hover:bg-sky-50 ${service === opt ? 'bg-sky-100 text-sky-600' : 'text-gray-700'}`}
                  >{opt}</button>
                </li>
              ))}
            </ul>
          </PopoverCard>
        )}
      </div>

      {/* Price range */}
      {/* <div className="relative" ref={rootRefs.price}>
        <PillButton
          active={open === 'price' || priceRange[0] !== 50 || priceRange[1] !== 5000}
          label="Price range"
          onClick={() => setOpen(o => o === 'price' ? null : 'price')}
        />
        {open === 'price' && (
          <PopoverCard innerRef={rootRefs.price}>
            <div className="space-y-3">
              <div className="flex justify-between text-[11px] text-slate-500">
                <span>VND</span><span>min</span><span>max</span>
              </div>
              <input
                type="range"
                min={50}
                max={5000}
                step={10}
                value={priceRange[1]}
                onChange={e => setPriceRange(r => [r[0], Number(e.target.value)])}
                className="w-full accent-sky-500"
              />
              <div className="flex items-center gap-2">
                <input
                  type="number"
                  className="w-20 px-2 py-1 border rounded-md text-sm"
                  value={priceRange[0]}
                  min={0}
                  onChange={e => setPriceRange(r => [Number(e.target.value), r[1]])}
                />
                <span className="text-xs text-slate-500">to</span>
                <input
                  type="number"
                  className="w-24 px-2 py-1 border rounded-md text-sm"
                  value={priceRange[1]}
                  onChange={e => setPriceRange(r => [r[0], Number(e.target.value)])}
                />
              </div>
              <button
                onClick={() => { setPriceRange([50, 5000]); setOpen(null); }}
                className="text-xs text-sky-600 hover:underline"
              >Reset</button>
            </div>
          </PopoverCard>
        )}
      </div> */}

      {/* Region */}
      <div className="relative" ref={rootRefs.region}>
        <PillButton
          active={open === 'region' || region !== 'All'}
          label={region === 'All' ? 'Region' : region}
          onClick={() => setOpen(o => o === 'region' ? null : 'region')}
        />
        {open === 'region' && (
          <PopoverCard innerRef={rootRefs.region}>
            <div className="space-y-2">
              <div className="flex items-center gap-1 px-2 py-1 border rounded-md">
                <MagnifyingGlassIcon className="w-4 h-4 text-slate-400" />
                <input
                  value={regionQuery}
                  onChange={e => setRegionQuery(e.target.value)}
                  placeholder="Search region"
                  className="flex-1 outline-none text-sm"
                />
              </div>
              <ul className="max-h-40 overflow-auto text-sm space-y-1 pr-1">
                {['All', ...filteredRegions].map(r => (
                  <li key={r}>
                    <button
                      onClick={() => { setRegion(r); setOpen(null); setRegionQuery(''); }}
                      className={`w-full text-left px-2 py-1 rounded-md hover:bg-sky-50 ${region === r ? 'bg-sky-100 text-sky-600' : 'text-gray-700'}`}
                    >{r}</button>
                  </li>
                ))}
                {filteredRegions.length === 0 && <li className="text-xs text-slate-400 px-2 py-1">No match</li>}
              </ul>
            </div>
          </PopoverCard>
        )}
      </div>

      {/* Rating */}
      <div className="relative" ref={rootRefs.rating}>
        <PillButton
          active={open === 'rating' || minRating > 0}
          label={minRating > 0 ? `Rating ≥ ${minRating}` : 'Rating'}
          onClick={() => setOpen(o => o === 'rating' ? null : 'rating')}
        />
        {open === 'rating' && (
          <PopoverCard innerRef={rootRefs.rating}>
            <div className="space-y-3">
              <div className="flex items-center gap-2">
                <Stars value={minRating} />
                <select
                  value={minRating}
                  onChange={e => setMinRating(Number(e.target.value))}
                  className="border rounded-md px-2 py-1 text-sm"
                >
                  {[0,1,2,3,4,5].map(v => <option key={v} value={v}>{v === 0 ? 'Any' : `≥ ${v}`}</option>)}
                </select>
              </div>
              <button
                onClick={() => { setMinRating(0); setOpen(null); }}
                className="text-xs text-sky-600 hover:underline"
              >Reset</button>
            </div>
          </PopoverCard>
        )}
      </div>
    </div>
  );
}