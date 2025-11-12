import Footer from '../../components/Footer';
import EventManagement from '../../components/OrganizerComponent/EventManagement';
import PageNav from '../../components/PageNav';
import styles from './Organizer.module.css';
import { useEffect, useState } from 'react';
import { useLocation } from 'react-router-dom';
import SegmentedControl from '../../components/OrganizerComponent/SegmentedControl';
import SideBar from '../../components/OrganizerComponent/SideBar';
import Dashboard from '../../components/OrganizerComponent/DashBoard';
import SegmentedControlItem from '../../components/OrganizerComponent/SegmentedControlItem';
import PartnersList from '../../components/OrganizerComponent/PartnersList';
import PartnersFilterBar from '../../components/OrganizerComponent/PartnersFilterBar';
import AccountSetting from '../../components/OrganizerComponent/AccountSetting';
import MessageContent from '../../components/OrganizerComponent/MessageContent';
import MessagesPage from '../../components/OrganizerComponent/MessagesPage';
import BrandProfile from '../../components/OrganizerComponent/BrandProfile';
import EventDetail from '../../components/OrganizerComponent/EventDetail';
import Chatbot from '../../components/OrganizerComponent/Chatbot';
import partnershipService from '../../services/partnershipService';
import toast from 'react-hot-toast';
import PaymentHistory from '../../components/PaymentHistory';
import { messageService } from '../../services/messageService';
import signalRService from '../../services/signalRService';



const Icon = {
    dashboard: (
        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden>
            <path d="M3 13h8V3H3zM13 21h8v-8h-8z" />
            <path d="M13 3h8v6h-8zM3 15h8v6H3z" />
        </svg>
    ),
    manage: (
        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden>
            <path d="M12 2v4M12 18v4M4.93 4.93l2.83 2.83M16.24 16.24l2.83 2.83M2 12h4M18 12h4M4.93 19.07l2.83-2.83M16.24 7.76l2.83-2.83" />
        </svg>
    ),
    search: (
        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden>
            <circle cx="11" cy="11" r="8" />
            <path d="M21 21l-3.5-3.5" />
        </svg>
    ),
    folder: (
        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden>
            <path d="M3 7h5l2 3h11v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z" />
        </svg>
    ),
    message: (
        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden>
            <path d="M21 15a4 4 0 0 1-4 4H7l-4 4V7a4 4 0 0 1 4-4h10a4 4 0 0 1 4 4z" />
        </svg>
    ),
    ai: (
        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden>
            <rect x="8" y="8" width="8" height="8" rx="2" />
            <path d="M12 2v4M12 18v4M2 12h4M18 12h4" />
            <path d="M4.93 4.93l2.83 2.83M16.24 16.24l2.83 2.83M4.93 19.07l2.83-2.83M16.24 7.76l2.83-2.83" />
        </svg>
    ),
    users: (
        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden>
            <path d="M17 21v-2a4 4 0 0 0-4-4H7a4 4 0 0 0-4 4v2" />
            <circle cx="9" cy="7" r="4" />
            <path d="M23 21v-2a4 4 0 0 0-3-3.87" />
            <path d="M16 3.13a4 4 0 0 1 0 7.75" />
        </svg>
    ),
};

const items = [
    { key: 'dashboard', label: 'Bảng điều khiển', icon: Icon.dashboard },
    { key: 'discovery', label: 'Khám phá', icon: Icon.search },
    { key: 'projects', label: 'Lịch sử giao dịch', icon: Icon.folder },
    { key: 'messages', label: 'Tin nhắn', icon: Icon.message },
    { key: 'ai', label: 'Trợ lý AI', icon: Icon.ai },
    { key: 'profile', label: 'Hồ sơ & Cài đặt', icon: Icon.users },
];

