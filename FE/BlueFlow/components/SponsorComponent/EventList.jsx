import { useMemo, useState, useEffect } from "react";
import toast from "react-hot-toast";
import { useEvent } from "../../hooks/useEvent";
import { useAuth } from "../../contexts/AuthContext";
import Loading from "../Loading";

// A reusable DotMenu with simple hover popover as in the screenshot
function ActionMenu({ onViewDetail, onDelete }) {
  const [open, setOpen] = useState(false);
  const [buttonRef, setButtonRef] = useState(null);
  
  // Calculate menu position based on button
  const getMenuPosition = () => {
    if (!buttonRef) return {};
    const rect = buttonRef.getBoundingClientRect();
    return {
      top: rect.bottom + 8, // 8px below button
      right: window.innerWidth - rect.right, // Align to right edge of button
    };
  };
  
  const menuPosition = open ? getMenuPosition() : {};
  
  return (
    <>
      <button
        ref={setButtonRef}
        onClick={() => setOpen((o) => !o)}
        className="px-2 py-2 rounded-full hover:bg-gray-100/60"
        aria-label="actions"
      >
        <div className="flex items-center space-x-1 text-purple-900">
          <span className="h-1.5 w-1.5 bg-purple-900 rounded-full" />
          <span className="h-1.5 w-1.5 bg-purple-900 rounded-full" />
          <span className="h-1.5 w-1.5 bg-purple-900 rounded-full" />
        </div>
      </button>
      {open && (
        <>
          {/* Backdrop to close menu when clicking outside */}
          <div 
            className="fixed inset-0 z-40" 
            onClick={() => setOpen(false)}
          />
          <div
            style={{
              position: 'fixed',
              top: `${menuPosition.top}px`,
              right: `${menuPosition.right}px`,
            }}
            className="w-28 bg-white shadow-lg rounded-md text-xs text-gray-700 py-1 z-50 border border-gray-200"
          >
            <button 
              className="block w-full text-left px-3 py-1 hover:bg-gray-50" 
              onClick={() => {
                onViewDetail();
                setOpen(false);
              }}
            >
              View Detail
            </button>
            <button 
              className="block w-full text-left px-3 py-1 hover:bg-gray-50" 
              onClick={() => {
                onDelete();
                setOpen(false);
              }}
            >
              Delete
            </button>
          </div>
        </>
      )}
    </>
  );
}

function StatusPill({ status }) {
  const config = useMemo(() => {
    switch (status) {
      case "Ongoing":
        return { dot: "bg-green-400", text: "text-green-500" };
      case "Pending":
        return { dot: "bg-yellow-400", text: "text-yellow-500" };
      case "Cancelled":
        return { dot: "bg-red-500", text: "text-red-500" };
      default:
        return { dot: "bg-blue-500", text: "text-blue-500" };
    }
  }, [status]);
  return (
    <div className={`inline-flex items-center space-x-2 ${config.text}`}>
      <span className={`h-2 w-2 rounded-full ${config.dot}`} />
      <span className="font-medium">{status}</span>
    </div>
  );
}

