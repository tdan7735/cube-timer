"use client";

import { type Solve, Penalty, finalTime } from "../lib/types";
import { formatTime } from "../lib/format";

interface SolveListProps {
  solves: Solve[];
  onDelete: (id: number) => void;
}

export function SolveList({ solves, onDelete }: SolveListProps) {
  if (solves.length === 0) {
    return <p className="empty">No solves yet</p>;
  }

  return (
    <div className="solve-list">
      {solves.map((s) => (
        <div key={s.id} className="solve-row">
          <span className="solve-time">{formatTime(finalTime(s))}</span>
          {s.penalty === Penalty.Plus2 && (
            <span className="solve-penalty plus2">+2</span>
          )}
          {s.penalty === Penalty.DNF && (
            <span className="solve-penalty dnf">DNF</span>
          )}
          <button
            className="solve-delete"
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