function OrganizerPage() {
    const location = useLocation();
    const [tab, setTab] = useState(() => localStorage.getItem('organizer.tab') || 'dashboard');
    const [active, setActive] = useState(() => localStorage.getItem('organizer.active') || 'dashboard');
    const [subChange, setSubChange] = useState(() => localStorage.getItem('organizer.discoverySub') || ''); // 'find' | 'saved'
    const [partnersData, setPartnersData] = useState([]);
    const [filteredPartnersData, setFilteredPartnersData] = useState([]);
    const [loadingPartners, setLoadingPartners] = useState(false);
    const [selectedEvent, setSelectedEvent] = useState(null);
    const [selectedPartnerId, setSelectedPartnerId] = useState(null);
    const [selectedPartnerName, setSelectedPartnerName] = useState(null);
    const [unreadCount, setUnreadCount] = useState(0);

    // Handler functions
    const handleViewEventDetail = (event) => {
        setSelectedEvent(event);
        setTab('eventDetail');
    };

    const handleBackToEventList = () => {
        setSelectedEvent(null);
        setTab('event');
    };

    const handleGoToMessages = () => {
        setActive('messages');
    };

    const handleMessageClick = (partnerId, partnerName) => {
        console.log('📨 Message clicked for partner:', partnerId, partnerName);
        setSelectedPartnerId(partnerId);
        setSelectedPartnerName(partnerName);
        setActive('messages');
        setTab('messages');
    };

    // Check if redirected from payment pages
    useEffect(() => {
        if (location.state?.activeTab === 'projects') {
            setActive('projects');
            setTab('projects');
        }
    }, [location]);

    // 🔔 Global polling for unread messages - runs regardless of active tab
    useEffect(() => {
        console.log('🚀 Starting global unread count polling...');
        
        // Function to fetch and update unread count
        const updateUnreadCount = async () => {
            try {
                const response = await messageService.getPartnerListChat();
                if (response.success && response.data) {
                    const total = response.data.reduce((sum, chat) => sum + (chat.unreadCount || 0), 0);
                    console.log('📊 Global unread count:', total);
                    setUnreadCount(total);
                }
            } catch (error) {
                console.error('❌ Error fetching unread count:', error);
            }
        };

        // Initial fetch
        updateUnreadCount();

        // Always use polling as fallback (SignalR might miss some events)
        console.log('⏰ Starting polling for unread count (every 5 seconds)');
        const pollInterval = setInterval(updateUnreadCount, 5000);

        // Also try SignalR for real-time updates
        const initSignalR = async () => {
            try {
                if (!signalRService.isConnectionActive()) {
                    await signalRService.startConnection();
                }
                
                if (signalRService.isConnectionActive()) {
                    console.log('✅ SignalR connected, listening for conversation updates');
                    // Listen for conversation updates (immediate refresh)
                    signalRService.onConversationUpdated(() => {
                        console.log('🔔 Conversation updated via SignalR, refreshing unread count...');
                        updateUnreadCount();
                    });
                }
            } catch (err) {
                console.error('SignalR error (polling still active):', err);
            }
        };

        initSignalR();

        // Cleanup on unmount
        return () => {
            console.log('🛑 Cleanup: Stopping unread count polling');
            clearInterval(pollInterval);
            signalRService.off('conversationUpdated');
        };
    }, []);

    // Persist to localStorage whenever these change
    useEffect(() => {
        localStorage.setItem('organizer.tab', tab);
    }, [tab]);

    useEffect(() => {
        localStorage.setItem('organizer.active', active);
    }, [active]);

    useEffect(() => {
        localStorage.setItem('organizer.discoverySub', subChange);
    }, [subChange]);

    // Fetch partnerships when discovery tab is active
    useEffect(() => {
        const fetchPartnerships = async () => {
            if (active === 'discovery') {
                setLoadingPartners(true);
                try {
                    const response = await partnershipService.getAllPartnerships();
                    if (response.success && response.data) {
                        // Transform API data to match PartnersItems format
                        const transformedData = response.data.map(partnership => {
                            // Kiểm tra partnerType để lấy data từ đúng nguồn
                            const isFromSponsor = partnership.partnerType === 'Sponsor';
                            
                            if (isFromSponsor) {
                                // Lấy từ Partner's BrandProfile khi partnerType = "Sponsor"
                                const brandProfile = partnership.partner?.brandProfile;
                                return {
                                    id: partnership.id,
                                    partnerId: partnership.partnerId,
                                    partnerName: partnership.partner?.fullName || brandProfile?.brandName || 'Unknown Partner',
                                    partnerType: partnership.partnerType, // Thêm partnerType
                                    location: brandProfile?.location || 'N/A',
                                    forcus: brandProfile?.industry || 'N/A',
                                    title: brandProfile?.brandName || 'Untitled Brand',
                                    tags: partnership.preferredContactMethod 
                                        ? partnership.preferredContactMethod.split(',').map(t => t.trim())
                                        : [],
                                    rating: null, // null cho Sponsor
                                    logo: partnership.partnershipImage || brandProfile?.brandLogo || 'imgs/SaiGon.png',
                                    summaryPoints: (() => {
                                        const points = [];
                                        // Thêm serviceDescription nếu có
                                        if (partnership.serviceDescription) {
                                            points.push(partnership.serviceDescription);
                                        }
                                        // Thêm initialMessage (split bởi dấu ;) nếu có
                                        if (partnership.initialMessage) {
                                            const messages = partnership.initialMessage.split(';').map(s => s.trim()).filter(s => s);
                                            points.push(...messages);
                                        }
                                        return points.length > 0 ? points : ['No information available'];
                                    })(),
                                    eventHighlights: [],
                                    targetAudienceList: [],
                                    focusAreas: brandProfile?.industry ? [brandProfile.industry] : ['General'],
                                    averageSponsorship: partnership.proposedBudget 
                                        ? `${partnership.proposedBudget.toLocaleString()} VND`
                                        : 'N/A',
                                    pastEvents: [],
                                    statuses: [partnership.status || 'Pending', 'Chat now']
                                };
                            } else {
                                // Lấy từ Event data khi partnerType = "Organizer" (logic cũ)
                                return {
                                    id: partnership.id,
                                    partnerId: partnership.partnerId,
                                    partnerName: partnership.partner?.fullName || partnership.event?.title || 'Unknown Partner',
                                    partnerType: partnership.partnerType, // Thêm partnerType
                                    location: partnership.event?.location || 'N/A',
                                    forcus: partnership.event?.eventType || 'N/A',
                                    title: partnership.event?.title || 'Untitled Event',
                                    tags: partnership.event?.tags ? JSON.parse(partnership.event.tags) : [],
                                    rating: 4.8,
                                    logo: partnership.partnershipImage || 'imgs/SaiGon.png',
                                    summaryPoints: [
                                        partnership.serviceDescription || 'No description available'
                                    ],
                                    eventHighlights: partnership.event?.eventHighlights ? JSON.parse(partnership.event.eventHighlights) : [],
                                    targetAudienceList: partnership.event?.targetAudienceList ? JSON.parse(partnership.event.targetAudienceList) : [],
                                    focusAreas: partnership.event?.category ? [partnership.event.category] : ['General'],
                                    averageSponsorship: partnership.proposedBudget 
                                        ? `${partnership.proposedBudget.toLocaleString()} VND`
                                        : 'N/A',
                                    pastEvents: [partnership.event?.title || 'N/A'],
                                    statuses: [partnership.status || 'Pending', 'Chat now']
                                };
                            }
                        });
                        console.log('✨ Transformed data:', transformedData);
                        setPartnersData(transformedData);
                    }
                } catch (error) {
                    console.error('Error fetching partnerships:', error);
                    toast.error('Không thể tải danh sách partnerships');
                } finally {
                    setLoadingPartners(false);
                }
            }
        };

        fetchPartnerships();
    }, [active]);



    const renderContent = () => {
    switch (active) {
      case "dashboard":
        if(tab === 'event') {
            return <EventManagement 
              onViewDetail={handleViewEventDetail}
              onMessage={handleGoToMessages}
            />;
        }
        if(tab === 'eventDetail') {
            return <EventDetail 
              event={selectedEvent}
              onBack={handleBackToEventList}
            />;
        }
        return <Dashboard />;
      case "discovery":
        if (loadingPartners) {
            return <div className="flex justify-center items-center h-64">Đang tải...</div>;
        }
        return (
            <>
                <PartnersFilterBar 
                    data={partnersData} 
                    onFilter={setFilteredPartnersData}
                />
                <PartnersList 
                    partnersItem={filteredPartnersData}
                    onMessageClick={handleMessageClick}
                />
            </>
        );
      case "projects":
        return <PaymentHistory />;
      case "messages":
        return <MessagesPage 
            initialPartnerId={selectedPartnerId}
            initialPartnerName={selectedPartnerName}
        />;
      case "ai":
        return <Chatbot />;
    case "profile":
         if(subChange === 'brand') {
            return <BrandProfile />;
        }else if(subChange === 'account') {
            return <AccountSetting />;
        }else if(subChange === 'marketing') {
            return;
        }
        return ;
      default:
        return ;
    }
  };
    
    return (
        <div className={styles.organizerPage}>
            <PageNav />
                        <div className="max-w-6xl mx-auto px-4 py-6">
                
                {active === 'dashboard' ? (
                    <SegmentedControlItem tab={tab} setTab={setTab} />
                ) : (
                    <div className=' p-1 mb-[57px]'>
                    </div>
                )}

                <div className='flex space-x-10'>
                    <SideBar opts={items} activeItem={active} onChange={setActive} onSubChange={setSubChange} subChange={subChange} unreadCount={unreadCount}/>
                    <div className='flex-1'>
                        {renderContent()}

                    </div>
                </div>

            </div>
            <Footer/>
        </div>
    )
}
export default OrganizerPage;