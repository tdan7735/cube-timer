"use client";

import Link from "next/link";
import { PllDiagram } from "./PllDiagram";
import { OllDiagram } from "./OllDiagram";
import { useEffect, useState } from "react";
import { getAlgorithmSet, setStandardAlgorithm } from "../lib/api";
import type { AlgorithmCase, AlgorithmSet } from "../lib/types";

function AlgorithmPicker({ item }: { item: AlgorithmCase }) {
  const algorithms = [...item.algorithms].sort((a, b) => a.id - b.id);
  const initial = algorithms.find((a) => a.isStandard) ?? algorithms[0];
  const [selectedId, setSelectedId] = useState(initial.id);
  const [draftId, setDraftId] = useState(initial.id);
  const [editing, setEditing] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [status, setStatus] = useState("");

  return (
    <div className="algorithm-entry">
      {editing ? (
        <form onSubmit={async (event) => {
          event.preventDefault();
          if (saving) return;
          setSaving(true);
          setError("");
          try {
            const saved = await setStandardAlgorithm(draftId);
            setSelectedId(saved.id);
            setEditing(false);
            setStatus("Standard algorithm saved.");
          } catch {
            setError("Could not save your changes. Please try again.");
          } finally {
            setSaving(false);
          }
        }}>
          <fieldset className="algorithm-options" disabled={saving}>
            <legend>Choose the standard algorithm for {item.name}</legend>
            {algorithms.map((algorithm) => (
              <label className="algorithm-option" key={algorithm.id}>
                <input type="radio" name={`standard-${item.id}`} value={algorithm.id}
                  checked={draftId === algorithm.id} onChange={() => setDraftId(algorithm.id)} />
                <span>{algorithm.moves}</span>
                {algorithm.id === selectedId && <span className="algorithm-edit-label">Standard</span>}
              </label>
            ))}
          </fieldset>
          <div className="algorithm-actions">
            <button type="submit" disabled={saving}>{saving ? "Saving…" : "Save standard"}</button>
            <button type="button" disabled={saving} onClick={() => { setEditing(false); setError(""); }}>Cancel</button>
          </div>
          {error && <p role="alert">{error}</p>}
        </form>
      ) : (
        <button className="algorithm-moves" onClick={() => {
          setDraftId(selectedId);
          setStatus("");
          setEditing(true);
        }} aria-label={`Choose standard algorithm for ${item.name}`}>
          <span>{algorithms.find((a) => a.id === selectedId)?.moves}</span>
          <span className="algorithm-edit-label">Change standard</span>
        </button>
      )}
      <span role="status">{status}</span>
    </div>
  );
}

function Cases({ cases, setName }: { cases: AlgorithmCase[]; setName: string }) {
  return <>{[...cases].sort((a, b) =>
    (a.caseNumber ?? 0) - (b.caseNumber ?? 0) || a.name.localeCompare(b.name)
  ).map((item) => (
    <section className="algorithm-case" key={item.id}>
      <h3>{item.name}</h3>
      <div className="case-content">
      {setName === "PLL" && <PllDiagram name={item.name} />}
      {setName === "OLL" && item.caseNumber != null && <OllDiagram caseNumber={item.caseNumber} />}
      <div className="algorithm-case-moves">
      {item.algorithms.length ? (
        <AlgorithmPicker item={item} />
      ) : <p>No algorithms saved for this case yet.</p>}
      </div>
      </div>
    </section>
  ))}</>;
}

export function AlgorithmBrowser({ name }: { name: string }) {
  const [data, setData] = useState<AlgorithmSet | null>(null);
  const [error, setError] = useState("");
  const [attempt, setAttempt] = useState(0);

  useEffect(() => {
    const controller = new AbortController();
    getAlgorithmSet(name, controller.signal).then(setData).catch(() => {
      if (!controller.signal.aborted) setError(`Could not load ${name} algorithms. Please try again.`);
    });
    return () => controller.abort();
  }, [name, attempt]);

  return (
    <section className="algorithms-page">
      <Link className="algorithm-back" href="/algorithms">← All algorithms</Link>
      {error ? <div role="alert" className="algorithm-case">
        <p>{error}</p><button onClick={() => { setError(""); setAttempt(attempt + 1); }}>Retry</button>
      </div> : !data ? <p role="status">Loading algorithms…</p> : (
        <>
          {data.groups.map((group) => (
            <section className="algorithm-group" key={group.id}>
              <h2>{group.name}</h2>
              <Cases cases={group.cases} setName={name} />
            </section>
          ))}
          <Cases cases={data.cases} setName={name} />
          {!data.groups.length && !data.cases.length && <p>No cases available yet.</p>}
        </>
      )}
    </section>
  );
}
