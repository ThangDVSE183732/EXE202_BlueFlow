import React, { useState, useEffect } from 'react';

const EventTimeline = ({ 
  initialRows = null, 
  isCreateMode = false, 
  onChange = null, 
  initialTimeSlots = null,
  eventId = null,
  onDone = null
}) => {
  const [isEditMode, setIsEditMode] = useState(isCreateMode);
  const [editingEventIndex, setEditingEventIndex] = useState(null);
  const [showEventModal, setShowEventModal] = useState(false);
  const [currentRowId, setCurrentRowId] = useState(null);
  const [newEventData, setNewEventData] = useState({
    startTime: '8:30 AM',
    endTime: '9:00 AM',
    description: ''
  });
  
  // Default data for view mode
  const defaultRows = [
    {
      id: 1,
      title: 'Registration & Welcome Coffee',
      events: [
        { id: 'e1', time: '8:30 AM', description: 'Check-in', duration: 1 }
      ]
    },
    {
      id: 2,
      title: 'Opening Keynote',
      events: [
        { id: 'e2', time: '9:30 AM', description: 'Dr. Smith', duration: 1 }
      ]
    },
    {
      id: 3,
      title: 'Panel Discussion',
      events: [
        { id: 'e3', time: '10:30 AM', description: 'Industry experts', duration: 1 }
      ]
    },
    {
      id: 4,
      title: 'Coffee Break',
      events: [
        { id: 'e4', time: '11:30 AM', description: 'Networking', duration: 0.5 }
      ]
    },
    {
      id: 5,
      title: 'Closing Session',
      events: [
        { id: 'e5', time: '12:00 PM', description: 'Wrap up', duration: 0.5 }
      ]
    }
  ];
  
  // Use initialRows if provided (for create mode), otherwise use defaultRows
  const [rows, setRows] = useState(initialRows || (isCreateMode ? [] : defaultRows));

  // Update parent component when rows change (for create mode)
  useEffect(() => {
    if (onChange && isCreateMode) {
      onChange(rows);
    }
  }, [rows, onChange, isCreateMode]);

  // Use initialTimeSlots if provided, otherwise use default
  const defaultTimeSlots = [
    '8:30\nAM',
    '9:00\nAM',
    '9:30\nAM',
    '10:00\nAM',
    '10:30\nAM',
    '11:00\nAM',
    '11:30\nAM',
    '12:00\nPM'
  ];
  
  const [timeSlots, setTimeSlots] = useState(initialTimeSlots || defaultTimeSlots);

  // Dynamically generate time slot positions from timeSlots
  // Convert "8:30\nAM" format to "8:30 AM" and map to index
  const timeSlotPositions = React.useMemo(() => {
    const positions = {};
    timeSlots.forEach((slot, index) => {
      // Convert "8:30\nAM" to "8:30 AM"
      const formattedSlot = slot.replace('\n', ' ');
      positions[formattedSlot] = index;
    });
    console.log('🕒 Dynamic timeSlotPositions:', positions);
    return positions;
  }, [timeSlots]);

  // Get the column index for a given time
  const getTimeSlotIndex = (timeStr) => {
    const index = timeSlotPositions[timeStr];
    if (index === undefined) {
      console.warn(`⚠️ Time "${timeStr}" not found in timeSlotPositions. Available:`, Object.keys(timeSlotPositions));
    }
    return index !== undefined ? index : 0;
  };

  // Calculate position in pixels (fixed width per slot)
  const getPosition = (time) => {
    const index = getTimeSlotIndex(time);
    const slotWidth = 80; // Fixed width in pixels for each time slot
    return index * slotWidth;
  };

  // Calculate width in pixels based on duration
  const getWidth = (startTime, durationHours) => {
    const slotWidth = 80; // Fixed width in pixels
    // Approximate: each hour = 1 slot worth of width
    const columnsSpan = durationHours;
    return columnsSpan * slotWidth;
  };

  const handleEditToggle = async () => {
    if (!isCreateMode) {
      // If switching from edit mode to view mode (clicking Done)
      if (isEditMode && eventId && onDone) {
        console.log('🔵 Timeline Done clicked - calling onDone callback');
        await onDone(rows);
      }
      
      setIsEditMode(!isEditMode);
      setEditingEventIndex(null);
    }
  };

  // Add new row (title)
  const handleAddRow = () => {
    const newRow = {
      id: Date.now(),
      title: 'New Event Title',
      events: []
    };
    setRows([...rows, newRow]);
  };

  // Open modal to add new event
  const handleAddEvent = (rowId) => {
    setCurrentRowId(rowId);
    setNewEventData({
      startTime: '8:30 AM',
      endTime: '9:00 AM',
      description: 'New Event'
    });
    setShowEventModal(true);
  };

  // Calculate duration from start and end time
  const calculateDuration = (startTime, endTime) => {
    const startIndex = getTimeSlotIndex(startTime);
    const endIndex = getTimeSlotIndex(endTime);
    return Math.max(0.5, endIndex - startIndex);
  };

  // Save new event from modal
  const handleSaveEvent = () => {
    const duration = calculateDuration(newEventData.startTime, newEventData.endTime);
    const newEvent = {
      id: `e${Date.now()}`,
      time: newEventData.startTime,
      description: newEventData.description,
      duration: duration
    };
    setRows(rows.map(row => 
      row.id === currentRowId ? { ...row, events: [...row.events, newEvent] } : row
    ));
    setShowEventModal(false);
  };

  // Cancel modal
  const handleCancelEvent = () => {
    setShowEventModal(false);
  };

  // Delete event
  const handleDeleteEvent = (rowId, eventId) => {
    setRows(rows.map(row => 
      row.id === rowId ? { ...row, events: row.events.filter(e => e.id !== eventId) } : row
    ));
  };

  // Delete row
  const handleDeleteRow = (rowId) => {
    setRows(rows.filter(row => row.id !== rowId));
  };

  // Edit row title
  const handleEditRowTitle = (rowId, newTitle) => {
    setRows(rows.map(row => 
      row.id === rowId ? { ...row, title: newTitle } : row
    ));
  };

  // Edit event
  const handleEditEvent = (rowId, eventId, field, value) => {
    setRows(rows.map(row => 
      row.id === rowId ? {
        ...row,
        events: row.events.map(event =>
          event.id === eventId ? { ...event, [field]: value } : event
        )
      } : row
    ));
  };

  // Add new timeslot
  const handleAddTimeSlot = () => {
    const newSlot = '22:00\nPM';
    setTimeSlots([...timeSlots, newSlot]);
  };

  // Delete timeslot
  const handleDeleteTimeSlot = (index) => {
    const newTimeSlots = timeSlots.filter((_, i) => i !== index);
    setTimeSlots(newTimeSlots);
  };

  return (
    <div className="bg-white rounded-lg shadow-sm p-3 w-full">
      {/* Header with title and edit button */}
      <div className="flex justify-between items-center mb-3">
        <h2 className="text-base font-bold text-gray-900 text-left"></h2>
        <div className="flex gap-2">
          {isEditMode && (
            <>
              <button
                onClick={handleAddRow}
                className="flex items-center gap-1 px-2.5 py-1.5 text-xs font-medium text-white bg-green-600 hover:bg-green-700 rounded-md transition-colors duration-200"
                title="Add Row (Title)"
              >
                <svg className="h-3.5 w-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
                </svg>
                Row
              </button>
              <button
                onClick={handleAddTimeSlot}
                className="flex items-center gap-1 px-2.5 py-1.5 text-xs font-medium text-white bg-purple-600 hover:bg-purple-700 rounded-md transition-colors duration-200"
                title="Add Time Slot (Column)"
              >
                <svg className="h-3.5 w-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
                </svg>
                Time
              </button>
            </>
          )}
          {!isCreateMode && (
            <button
              onClick={handleEditToggle}
              className="flex items-center gap-1.5 px-3 py-1.5 text-xs font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-md transition-colors duration-200"
            >
              <svg 
                xmlns="http://www.w3.org/2000/svg" 
                className="h-3.5 w-3.5" 
                fill="none" 
                viewBox="0 0 24 24" 
                stroke="currentColor"
              >
                <path 
                  strokeLinecap="round" 
                  strokeLinejoin="round" 
                  strokeWidth={2} 
                  d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" 
                />
              </svg>
              {isEditMode ? 'Done' : 'Edit'}
            </button>
          )}
        </div>
      </div>
      
      {/* Scroll container to keep overflow localized */}
      <div className="overflow-x-auto overflow-y-auto max-w-[800px] max-h-[400px]">
        <div className="min-w-[800px] flex">
          {/* Event titles column - fixed */}
          <div className="w-36 flex-shrink-0">
            {/* Time slot header spacer */}
            <div className="h-6 mb-3"></div>
            
            {/* Event titles (rows) */}
            {rows.map((row) => (
              <div key={row.id} className="h-8 border-t border-gray-200 flex items-center pr-1">
                {isEditMode ? (
                  <>
                    <button
                      onClick={() => handleDeleteRow(row.id)}
                      className="text-red-500 hover:text-red-700 flex-shrink-0 p-0.5"
                      title="Delete row"
                    >
                      <svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                      </svg>
                    </button>
                    <input
                      type="text"
                      value={row.title}
                      onChange={(e) => handleEditRowTitle(row.id, e.target.value)}
                      className="text-[10px] w-26 font-semibold text-gray-900 leading-tight text-left flex-1 border rounded px-1"
                    />
                  </>
                ) : (
                  <p className="text-[10px] font-semibold text-gray-900 leading-tight text-right w-full">
                    {row.title}
                  </p>
                )}
              </div>
            ))}
          </div>
          
          {/* Timeline chart area */}
          <div className="flex-shrink-0">
            {/* Time slots header */}
            <div className="mb-3">
              <div className="flex h-6 items-start">
                {timeSlots.map((slot, index) => (
                  <div 
                    key={index} 
                    className="text-[10px] text-left text-gray-700 font-medium flex items-center gap-1 flex-shrink-0"
                    style={{ width: '80px' }}
                  >
                    {isEditMode ? (
                      <input
                        type="text"
                        value={slot}
                        onChange={(e) => {
                          const newSlots = [...timeSlots];
                          newSlots[index] = e.target.value;
                          setTimeSlots(newSlots);
                        }}
                        className="w-16 text-[10px] font-medium text-gray-700 border-b border-gray-300 focus:border-blue-500 focus:outline-none bg-transparent whitespace-pre-line"
                        placeholder="HH:MM"
                      />
                    ) : (
                      <span className="whitespace-pre-line">{slot}</span>
                    )}
                    {isEditMode && (
                      <button
                        onClick={() => handleDeleteTimeSlot(index)}
                        className="text-red-500 hover:text-red-700 transition-opacity"
                        title="Delete time slot"
                      >
                        <svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                        </svg>
                      </button>
                    )}
                  </div>
                ))}
              </div>
            </div>

            {/* Timeline bars */}
            <div className="relative">
              {rows.map((row) => (
                <div key={row.id} className="relative h-8 border-t border-gray-200 flex items-center">
                  {/* Render all events for this row */}
                  {row.events.map((event) => (
                    <div
                      key={event.id}
                      className="absolute h-5 bg-blue-300 rounded flex items-center justify-center px-2 group"
                      style={{
                        left: `${getPosition(event.time)}px`,
                        width: `${getWidth(event.time, event.duration)}px`
                      }}
                    >
                      {isEditMode ? (
                        <div className="flex items-center gap-1 w-full">
                          <input
                            type="text"
                            value={event.description}
                            onChange={(e) => handleEditEvent(row.id, event.id, 'description', e.target.value)}
                            className="text-[10px] text-blue-700 font-medium bg-white rounded px-1 flex-1 min-w-0"
                          />
                          <button
                            onClick={() => handleDeleteEvent(row.id, event.id)}
                            className="text-red-600 hover:text-red-800 flex-shrink-0"
                            title="Delete event"
                          >
                            <svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                            </svg>
                          </button>
                        </div>
                      ) : (
                        <span className="text-[10px] text-blue-700 font-medium truncate">
                          {event.description}
                        </span>
                      )}
                    </div>
                  ))}
                  
                  {/* Add event button for this row when in edit mode */}
                  {isEditMode && (
                    <button
                      onClick={() => handleAddEvent(row.id)}
                      className="absolute right-2 h-5 px-2 bg-green-500 hover:bg-green-600 text-white rounded text-[10px] font-medium flex items-center gap-1"
                      title="Add event to this row"
                    >
                      <svg className="w-2.5 h-2.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
                      </svg>
                      Event
                    </button>
                  )}
                </div>
              ))}
              {/* Bottom border */}
              <div className="border-t border-gray-200"></div>
            </div>
          </div>
        </div>
      </div>

      {/* Event Modal */}
      {showEventModal && (
        <div className="fixed inset-0  flex items-center justify-center z-50">
          <div className="bg-white border rounded-lg shadow-xl p-6 w-96">
            <h3 className="text-lg font-bold text-gray-900 mb-4">Add New Event</h3>
            
            <div className="space-y-4"> 
              {/* Description */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Description
                </label>
                <input
                  type="text"
                  value={newEventData.description}
                  onChange={(e) => setNewEventData({...newEventData, description: e.target.value})}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="Enter event description"
                />
              </div>

              {/* Start Time */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Start Time
                </label>
                <select
                  value={newEventData.startTime}
                  onChange={(e) => setNewEventData({...newEventData, startTime: e.target.value})}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  {timeSlots.map((slot, index) => (
                    <option key={index} value={slot.replace('\n', ' ')}>
                      {slot.replace('\n', ' ')}
                    </option>
                  ))}
                </select>
              </div>

              {/* End Time */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  End Time
                </label>
                <select
                  value={newEventData.endTime}
                  onChange={(e) => setNewEventData({...newEventData, endTime: e.target.value})}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  {timeSlots.map((slot, index) => (
                    <option key={index} value={slot.replace('\n', ' ')}>
                      {slot.replace('\n', ' ')}
                    </option>
                  ))}
                </select>
              </div>
            </div>

            {/* Buttons */}
            <div className="flex gap-3 mt-6">
              <button
                onClick={handleCancelEvent}
                className="flex-1 px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 hover:bg-gray-200 rounded-md transition-colors"
              >
                Cancel
              </button>
              <button
                onClick={handleSaveEvent}
                className="flex-1 px-4 py-2 text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-md transition-colors"
              >
                Add Event
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default EventTimeline;
