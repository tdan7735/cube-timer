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
    <section className="px-4 pt-6 pb-8 [padding-inline:clamp(16px,3vw,48px)]" aria-label={`${name} trainer`}>
      <div className="mt-5 overflow-hidden rounded-lg border border-[#383838]">
        <div className="flex min-h-[92px] items-center justify-between gap-5 border-b border-[#383838] bg-cube-surface px-4 py-3">
          <button className="cursor-pointer border-0 bg-transparent px-2 text-3xl text-cube-text disabled:cursor-default disabled:opacity-40" aria-label="Previous scramble" disabled={busy || loading || index === 0} onClick={() => setIndex((i) => i - 1)}>‹</button>
          <div className="min-w-0 text-center"><span className="text-[10px] uppercase tracking-[1.5px] text-[#999]">Case scramble</span><p className="mt-1.5 font-mono text-[clamp(16px,1.6vw,24px)] leading-relaxed wrap-anywhere select-text">{loading ? "" : selected.length ? current?.scramble : "Select cases to begin"}</p></div>
          <button className="cursor-pointer border-0 bg-transparent px-2 text-3xl text-cube-text disabled:cursor-default disabled:opacity-40" aria-label="Next scramble" disabled={busy || loading || !selected.length} onClick={() => index < history.length - 1 ? setIndex(index + 1) : void nextScramble()}>›</button>
        </div>
        {error && <p role="alert" className="p-4 text-[13px] leading-relaxed text-[#999]">{error} <button className="cursor-pointer rounded border border-[#444] bg-transparent px-3 py-2 text-cube-text hover:border-cube-green" onClick={() => available.length ? void nextScramble(true) : setRetry((n) => n + 1)}>Retry</button></p>}
        <p className="p-4 text-[13px] leading-relaxed text-[#999]">Only cases with saved algorithms are available. Practice times stay in this tab.</p>
        <div className="grid min-h-[min(650px,70vh)] grid-cols-1 lg:grid-cols-[minmax(0,1fr)_230px_230px]">
          <div className="relative flex min-w-0 flex-col items-center border-b border-[#383838] p-5 lg:col-span-1 lg:border-b-0">
            <button className="cursor-pointer border border-transparent bg-transparent px-3 py-2 text-sm text-cube-green disabled:cursor-default disabled:opacity-40" disabled={busy} aria-expanded={selecting} aria-controls="trainer-case-selection"
              onClick={() => setSelecting(!selecting)}>{selected.length} cases selected <span className="text-[#999]">· Choose cases</span></button>
            {selecting && <div className="mt-3 w-full rounded-md border border-[#444] bg-cube-surface p-4" id="trainer-case-selection">
              <div className="mb-4 flex flex-wrap gap-2">
                <button className="cursor-pointer rounded border border-[#444] bg-transparent px-3 py-2 text-cube-text hover:border-cube-green" onClick={() => { setSelected(available.map((c) => c.name)); setIndex(0); }}>Select all</button>
                <button className="cursor-pointer rounded border border-[#444] bg-transparent px-3 py-2 text-cube-text hover:border-cube-green" onClick={() => { setSelected([]); setIndex(0); }}>Clear</button>
                <button className="cursor-pointer rounded border border-[#444] bg-transparent px-3 py-2 text-cube-text hover:border-cube-green" onClick={() => setSelecting(false)}>Done</button>
              </div>
              <div className="grid max-h-[230px] grid-cols-[repeat(auto-fit,minmax(86px,1fr))] gap-3 overflow-auto">{cases.map((item) => (
                <label className="flex cursor-pointer items-center gap-2 text-[13px]" key={item}><input className="accent-cube-green" type="checkbox" disabled={!available.some((c) => c.name === item)} checked={selected.includes(item)} onChange={() => {
                  setSelected((previous) => previous.includes(item) ? previous.filter((c) => c !== item) : [...previous, item]);
                  setIndex(0);
                }} />{item}</label>
              ))}</div>
            </div>}
            <div className="flex min-h-[320px] flex-1 flex-col items-center justify-center gap-6">
              <Timer disabled={selecting || loading || !!error || !current || !selected.includes(current.caseName)} onPhaseChange={setPhase} onSolve={(time, penalty) => {
                if (!current) return;
                setAttempts((previous) => [{ id: Date.now(), caseName: current.caseName, scramble: current.scramble, time, penalty }, ...previous]);
                void nextScramble();
              }} />
              {!busy && <p className="text-[13px] text-[#999]">Practice case · {currentCase ?? "None selected"}</p>}
            </div>
          </div>
          <aside className="border-l border-[#383838] bg-[#151515] px-[18px] py-6" aria-label="Practice attempts">
            <h2 className="text-sm font-medium">Attempts <span className="float-right text-[#999]">{attempts.length}</span></h2>
            {!attempts.length ? <p className="mt-8 text-center text-[13px] leading-relaxed text-[#888]">Your practice times will appear here.</p> : <ol className="mt-5 max-h-[510px] list-none overflow-y-auto">
              {attempts.map((attempt) => <li className="flex items-center gap-2 border-b border-cube-border py-2.5 text-[13px]" key={attempt.id}>
                <span className="flex-1 text-[#aaa]">{attempt.caseName}</span>
                <strong className="font-mono font-normal">{attempt.penalty === Penalty.DNF ? "DNF" : formatTime(attempt.time)}</strong>
                <button className="cursor-pointer border-0 bg-transparent px-1.5 py-0.5 text-cube-text disabled:cursor-default disabled:opacity-40" aria-label={`Delete ${attempt.caseName} attempt`} disabled={busy}
                  onClick={() => setAttempts((previous) => previous.filter((a) => a.id !== attempt.id))}>×</button>
              </li>)}
            </ol>}
          </aside>
          <aside className="border-l border-[#383838] bg-[#151515] px-[18px] py-6" aria-label="Training statistics">
            <div className="flex items-center justify-between"><h2 className="text-sm font-medium">Session</h2><button className="cursor-pointer rounded border border-[#444] bg-transparent px-3 py-2 text-cube-text disabled:cursor-default disabled:opacity-40" disabled={busy || !attempts.length} onClick={() => setAttempts([])} aria-label="Reset practice times">↻</button></div>
            <div className="my-7 grid grid-cols-2 gap-3"><div><span className="mb-2 block text-xs text-[#999]">Current</span><strong className="font-mono text-2xl font-normal">{latest ? latest.penalty === Penalty.DNF ? "DNF" : formatTime(latest.time) : "—"}</strong></div><div><span className="mb-2 block text-xs text-[#999]">Best</span><strong className="font-mono text-2xl font-normal text-cube-green">{display(best)}</strong></div></div>
            <dl className="mb-5"><div className="flex justify-between border-b border-cube-border py-2.5 text-sm"><dt className="text-[#aaa]">Solves</dt><dd className="font-mono">{attempts.length}</dd></div><div className="flex justify-between border-b border-cube-border py-2.5 text-sm"><dt className="text-[#aaa]">Mean</dt><dd className="font-mono">{display(mean)}</dd></div><div className="flex justify-between border-b border-cube-border py-2.5 text-sm"><dt className="text-[#aaa]">DNFs</dt><dd className="font-mono">{attempts.length - valid.length}</dd></div></dl>
            <p className="text-[13px] leading-relaxed text-[#999]">Mean and best exclude DNFs.</p>
            <div className="mt-10 text-xs leading-loose text-[#888]"><p>Hold space to get ready</p><p>Release to start</p><p>Space to stop · Esc for DNF</p></div>
          </aside>
        </div>
      </div>
    </section>
  );
}
