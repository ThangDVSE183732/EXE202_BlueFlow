import Footer from '../../components/Footer';
import EventManagement from '../../components/SponsorComponent/EventManagement';
import PageNav from '../../components/PageNav';
import styles from './Sponsor.module.css';
import { useEffect, useState } from 'react';
import SegmentedControl from '../../components/SponsorComponent/SegmentedControl';
import SideBar from '../../components/SponsorComponent/SideBar';
import Dashboard from '../../components/SponsorComponent/DashBoard';
import SegmentedControlItem from '../../components/SponsorComponent/SegmentedControlItem';
import PartnersList from '../../components/SponsorComponent/PartnersList';
import AccountSetting from '../../components/SponsorComponent/AccountSetting';
import MessageContent from '../../components/SponsorComponent/MessageContent';
import MessagesPage from '../../components/SponsorComponent/MessagesPage';
import BrandProfile from '../../components/SponsorComponent/BrandProfile';
import EventDetail from '../../components/SponsorComponent/EventDetail';



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
    { key: 'profile', label: 'Profile & Settings', icon: Icon.users },
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





function SponsorPage() {
    const [tab, setTab] = useState(() => localStorage.getItem('sponsor.tab') || 'dashboard');
    const [active, setActive] = useState(() => localStorage.getItem('sponsor.active') || 'dashboard');
    const [subChange, setSubChange] = useState(() => localStorage.getItem('sponsor.discoverySub') || ''); // 'find' | 'saved'
  

    // Persist to localStorage whenever these change
    useEffect(() => {
        localStorage.setItem('sponsor.tab', tab);
    }, [tab]);

    useEffect(() => {
        localStorage.setItem('sponsor.active', active);
    }, [active]);

    useEffect(() => {
        localStorage.setItem('sponsor.discoverySub', subChange);
    }, [subChange]);



    const renderContent = () => {
    switch (active) {
      case "dashboard":
        if(tab === 'event') {
            return <EventManagement />;
        }
        return <EventDetail />;
      case "discovery":
        if(subChange === 'find') {
            return <PartnersList
            partnersItem={partnersItem}
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
        return <MessagesPage/>;
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
        <div className={styles.sponsorPage}>
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
export default SponsorPage;
