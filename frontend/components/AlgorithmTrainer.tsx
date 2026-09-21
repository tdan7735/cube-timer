"use client";

import { useState } from "react";
import { Timer, type Phase } from "./Timer";
import { Penalty } from "../lib/types";
import { formatTime } from "../lib/format";

const pllCases = ["Aa", "Ab", "E", "F", "Ga", "Gb", "Gc", "Gd", "H", "Ja", "Jb", "Na", "Nb", "Ra", "Rb", "T", "Ua", "Ub", "V", "Y", "Z"];
// Display-only sequences. These are not case-generating scrambles.
const examples = ["R U2 R' U' R U' R'", "F R U R' U' F'", "R U R' U R U2 R'"];
type Attempt = { id: number; caseName: string; time: number; penalty: Penalty };

export function AlgorithmTrainer({ name }: { name: string }) {
  const cases = name === "OLL" ? Array.from({ length: 57 }, (_, i) => `OLL ${i + 1}`) : pllCases;
  const [selected, setSelected] = useState(cases);
  const [selecting, setSelecting] = useState(false);
  const [index, setIndex] = useState(0);
  const [attempts, setAttempts] = useState<Attempt[]>([]);
  const [phase, setPhase] = useState<Phase>("idle");
  const busy = phase !== "idle";
  const currentCase = selected[index % selected.length];
  const valid = attempts.filter((a) => a.penalty !== Penalty.DNF);
  const best = valid.length ? Math.min(...valid.map((a) => a.time)) : null;
  const mean = valid.length ? valid.reduce((sum, a) => sum + a.time, 0) / valid.length : null;
  const latest = attempts[0];
  const display = (value: number | null) => value === null ? "—" : formatTime(value);

  return (
    <section className="trainer-page" aria-label={`${name} trainer`}>
      <div className="trainer-frame">
        <div className="trainer-scramble-strip">
          <button aria-label="Previous example" disabled={busy || !selected.length} onClick={() => setIndex((i) => (i - 1 + selected.length) % selected.length)}>‹</button>
          <div><span className="trainer-eyebrow">Example scramble</span><p>{selected.length ? examples[index % examples.length] : "Select cases to begin"}</p></div>
          <button aria-label="Next example" disabled={busy || !selected.length} onClick={() => setIndex((i) => i + 1)}>›</button>
        </div>
        <div className="trainer-columns">
          <div className="trainer-stage">
            <button className="trainer-selection" disabled={busy} aria-expanded={selecting} aria-controls="trainer-case-selection"
              onClick={() => setSelecting(!selecting)}>{selected.length} cases selected <span>· Choose cases</span></button>
            {selecting && <div className="trainer-case-selection" id="trainer-case-selection">
              <div className="trainer-selection-actions">
                <button onClick={() => { setSelected(cases); setIndex(0); }}>Select all</button>
                <button onClick={() => { setSelected([]); setIndex(0); }}>Clear</button>
                <button onClick={() => setSelecting(false)}>Done</button>
              </div>
              <div className="trainer-case-grid">{cases.map((item) => (
                <label key={item}><input type="checkbox" checked={selected.includes(item)} onChange={() => {
                  setSelected((previous) => previous.includes(item) ? previous.filter((c) => c !== item) : [...previous, item]);
                  setIndex(0);
                }} />{item}</label>
              ))}</div>
            </div>}
            <div className="trainer-clock">
              <Timer disabled={selecting || !selected.length} onPhaseChange={setPhase} onSolve={(time, penalty) => {
                setAttempts((previous) => [{ id: Date.now(), caseName: currentCase, time, penalty }, ...previous]);
                setIndex((i) => i + 1);
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
