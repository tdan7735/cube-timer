"use client";

import Link from "next/link";
import { PllDiagram } from "./PllDiagram";
import { OllDiagram } from "./OllDiagram";
import { useEffect, useState } from "react";
import { getAlgorithmSet, getCaseStatistics, setStandardAlgorithm } from "../lib/api";
import type { AlgorithmCase, AlgorithmSet, CaseStatistics } from "../lib/types";
import { formatTime } from "../lib/format";

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
    <div className="mb-3">
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
          <fieldset className="rounded-md border border-[#444] p-4" disabled={saving}>
            <legend>Choose the standard algorithm for {item.name}</legend>
            {algorithms.map((algorithm) => (
              <label className="flex cursor-pointer items-center gap-3 p-3 font-mono wrap-anywhere has-checked:bg-cube-green/12" key={algorithm.id}>
                <input type="radio" name={`standard-${item.id}`} value={algorithm.id}
                  checked={draftId === algorithm.id} onChange={() => setDraftId(algorithm.id)} />
                <span>{algorithm.moves}</span>
                {algorithm.id === selectedId && <span className="text-[13px] text-cube-green">Standard</span>}
              </label>
            ))}
          </fieldset>
          <div className="mt-2 flex gap-2">
            <button className="cursor-pointer rounded-md border border-[#444] bg-cube-surface px-5 py-2.5 text-[15px] hover:border-cube-green disabled:cursor-default disabled:opacity-50" type="submit" disabled={saving}>{saving ? "Saving…" : "Save standard"}</button>
            <button className="cursor-pointer rounded-md border border-[#444] bg-cube-surface px-5 py-2.5 text-[15px] hover:border-cube-green disabled:cursor-default disabled:opacity-50" type="button" disabled={saving} onClick={() => { setEditing(false); setError(""); }}>Cancel</button>
          </div>
          {error && <p role="alert">{error}</p>}
        </form>
      ) : (
        <button className="flex w-full cursor-pointer items-center justify-between gap-4 rounded-md border border-[#444] bg-cube-surface p-4 text-left font-mono wrap-anywhere hover:border-cube-green" onClick={() => {
          setDraftId(selectedId);
          setStatus("");
          setEditing(true);
        }} aria-label={`Choose standard algorithm for ${item.name}`}>
          <span>{algorithms.find((a) => a.id === selectedId)?.moves}</span>
          <span className="text-[13px] text-cube-green">Change standard</span>
        </button>
      )}
      <span className="text-[13px] text-cube-green" role="status">{status}</span>
    </div>
  );
}

function CaseStats({ stats }: { stats?: CaseStatistics }) {
  return (
    <dl className="flex shrink-0 gap-4 text-right text-xs">
      <div><dt className="uppercase tracking-[1px] text-cube-dim">Best</dt><dd className="mt-0.5 font-mono text-cube-green">{stats?.bestTime == null ? "—" : formatTime(stats.bestTime)}</dd></div>
      <div><dt className="uppercase tracking-[1px] text-cube-dim">Avg</dt><dd className="mt-0.5 font-mono">{stats?.averageTime == null ? "—" : formatTime(stats.averageTime)}</dd></div>
    </dl>
  );
}

function Cases({ cases, setName, statistics }: { cases: AlgorithmCase[]; setName: string; statistics: Map<number, CaseStatistics> }) {
  return <>{[...cases].sort((a, b) =>
    (a.caseNumber ?? 0) - (b.caseNumber ?? 0) || a.name.localeCompare(b.name)
  ).map((item) => (
    <section className="mt-6" key={item.id}>
      <div className="mb-3 flex items-end justify-between gap-4"><h3>{item.name}</h3><CaseStats stats={statistics.get(item.id)} /></div>
      <div className="flex items-center gap-6 max-[600px]:flex-col max-[600px]:items-stretch max-[600px]:gap-3">
      {setName === "PLL" && <PllDiagram name={item.name} />}
      {setName === "OLL" && item.caseNumber != null && <OllDiagram caseNumber={item.caseNumber} />}
      <div className="min-w-0 flex-1">
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
  const [statistics, setStatistics] = useState<Map<number, CaseStatistics>>(new Map());
  const [error, setError] = useState("");
  const [attempt, setAttempt] = useState(0);

  useEffect(() => {
    const controller = new AbortController();
    Promise.all([getAlgorithmSet(name, controller.signal), getCaseStatistics(name, controller.signal)]).then(([set, caseStatistics]) => {
      setData(set);
      setStatistics(new Map(caseStatistics.map((stat) => [stat.algorithmCaseId, stat])));
    }).catch(() => {
      if (!controller.signal.aborted) setError(`Could not load ${name} algorithms. Please try again.`);
    });
    return () => controller.abort();
  }, [name, attempt]);

  return (
    <section className="mx-auto max-w-[1100px] px-6 py-10">
      <Link className="mb-6 inline-block text-cube-green" href="/algorithms">← All algorithms</Link>
      {error ? <div role="alert" className="mt-6">
        <p>{error}</p><button className="mt-2 cursor-pointer rounded-md border border-[#444] bg-cube-surface px-5 py-2.5 text-[15px] hover:border-cube-green" onClick={() => { setError(""); setAttempt(attempt + 1); }}>Retry</button>
      </div> : !data ? <p role="status">Loading algorithms…</p> : (
        <>
          {data.groups.map((group) => (
            <section className="mt-8" key={group.id}>
              <h2>{group.name}</h2>
              <Cases cases={group.cases} setName={name} statistics={statistics} />
            </section>
          ))}
          <Cases cases={data.cases} setName={name} statistics={statistics} />
          {!data.groups.length && !data.cases.length && <p>No cases available yet.</p>}
        </>
      )}
    </section>
  );
}
