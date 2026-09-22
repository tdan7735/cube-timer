"use client";

import { useState, useEffect, useCallback } from "react";
import { Timer } from "../components/Timer";
import { Stats } from "../components/Stats";
import { SolveList } from "../components/SolveList";
import { SessionManager } from "../components/SessionManager";
import { SolveEditor } from "../components/SolveEditor";
import { getSolves, getStatistics, createSession, createSolve, deleteSession, deleteSolve, getScramble, getSessions, updateSession, updateSolve } from "../lib/api";
import type { Session, Solve, Statistics } from "../lib/types";
import type { Phase } from "../components/Timer";
import { Penalty, SessionType } from "../lib/types";

const ACTIVE_SESSION_KEY = "cube-timer.active-session-id";

export default function Home() {
  const [solves, setSolves] = useState<Solve[]>([]);
  const [stats, setStats] = useState<Statistics | null>(null);
  const [sessions, setSessions] = useState<Session[]>([]);
  const [activeSessionId, setActiveSessionId] = useState<number | null>(null);
  const [loadingSessions, setLoadingSessions] = useState(true);
  const [phase, setPhase] = useState<Phase>("idle");
  const [scramble, setScramble] = useState("");
  const [loadingScramble, setLoadingScramble] = useState(true);
  const [scrambleError, setScrambleError] = useState("");
  const [solveError, setSolveError] = useState("");
  const [saving, setSaving] = useState(false);
  const [editingSolve, setEditingSolve] = useState<Solve | null>(null);

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

  const refresh = useCallback(async (sessionId: number) => {
    const [s, st] = await Promise.all([getSolves(sessionId), getStatistics(sessionId)]);
    setSolves(s);
    setStats(st);
  }, []);

  useEffect(() => {
    const loadSessions = async () => {
      setLoadingSessions(true);
      setSolveError("");
      try {
        const loadedSessions = await getSessions();
        const savedSessionId = Number(window.localStorage.getItem(ACTIVE_SESSION_KEY));
        let activeSession = loadedSessions.find((session) => session.id === savedSessionId)
          ?? loadedSessions[0];

        if (!activeSession) {
          activeSession = await createSession({ type: SessionType.Solves });
          loadedSessions.push(activeSession);
        }

        setSessions(loadedSessions);
        setActiveSessionId(activeSession.id);
        window.localStorage.setItem(ACTIVE_SESSION_KEY, activeSession.id.toString());
        await refresh(activeSession.id);
      } catch {
        setSolveError("Could not load your sessions, solve history, and statistics.");
      } finally {
        setLoadingSessions(false);
      }
    };

    void loadSessions();
    void loadScramble();
  }, [refresh, loadScramble]);

  const handleSessionChange = async (sessionId: number) => {
    if (sessionId === activeSessionId) return;

    setActiveSessionId(sessionId);
    window.localStorage.setItem(ACTIVE_SESSION_KEY, sessionId.toString());
    setSolves([]);
    setStats(null);
    setSolveError("");

    try {
      await refresh(sessionId);
    } catch {
      setSolveError("Could not load solve history and statistics.");
    }
  };

  const handleCreateSession = async (name: string) => {
    const session = await createSession({ name, type: SessionType.Solves });
    setSessions((current) => [...current, session]);
    setActiveSessionId(session.id);
    window.localStorage.setItem(ACTIVE_SESSION_KEY, session.id.toString());
    setSolves([]);
    setStats(null);
    await refresh(session.id);
  };

  const handleUpdateSession = async (id: number, name: string) => {
    const existingSession = sessions.find((session) => session.id === id);
    if (!existingSession) return;

    const session = await updateSession(id, { name, type: existingSession.type });
    setSessions((current) => current.map((item) => item.id === id ? session : item));
  };

  const handleDeleteSession = async (id: number) => {
    await deleteSession(id);
    let remainingSessions = sessions.filter((session) => session.id !== id);

    if (remainingSessions.length === 0) {
      const session = await createSession({ type: SessionType.Solves });
      remainingSessions = [session];
    }

    setSessions(remainingSessions);
    if (id === activeSessionId) {
      const nextSession = remainingSessions[0];
      setActiveSessionId(nextSession.id);
      window.localStorage.setItem(ACTIVE_SESSION_KEY, nextSession.id.toString());
      setSolves([]);
      setStats(null);
      await refresh(nextSession.id);
    }
  };

  const handleSolve = async (timeMs: number, penalty: Penalty) => {
    if (!scramble.trim() || saving || loadingScramble || activeSessionId === null) return;
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
        sessionId: activeSessionId,
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
      getStatistics(activeSessionId).then(setStats).catch(() => setSolveError("Solve saved, but statistics could not refresh.")),
      loadScramble(),
    ]);
    setSaving(false);
  };

  const handleDelete = async (id: number) => {
    setSolves((prev) => prev.filter((s) => s.id !== id));
    try {
      await deleteSolve(id);
      if (activeSessionId !== null) await refresh(activeSessionId);
    } catch {
      if (activeSessionId !== null) await refresh(activeSessionId);
    }
  };

  const handleEditSolve = async (scramble: string, penalty: Penalty, solveTime: number) => {
    if (!editingSolve || activeSessionId === null) return;
    await updateSolve(editingSolve.id, { scramble, penalty, solveTime, sessionId: activeSessionId });
    await refresh(activeSessionId);
    setEditingSolve(null);
  };

  const timing = phase !== "idle";

  return (
    <div className="flex h-full">
      <div className="relative flex flex-1 flex-col items-center justify-center gap-8">
        {!timing && <div className="absolute top-8 right-0 left-0 animate-fade-in text-center font-mono text-2xl leading-normal tracking-[1.5px] text-cube-text">
          {loadingScramble ? "" : scramble}
          {scrambleError && <div role="alert">{scrambleError} <button onClick={() => void loadScramble()}>Retry</button></div>}
        </div>}
        <Timer onSolve={handleSolve} onPhaseChange={setPhase}
          disabled={saving || loadingScramble || loadingSessions || activeSessionId === null || !scramble.trim()} />
        {!timing && solveError && <p role="alert">{solveError}</p>}
        {!timing && <Stats stats={stats} />}
      </div>
      {!timing && (
        <aside className="order-first flex w-[280px] flex-col overflow-hidden border-r border-cube-border animate-fade-in">
          <SessionManager
            sessions={sessions}
            activeSessionId={activeSessionId}
            disabled={loadingSessions || saving}
            onSelect={handleSessionChange}
            onCreate={handleCreateSession}
            onUpdate={handleUpdateSession}
            onDelete={handleDeleteSession}
          />
          <SolveList solves={solves} onDelete={handleDelete} onEdit={setEditingSolve} />
        </aside>
      )}
      {editingSolve && <SolveEditor solve={editingSolve} onSave={handleEditSolve} onCancel={() => setEditingSolve(null)} />}
    </div>
  );
}
