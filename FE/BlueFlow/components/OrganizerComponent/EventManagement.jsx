import { useState } from "react";
import toast from "react-hot-toast";
import SearchBar from "../SearchBar";
import EventList from "./EventList";
import EventCreate from "./EventCreate";
import EventDetail from "./EventDetail";
import { useEvent } from "../../hooks/useEvent";
import Loading from "../Loading";

function EventManagement() {
    const [showCreateForm, setShowCreateForm] = useState(false);
    const [selectedEventId, setSelectedEventId] = useState(null);
    const [newEventId, setNewEventId] = useState(null);
    const [isVerifying, setIsVerifying] = useState(false);
    const { getEventById } = useEvent();

    const handleBackFromCreate = async (eventId) => {
        if (eventId) {
            setIsVerifying(true);
            try {
                // Call API to get the newly created event to ensure data is synced
                const result = await getEventById(eventId);
                
                if (result.success) {
                    // Only navigate back and update list after successfully fetching the event
                    setNewEventId(eventId);
                    setShowCreateForm(false);
                } else {
                    // If fetch fails, still navigate back but show warning
                    toast('Event đã tạo nhưng không thể tải lại dữ liệu', { icon: '⚠️' });
                    setShowCreateForm(false);
                }
            } finally {
                setIsVerifying(false);
            }
        } else {
            // No eventId means user cancelled
            setShowCreateForm(false);
        }
    };

    const handleViewDetail = (eventId) => {
        setSelectedEventId(eventId);
    };

    const handleBackFromDetail = () => {
        setSelectedEventId(null);
    };

    if (showCreateForm) {
        return <EventCreate onBack={handleBackFromCreate} />;
    }

    if (selectedEventId) {
        return <EventDetail eventId={selectedEventId} onBack={handleBackFromDetail} />;
    }

    if (isVerifying) {
        return <Loading message="Đang xác thực dữ liệu..." />;
    }

    return(
        <div>
            <div className="space-y-1 text-left">
            <h1 className="text-2xl font-semibold text-sky-500">Workspace</h1>
            <h className="text-sm text-gray-400">Real-time updates on sponsorships, events, and audience reach.</h>
            </div>
            <div className="h-px w-full bg-gray-300 mx-1 mb-3 mt-6" />
            <div className="w-full h-60 rounded-lg relative" style={{ backgroundImage: "url('/imgs/managerEvent.png')" }}>
            <div className="absolute inset-0 bg-black opacity-50 rounded-lg"></div>
            <div className="relative z-10 text-white space-y-4 text-left pl-25 pt-15">
                <h1 className="text-4xl font-semibold">Manager your events</h1>
                <h3 className="text-gray-300 font-medium">SIMPLE - FAST - EASY</h3>
            </div>
            </div>
            <div className="flex justify-between mt-6">
                <SearchBar sizeClass="w-65" button={"left-57"} input={"w-full rounded-xl py-0.5"}/>
                <button 
                    onClick={() => setShowCreateForm(true)}
                    className="text-white bg-sky-500 hover:bg-sky-600 px-8 rounded-full py-1 text-base font-semibold transition-colors"
                >
                    Add new event
                </button>
            </div>
            <div className="mt-10 mb-15">
                <EventList 
                    newEventId={newEventId}
                    onViewDetail={handleViewDetail}
                />
            </div>
        </div>
    )
}
export default EventManagement;