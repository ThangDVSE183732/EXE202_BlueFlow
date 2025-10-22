import React, { useMemo } from "react";
import { ResponsiveContainer, PieChart, Pie, Cell, Tooltip } from "recharts";

// Donut chart with center total and labeled slices
export default function PieChartBox({
	title = "",
	chartWidth, // CSS width or number (px). Default 100%
	chartHeight = 143, // CSS height or number (px)
	maxChartWidth = 240, // cap chart width inside the card to avoid overflow
	showSliceLabels = false,
	data = [
		{ name: "Events", value: 3, color: "#8B5CF6" }, // violet-500
		{ name: "New Partners", value: 10, color: "#FB7185" }, // rose-400
		{ name: "Pending", value: 7, color: "#14B8A6" }, // teal-500
	],
}) {
	const total = useMemo(() => data.reduce((s, d) => s + (Number(d.value) || 0), 0), [data]);

	const RADIAN = Math.PI / 180;
	const renderLabel = (props) => {
		const { cx, cy, midAngle, outerRadius, name, value, fill } = props;
		const r = outerRadius + 18;
		const x = cx + r * Math.cos(-midAngle * RADIAN);
		const y = cy + r * Math.sin(-midAngle * RADIAN);
		const anchor = x > cx ? "start" : "end";
		return (
			<g>
				<text x={x} y={y} textAnchor={anchor} dominantBaseline="central" fill="#6B7280" fontSize={6}>
					{name}
				</text>
				<text x={x} y={y + 14} textAnchor={anchor} dominantBaseline="central" fill={fill} fontSize={6} fontWeight={600}>
					{value}
				</text>
			</g>
		);
	};

	return (
		<div className="bg-white rounded-lg shadow-xl border border-gray-100 p-5 w-full max-w-full overflow-hidden min-w-0">
			{title ? (
				<h3 className="text-2xl font-extrabold text-center mb-2 text-gray-900">{title}</h3>
			) : null}
			<div className="flex items-center justify-center gap-4 sm:gap-6 flex-wrap min-w-0">
				{/* Donut chart */}
				<div
					className="relative min-w-0"
					style={{ width: chartWidth ?? "100%", maxWidth: maxChartWidth, height: chartHeight }}
				>
					{(() => {
						const h = Number(chartHeight) || 200;
						const outerR = Math.max(50, Math.min(110, Math.floor(h / 2) - 10));
						const innerR = Math.max(24, outerR - 36);
						const centerSize = Math.max(28, Math.min(innerR * 2 - 4, 96));
						const fontSize = Math.max(16, Math.min(28, Math.floor(centerSize / 2.8)));
						return (
							<>
								<ResponsiveContainer width="100%" height="100%">
									<PieChart>
										<Tooltip formatter={(v, n) => [v, n]} />
										<Pie
											data={data}
											dataKey="value"
											nameKey="name"
											innerRadius={innerR}
											outerRadius={outerR}
											paddingAngle={2}
											labelLine={showSliceLabels}
											label={showSliceLabels ? renderLabel : false}
										>
											{data.map((entry, idx) => (
												<Cell key={`c-${idx}`} fill={entry.color} />
											))}
										</Pie>
									</PieChart>
								</ResponsiveContainer>

								{/* Center total */}
								<div className="absolute inset-0 flex items-center justify-center pointer-events-none">
									<div
										className="bg-white rounded-full shadow-sm flex items-center justify-center"
										style={{ width: centerSize, height: centerSize }}
									>
										<span className="font-semibold text-gray-900" style={{ fontSize }}>
											{total}
										</span>
									</div>
								</div>
							</>
						);
					})()}
				</div>

				{/* Legend on the right */}
				<div className="hidden sm:flex flex-col gap-1 pr-4 min-w-0">
					{data.map((d) => (
						<div key={d.name} className="flex items-center gap-2 text-gray-700">
							<span className="h-3 w-3 rounded-full" style={{ backgroundColor: d.color }} />
							<span>{d.name}</span>
							<span className="ml-2 font-semibold" style={{ color: d.color }}>
								{d.value}
							</span>
						</div>
					))}
				</div>
			</div>
		</div>
	);
}

