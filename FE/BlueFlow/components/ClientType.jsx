import ClientTypeItem from "./ClientTypeItem";

const clientType = [
    {logo: "/imgs/Icon (6).png", type: "Organizers", description: "Lên kế hoạch và quản lý sự kiện của bạn một cách hiệu quả để tạo nên những trải nghiệm đáng nhớ."},
    {logo: "/imgs/Member (3).png", type: "Sponsors", description: "Dễ dàng tìm kiếm và kết nối với các cơ hội tài trợ lý tưởng."},
    {logo: "/imgs/Icon (5).png", type: " Supplier", description: "Tận hưởng quyền truy cập nhanh chóng và dễ dàng vào một mạng lưới rộng lớn các nhà cung cấp dịch vụ đã được kiểm duyệt."},

]

function ClientType() {
    return(
        <div className="flex justify-center items-center space-x-32">
            {clientType.map((item, index) => ((<ClientTypeItem key={index} item={item} />)))}
        </div>
    )
}
export default ClientType;