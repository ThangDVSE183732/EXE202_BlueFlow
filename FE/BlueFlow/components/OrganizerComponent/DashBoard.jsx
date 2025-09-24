import Note from "./Note";
import PieChartBox from "./PieChart";
import ProfileInteraction from "./ProfileInteraction";
import StatisticCards from "./statisticCards";

const cards = [
    { title: "Active Partnerships ", value: "47"},
     { title: "Pending Proposals", value: "12"},
      { title: "Events Participated", value: "156"},
       { title: "Average Rating", value: "4.5"}
]

function Dashboard() {
    return(
        <div>
             <div className="space-y-1 text-left">
            <h1 className="text-3xl font-bold text-sky-500">Workspace</h1>
            <h className="text-sm text-gray-400">Real-time updates on sponsorships, events, and audience reach.</h>
            </div>
            <div className="h-px w-full bg-gray-300 mx-1 mb-3 mt-6" />
            <div className="flex space-x-5 h-[290px] mt-4">
                <div className="w-42 h-full flex-auto justify-center items-center">
                    <StatisticCards cards={cards} />
                </div>
                <div className="w-96 h-full flex-auto bg-blue-400 rounded-lg">
                    <ProfileInteraction/>
                </div>
            </div>
            <div className="flex space-x-11 h-[290px] mt-7 mb-10">
                <div className="w-96 h-[288px] rounded-lg flex-auto justify-center items-cente bg-red-400">
                    <Note/>
                </div>
                <div className="w-24 h-48 flex-auto bg-blue-400 rounded-lg">
                    <PieChartBox />
                </div>
            </div>
        </div>
    )
}
export default Dashboard;