function ServicesItem({
  title,
  subtitle,
  img,
  bgColor = "bg-white",
  textColor = "text-black",
}) {
  return (
    <div
      className={`w-full h-60 ${bgColor} rounded-lg aspect-[4/3] p-10 border-1 border-b-4 border-b-black`}
    >
      <div className="flex space-x-8 justify-center">
        <div>
          <div className="mb-22">
            <h1 className={`p-2 bg-blue-600 h-6 rounded-md text-left w-fit flex justify-center items-center text-2xl font-normal ${textColor}`}>
              {title}
            </h1>
            <h1 className={`p-2 bg-blue-600 h-6 rounded-md text-left w-fit flex justify-center items-center text-2xl font-normal ${textColor}`}>
              {subtitle}
            </h1>
          </div>
          <div className="flex items-center gap-2 w-32">
            <span className="bg-gray-900 rounded-full w-8 h-8 flex items-center justify-center">
              <svg
                xmlns="http://www.w3.org/2000/svg"
                className="w-5 h-5 text-blue-400"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
                strokeWidth={2}
              >
                <path
                  d="M7 17L17 7M7 7h10v10"
                  strokeLinecap="round"
                  strokeLinejoin="round"
                />
              </svg>
            </span>
            <span className={`${textColor} text-md`}>Learn more</span>
          </div>
        </div>
        <img
          src={img}
          alt={`${title} photo`}
          className="w-8/13 h-42"
        />
      </div>
    </div>
  );
}

export default ServicesItem;
