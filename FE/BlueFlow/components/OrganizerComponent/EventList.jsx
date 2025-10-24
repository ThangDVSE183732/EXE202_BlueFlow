import { useMemo, useState } from "react";

// A reusable DotMenu with simple hover popover as in the screenshot
function ActionMenu({ onViewDetail, onMessage }) {
  const [open, setOpen] = useState(false);
  return (
    <div className="relative inline-block">
      <button
        // onMouseEnter={() => setOpen(true)}
        // onMouseLeave={() => setOpen(false)}
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
        <div
          onMouseEnter={() => setOpen(true)}
          onMouseLeave={() => setOpen(false)}
          className="absolute left-1/2 -translate-x-1/2 mt-2 w-28 bg-white shadow-lg rounded-md text-xs text-gray-700 py-1 z-10"
        >
          <button className="block w-full text-left px-3 py-1 hover:bg-gray-50" onClick={onViewDetail}>View Detail</button>
          <button className="block w-full text-left px-3 py-1 hover:bg-gray-50" onClick={onMessage}>Delete</button>
        </div>
      )}
    </div>
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

const sampleRows = [
  {
    event: "Tech Event",
    status: "Ongoing",
    createdAt: "10 July 2025",
    location: "Main Hall – Campus A",
  },
  {
    event: "Orientation Day 2025",
    status: "Ongoing",
    createdAt: "10 July 2025",
    location: "Main Hall – Campus A",
  },
  {
    event: "Tech Event",
    status: "Ongoing",
    createdAt: "10 July 2025",
    location: "Main Hall – Campus A",
  },
  {
    event: "Orientation Day 2025",
    status: "Ongoing",
    createdAt: "10 July 2025",
    location: "Main Hall – Campus A",
  },
  {
    event: "Orientation Day 2025",
    status: "Ongoing",
    createdAt: "10 July 2025",
    location: "Main Hall – Campus A",
  },
  {
    event: "Orientation Day 2025",
    status: "Ongoing",
    createdAt: "10 July 2025",
    location: "Main Hall – Campus A",
  },
  {
    event: "Orientation Day 2025",
    status: "Ongoing",
    createdAt: "10 July 2025",
    location: "Main Hall – Campus A",
  },
];

export default function EventList({ rows = sampleRows, onViewDetail, onMessage }) {
  return (
    <div className="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
      <table className="w-full table-fixed text-sm">
        <thead className="bg-gray-50 text-gray-600">
          <tr>
            <th scope="col" className="px-4 pl-21 py-3 text-left font-semibold w-[30%]">Event</th>
            <th scope="col" className="px-4 py-3 pl-12 text-left font-semibold w-[15%]">Status</th>
            <th scope="col" className="px-4 py-3 text-left pl-8 font-semibold w-[15%]">Create At</th>
            <th scope="col" className="px-4 py-3 text-left pl-24 font-semibold w-[30%]">Location</th>
            <th scope="col" className="px-4 py-3 text-center font-semibold w-[10%]">Action</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-gray-200">
          {rows.map((r, idx) => (
            <tr key={idx} className="hover:bg-gray-50 ">
              <td className="px-4 py-3 text-gray-800 font-medium truncate text-left pl-7">{r.event}</td>
              <td className="px-4 py-3"><StatusPill status={r.status} /></td>
              <td className="px-4 py-3 text-gray-600 whitespace-nowrap">{r.createdAt}</td>
              <td className="px-4 py-3 text-gray-700 truncate">{r.location}</td>
              <td className="px-4 py-3">
                <div className="flex justify-center">
                  <ActionMenu
                    onViewDetail={() => onViewDetail?.(r, idx)}
                    onMessage={() => onMessage?.(r, idx)}
                  />
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
