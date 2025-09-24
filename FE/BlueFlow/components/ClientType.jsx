import ClientTypeItem from "./ClientTypeItem";

const clientType = [
    {logo: "/imgs/Icon (6).png", type: "Organizers", description: "Plan and manage your events efficiently to create memorable experiences."},
    {logo: "/imgs/Member (3).png", type: "Sponsors", description: "Easily find and connect ideal sponsorship opportunities"},
    {logo: "/imgs/Icon (5).png", type: "Service provides", description: "Enjoy quick and easy access to a vast network of vetted service providers."},

]

function ClientType() {
    return(
        <div className="flex justify-center items-center space-x-32">
            {clientType.map((item, index) => ((<ClientTypeItem key={index} item={item} />)))}
        </div>
    )
}
export default ClientType;