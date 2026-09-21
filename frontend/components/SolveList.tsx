"use client";

import { type Solve, Penalty, finalTime } from "../lib/types";
import { formatTime } from "../lib/format";

interface SolveListProps {
  solves: Solve[];
  onDelete: (id: number) => void;
}

export function SolveList({ solves, onDelete }: SolveListProps) {
  if (solves.length === 0) {
    return <p className="p-5 text-center text-sm text-cube-dim">No solves yet</p>;
  }

  return (
    <div className="flex w-full flex-1 flex-col overflow-y-auto [&::-webkit-scrollbar]:w-1 [&::-webkit-scrollbar-thumb]:rounded-sm [&::-webkit-scrollbar-thumb]:bg-cube-border">
      {solves.map((s) => (
        <div key={s.id} className="group flex items-center gap-2 border-b border-cube-border bg-cube-surface px-5 py-2 font-mono text-sm last:border-b-0">
          <span className="flex-1">{formatTime(finalTime(s))}</span>
          {s.penalty === Penalty.Plus2 && (
            <span className="rounded-[3px] bg-cube-red/12 px-[5px] py-px text-[11px] font-semibold text-cube-red">+2</span>
          )}
          {s.penalty === Penalty.DNF && (
            <span className="rounded-[3px] bg-cube-red/12 px-[5px] py-px text-[11px] font-semibold text-cube-red">DNF</span>
          )}
          <button
            className="px-1 text-base leading-none text-cube-dim opacity-0 transition-[opacity,color] duration-150 group-hover:opacity-100 hover:text-cube-red"
            onClick={() => onDelete(s.id)}
            title="Delete"
          >
            ×
          </button>
        </div>
      ))}
    </div>
  );
}
