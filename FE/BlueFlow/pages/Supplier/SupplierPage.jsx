import Footer from '../../components/Footer';
import EventManagement from '../../components/SupplierComponent/EventManagement';
import PageNav from '../../components/PageNav';
import styles from './Supplier.module.css';
import { useEffect, useState } from 'react';
import { useLocation } from 'react-router-dom';
import SegmentedControl from '../../components/SupplierComponent/SegmentedControl';
import SideBar from '../../components/SupplierComponent/SideBar';
import Dashboard from '../../components/SupplierComponent/DashBoard';
import SegmentedControlItem from '../../components/SupplierComponent/SegmentedControlItem';
import PartnersList from '../../components/SupplierComponent/PartnersList';
import PartnersFilterBar from '../../components/SupplierComponent/PartnersFilterBar';
import AccountSetting from '../../components/SupplierComponent/AccountSetting';
import MessagesPage from '../../components/SupplierComponent/MessagesPage';
import BrandProfile from '../../components/SupplierComponent/BrandProfile';
import EventDetail from '../../components/SupplierComponent/EventDetail';
import Chatbot from '../../components/SupplierComponent/Chatbot';
import partnershipService from '../../services/partnershipService';
import toast from 'react-hot-toast';
import PaymentHistory from '../../components/PaymentHistory';



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

const partnersItem = [
    {
    location :"Da Nang",
    forcus : "Green Tech, Education, Entertainment",
    title : "Music in the park: Summer Concert Series",
    tags : ["Organizer", "Event"],
    rating: 4.5
},
{
    location :"Da Nang",
    forcus : "Green Tech, Education, Entertainment",
    title : "Club Creative",
    tags : ["Organizer", "Event"],
    rating: 3.5
},
{
    location :"Da Nang",
    forcus : "Green Tech, Education, Entertainment",
    title : "Lumire",
    tags : ["Organizer", "Event"],
     rating: 2.5
},
{
    location :"Da Nang",
    forcus : "Green Tech, Education, Entertainment",
    title : "Rimberio",
    tags : ["Organizer", "Event"],
     rating: 5
},
{
    location :"Da Nang",
    forcus : "Green Tech, Education, Entertainment",
    title : "BlissSprhere",
    tags : ["Organizer", "Event"],
     rating: 3
},
{
    location :"Da Nang",
    forcus : "Green Tech, Education, Entertainment",
    title : "Momemtum",
    tags : ["Organizer", "Event"],
     rating: 1
},
{
    location :"Da Nang",
    forcus : "Green Tech, Education, Entertainment",
    title : "Veloria",
    tags : ["Organizer", "Event"],
     rating: 4.5
},
{
    location :"Da Nang",
    forcus : "Green Tech, Education, Entertainment",
    title : "Bela Lumiere",
    tags : ["Organizer", "Event"],
     rating: 2.5
},
{
    location :"Da Nang",
    forcus : "Green Tech, Education, Entertainment",
    title : "B.I.R",
    tags : ["Organizer", "Event"],
     rating: 5
},


]





function SupplierPage() {
    const location = useLocation();
    const [tab, setTab] = useState(() => localStorage.getItem('supplier.tab') || 'dashboard');
    const [active, setActive] = useState(() => localStorage.getItem('supplier.active') || 'dashboard');
    const [subChange, setSubChange] = useState(() => localStorage.getItem('supplier.discoverySub') || ''); // 'find' | 'saved'
    const [partnersData, setPartnersData] = useState([]);
    const [filteredPartnersData, setFilteredPartnersData] = useState([]);
    const [loadingPartners, setLoadingPartners] = useState(false);

    // Check if redirected from payment pages
    useEffect(() => {
        if (location.state?.activeTab === 'projects') {
            setActive('projects');
            setTab('projects');
        }
    }, [location]);

    // Persist to localStorage whenever these change
    useEffect(() => {
        localStorage.setItem('supplier.tab', tab);
    }, [tab]);

    useEffect(() => {
        localStorage.setItem('supplier.active', active);
    }, [active]);

    useEffect(() => {
        localStorage.setItem('supplier.discoverySub', subChange);
    }, [subChange]);

    // Fetch partnerships when discovery tab is active
    useEffect(() => {
        const fetchPartnerships = async () => {
            if (active === 'discovery' && subChange === 'find') {
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
    }, [active, subChange]);



    const renderContent = () => {
    switch (active) {
      case "dashboard":
        if(tab === 'event') {
            return <EventManagement />;
        }
        return <EventDetail />;
      case "discovery":
        if(subChange === 'find') {
            return (
                <>
                    <PartnersFilterBar 
                        data={partnersData} 
                        onFilter={setFilteredPartnersData}
                    />
                    <PartnersList
                        partnersItem={filteredPartnersData.length > 0 ? filteredPartnersData : partnersData}
                        loading={loadingPartners}
                    />
                </>
            );
        }else if(subChange === 'saved') {
            return;
        }
        return ;
      case "projects":
        return <PaymentHistory />;
      case "messages":
        return <MessagesPage />;
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
        <div className={styles.supplierPage}>
            <PageNav />
                        <div className="max-w-6xl mx-auto px-4 py-6">
                
                {active === 'dashboard' ? (
                    <SegmentedControlItem tab={tab} setTab={setTab} />
                ) : (
                    <div className=' p-1 mb-[57px]'>
                    </div>
                )}

                <div className='flex space-x-10'>
                    <SideBar opts={items} activeItem={active} onChange={setActive} onSubChange={setSubChange} subChange={subChange}/>
                    <div className='flex-1'>
                        {renderContent()}

                    </div>
                </div>

            </div>
            <Footer/>
        </div>
    )
}
export default SupplierPage;
