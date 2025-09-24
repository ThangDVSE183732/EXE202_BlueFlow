function ClientTypeItem({item}) {
    const {logo, type, description} = item
    return (
        <div className="w-[14rem] h-50">
           <img className="mx-auto w-15 h-15 object-contain mb-2" src={logo} alt="Client Type Icon" />
           <h1 className="text-3xl font-semibold text-blue-400 mb-4">{type}</h1>
           <p className="text-center text-wrap text-sm text-gray-500">{description}</p>
        </div>
    );
}
export default ClientTypeItem;