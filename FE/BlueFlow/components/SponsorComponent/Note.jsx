import React, { useEffect, useState } from "react";
import { Rnd } from "react-rnd";
const DEFAULT_NOTES = [
	{ id: "n1", title: "Note", text: "About", x: 20, y: 20, w: 135, h: 160, color: "#FEF9C3" },
	{ id: "n2", title: "Note", text: "Something", x: 280, y: 36, w: 135, h: 170, color: "#FEF9C3" },
];

export default function Note() {
	const [notes, setNotes] = useState(() => {
		try {
			const raw = localStorage.getItem("sticky.notes");
			return raw ? JSON.parse(raw) : DEFAULT_NOTES;
		} catch {
			return DEFAULT_NOTES;
		}
	});

	useEffect(() => {
		localStorage.setItem("sticky.notes", JSON.stringify(notes));
	}, [notes]);

	const addNote = () => {
		const id = crypto.randomUUID();
		setNotes((arr) => [
			...arr,
			{ id, title: "Note", text: "New note", x: 40, y: 60, w: 135, h: 170, color: "#FEF9C3" },
		]);
	};

	const removeNote = (id) => setNotes((arr) => arr.filter((n) => n.id !== id));
	const updateNote = (id, patch) =>
		setNotes((arr) => arr.map((n) => (n.id === id ? { ...n, ...patch } : n)));

	return (
		<div className="bg-white rounded-lg shadow-xl border border-gray-100 p-5 h-full">
			<div className="flex items-center justify-between mb-3">
				<h3 className="text-xl font-bold text-sky-500">Summary Note</h3>
				<button
					onClick={addNote}
					className="px-3 py-1.5 rounded-md bg-sky-500 text-white text-sm font-medium"
				>
					Add note
				</button>
			</div>

			{/* Canvas */}
			<div className="relative w-full min-h-[205px] bg-gray-50/60 rounded-xl border border-gray-100 overflow-hidden">
				{notes.map((n) => (
					<Rnd
						key={n.id}
						default={{ x: n.x, y: n.y, width: n.w, height: n.h }}
						bounds="parent"
						dragHandleClassName="drag-handle"
						onDragStop={(_, d) => updateNote(n.id, { x: d.x, y: d.y })}
						onResizeStop={(_, __, ref, ___, pos) =>
							updateNote(n.id, { w: ref.offsetWidth, h: ref.offsetHeight, x: pos.x, y: pos.y })
						}
						minWidth={100}
						minHeight={100}
					>
						<div
							className="h-full w-full rounded-xl shadow-sm border border-yellow-200"
							style={{ backgroundColor: n.color }}
						>
							{/* tape */}
							<div className="drag-handle cursor-move select-none h-8 flex items-center justify-between px-3">
								<div className="h-2 w-10 bg-pink-300 rounded-b-sm" />
								<button
									onClick={() => removeNote(n.id)}
									className="text-gray-500 hover:text-red-500 text-sm"
									title="Delete"
								>
									✕
								</button>
							</div>
							<div className="px-4 pb-4 text-left">
								<input
									className="w-full bg-transparent outline-none font-semibold text-gray-800"
									value={n.title}
									onChange={(e) => updateNote(n.id, { title: e.target.value })}
								/>
								<textarea
									className="mt-2 w-full bg-transparent outline-none text-gray-700 resize-none"
									rows={4}
									value={n.text}
									onChange={(e) => updateNote(n.id, { text: e.target.value })}
								/>
							</div>
						</div>
					</Rnd>
				))}
			</div>
		</div>
	);
}

