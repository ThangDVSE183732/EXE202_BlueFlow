function FloatingInput({id, type, label}) {
    return(
        <div className="relative">
      <input
        id={id}
        type={type}
        placeholder={label}
        className="peer w-full border-1 border-gray-400 rounded-lg px-5 py-1 text-lg outline-none placeholder-transparent
                   focus:border-sky-500"
      />
      <label
        htmlFor={id}
        className="absolute -top-3 left-4 px-2 bg-white text-sm text-gray-600
                   peer-focus:text-sky-600 text-xs"
      >
        {label}
      </label>
    </div>
    )

}
export default FloatingInput;