"use client";

import { useState, useEffect, useCallback } from "react";
import { Timer } from "../components/Timer";
import { Stats } from "../components/Stats";
import { SolveList } from "../components/SolveList";
import { getSolves, getStatistics, createSolve, deleteSolve, getScramble } from "../lib/api";
import type { Solve, Statistics } from "../lib/types";
import type { Phase } from "../components/Timer";
import { Penalty } from "../lib/types";

export default function Home() {
  const [solves, setSolves] = useState<Solve[]>([]);
  const [stats, setStats] = useState<Statistics | null>(null);
  const [phase, setPhase] = useState<Phase>("idle");
  const [scramble, setScramble] = useState("");
  const [loadingScramble, setLoadingScramble] = useState(true);
  const [scrambleError, setScrambleError] = useState("");
  const [solveError, setSolveError] = useState("");
  const [saving, setSaving] = useState(false);

  const loadScramble = useCallback(async () => {
    setLoadingScramble(true);
    setScrambleError("");
    setScramble("");
    try {
      setScramble(await getScramble());
    } catch {
      setScrambleError("Could not load a scramble. Please try again.");
    } finally {
      setLoadingScramble(false);
    }
  }, []);

  const refresh = useCallback(async () => {
    const [s, st] = await Promise.all([getSolves(), getStatistics()]);
    setSolves(s);
    setStats(st);
  }, []);

  useEffect(() => {
    void refresh().catch(() => setSolveError("Could not load solve history and statistics."));
    void loadScramble();
  }, [refresh, loadScramble]);

  const handleSolve = async (timeMs: number, penalty: Penalty) => {
    if (!scramble.trim() || saving || loadingScramble) return;
    // Capture the displayed scramble before requesting the next one.
    const solvedScramble = scramble;
    setSaving(true);
    setSolveError("");
    const tempId = Date.now();
    const optimistic: Solve = {
      id: tempId,
      scramble: solvedScramble,
      penalty,
      solveTime: timeMs,
      timeSolved: new Date().toISOString(),
    };
    setSolves((prev) => [optimistic, ...prev]);

    try {
      const saved = await createSolve({
        scramble: solvedScramble,
        penalty,
        solveTime: timeMs,
      });
      setSolves((prev) =>
        prev.map((s) => (s.id === tempId ? saved : s))
      );

    } catch {
      setSolves((prev) => prev.filter((s) => s.id !== tempId));
      setSolveError("Could not save your solve. The current scramble has been kept.");
      setSaving(false);
      return;
    }

    // A statistics or scramble failure must not undo a successfully saved solve.
    await Promise.all([
      getStatistics().then(setStats).catch(() => setSolveError("Solve saved, but statistics could not refresh.")),
      loadScramble(),
    ]);
    setSaving(false);
  };

  const handleDelete = async (id: number) => {
    setSolves((prev) => prev.filter((s) => s.id !== id));
    try {
      await deleteSolve(id);
      await refresh();
    } catch {
      await refresh();
    }
  };

  const timing = phase !== "idle";

  return (
    <div className="app">
      <div className="center">
        {!timing && <div className="scramble">
          {loadingScramble ? "" : scramble}
          {scrambleError && <div role="alert">{scrambleError} <button onClick={() => void loadScramble()}>Retry</button></div>}
        </div>}
        <Timer onSolve={handleSolve} onPhaseChange={setPhase}
          disabled={saving || loadingScramble || !scramble.trim()} />
        {!timing && solveError && <p role="alert">{solveError}</p>}
        {!timing && <Stats stats={stats} />}
      </div>
      {!timing && (
        <aside className="sidebar">
          <div className="sidebar-header">Solves</div>
          <SolveList solves={solves} onDelete={handleDelete} />
        </aside>
      )}
    </div>
  );
}
