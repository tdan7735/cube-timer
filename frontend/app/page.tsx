"use client";

import { useState, useEffect, useCallback } from "react";
import { Timer } from "../components/Timer";
import { Stats } from "../components/Stats";
import { SolveList } from "../components/SolveList";
import { getSolves, getStatistics, createSolve, deleteSolve } from "../lib/api";
import type { Solve, Statistics } from "../lib/types";
import type { Phase } from "../components/Timer";
import { Penalty } from "../lib/types";

const SCRAMBLE = "R U R' U' R' F R2 U' R' U' R U R' F'";

export default function Home() {
  const [solves, setSolves] = useState<Solve[]>([]);
  const [stats, setStats] = useState<Statistics | null>(null);
  const [phase, setPhase] = useState<Phase>("idle");

  const refresh = useCallback(async () => {
    const [s, st] = await Promise.all([getSolves(), getStatistics()]);
    setSolves(s);
    setStats(st);
  }, []);

  useEffect(() => {
    refresh();
  }, [refresh]);

  const handleSolve = async (timeMs: number, penalty: Penalty) => {
    const tempId = Date.now();
    const optimistic: Solve = {
      id: tempId,
      scramble: SCRAMBLE,
      penalty,
      solveTime: timeMs,
      timeSolved: new Date().toISOString(),
    };
    setSolves((prev) => [optimistic, ...prev]);

    try {
      const saved = await createSolve({
        scramble: SCRAMBLE,
        penalty,
        solveTime: timeMs,
      });
      setSolves((prev) =>
        prev.map((s) => (s.id === tempId ? saved : s))
      );
      refresh();
    } catch {
      setSolves((prev) => prev.filter((s) => s.id !== tempId));
    }
  };

  const handleDelete = async (id: number) => {
    setSolves((prev) => prev.filter((s) => s.id !== id));
    try {
      await deleteSolve(id);
      refresh();
    } catch {
      refresh();
    }
  };

  const timing = phase !== "idle";

  return (
    <div className="app">
      <div className="center">
        {!timing && <div className="scramble">{SCRAMBLE}</div>}
        <Timer onSolve={handleSolve} onPhaseChange={setPhase} />
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
