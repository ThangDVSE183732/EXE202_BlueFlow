import { BookmarkIcon as BookmarkOutline, TagIcon, CurrencyDollarIcon, LinkIcon } from '@heroicons/react/24/outline';
import { BookmarkIcon as BookmarkSolid } from '@heroicons/react/24/solid';
import { StarIcon as StarSolid } from '@heroicons/react/24/solid';
import { useState } from 'react';
import { useAuth } from '../../contexts/AuthContext';

function PartnersItems({ partnersItem, onMessageClick }) {
    const { user } = useAuth();
    const {
        partnerId,
        partnerName,
        partnerType, // Thêm partnerType
        location,
        forcus,
        title,
        tags = [],
        rating = 0,
        summaryPoints = [
            'Provides financial support for tech and startup-related events in Vietnam.',
            'Sponsored over 50 events from 2019 to 2025.'
        ],
        eventHighlights = [],
        targetAudienceList = [],
        focusAreas = ['Tech', 'Green startups', 'Educational events'],
        averageSponsorship = '50M – 500M VND',
        pastEvents = ['GreenFest', 'StartUp Next', 'EduInnovate'],
        statuses = ['Cancelled', 'Chat now'],
        logo = 'imgs/SaiGon.png'
    } = partnersItem;

    const [bookmarked, setBookmarked] = useState(false);
    const [showDetail, setShowDetail] = useState(false);

    // Check if this is current user's own partnership
    const isOwnPartnership = user?.id === partnerId;

    const handleMessageClick = () => {
        // Prevent messaging yourself
        if (isOwnPartnership) {
            console.log('❌ Cannot message yourself');
            return;
        }

        console.log('🔵 Click Message button');
        console.log('partnerId:', partnerId);
        console.log('partnerName:', partnerName);
        console.log('onMessageClick:', onMessageClick);
        
        if (onMessageClick && partnerId) {
            console.log('✅ Calling onMessageClick');
            onMessageClick(partnerId, partnerName || title);
        } else {
            console.log('❌ Missing:', { hasCallback: !!onMessageClick, hasPartnerId: !!partnerId });
        }
    };

     // const formattedDate = (() => {
    //     try {
    //         const d = date instanceof Date ? date : new Date(date);
    //         return new Intl.DateTimeFormat("en-US", {
    //             month: "short",
    //             day: "numeric",
    //             year: "numeric",
    //         }).format(d);
    //     } catch {

    const IconPin = (
        <svg
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 24 24"
            fill="none"
            stroke="#38bdf8"
            strokeWidth="2"
            strokeLinecap="round"
            strokeLinejoin="round"
            className="w-4 h-4 flex-shrink-0"
        >
            <path d="M12 21s-6-5.3-6-10a6 6 0 1 1 12 0c0 4.7-6 10-6 10Z" />
            <circle cx="12" cy="11" r="2.5" />
        </svg>
    );

    const IconCalendar = (
        <svg
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 24 24"
            fill="none"
            stroke="#38bdf8"
            strokeWidth="2"
            strokeLinecap="round"
            strokeLinejoin="round"
            className="w-4 h-4 flex-shrink-0"
        >
            <rect x="3" y="5" width="18" height="16" rx="2" />
            <path d="M16 3v4M8 3v4M3 11h18" />
        </svg>
    );

    return (
        <div className="relative partner-card-root select-none">
            {/* Compact card */}
            <div className="w-58 h-83 shadow-lg rounded-xl bg-white p-4 flex flex-col transition ring-1 ring-transparent hover:ring-sky-300">
                <div 
                    className="relative"
                    onMouseEnter={() => setShowDetail(true)}
                    onMouseLeave={() => setShowDetail(false)}
                >
                    <img src={logo} alt={title} className="w-full h-32 object-cover rounded-md" />
                    <button
                        onClick={() => setBookmarked(b => !b)}
                        className="absolute top-0 right-0 p-1 rounded-md hover:bg-sky-300"
                    >
                        {bookmarked ? (
                            <BookmarkSolid className="w-5 h-5 text-yellow-400" />
                        ) : (
                            <BookmarkSolid className="w-5 h-5 text-white" />
                        )}
                    </button>
                </div>
                <div className="flex-1 mt-2">
                    <div className="flex flex-wrap items-center gap-1 mb-1">
                        {tags.slice(0, 3).map(t => (
                            <span
                                key={t}
                                className="px-1.5 py-0.5 rounded-full border border-sky-300 text-[9px] font-medium text-sky-500"
                            >
                                {t}
                            </span>
                        ))}
                        {tags.length > 2 && (
                            <span className="px-1.5 py-0.5 rounded-full border border-sky-300 text-[9px] font-medium text-sky-500">
                                +{tags.length - 2}
                            </span>
                        )}
                    </div>
                    <h6 className="text-sm font-semibold text-left text-gray-800 line-clamp-2 leading-snug mt-1">
                        {title}
                    </h6>
                    <div className="mt-1 space-y-1 text-[11px]">
                        <div className=" flex items-center gap-2 text-gray-800">
                            {IconPin}
                            <span>{location}</span>
                        </div>
                        <div className="flex items-center gap-2 text-gray-800 min-w-0">
                         <TagIcon className="w-4 h-4 text-sky-500 flex-shrink-0" aria-hidden="true" />

                            <span className="flex-1 truncate whitespace-nowrap overflow-hidden text-left">{`${forcus}`}</span>
                        </div>
                        {rating !== null && (
                            <div className="flex items-center gap-2 text-gray-800">
                                <StarSolid className="w-4 h-4 text-yellow-400" />
                                <span>{`${rating}/5`}</span>
                            </div>
                        )}
                    </div>
                </div>
                <div className="mt-1 flex space-x-1 text-[11px] font-medium">
                    <button className="flex-1 px-3 py-1.5 rounded-full bg-sky-500 hover:bg-sky-600 text-white transition">Send</button>
                    {!isOwnPartnership && (
                        <button 
                            onClick={handleMessageClick}
                            className="flex-1 px-3 py-1.5 rounded-full bg-sky-100 text-sky-600 hover:bg-sky-200 transition"
                        >
                            Message
                        </button>
                    )}
                </div>
            </div>

            {/* Click detail panel */}
            {showDetail && (
                <div 
                    className="absolute -top-14 right-20  ml-4 z-30 w-80 bg-white rounded-2xl shadow-xl border border-sky-200 p-4 animate-fadeIn" 
                    role="dialog" 
                    aria-label={title}
                    onMouseEnter={() => setShowDetail(true)}
                    onMouseLeave={() => setShowDetail(false)}
                >
                    <div className="flex gap-4">
                        <div className="w-20 h-20 rounded-full border-2 border-sky-300 flex items-center justify-center overflow-hidden bg-sky-50">
                            <img src={logo} alt={title} className="object-cover w-full h-full" />
                        </div>
                        <div className="flex-1 min-w-0">
                            <h3 className="font-semibold text-left text-gray-800 text-sm ">{title}</h3>
                            <div className="mt-1 flex flex-wrap items-center gap-1">
                                {partnerType === 'Sponsor' 
                                    ? (
                                        <>
                                            {tags.slice(0, 3).map(t => (
                                                <span key={t} className="px-2 py-0.5 rounded-full border border-sky-300 text-[10px] font-medium text-sky-600 bg-sky-50">{t}</span>
                                            ))}
                                            {tags.length > 3 && (
                                                <span className="px-2 py-0.5 rounded-full border border-sky-300 text-[10px] font-medium text-sky-600 bg-sky-50">
                                                    +{tags.length - 3}
                                                </span>
                                            )}
                                        </>
                                    )
                                    : tags.map(t => (
                                        <span key={t} className="px-2 py-0.5 rounded-full border border-sky-300 text-[10px] font-medium text-sky-600 bg-sky-50">{t}</span>
                                    ))
                                }
                            </div>
                            {rating !== null && (
                                <div className="mt-1 flex items-center gap-1 text-[11px] text-slate-600">
                                    <StarSolid className="w-4 h-4 text-yellow-400" />
                                    <span>{`${rating}/5`}</span>
                                </div>
                            )}
                        </div>
                    </div>
                    
                    <ul className="mt-3 space-y-1 text-[11px] text-slate-700 list-disc ml-5 text-left">
                        {partnerType === 'Sponsor' 
                            ? summaryPoints.slice(0, 2).map((p, i) => (
                                <li key={i} className="leading-snug">
                                    {p}
                                    {i === 1 && summaryPoints.length > 2 && <span className="text-slate-500"> ...</span>}
                                </li>
                            ))
                            : summaryPoints.map((p, i) => (
                                <li key={i} className="leading-snug">{p}</li>
                            ))
                        }
                    </ul>
                    
                    {eventHighlights && eventHighlights.length > 0 && (
                        <div className="mt-3">
                            <h4 className="text-sm font-semibold text-left text-slate-800 mb-2">What to Expect:</h4>
                            <ul className="space-y-1 text-[11px] text-slate-700 list-disc ml-5 text-left">
                                {eventHighlights.slice(0, 1).map((highlight, i) => (
                                    <li key={i} className="leading-snug">
                                        {highlight}
                                        {eventHighlights.length > 1 && <span className="text-slate-500"> ...</span>}
                                    </li>
                                ))}
                            </ul>
                        </div>
                    )}
                    
                    {targetAudienceList && targetAudienceList.length > 0 && (
                        <div className="mt-3">
                            <h4 className="text-sm font-semibold text-left text-slate-800 mb-2">Target Audience:</h4>
                            <ul className="space-y-1 text-[11px] text-slate-700 list-disc ml-5 text-left">
                                {targetAudienceList.slice(0, 1).map((audience, i) => (
                                    <li key={i} className="leading-snug">
                                        {audience}
                                        {targetAudienceList.length > 1 && <span className="text-slate-500"> ...</span>}
                                    </li>
                                ))}
                            </ul>
                        </div>
                    )}
                    
                    {partnerType !== 'Sponsor' && (
                        <div className="mt-3 space-y-2 text-[11px]">
                            <div className="flex items-center gap-2">
                                <CurrencyDollarIcon className="w-4 h-4 text-sky-500 flex-shrink-0" />
                                <span className="text-slate-700"><span className="text-sky-600 font-medium">{averageSponsorship}</span></span>
                            </div>
                        </div>
                    )}
                    {/* <div className="mt-4 flex gap-2">
                        {statuses.map(s => (
                            <button
                                key={s}
                                className={
                                    'px-3 py-1 rounded-full text-[11px] font-medium border transition ' +
                                    (s.toLowerCase().includes('chat')
                                        ? 'bg-sky-500 text-white border-sky-500 hover:bg-sky-600'
                                        : 'bg-slate-100 text-slate-600 border-slate-200 hover:bg-slate-200')
                                }
                            >{s}</button>
                        ))}
                    </div> */}
                </div>
            )}
        </div>
    );
}
export default PartnersItems;