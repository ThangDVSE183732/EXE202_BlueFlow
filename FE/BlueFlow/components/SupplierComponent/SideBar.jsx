



function SideBar({ activeItem, onChange, opts, subChange, onSubChange }) {

    const handleClick = (key) => {
        onChange(key);
        if (key === 'discovery') onSubChange('find');
        if (key === 'projects') onSubChange('pending');
        if (key === 'profile') onSubChange('brand');
    };
    const selected = activeItem === opts[0].key;
    


    return (
        <aside className="w-[220px] h-lvh rounded-3xl border border-gray-200 bg-white p-4 shadow-[0_8px_24px_rgba(0,0,0,0.08)]">
            
            {/* Active pill */}
            <nav className="flex flex-col gap-3">
                        <button
                            key={opts[0].key}
                            type="button"
                            onClick={() => handleClick(opts[0].key)}
                            className={[
                                'group flex items-center gap-3 rounded-2xl px-3 py-2 text-sm transition',
                                selected
                                    ? 'bg-sky-500 text-white font-semibold shadow'
                                    : 'text-gray-700 hover:bg-gray-50 hover:text-black',
                            ].join(' ')}
                        >
                            <span
                                className={[
                                    'inline-flex items-center justify-center h-5 w-5 rounded-md',
                                    selected ? 'bg-white/20 text-white' : 'text-gray-700 group-hover:text-black',
                                ].join(' ')}
                            >
                                {opts[0].icon}
                            </span>
                            <span>{opts[0].label}</span>
                        </button>

            </nav>

            <div className="h-px bg-gray-200 mx-1 mb-3 mt-6" />
            
            <nav className="flex flex-col gap-3">
                {opts.slice(1).map((it) => {
                    const selected = activeItem === it.key;
                    return (
                            <div key={it.key}>
                                <button
                                    type="button"
                                    onClick={() => handleClick(it.key)}
                                    className={[
                                        'group flex items-center gap-3 rounded-2xl px-3 py-2 text-sm transition w-full text-left',
                                        selected
                                            ? 'bg-sky-500 text-white font-semibold shadow'
                                            : 'text-gray-700 hover:bg-gray-50 hover:text-black',
                                    ].join(' ')}
                                >
                                    <span
                                        className={[
                                            'inline-flex items-center justify-center h-5 w-5 rounded-md',
                                            selected ? 'bg-white/20 text-white' : 'text-gray-700 group-hover:text-black',
                                        ].join(' ')}
                                    >
                                        {it.icon}
                                    </span>
                                    <span>{it.label}</span>
                                </button>

                                {/* Discovery sub-items when selected */}
                                {it.key === 'discovery' && selected && (
                                    <div className="ml-2 mt-2 flex flex-col gap-2">
                                        <button type="button" onClick={() => onSubChange && onSubChange('find')} className="flex items-center gap-3 text-left">
                                            <span className={[
                                                'h-6 w-1 rounded-full',
                                                subChange === 'find' ? 'bg-sky-500' : 'bg-transparent'
                                            ].join(' ')} />
                                            <span className={[
                                                'text-sm',
                                                subChange === 'find' ? 'font-semibold text-black' : 'text-gray-400'
                                            ].join(' ')}>Find Partners</span>
                                        </button>
                                        <button type="button" onClick={() => onSubChange && onSubChange('saved')} className="flex items-center gap-3 text-left">
                                            <span className={[
                                                'h-6 w-1 rounded-full',
                                                subChange === 'saved' ? 'bg-sky-500' : 'bg-transparent'
                                            ].join(' ')} />
                                            <span className={[
                                                'text-sm',
                                                subChange === 'saved' ? 'font-semibold text-black' : 'text-gray-400'
                                            ].join(' ')}>Saved Events</span>
                                        </button>
                                    </div>
                                )}


                                 {/* Profile and setting sub-items when selected */}
                                {it.key === 'profile' && selected && (
                                    <div className="ml-2 mt-2 flex flex-col gap-2">
                                        <button type="button" onClick={() => onSubChange && onSubChange('brand')} className="flex items-center gap-3 text-left">
                                            <span className={[
                                                'h-6 w-1 rounded-full',
                                                subChange === 'brand' ? 'bg-sky-500' : 'bg-transparent'
                                            ].join(' ')} />
                                            <span className={[
                                                'text-sm',
                                                subChange === 'brand' ? 'font-semibold text-black' : 'text-gray-400'
                                            ].join(' ')}>Brand Profile</span>
                                        </button>
                                        <button type="button" onClick={() => onSubChange && onSubChange('account')} className="flex items-center gap-3 text-left">
                                            <span className={[
                                                'h-6 w-1 rounded-full',
                                                subChange === 'account' ? 'bg-sky-500' : 'bg-transparent'
                                            ].join(' ')} />
                                            <span className={[
                                                'text-sm',
                                                subChange === 'account' ? 'font-semibold text-black' : 'text-gray-400'
                                            ].join(' ')}>Account Setting</span>
                                        </button>
                                        {/* <button type="button" onClick={() => onSubChange && onSubChange('marketing')} className="flex items-center gap-3 text-left">
                                            <span className={[
                                                'h-6 w-1 rounded-full',
                                                subChange === 'marketing' ? 'bg-sky-500' : 'bg-transparent'
                                            ].join(' ')} />
                                            <span className={[
                                                'text-sm',
                                                subChange === 'marketing' ? 'font-semibold text-black' : 'text-gray-400'
                                            ].join(' ')}>Marketing Preferences</span>
                                        </button> */}
                                    </div>
                                )}
                            </div>
                    );
                })}
            </nav>
        </aside>
    );
}

export default SideBar;