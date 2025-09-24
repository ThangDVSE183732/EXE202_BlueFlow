import LocationItem from "./LocationItem";
import { useState } from "react";

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

  const handlePrev = () => {
    setDirection("left");
    setAnimating(true);
    setTimeout(() => {
      setGroupIdx((idx) => (idx === 0 ? locations.length - 1 : idx - 1)); //Nếu ở nhóm đầu thì lùi về nhóm cuối
      setAnimating(false);
    }, 300); // Giả sử animation kéo dài 500ms
  };
  const handleNext = () => {
    setDirection("right");
    setAnimating(true);
    setTimeout(() => {
      setGroupIdx((idx) => (idx === locations.length - 1 ? 0 : idx + 1)); //Nếu ở nhóm cuối thì lùi về nhóm đầu
      setAnimating(false);
    }, 300); // Giả sử animation kéo dài 500ms
  };
  const handleDotClick = (i) => {
    setDirection(i > groupIdx ? "right" : "left");
    setAnimating(true);
    setTimeout(() => {
      setGroupIdx(i);
      setAnimating(false);
    }, 300);
  };

  return (
    <div className="mx-26">
            <h1 className="mb-12 text-2xl text-white bg-blue-400 w-fit rounded-lg p-1 mx-auto">Featured location</h1>

      <div
        className={`grid grid-cols-3  gap-8 transition-transform duration-300
                ${
                  animating
                    ? direction === "right"
                      ? "translate-x-16 opacity-50"
                      : "-translate-x-16 opacity-50"
                    : "translate-x-0 opacity-100"
                }`}
      >
        {locations[groupIdx].map((location, idx) => (
          <LocationItem key={idx} location={location} />
        ))}
      </div>

            <div className="flex justify-center items-center mt-40">
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
