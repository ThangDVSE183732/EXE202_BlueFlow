import SegmentedControl from '../../components/OrganizerComponent/SegmentedControl';
function SegmentedControlItem({tab, setTab}) {

    return (
        <div >
                <SegmentedControl
                    options={[
                        { label: 'Dashboard', value: 'dashboard' },
                        { label: 'Event Management', value: 'event' },
                    ]}
                    value={tab}
                    onChange={setTab}
                    className="mb-6"
                />
                
            </div>
    );
}

export default SegmentedControlItem;
