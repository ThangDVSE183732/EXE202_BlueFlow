function SearchBar({ iconClass = "h-3 w-3", sizeClass = "w-48", button = "left-39", input ="w-full rounded-full" }) {
  return (
    <div className={['relative flex items-center', sizeClass].join(" ")}>
      <input
        type="text"
        // placeholder="Search"
        className={`border  ${input}`}
      />

      <button className={["absolute  bg-blue-400 rounded-full w-5 h-5 flex items-center justify-center", button].join(" ")}>
        {/* SVG kính lúp */}

        <svg
          xmlns="http://www.w3.org/2000/svg"
          className={`${iconClass} text-white`}
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
          strokeWidth={2}
        >
          <circle cx="11" cy="11" r="8" stroke="white" strokeWidth="2" />
          <line
            x1="21"
            y1="21"
            x2="16.65"
            y2="16.65"
            stroke="white"
            strokeWidth="2"
          />
        </svg>
      </button>
    </div>
  );
}

export default SearchBar;