export default function EventList({ onViewDetail, onMessage, newEventId }) {
  const { getEventById, deleteEvent, loading } = useEvent();
  const { user } = useAuth();
  const [events, setEvents] = useState([]);
  const [isDeleting, setIsDeleting] = useState(false);

  const fetchEvents = async () => {
    // Get userId from logged in user
    const userId = user?.id || user?.userId;
    
    if (!userId) {
      console.error('No userId found in user context');
      setEvents([]); // Set empty array when no user
      return;
    }

    // Call API with userId to get all events of this user
    const result = await getEventById(userId);
    
    // Check if result is successful
    if (result && result.success) {
      // Check if data exists and is an array
      if (result.data && Array.isArray(result.data)) {
        // If data array is empty, show "There are no events yet. Please create one!"
        if (result.data.length === 0) {
          setEvents([]);
          return;
        }
        
        // Process events data - chỉ hiển thị data từ API, không tự tạo mới
        const fetchedEvents = result.data.map(eventData => ({
          id: eventData.id || eventData.eventId,
          event: eventData.title,
          status: "Ongoing", // Hardcoded as requested
          createdAt: eventData.eventDate ? new Date(eventData.eventDate).toLocaleDateString('en-GB', { 
            day: 'numeric', 
            month: 'long', 
            year: 'numeric' 
          }) : 'N/A',
          location: eventData.location || 'N/A',
          rawData: eventData // Keep full data for detail view
        }));
        
        // Remove duplicates by id using Map
        const uniqueEvents = Array.from(
          new Map(fetchedEvents.map(event => [event.id, event])).values()
        );
        
        setEvents(uniqueEvents);
      } else {
        // If no data or not array, set empty array
        setEvents([]);
      }
    } else {
      // If not successful, set empty array
      setEvents([]);
    }
  };

  useEffect(() => {
    fetchEvents();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [user, newEventId]); // Gọi lại API khi user đăng nhập hoặc có event mới được tạo

  // Handle delete event
  const handleDeleteEvent = async (eventId) => {
    if (!eventId) {
      console.error('No eventId provided for deletion');
      return;
    }

    // Confirm deletion
    if (!window.confirm('Bạn có chắc chắn muốn xóa event này?')) {
      return;
    }

    console.log('🗑️ Deleting event:', eventId);
    setIsDeleting(true);

    try {
      const result = await deleteEvent(eventId);

      if (result && result.success) {
        console.log('✅ Event deleted successfully');
        toast.success('Đã xóa event thành công');

        // Reload event list
        await fetchEvents();
      } else {
        console.error('❌ Delete failed:', result?.message);
        toast.error(result?.message || 'Không thể xóa event');
      }
    } catch (error) {
      console.error('❌ Delete error:', error);
      toast.error('Có lỗi xảy ra khi xóa event');
    } finally {
      setIsDeleting(false);
    }
  };

  if (loading || isDeleting) {
    return <Loading message={isDeleting ? 'Đang xóa event...' : 'Đang tải danh sách sự kiện...'} />;
  }

  return (
    <div className="bg-white rounded-2xl shadow-sm border border-gray-100">
      <div className="overflow-x-auto">
        <table className="w-full table-fixed text-sm">
        <thead className="bg-gray-50 text-gray-600">
          <tr>
            <th scope="col" className="px-4 py-3 text-center font-semibold w-[25%]">Event</th>
            <th scope="col" className="px-4 py-3 text-center font-semibold w-[13%]">Status</th>
            <th scope="col" className="px-4 py-3 text-center font-semibold w-[20%]">Create At</th>
            <th scope="col" className="px-4 py-3 text-center font-semibold w-[32%]">Location</th>
            <th scope="col" className="px-4 py-3 text-center font-semibold w-[10%]">Action</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-gray-200">
          {events.length === 0 ? (
            <tr>
              <td colSpan={5} className="px-4 py-8 text-center text-gray-500">
                There are no events yet. Please create one!
              </td>
            </tr>
          ) : (
            events.map((r, idx) => (
              <tr key={r.id || idx} className="hover:bg-gray-50">
                <td className="px-4 py-3 text-gray-800 font-medium truncate text-left" title={r.event}>{r.event}</td>
                <td className="px-4 py-3"><StatusPill status={r.status} /></td>
                <td className="px-4 py-3 text-gray-600 whitespace-nowrap">{r.createdAt}</td>
                <td className="px-4 py-3 text-gray-700 truncate" title={r.location}>{r.location}</td>
                <td className="px-4 py-3 relative overflow-visible">
                  <div className="flex justify-center">
                    <ActionMenu
                      onViewDetail={() => onViewDetail?.(r.id)}
                      onDelete={() => handleDeleteEvent(r.id)}
                    />
                  </div>
                </td>
              </tr>
            ))
          )}
        </tbody>
        </table>
      </div>
    </div>
  );
}
