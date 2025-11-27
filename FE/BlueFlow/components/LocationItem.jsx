function LocationItem({location}) {
    const {img, name, address} = location;
    return (
        <div className="group relative w-full rounded-xl overflow-hidden
          cursor-pointer
          shadow-lg hover:shadow-2xl
          transition-all duration-300 ease-in-out
          hover:-translate-y-2">
            {/* Image */}
            <div className="relative h-80 overflow-hidden">
                <img 
                  src={img} 
                  alt={name}
                  className="w-full h-full object-cover transition-transform duration-700 ease-out
                    group-hover:scale-110"
                />
                {/* Gradient overlay */}
                <div className="absolute inset-0 bg-gradient-to-t from-black/80 via-black/20 to-transparent"></div>
            </div>
            
            {/* Content Card */}
            <div className="absolute bottom-0 left-0 right-0 bg-white/95 backdrop-blur-md p-6
              transform transition-all duration-300
              group-hover:bg-white">
                <h3 className="text-xl font-bold text-gray-900 mb-2
                  line-clamp-1">
                    {name}
                </h3>
                <p className="text-sm text-gray-600 mb-4
                  line-clamp-2">
                    {address}
                </p>
                
                {/* CTA Link */}
                <div className="inline-flex items-center gap-2 w-fit">
                  <a 
                    href="#" 
                    className="relative text-blue-600 text-sm font-semibold
                      after:absolute after:left-0 after:bottom-0 after:w-0 after:h-[2px] 
                      after:bg-blue-600 after:transition-all after:duration-300 
                      hover:after:w-full
                      transition-all duration-300"
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
    )
}
export default LocationItem;