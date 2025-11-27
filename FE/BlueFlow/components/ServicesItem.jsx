function ServicesItem({
  title,
  subtitle,
  img,
  bgColor = "bg-white",
  textColor = "text-black",
}) {
  return (
    <div
      className={`group relative w-full h-60 ${bgColor} rounded-xl overflow-hidden
        cursor-pointer
        shadow-md hover:shadow-xl
        transition-all duration-300 ease-in-out
        hover:-translate-y-1`}
    >
      {/* Background Image */}
      <div className="absolute inset-0">
        <img
          src={img}
          alt={`${title} ${subtitle}`}
          className="w-full h-full object-cover transition-transform duration-700 ease-out
            group-hover:scale-110"
        />
        {/* Dark overlay for better text readability */}
        <div className="absolute inset-0 bg-gradient-to-b from-black/70 via-black/40 to-black/70"></div>
      </div>

      {/* Content */}
      <div className="relative z-10 h-full flex flex-col justify-between p-5">
        {/* Top - Title Section */}
        <div className="flex flex-col gap-2.5">
          <div className="flex flex-col gap-1">
            <h3 className="text-white text-2xl font-bold leading-tight tracking-tight
              drop-shadow-[0_2px_4px_rgba(0,0,0,0.5)]">
              {title}
            </h3>
            <p className="text-white/90 text-base font-medium
              drop-shadow-[0_1px_2px_rgba(0,0,0,0.5)]">
              {subtitle}
            </p>
          </div>
        </div>

        {/* Bottom - CTA Link */}
        <div className="inline-flex items-center gap-2 w-fit">
          <a 
            href="#" 
            className="relative text-white text-sm font-medium tracking-wide
              drop-shadow-[0_1px_2px_rgba(0,0,0,0.5)]
              after:absolute after:left-0 after:bottom-0 after:w-0 after:h-[2px] 
              after:bg-white after:transition-all after:duration-300 
              hover:after:w-full
              group-hover:gap-3 transition-all duration-300"
            onClick={(e) => e.preventDefault()}
          >
            Xem thêm
            <svg
              xmlns="http://www.w3.org/2000/svg"
              className="inline-block w-4 h-4 ml-2 align-middle
                transition-transform duration-300
                group-hover:translate-x-1"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
              strokeWidth={2.5}
            >
              <path
                d="M9 5l7 7-7 7"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
            </svg>
          </a>
        </div>
      </div>
    </div>
  );
}

export default ServicesItem;
