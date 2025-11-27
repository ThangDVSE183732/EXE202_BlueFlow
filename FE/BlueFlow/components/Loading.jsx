import EqualizerLoader from './EqualizerLoader';

function Loading({ message = "Đang tải thông tin event...", color = "#00A6F4" }) {
  return <EqualizerLoader message={message} color={color} />;
}

export default Loading;
