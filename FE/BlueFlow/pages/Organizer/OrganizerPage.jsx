import Footer from '../../components/Footer';
import EventManagement from '../../components/OrganizerComponent/EventManagement';
import PageNav from '../../components/PageNav';
import styles from './Organizer.module.css';
import { useEffect, useState } from 'react';
import SegmentedControl from '../../components/OrganizerComponent/SegmentedControl';
import SideBar from '../../components/OrganizerComponent/SideBar';
import Dashboard from '../../components/OrganizerComponent/DashBoard';
import SegmentedControlItem from '../../components/OrganizerComponent/SegmentedControlItem';
import PartnersList from '../../components/OrganizerComponent/PartnersList';
import AccountSetting from '../../components/OrganizerComponent/AccountSetting';
import MessageContent from '../../components/OrganizerComponent/MessageContent';
import MessagesPage from '../../components/OrganizerComponent/MessagesPage';
import BrandProfile from '../../components/OrganizerComponent/BrandProfile';
import EventDetail from '../../components/OrganizerComponent/EventDetail';
import Chatbot from '../../components/OrganizerComponent/Chatbot';



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
    { key: 'dashboard', label: 'Dashboard', icon: Icon.dashboard },
    { key: 'discovery', label: 'Discovery', icon: Icon.search },
    { key: 'projects', label: 'My Projects', icon: Icon.folder },
    { key: 'messages', label: 'Messages', icon: Icon.message },
    { key: 'ai', label: 'AI Assistant', icon: Icon.ai },
    { key: 'profile', label: 'Profile & Settings', icon: Icon.users },
];

const partnersItem = [
    {
    location :"Da Nang",
    userId: "4602403a-1002-40e4-ade4-86c2c9200a76",
    logo:"imgs/SaiGon.png",
    forcus : "Green Tech, Education, Entertainment",
    title : "Music in the park: Summer Concert Series",
    tags : ["Sponsor", "Financial"],
    rating: 4.5
},
{
    location :"Da Nang",
    userId: "f31982d1-4846-4efd-ade9-2c2eede6d5c2",
    logo:"imgs/creative.jpg",
    forcus : "Green Tech, Education, Entertainment",
    title : "Club Creative",
    tags : ["Supplier", "Financial"],
    rating: 3.5
},
{
    location :"Da Nang",
    userId:"12345",
    logo:"imgs/lumire.jpg",
    forcus : "Green Tech, Education, Entertainment",
    title : "Lumire",
    tags : ["Sponsor", "Financial"],
     rating: 2.5
},
{
    location :"Da Nang",
    logo:"imgs/Rimberio.jpg",
    forcus : "Green Tech, Education, Entertainment",
    title : "Rimberio",
    tags : ["Supplier", "Financial"],
     rating: 5
},
{
    location :"Da Nang",
    logo:"imgs/Bliss.jpg",
    forcus : "Green Tech, Education, Entertainment",
    title : "BlissSprhere",
    tags : ["Sponsor", "Financial"],
     rating: 3
},
{
    location :"Da Nang",
    logo:"imgs/Momentum.jpg",
    forcus : "Green Tech, Education, Entertainment",
    title : "Momemtum",
    tags : ["Supplier", "Financial"],
     rating: 1
},
{
    location :"Da Nang",
    logo:"imgs/Veloria.jpg",
    forcus : "Green Tech, Education, Entertainment",
    title : "Veloria",
    tags : ["Sponsor", "Financial"],
     rating: 4.5
},
{
    location :"Da Nang",
    logo:"imgs/lumé.jpg",
    forcus : "Green Tech, Education, Entertainment",
    title : "Bela Lumiere",
    tags : ["Supplier", "Financial"],
     rating: 2.5
},
{
    location :"Da Nang",
     logo:"imgs/B.I.R.jpg",
    forcus : "Green Tech, Education, Entertainment",
    title : "B.I.R",
    tags : ["Sponsor", "Financial"],
     rating: 5
},


]





function OrganizerPage() {
    const [tab, setTab] = useState(() => localStorage.getItem('organizer.tab') || 'dashboard');
    const [active, setActive] = useState(() => localStorage.getItem('organizer.active') || 'dashboard');
    const [subChange, setSubChange] = useState(() => localStorage.getItem('organizer.discoverySub') || ''); // 'find' | 'saved'
    const [messagePartnerId, setMessagePartnerId] = useState(null);
    const [messagePartnerName, setMessagePartnerName] = useState(null);
    const [showEventDetail, setShowEventDetail] = useState(false);
    const [selectedEvent, setSelectedEvent] = useState(null);

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

    // Hàm để navigate đến Messages với partnerId
    const handleGoToMessages = (partnerId, partnerName) => {
        setMessagePartnerId(partnerId);
        setMessagePartnerName(partnerName);
        setActive('messages');
    };

    // Hàm để hiển thị Event Detail
    const handleViewEventDetail = (event, index) => {
        setSelectedEvent(event);
        setShowEventDetail(true);
        setTab('eventDetail');
    };

    // Hàm để quay lại Event Management
    const handleBackToEventList = () => {
        setShowEventDetail(false);
        setSelectedEvent(null);
        setTab('event');
    };

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
        if(subChange === 'find') {
            return <PartnersList
            partnersItem={partnersItem}
            onMessageClick={handleGoToMessages}
          />;
        }else if(subChange === 'saved') {
            return;
        }
        return ;
      case "projects":
        if(subChange === 'pending') {
            return;
        }else if(subChange === 'completed') {
            return;
        }
        return ;
      case "messages":
        return <MessagesPage 
          initialPartnerId={messagePartnerId}
          initialPartnerName={messagePartnerName}
        />;
      case "ai":
        return <Chatbot/>;
    case "profile":
         if(subChange === 'brand') {
            return <BrandProfile/>;
        }else if(subChange === 'account') {
            return <AccountSetting/>;
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
export default OrganizerPage;