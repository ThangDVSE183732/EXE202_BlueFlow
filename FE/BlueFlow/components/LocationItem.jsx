import ArrowRight from "./ArrowRight";

function LocationItem({location}) {
    const {img, name, address} = location;
    return (
        <div className="relative">
            <div>
                <img src = {img} alt="Location" />
            </div>
            <div className="absolute top-9/12 ml-6 bg-sky-100 p-4 w-68 h-3/5 rounded-lg">
                <h3 className="text-lg font-semibold">{name}</h3>
                <p className=" ml-4 text-center text-sm h-auto w-53 text-wrap">{address}</p>
                <div className="flex justify-center items-center space-x-2 mt-4">
                    <h1 className="text-lg font-semibold text-blue-400 ">Readmore</h1>
                    <ArrowRight size={18}/>
                </div>
            </div>
        </div>
    )
}
export default LocationItem;