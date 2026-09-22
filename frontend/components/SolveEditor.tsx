"use client";

import { useState } from "react";
import { type Penalty, Penalty as Penalties, type Solve } from "../lib/types";

interface SolveEditorProps {
  solve: Solve;
  onSave: (scramble: string, penalty: Penalty, solveTime: number) => Promise<void>;
  onCancel: () => void;
}

export function SolveEditor({ solve, onSave, onCancel }: SolveEditorProps) {
  const [scramble, setScramble] = useState(solve.scramble);
  const [solveTime, setSolveTime] = useState((solve.solveTime / 1000).toString());
  const [penalty, setPenalty] = useState<Penalty>(solve.penalty);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  const save = async () => {
    const seconds = Number(solveTime);
    if (!scramble.trim() || !Number.isFinite(seconds) || seconds < 0) {
      setError("Enter a scramble and a non-negative time in seconds.");
      return;
    }

    setSaving(true);
    setError("");
    try {
      await onSave(scramble, penalty, Math.round(seconds * 1000));
    } catch {
      setError("Could not save this solve.");
      setSaving(false);
    }
  };

  return (
    <div className="fixed inset-0 z-10 flex items-center justify-center bg-black/60 p-4">
      <div className="w-full max-w-md border border-cube-border bg-cube-surface p-5 shadow-xl">
        <h2 className="mb-4 text-lg font-semibold">Edit solve</h2>
        <div className="flex flex-col gap-3">
          <label className="text-sm text-cube-dim">Time (seconds)<input className="mt-1 w-full border border-cube-border bg-cube-bg px-2 py-1 text-cube-text outline-none focus:border-cube-green" type="number" min="0" step="0.001" value={solveTime} onChange={(event) => setSolveTime(event.target.value)} /></label>
          <label className="text-sm text-cube-dim">Penalty<select className="mt-1 w-full border border-cube-border bg-cube-bg px-2 py-1 text-cube-text outline-none focus:border-cube-green" value={penalty} onChange={(event) => setPenalty(Number(event.target.value) as Penalty)}><option value={Penalties.None}>None</option><option value={Penalties.Plus2}>+2</option><option value={Penalties.DNF}>DNF</option></select></label>
          <label className="text-sm text-cube-dim">Scramble<input className="mt-1 w-full border border-cube-border bg-cube-bg px-2 py-1 text-cube-text outline-none focus:border-cube-green" value={scramble} onChange={(event) => setScramble(event.target.value)} /></label>
          {error && <p className="text-sm text-cube-red" role="alert">{error}</p>}
          <div className="flex justify-end gap-3 text-sm"><button className="text-cube-dim hover:text-cube-text" disabled={saving} onClick={onCancel}>Cancel</button><button className="text-cube-green hover:underline" disabled={saving} onClick={() => void save()}>Save</button></div>
        </div>
      </div>
    </div>
  );
}
