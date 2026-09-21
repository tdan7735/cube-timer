"use client";

import { useState, useEffect, useRef, useCallback } from "react";
import { getAlgorithmSet } from "../lib/api";
import { generateCaseScramble, type CaseScramble } from "../lib/caseScramble";
import type { AlgorithmCase } from "../lib/types";
import { Timer, type Phase } from "./Timer";
import { Penalty } from "../lib/types";
import { formatTime } from "../lib/format";

const pllCases = ["Aa", "Ab", "E", "F", "Ga", "Gb", "Gc", "Gd", "H", "Ja", "Jb", "Na", "Nb", "Ra", "Rb", "T", "Ua", "Ub", "V", "Y", "Z"];
type Attempt = { id: number; caseName: string; scramble: string; time: number; penalty: Penalty };

export function AlgorithmTrainer({ name }: { name: string }) {
  const cases = name === "OLL" ? Array.from({ length: 57 }, (_, i) => `OLL ${i + 1}`) : pllCases;
  const [selected, setSelected] = useState(cases);
  const [selecting, setSelecting] = useState(false);
  const [index, setIndex] = useState(0);
  const [available, setAvailable] = useState<AlgorithmCase[]>([]);
  const [history, setHistory] = useState<CaseScramble[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [retry, setRetry] = useState(0);
  const requestId = useRef(0);
  const current = history[index];

  useEffect(() => {
    const controller = new AbortController();
    setLoading(true);
    setError("");
    getAlgorithmSet(name, controller.signal).then((set) => {
      const byId = new Map([...set.cases, ...set.groups.flatMap((g) => g.cases)].map((c) => [c.id, c]));
      const usable = [...byId.values()].filter((c) => c.algorithms.some((a) => a.moves.trim()));
      setAvailable(usable);
      setSelected(usable.map((c) => c.name));
      setLoading(false);
    }).catch(() => {
      if (!controller.signal.aborted) { setError("Could not load trainer cases."); setLoading(false); }
    });
    return () => { controller.abort(); requestId.current++; };
  }, [name, retry]);

  const nextScramble = useCallback(async (reset = false) => {
    const id = ++requestId.current;
    const pool = available.filter((c) => selected.includes(c.name));
    if (reset) { setHistory([]); setIndex(0); }
    setError("");
    if (!pool.length) { setLoading(false); return; }
    setLoading(true);
    try {
      const generated = await generateCaseScramble(pool);
      if (id !== requestId.current) return;
      setHistory((previous) => [...(reset ? [] : previous.slice(0, index + 1)), generated]);
      setIndex(reset ? 0 : index + 1);
    } catch {
      if (id === requestId.current) setError("Could not generate a case scramble. Check the case algorithm or retry.");
    } finally {
      if (id === requestId.current) setLoading(false);
    }
  }, [available, selected, index]);

  useEffect(() => {
    // Changing the selection invalidates the old scramble history.
    void nextScramble(true);
    return () => { requestId.current++; };
    // Index changes navigate history and must not reset it.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [available, selected]);
  const [attempts, setAttempts] = useState<Attempt[]>([]);
  const [phase, setPhase] = useState<Phase>("idle");
  const busy = phase !== "idle";
  const currentCase = current?.caseName;
  const valid = attempts.filter((a) => a.penalty !== Penalty.DNF);
  const best = valid.length ? Math.min(...valid.map((a) => a.time)) : null;
  const mean = valid.length ? valid.reduce((sum, a) => sum + a.time, 0) / valid.length : null;
  const latest = attempts[0];
  const display = (value: number | null) => value === null ? "—" : formatTime(value);

  return (
    <section className="trainer-page" aria-label={`${name} trainer`}>
      <div className="trainer-frame">
        <div className="trainer-scramble-strip">
          <button aria-label="Previous scramble" disabled={busy || loading || index === 0} onClick={() => setIndex((i) => i - 1)}>‹</button>
          <div><span className="trainer-eyebrow">Case scramble</span><p>{loading ? "" : selected.length ? current?.scramble : "Select cases to begin"}</p></div>
          <button aria-label="Next scramble" disabled={busy || loading || !selected.length} onClick={() => index < history.length - 1 ? setIndex(index + 1) : void nextScramble()}>›</button>
        </div>
        {error && <p role="alert" className="trainer-note">{error} <button onClick={() => available.length ? void nextScramble(true) : setRetry((n) => n + 1)}>Retry</button></p>}
        <p className="trainer-note">Only cases with saved algorithms are available. Practice times stay in this tab.</p>
        <div className="trainer-columns">
          <div className="trainer-stage">
            <button className="trainer-selection" disabled={busy} aria-expanded={selecting} aria-controls="trainer-case-selection"
              onClick={() => setSelecting(!selecting)}>{selected.length} cases selected <span>· Choose cases</span></button>
            {selecting && <div className="trainer-case-selection" id="trainer-case-selection">
              <div className="trainer-selection-actions">
                <button onClick={() => { setSelected(available.map((c) => c.name)); setIndex(0); }}>Select all</button>
                <button onClick={() => { setSelected([]); setIndex(0); }}>Clear</button>
                <button onClick={() => setSelecting(false)}>Done</button>
              </div>
              <div className="trainer-case-grid">{cases.map((item) => (
                <label key={item}><input type="checkbox" disabled={!available.some((c) => c.name === item)} checked={selected.includes(item)} onChange={() => {
                  setSelected((previous) => previous.includes(item) ? previous.filter((c) => c !== item) : [...previous, item]);
                  setIndex(0);
                }} />{item}</label>
              ))}</div>
            </div>}
            <div className="trainer-clock">
              <Timer disabled={selecting || loading || !!error || !current || !selected.includes(current.caseName)} onPhaseChange={setPhase} onSolve={(time, penalty) => {
                if (!current) return;
                setAttempts((previous) => [{ id: Date.now(), caseName: current.caseName, scramble: current.scramble, time, penalty }, ...previous]);
                void nextScramble();
              }} />
              {!busy && <p className="trainer-current">Practice case · {currentCase ?? "None selected"}</p>}
            </div>
          </div>
          <aside className="trainer-history" aria-label="Practice attempts">
            <h2>Attempts <span>{attempts.length}</span></h2>
            {!attempts.length ? <p className="trainer-empty">Your practice times will appear here.</p> : <ol>
              {attempts.map((attempt) => <li key={attempt.id}>
                <span>{attempt.caseName}</span>
                <strong>{attempt.penalty === Penalty.DNF ? "DNF" : formatTime(attempt.time)}</strong>
                <button aria-label={`Delete ${attempt.caseName} attempt`} disabled={busy}
                  onClick={() => setAttempts((previous) => previous.filter((a) => a.id !== attempt.id))}>×</button>
              </li>)}
            </ol>}
          </aside>
          <aside className="trainer-summary" aria-label="Training statistics">
            <div className="trainer-summary-heading"><h2>Session</h2><button disabled={busy || !attempts.length} onClick={() => setAttempts([])} aria-label="Reset practice times">↻</button></div>
            <div className="trainer-times"><div><span>Current</span><strong>{latest ? latest.penalty === Penalty.DNF ? "DNF" : formatTime(latest.time) : "—"}</strong></div><div><span>Best</span><strong>{display(best)}</strong></div></div>
            <dl><div><dt>Solves</dt><dd>{attempts.length}</dd></div><div><dt>Mean</dt><dd>{display(mean)}</dd></div><div><dt>DNFs</dt><dd>{attempts.length - valid.length}</dd></div></dl>
            <p className="trainer-note">Mean and best exclude DNFs.</p>
            <div className="trainer-shortcuts"><p>Hold space to get ready</p><p>Release to start</p><p>Space to stop · Esc for DNF</p></div>
          </aside>
        </div>
      </div>
    </section>
  );
}
