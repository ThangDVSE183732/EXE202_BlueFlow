import ServicesItem from "./ServicesItem";
import {useState, useEffect, useRef, useCallback, useMemo } from "react";



const servicesGroup = [
  [
    {title: "Tiệc Cưới", subtitle: "Trang trí & Tổ chức", img: "/imgs/wedding.png", color: "bg-white", bgColor: "bg-wh"},
    {title: "Sự Kiện Âm Nhạc", subtitle: "Concert & Live Show", img: "/imgs/amNhac.jpg", color: "bg-white", bgColor: "bg-white"},
    {title: "Hội Thảo", subtitle: "Workshop & Training", img: "/imgs/hoiThao.jpg", color: "bg-white", bgColor: "bg-white"},
    {title: "Sự Kiện Thời Trang", subtitle: "Fashion Show & Launch", img: "/imgs/thoiTrang.jpg", color: "bg-white", bgColor: "bg-white"},
  ],
  [
    {title: "Hội Nghị", subtitle: "Conference & Summit", img: "/imgs/hoiNghi.jpg", color: "bg-white", bgColor: "bg-sky-200"},
    {title: "Lễ Khai Trương", subtitle: "Grand Opening Event", img: "/imgs/leKhaiTruong.jpg", color: "bg-white", bgColor: "bg-rose-200"},
    {title: "Triển Lãm", subtitle: "Exhibition & Trade Show", img: "/imgs/trienLam.jpg", color: "bg-white", bgColor: "bg-purple-200"},
    {title: "Sinh Nhật", subtitle: "Birthday Celebration", img: "/imgs/sinhNhat.jpg", color: "bg-white", bgColor: "bg-pink-200"},
  ],
  [
    {title: "Team Building", subtitle: "Corporate Events", img: "/imgs/teamBuilding.jpg", color: "bg-white", bgColor: "bg-wh"},
    {title: "Gala Dinner", subtitle: "Awards & Recognition", img: "/imgs/galaDinner.jpg", color: "bg-white", bgColor: "bg-white"},
    {title: "Product Launch", subtitle: "Sản Phẩm Mới", img: "/imgs/productLaunch.jpg", color: "bg-white", bgColor: "bg-white"},
    {title: "Festival", subtitle: "Lễ Hội & Văn Hóa", img: "/imgs/festival.jpg", color: "bg-white", bgColor: "bg-white"},
  ],
  [
    {title: "Tiệc Cưới", subtitle: "Trang trí & Tổ chức", img: "/imgs/wedding.png", color: "bg-white", bgColor: "bg-sky-200"},
    {title: "Sự Kiện Âm Nhạc", subtitle: "Concert & Live Show", img: "/imgs/amNhac.jpg", color: "bg-white", bgColor: "bg-rose-200"},
    {title: "Hội Thảo", subtitle: "Workshop & Training", img: "/imgs/hoiThao.jpg", color: "bg-white", bgColor: "bg-purple-200"},
    {title: "Sự Kiện Thời Trang", subtitle: "Fashion Show & Launch", img: "/imgs/thoiTrang.jpg", color: "bg-white", bgColor: "bg-pink-200"},
  ],
  [
    {title: "Hội Nghị", subtitle: "Conference & Summit", img: "/imgs/hoiNghi.jpg", color: "bg-white", bgColor: "bg-sky-200"},
    {title: "Lễ Khai Trương", subtitle: "Grand Opening Event", img: "/imgs/leKhaiTruong.jpg", color: "bg-white", bgColor: "bg-rose-200"},
    {title: "Triển Lãm", subtitle: "Exhibition & Trade Show", img: "/imgs/trienLam.jpg", color: "bg-white", bgColor: "bg-purple-200"},
    {title: "Sinh Nhật", subtitle: "Birthday Celebration", img: "/imgs/sinhNhat.jpg", color: "bg-white", bgColor: "bg-pink-200"},
  ]

];


