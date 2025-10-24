function StatisticCards({cards}) {
    return(
        <div className="grid grid-cols-2  w-ful h-full gap-4">
            {cards.map((card, index) => (
                <div key={index} className="w-full h-full rounded-lg p-3  text-left shadow-xl border border-gray-300  ">
                    <div className="">
                        <h1 className="w-25">{card.title}</h1>
                        <p className=" text-3xl font-semibold">{card.value}</p>
                        <h6 className="text-xs text-gray-400 font-medium pt-3">In this month</h6>
                    </div>
                </div>
            ))}
        </div>

    );
}
export default StatisticCards;