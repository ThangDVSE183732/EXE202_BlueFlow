import SearchBar from "../SearchBar";
import EventList from "./EventList";

function EventManagement() {
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
                <button className="text-white  bg-sky-500 px-8 rounded-full py-1 text-base font-semibold">Event list</button>
            </div>
            <div className="mt-10 mb-15">
                <EventList />
            </div>
        </div>
    )
}
export default EventManagement;