function ServiceCarousel() {

  const[groupIdx, setGroupIdx] = useState(0);
  const[animating, setAnimating] = useState(false);
  const[direction, setDirection] = useState("right"); // 'next' or 'prev'
  const autoPlayRef = useRef(null);
  const isPausedRef = useRef(false);

  const handlePrev = useCallback(() => {
    if (animating) return;
    setDirection("left");
    setAnimating(true);
    setTimeout(() => {
          setGroupIdx((idx) => (idx === 0 ? servicesGroup.length - 1 : idx - 1));
          setAnimating(false);
    }, 500);
  }, [animating]);

  const handleNext = useCallback(() => {
    if (animating) return;
    setDirection("right");
    setAnimating(true);
    setTimeout(() => {
          setGroupIdx((idx) => (idx === servicesGroup.length - 1 ? 0 : idx + 1));
          setAnimating(false);
    }, 500);
  }, [animating]);

  const handleDotClick = useCallback((i) => {
    if (animating || i === groupIdx) return;
    setDirection(i > groupIdx ? "right" : "left");
    setAnimating(true); 
    setTimeout(() => {
          setGroupIdx(i);
          setAnimating(false);
    }, 500); 
  }, [animating, groupIdx]);

  // Auto-play functionality
  useEffect(() => {
    if (autoPlayRef.current) {
      clearInterval(autoPlayRef.current);
    }
    
    // Chỉ chạy auto-play khi không đang animate và không bị pause
    if (!animating) {
      autoPlayRef.current = setInterval(() => {
        if (!isPausedRef.current) {
          setDirection("right");
          setAnimating(true);
          setTimeout(() => {
            setGroupIdx((idx) => (idx === servicesGroup.length - 1 ? 0 : idx + 1));
            setAnimating(false);
          }, 500);
        }
      }, 2000); // Tự động chuyển sau 2 giây
    }

    return () => {
      if (autoPlayRef.current) {
        clearInterval(autoPlayRef.current);
      }
    };
  }, [groupIdx, animating]); // Re-run khi groupIdx hoặc animating thay đổi

  // Pause auto-play khi user hover vào carousel
  const handleMouseEnter = useCallback(() => {
    isPausedRef.current = true;
  }, []);

  const handleMouseLeave = useCallback(() => {
    isPausedRef.current = false;
  }, []);

  // Memoize current group items
  const currentItems = useMemo(() => servicesGroup[groupIdx], [groupIdx]);
  return (
    <div 
      className=" mx-26 mb-16"
      onMouseEnter={handleMouseEnter}
      onMouseLeave={handleMouseLeave}
    >
      <h1 className="mb-12 text-2xl text-white bg-blue-400 w-fit rounded-lg p-1">Most chosen service</h1>
      <div className="grid grid-cols-2 grid-rows-2 gap-8 mb-4">
        {currentItems.map((item, idx)=> (
          <div
            key={`${groupIdx}-${idx}`}
            className={`transition-opacity duration-500 ease-in-out
              ${animating ? "opacity-40" : "opacity-100"}`}
            style={{
              transitionDelay: !animating ? `${idx * 80}ms` : '0ms'
            }}
          >
            <ServicesItem 
              title={item.title} 
              subtitle={item.subtitle} 
              img={item.img} 
              color={item.color} 
              bgColor={item.bgColor}
            />
          </div>
        ))}      
      </div>
      <div className="flex justify-center items-center">
        <button onClick={handlePrev} className="mr-4" disabled={animating}>&lt;</button>
        <div className="flex space-x-2">
          {servicesGroup.map((_,i) =>(
            <button key = {i} className={`w-3 h-3 rounded-full ${ i === groupIdx ? "bg-blue-500" : "bg-gray-300"}`} onClick={() => !animating && handleDotClick(i)}/>
          ))}
        </div>
        <button onClick={handleNext} className="ml-4" disabled={animating}>&gt;</button>
              </div>

        
    </div>
  );
}

export default ServiceCarousel;
