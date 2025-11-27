import LocationItem from "./LocationItem";
import { useState, useEffect, useRef, useCallback, useMemo } from "react";

const locations = [
  [
    {
      img: "/imgs/SaiGon.png",
      name: "Saigon South Marina Club",
      address: "9A Tran Van Tra, Phu My Hung, Quan 7",
    },
    {
      img: "/imgs/RockKitchen.png",
      name: "Rock Kitchen & Bar",
      address: "12D1 Nguyen Thi Minh Khai, Quan 1",
    },
    {
      img: "/imgs/SweetSoon.png",
      name: "SWEET SOONG",
      address: "187 Nguyen Van Huong, Thao Dien, Quan 2",
    },
  ],
  [
    {
      img: "/imgs/SaiGon.png",
      name: "Saigon South Marina Club",
      address: "9A Tran Van Tra, Phu My Hung, Quan 7",
    },
    {
      img: "/imgs/SaiGon.png",
      name: "Rock Kitchen & Bar",
      address: "12D1 Nguyen Thi Minh Khai, Quan 1",
    },
    {
      img: "/imgs/SaiGon.png",
      name: "SWEET SOONG",
      address: "187 Nguyen Van Huong, Thao Dien, Quan 2",
    },
  ],
  [
    {
      img: "/imgs/SaiGon.png",
      name: "Saigon South Marina Club",
      address: "9A Tran Van Tra, Phu My Hung, Quan 7",
    },
    {
      img: "/imgs/SaiGon.png",
      name: "Rock Kitchen & Bar",
      address: "12D1 Nguyen Thi Minh Khai, Quan 1",
    },
    {
      img: "/imgs/SaiGon.png",
      name: "SWEET SOONG",
      address: "187 Nguyen Van Huong, Thao Dien, Quan 2",
    },
  ]
];

function LocationCarousel() {
  const [groupIdx, setGroupIdx] = useState(0);
  const [animating, setAnimating] = useState(false);
  const [direction, setDirection] = useState("right");
  const autoPlayRef = useRef(null);
  const isPausedRef = useRef(false);

  const handlePrev = useCallback(() => {
    if (animating) return;
    setDirection("left");
    setAnimating(true);
    setTimeout(() => {
      setGroupIdx((idx) => (idx === 0 ? locations.length - 1 : idx - 1));
      setAnimating(false);
    }, 500);
  }, [animating]);

  const handleNext = useCallback(() => {
    if (animating) return;
    setDirection("right");
    setAnimating(true);
    setTimeout(() => {
      setGroupIdx((idx) => (idx === locations.length - 1 ? 0 : idx + 1));
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
            setGroupIdx((idx) => (idx === locations.length - 1 ? 0 : idx + 1));
            setAnimating(false);
          }, 500);
        }
      }, 2500); // Tự động chuyển sau 2.5 giây
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

  // Memoize current group locations
  const currentLocations = useMemo(() => locations[groupIdx], [groupIdx]);

  return (
    <div 
      className="mx-26"
      onMouseEnter={handleMouseEnter}
      onMouseLeave={handleMouseLeave}
    >
      <h1 className="mb-12 text-2xl text-white bg-blue-400 w-fit rounded-lg p-1 mx-auto">Featured location</h1>

      <div
        className={`grid grid-cols-3 gap-8 mb-4 transition-opacity duration-500 ease-in-out
                ${
                  animating ? "opacity-40" : "opacity-100"
                }`}
      >
        {currentLocations.map((location, idx) => (
          <div
            key={`${groupIdx}-${idx}`}
            style={{
              transitionDelay: !animating ? `${idx * 80}ms` : '0ms'
            }}
          >
            <LocationItem location={location} />
          </div>
        ))}
      </div>

      <div className="flex justify-center items-center">
        <button onClick={handlePrev} className="mr-4" disabled={animating}>&lt;</button>
        <div className="flex space-x-2">
          {locations.map((_,i) =>(
            <button key = {i} className={`w-3 h-3 rounded-full ${ i === groupIdx ? "bg-blue-500" : "bg-gray-300"}`} onClick={() => !animating && handleDotClick(i)}/>
          ))}
        </div>
        <button onClick={handleNext} className="ml-4" disabled={animating}>&gt;</button>
              </div>

    </div>
  );
}

export default LocationCarousel;
