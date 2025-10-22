import React from "react";
import {
	LineChart,
	Line,
	CartesianGrid,
	XAxis,
	YAxis,
	ResponsiveContainer,
	Tooltip,
	Legend,
} from "recharts";

// Simple SVG Line Chart component with legend, grid, and 3 series
// Props allow passing data and labels; sensible defaults match the screenshot
export default function ProfileInteraction({
	title = "",
	chartWidth , // number (px) or CSS string, default 100%
	chartHeight = 229, // number (px) or CSS string
	labels = [
		"Jan",
		"Feb",
		"Mar",
		"Apr",
		"May",
		"Jun",
		"Jul",
		"Aug",
		"Sep",
		"Oct",
		"Nov",
		"Dec",
	],
	series = [
		{
			key: "views",
			name: "Profile Views",
			color: "#F59E0B",
			data: [1300, 1500, 1400, 1700, 2000, 2200, 2100, 1900, 2200, 2350, 2600, 2800],
		},
		{
			key: "invites",
			name: "Invitations",
			color: "#0EA5E9",
			data: [80, 120, 110, 140, 180, 220, 210, 160, 180, 200, 240, 300],
		},
		{
			key: "messages",
			name: "Messages",
			color: "#059669",
			data: [60, 90, 85, 120, 150, 180, 170, 130, 150, 170, 200, 240],
		},
	],
	maxY = 3000,
}) {
	// Build recharts-friendly dataset
	const data = labels.map((label, i) => {
		const row = { name: label };
		series.forEach((s) => {
			row[s.key] = s.data[i] ?? null;
		});
		return row;
	});

	return (
		<div className="bg-white rounded-lg shadow-xl border border-gray-100 p-4 max-w-full w-full overflow-hidden min-w-0">
			<div className="flex items-start justify-between px-1 gap-4 flex-wrap">
				<h2 className="text-3xl font-bold text-sky-500 leading-tight">{title}</h2>
				<div className="flex items-center gap-5 text-sm flex-wrap mr-3">
					{series.map((s) => (
						<div key={s.key} className="flex items-center space-x-2">
							<span
								className="inline-block h-3 w-3 rounded-full"
								style={{ backgroundColor: s.color }}
							/>
							<span className="text-gray-700 font-medium">{s.name}</span>
						</div>
					))}
				</div>
			</div>
			<div
				className="mt-2 w-full min-w-0"
				style={{ height: chartHeight, width: chartWidth ?? "100%" }}
			>
				<ResponsiveContainer width="100%" height="100%">
					<LineChart data={data} margin={{ top: 10, right: 20, bottom: 8, left: -4 }}>
						<CartesianGrid stroke="#E5E7EB" vertical={false} />
						<XAxis dataKey="name" stroke="#6B7280" tickMargin={8} interval="preserveStartEnd" />
						<YAxis domain={[0, maxY]} stroke="#6B7280" tickMargin={8} allowDecimals={false} />
						<Tooltip cursor={{ strokeDasharray: "3 3" }} />
						{/* Legend is custom-rendered above; can keep built-in hidden */}
						{series.map((s) => (
							<Line
								key={s.key}
								type="monotone"
								dataKey={s.key}
								name={s.name}
								stroke={s.color}
								strokeWidth={3}
								dot={{ r: 3 }}
								activeDot={{ r: 5 }}
							/>
						))}
					</LineChart>
				</ResponsiveContainer>
			</div>
		</div>
	);
}

