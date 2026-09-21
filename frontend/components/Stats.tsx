"use client";

import type { Statistics } from "../lib/types";
import { formatTime } from "../lib/format";

interface StatsProps {
  stats: Statistics | null;
}

export function Stats({ stats }: StatsProps) {
  if (!stats) return null;

  const items = [
    { label: "PB", value: stats.personalBest },
    { label: "Ao5", value: stats.ao5 },
    { label: "Ao12", value: stats.ao12 },
    { label: "Avg", value: stats.totalAverage },
  ];

  return (
    <div className="flex gap-8 animate-fade-in">
      {items.map(({ label, value }) => (
        <div key={label} className="flex flex-col items-center gap-0.5">
          <span className="text-[11px] uppercase tracking-[1px] text-cube-dim">{label}</span>
          <span className="font-mono text-lg font-normal">
            {value !== null && value >= 0 ? formatTime(value) : "—"}
          </span>
        </div>
      ))}
    </div>
  );
}
