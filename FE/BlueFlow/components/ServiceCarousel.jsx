import ServicesItem from "./ServicesItem";
import {useState } from "react";



const servicesGroup = [
  [
    {title: "Wedding", subtitle: "decoration", img: "/imgs/wedding.png", color: "bg-white", bgColor: "bg-wh"},
    {title: "Music", subtitle: "event", img: "/imgs/wedding.png", color: "bg-white", bgColor: "bg-white"},
    {title: "Workshop", subtitle: "event", img: "/imgs/wedding.png", color: "bg-white", bgColor: "bg-white"},
    {title: "Boutique", subtitle: "event", img: "/imgs/wedding.png", color: "bg-white", bgColor: "bg-white"},
  ],
  [
    {title: "Wedding", subtitle: "decoration", img: "/imgs/wedding.png", color: "bg-white", bgColor: "bg-sky-200"},
    {title: "Music", subtitle: "event", img: "/imgs/wedding.png", color: "bg-white", bgColor: "bg-rose-200"},
    {title: "Workshop", subtitle: "event", img: "/imgs/wedding.png", color: "bg-white", bgColor: "bg-purple-200"},
    {title: "Boutique", subtitle: "event", img: "/imgs/wedding.png", color: "bg-white", bgColor: "bg-pink-200"},
  ],
  [
    {title: "Wedding", subtitle: "decoration", img: "/imgs/wedding.png", color: "bg-white", bgColor: "bg-wh"},
    {title: "Music", subtitle: "event", img: "/imgs/wedding.png", color: "bg-white", bgColor: "bg-white"},
    {title: "Workshop", subtitle: "event", img: "/imgs/wedding.png", color: "bg-white", bgColor: "bg-white"},
    {title: "Boutique", subtitle: "event", img: "/imgs/wedding.png", color: "bg-white", bgColor: "bg-white"},
  ],
  [
    {title: "Wedding", subtitle: "decoration", img: "/imgs/wedding.png", color: "bg-white", bgColor: "bg-sky-200"},
    {title: "Music", subtitle: "event", img: "/imgs/wedding.png", color: "bg-white", bgColor: "bg-rose-200"},
    {title: "Workshop", subtitle: "event", img: "/imgs/wedding.png", color: "bg-white", bgColor: "bg-purple-200"},
    {title: "Boutique", subtitle: "event", img: "/imgs/wedding.png", color: "bg-white", bgColor: "bg-pink-200"},
  ],
  [
    {title: "Wedding", subtitle: "decoration", img: "/imgs/wedding.png", color: "bg-white", bgColor: "bg-sky-200"},
    {title: "Music", subtitle: "event", img: "/imgs/wedding.png", color: "bg-white", bgColor: "bg-rose-200"},
    {title: "Workshop", subtitle: "event", img: "/imgs/wedding.png", color: "bg-white", bgColor: "bg-purple-200"},
    {title: "Boutique", subtitle: "event", img: "/imgs/wedding.png", color: "bg-white", bgColor: "bg-pink-200"},
  ]

];


function ServiceCarousel() {

  const[groupIdx, setGroupIdx] = useState(0);
  const[animating, setAnimating] = useState(false);
  const[direction, setDirection] = useState("right"); // 'next' or 'prev'

  const handlePrev = () => {
    setDirection("left");
    setAnimating(true);
    setTimeout(() => {
          setGroupIdx((idx) => (idx === 0 ? servicesGroup.length - 1 : idx - 1));//Nếu ở nhóm đầu thì lùi về nhóm cuối
          setAnimating(false);
    }, 300); // Giả sử animation kéo dài 500ms
  }
  const handleNext = () => {
        setDirection("right");
    setAnimating(true);
    setTimeout(() => {
          setGroupIdx((idx) => (idx === servicesGroup.length - 1 ? 0 : idx + 1));//Nếu ở nhóm cuối thì lùi về nhóm đầu
          setAnimating(false);
    }, 300); // Giả sử animation kéo dài 500ms
  }
  const handleDotClick = (i) => {
        setDirection( i > groupIdx ? "right" : "left");
        setAnimating(true); 
        setTimeout(() => {
              setGroupIdx(i);
              setAnimating(false);
        }, 300); 
  }
  return (
    <div className=" mx-26 mb-16">
      <h1 className="mb-12 text-2xl text-white bg-blue-400 w-fit rounded-lg p-1">Most chosen service</h1>
      <div className={`grid grid-cols-2 grid-rows-2 gap-8 mb-10 transition-transform duration-300
      ${animating ? (direction === "right" ? "translate-x-16 opacity-50" : "-translate-x-16 opacity-50") : "translate-x-0 opacity-100"}`}>
          {servicesGroup[groupIdx].map((item, idx)=> (
            <ServicesItem key ={idx} title={item.title} subtitle={item.subtitle} img={item.img} color={item.color} bgColor={item.bgColor}/>
          ))}      
      </div>
      <div className="flex justify-center items-center ">
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
