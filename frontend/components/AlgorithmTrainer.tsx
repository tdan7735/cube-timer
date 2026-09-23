"use client";

import { useState, useEffect, useRef, useCallback, useMemo } from "react";
import { createTrainingAttempt, deleteTrainingAttempt, deleteTrainingAttempts, getAlgorithmSet, getCaseStatistics, getTraining, getTrainingConfiguration, saveTrainingPreferences, setCaseLearningStatus } from "../lib/api";
import { generateCaseScramble, type CaseScramble } from "../lib/caseScramble";
import { LearningStatus, TrainingFocus, TrainingOrder, type AlgorithmCase, type CaseStatistics, type Solve, type TrainingPreferences } from "../lib/types";
import { Timer, type Phase } from "./Timer";
import { Penalty } from "../lib/types";
import { formatTime } from "../lib/format";

const pllCases = ["Aa", "Ab", "E", "F", "Ga", "Gb", "Gc", "Gd", "H", "Ja", "Jb", "Na", "Nb", "Ra", "Rb", "T", "Ua", "Ub", "V", "Y", "Z"];
const statusSelectHighlight: Record<LearningStatus, string> = {
  [LearningStatus.NotLearned]: "border-[#444] bg-[#151515] text-cube-text",
  [LearningStatus.Learning]: "border-amber-400/70 bg-amber-400/15 text-amber-200",
  [LearningStatus.Learned]: "border-cube-green/70 bg-cube-green/15 text-green-200",
};

export function AlgorithmTrainer({ name }: { name: string }) {
  const cases = name === "OLL" ? Array.from({ length: 57 }, (_, i) => `OLL ${i + 1}`) : pllCases;
  const [selected, setSelected] = useState(cases);
  const [selecting, setSelecting] = useState(false);
  const [index, setIndex] = useState(0);
  const [available, setAvailable] = useState<AlgorithmCase[]>([]);
  const [statuses, setStatuses] = useState<Map<number, LearningStatus>>(new Map());
  const [statusShortcuts, setStatusShortcuts] = useState<LearningStatus[]>([]);
  const [caseStatistics, setCaseStatistics] = useState<Map<number, CaseStatistics>>(new Map());
  const [preferences, setPreferences] = useState<TrainingPreferences>({ includeNotLearned: true, includeLearning: true, includeLearned: true, focus: TrainingFocus.All, slowestCount: 10, order: TrainingOrder.Balanced, selectedCaseIds: null });
  const [history, setHistory] = useState<CaseScramble[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [retry, setRetry] = useState(0);
  const [attempts, setAttempts] = useState<Solve[]>([]);
  const [trainingError, setTrainingError] = useState("");
  const [savingAttempt, setSavingAttempt] = useState(false);
  const requestId = useRef(0);
  const balancedBag = useRef<number[]>([]);
  const current = history[index];

  useEffect(() => {
    const controller = new AbortController();
    setLoading(true);
    setError("");
    Promise.all([getAlgorithmSet(name, controller.signal), getTrainingConfiguration(name, controller.signal), getCaseStatistics(name, controller.signal)]).then(([set, configuration, statistics]) => {
      const byId = new Map([...set.cases, ...set.groups.flatMap((g) => g.cases)].map((c) => [c.id, c]));
      const usable = [...byId.values()].filter((c) => c.algorithms.some((a) => a.moves.trim()));
      const loadedStatuses = new Map(configuration.statuses.map((item) => [item.algorithmCaseId, item.status]));
      const selectedCases = usable.filter((item) => configuration.preferences.selectedCaseIds === null || configuration.preferences.selectedCaseIds.includes(item.id));
      setAvailable(usable);
      setSelected(selectedCases.map((c) => c.name));
      setStatuses(loadedStatuses);
      const includedStatuses = [
        configuration.preferences.includeNotLearned && LearningStatus.NotLearned,
        configuration.preferences.includeLearning && LearningStatus.Learning,
        configuration.preferences.includeLearned && LearningStatus.Learned,
      ].filter((status): status is LearningStatus => status !== false);
      const casesWithIncludedStatuses = usable.filter((item) => includedStatuses.includes(loadedStatuses.get(item.id) ?? LearningStatus.NotLearned));
      setStatusShortcuts(selectedCases.length === casesWithIncludedStatuses.length && selectedCases.every((item) => casesWithIncludedStatuses.some((candidate) => candidate.id === item.id)) ? includedStatuses : []);
      setPreferences(configuration.preferences);
      setCaseStatistics(new Map(statistics.map((item) => [item.algorithmCaseId, item])));
      setLoading(false);
    }).catch(() => {
      if (!controller.signal.aborted) { setError("Could not load trainer cases."); setLoading(false); }
    });
    return () => { controller.abort(); requestId.current++; };
  }, [name, retry]);

  const statusIncluded = useCallback((status: LearningStatus) =>
    status === LearningStatus.NotLearned ? preferences.includeNotLearned
      : status === LearningStatus.Learning ? preferences.includeLearning
        : preferences.includeLearned, [preferences]);

  const pool = useMemo(() => {
    const statusFiltered = available.filter((item) => selected.includes(item.name) && statusIncluded(statuses.get(item.id) ?? LearningStatus.NotLearned));
    return preferences.focus === TrainingFocus.Slowest
    ? [...statusFiltered]
      .filter((item) => caseStatistics.get(item.id)?.ao5 != null)
      .sort((a, b) => {
        const aValue = caseStatistics.get(a.id)!.ao5!;
        const bValue = caseStatistics.get(b.id)!.ao5!;
        const aScore = aValue < 0 ? Number.POSITIVE_INFINITY : aValue;
        const bScore = bValue < 0 ? Number.POSITIVE_INFINITY : bValue;
        return bScore - aScore || a.name.localeCompare(b.name);
      })
      .slice(0, preferences.slowestCount)
    : statusFiltered;
  }, [available, selected, statusIncluded, statuses, caseStatistics, preferences.focus, preferences.slowestCount]);
  const poolKey = pool.map((item) => item.id).join(",");
  const scheduleKey = `${poolKey}|${preferences.order}`;

  const chooseCase = useCallback((items: AlgorithmCase[]) => {
    if (preferences.order === TrainingOrder.Random) return items[Math.floor(Math.random() * items.length)];
    const ids = new Set(items.map((item) => item.id));
    balancedBag.current = balancedBag.current.filter((id) => ids.has(id));
    if (!balancedBag.current.length) {
      const previousId = current?.caseId;
      balancedBag.current = items.map((item) => item.id);
      for (let i = balancedBag.current.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [balancedBag.current[i], balancedBag.current[j]] = [balancedBag.current[j], balancedBag.current[i]];
      }
      if (balancedBag.current.length > 1 && balancedBag.current[0] === previousId) {
        [balancedBag.current[0], balancedBag.current[1]] = [balancedBag.current[1], balancedBag.current[0]];
      }
    }
    const id = balancedBag.current.shift();
    return items.find((item) => item.id === id) ?? items[0];
  }, [preferences.order, current?.caseId]);

  const nextScramble = useCallback(async (reset = false) => {
    const id = ++requestId.current;
    if (reset) { balancedBag.current = []; setHistory([]); setIndex(0); }
    setError("");
    if (!pool.length) { setLoading(false); return; }
    setLoading(true);
    try {
      const generated = await generateCaseScramble(pool, chooseCase(pool));
      if (id !== requestId.current) return;
      setHistory((previous) => [...(reset ? [] : previous.slice(0, index + 1)), generated]);
      setIndex(reset ? 0 : index + 1);
    } catch {
      if (id === requestId.current) setError("Could not generate a case scramble. Check the case algorithm or retry.");
    } finally {
      if (id === requestId.current) setLoading(false);
    }
  }, [pool, index, chooseCase]);

  const refreshTraining = useCallback(async () => {
    const [training, statistics] = await Promise.all([getTraining(name), getCaseStatistics(name)]);
    setAttempts(training.attempts);
    setCaseStatistics(new Map(statistics.map((item) => [item.algorithmCaseId, item])));
  }, [name]);

  useEffect(() => {
    setTrainingError("");
    void refreshTraining().catch(() => setTrainingError("Could not load saved training attempts."));
  }, [refreshTraining]);

  useEffect(() => {
    // Changing the selection invalidates the old scramble history.
    void nextScramble(true);
    return () => { requestId.current++; };
    // Index changes navigate history and must not reset it.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [scheduleKey]);

  const updatePreferences = (changes: Partial<TrainingPreferences>) => {
    const updated = { ...preferences, ...changes };
    setPreferences(updated);
    setTrainingError("");
    void saveTrainingPreferences(name, updated).catch(() => setTrainingError("Could not save training filters."));
  };

  const statusPreferenceChanges = (selectedNames: string[]): Pick<TrainingPreferences, "includeNotLearned" | "includeLearning" | "includeLearned"> => {
    const selectedStatuses = new Set(available
      .filter((item) => selectedNames.includes(item.name))
      .map((item) => statuses.get(item.id) ?? LearningStatus.NotLearned));
    return {
      includeNotLearned: selectedStatuses.has(LearningStatus.NotLearned),
      includeLearning: selectedStatuses.has(LearningStatus.Learning),
      includeLearned: selectedStatuses.has(LearningStatus.Learned),
    };
  };

  const updateStatusShortcut = (status: LearningStatus, checked: boolean) => {
    const nextStatuses = checked
      ? [...statusShortcuts, status]
      : statusShortcuts.filter((item) => item !== status);
    const matchingNames = available
      .filter((item) => nextStatuses.includes(statuses.get(item.id) ?? LearningStatus.NotLearned))
      .map((item) => item.name);
    setStatusShortcuts(nextStatuses);
    setSelected(matchingNames);
    updatePreferences({
      includeNotLearned: nextStatuses.includes(LearningStatus.NotLearned),
      includeLearning: nextStatuses.includes(LearningStatus.Learning),
      includeLearned: nextStatuses.includes(LearningStatus.Learned),
      selectedCaseIds: available.filter((item) => matchingNames.includes(item.name)).map((item) => item.id),
    });
    setIndex(0);
  };

  const updateSelected = (names: string[]) => {
    setStatusShortcuts([]);
    setSelected(names);
    updatePreferences({
      ...statusPreferenceChanges(names),
      selectedCaseIds: available.filter((item) => names.includes(item.name)).map((item) => item.id),
    });
  };

  const updateStatus = async (item: AlgorithmCase, status: LearningStatus) => {
    const previous = statuses.get(item.id) ?? LearningStatus.NotLearned;
    setStatuses((values) => new Map(values).set(item.id, status));
    try {
      await setCaseLearningStatus(name, item.id, status);
    } catch {
      setStatuses((values) => new Map(values).set(item.id, previous));
      setTrainingError(`Could not update ${item.name}.`);
    }
  };
  const [phase, setPhase] = useState<Phase>("idle");
  const busy = phase !== "idle" || savingAttempt;
  const valid = attempts.filter((a) => a.penalty !== Penalty.DNF);
  const best = valid.length ? Math.min(...valid.map((attempt) => attempt.solveTime + (attempt.penalty === Penalty.Plus2 ? 2000 : 0))) : null;
  const mean = valid.length ? valid.reduce((total, attempt) => total + attempt.solveTime + (attempt.penalty === Penalty.Plus2 ? 2000 : 0), 0) / valid.length : null;
  const latest = attempts[0];
  const display = (value: number | null) => value === null ? "—" : formatTime(value);

  const saveAttempt = async (time: number, penalty: Penalty) => {
    if (!current || savingAttempt) return;
    const temporaryId = -Date.now();
    const optimisticAttempt: Solve = {
      id: temporaryId,
      scramble: current.scramble,
      penalty,
      solveTime: time,
      timeSolved: new Date().toISOString(),
      algorithmCaseId: current.caseId,
      algorithmCaseName: current.caseName,
    };
    setSavingAttempt(true);
    setTrainingError("");
    // Show the result and start calculating the next case immediately. The
    // server response replaces this temporary attempt once it is persisted.
    setAttempts((previous) => [optimisticAttempt, ...previous]);
    void nextScramble();
    try {
      const savedAttempt = await createTrainingAttempt(name, {
        scramble: current.scramble,
        penalty,
        solveTime: time,
        algorithmCaseId: current.caseId,
      });
      setAttempts((previous) => previous.map((attempt) => attempt.id === temporaryId ? savedAttempt : attempt));
      void getCaseStatistics(name)
        .then((statistics) => setCaseStatistics(new Map(statistics.map((item) => [item.algorithmCaseId, item]))))
        .catch(() => setTrainingError("Attempt saved, but case statistics could not be refreshed."));
    } catch {
      setAttempts((previous) => previous.filter((attempt) => attempt.id !== temporaryId));
      setTrainingError("Could not save this training attempt.");
    } finally {
      setSavingAttempt(false);
    }
  };

  const removeAttempt = async (id: number) => {
    setTrainingError("");
    try {
      await deleteTrainingAttempt(name, id);
      await refreshTraining();
    } catch {
      setTrainingError("Could not delete this training attempt.");
    }
  };

  const clearAttempts = async () => {
    setTrainingError("");
    try {
      await deleteTrainingAttempts(name);
      await refreshTraining();
    } catch {
      setTrainingError("Could not reset this training session.");
    }
  };

  return (
    <section className="px-4 pb-8 [padding-inline:clamp(16px,3vw,48px)] lg:h-full lg:min-h-0 lg:pb-0" aria-label={`${name} trainer`}>
      <div className="overflow-hidden rounded-lg border border-[#383838] lg:flex lg:h-full lg:min-h-0 lg:flex-col lg:rounded-t-none lg:border-t-0">
        <div className="flex min-h-[92px] items-center justify-between gap-5 border-b border-[#383838] bg-cube-surface px-4 py-3">
          <button className="cursor-pointer border-0 bg-transparent px-2 text-3xl text-cube-text disabled:cursor-default disabled:opacity-40" aria-label="Previous scramble" disabled={busy || loading || index === 0} onClick={() => setIndex((i) => i - 1)}>‹</button>
          <div className="min-w-0 text-center"><span className="text-[10px] uppercase tracking-[1.5px] text-[#999]">Case scramble</span><p className="mt-1.5 font-mono text-[clamp(16px,1.6vw,24px)] leading-relaxed wrap-anywhere select-text">{loading ? "" : pool.length ? current?.scramble : "Select cases to begin"}</p></div>
          <button className="cursor-pointer border-0 bg-transparent px-2 text-3xl text-cube-text disabled:cursor-default disabled:opacity-40" aria-label="Next scramble" disabled={busy || loading || !pool.length} onClick={() => index < history.length - 1 ? setIndex(index + 1) : void nextScramble()}>›</button>
        </div>
        {error && <p role="alert" className="p-4 text-[13px] leading-relaxed text-[#999]">{error} <button className="cursor-pointer rounded border border-[#444] bg-transparent px-3 py-2 text-cube-text hover:border-cube-green" onClick={() => available.length ? void nextScramble(true) : setRetry((n) => n + 1)}>Retry</button></p>}
        <p className="p-4 text-[13px] leading-relaxed text-[#999]">Only cases with saved algorithms are available. Practice times are saved to your {name} training session.</p>
        {trainingError && <p role="alert" className="px-4 pb-4 text-[13px] leading-relaxed text-cube-red">{trainingError}</p>}
        <div className="grid min-h-[min(650px,70vh)] grid-cols-1 lg:min-h-0 lg:flex-1 lg:grid-cols-[minmax(0,1fr)_230px_230px]">
          <div className="relative flex min-w-0 flex-col items-center border-b border-[#383838] p-5 lg:col-span-1 lg:border-b-0">
            <button className="cursor-pointer border border-transparent bg-transparent px-3 py-2 text-sm text-cube-green disabled:cursor-default disabled:opacity-40" disabled={busy} aria-expanded={selecting} aria-controls="trainer-case-selection"
              onClick={() => setSelecting(!selecting)}>{pool.length} cases selected <span className="text-[#999]">· Choose cases</span></button>
            {selecting && <div className="mt-3 w-full rounded-md border border-[#444] bg-cube-surface p-4" id="trainer-case-selection">
              <div className="mb-4 grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
                <fieldset className="rounded border border-[#383838] p-3"><legend className="px-1 text-xs text-[#999]">Statuses</legend>
                  {([["Not learned", LearningStatus.NotLearned], ["Learning", LearningStatus.Learning], ["Learned", LearningStatus.Learned]] as const).map(([label, status]) => <label className="mr-4 inline-flex items-center gap-2 text-[13px]" key={status}><input className="accent-cube-green" type="checkbox" checked={statusShortcuts.includes(status)} onChange={(event) => updateStatusShortcut(status, event.target.checked)} />{label}</label>)}
                </fieldset>
                <label className="text-xs text-[#999]">Focus<select className="mt-1 block w-full rounded border border-[#444] bg-[#151515] p-2 text-cube-text" value={preferences.focus} onChange={(event) => updatePreferences({ focus: Number(event.target.value) as TrainingFocus })}><option value={TrainingFocus.All}>All eligible</option><option value={TrainingFocus.Slowest}>Slowest</option></select></label>
                <label className="text-xs text-[#999]">Order<select className="mt-1 block w-full rounded border border-[#444] bg-[#151515] p-2 text-cube-text" value={preferences.order} onChange={(event) => updatePreferences({ order: Number(event.target.value) as TrainingOrder })}><option value={TrainingOrder.Balanced}>Balanced</option><option value={TrainingOrder.Random}>Random</option></select></label>
                {preferences.focus === TrainingFocus.Slowest && <label className="text-xs text-[#999]">Number of slowest cases<input className="mt-1 block w-full rounded border border-[#444] bg-[#151515] p-2 text-cube-text" type="number" min="1" max="100" value={preferences.slowestCount} onChange={(event) => updatePreferences({ slowestCount: Math.max(1, Math.min(100, Number(event.target.value) || 1)) })} /></label>}
              </div>
              <div className="mb-4 flex flex-wrap gap-2">
                <button className="cursor-pointer rounded border border-[#444] bg-transparent px-3 py-2 text-cube-text hover:border-cube-green" onClick={() => { setStatusShortcuts([LearningStatus.NotLearned, LearningStatus.Learning, LearningStatus.Learned]); setSelected(available.map((c) => c.name)); updatePreferences({ includeNotLearned: true, includeLearning: true, includeLearned: true, selectedCaseIds: null }); setIndex(0); }}>Select all</button>
                <button className="cursor-pointer rounded border border-[#444] bg-transparent px-3 py-2 text-cube-text hover:border-cube-green" onClick={() => { updateSelected([]); setIndex(0); }}>Clear</button>
                <button className="cursor-pointer rounded border border-[#444] bg-transparent px-3 py-2 text-cube-text hover:border-cube-green" onClick={() => setSelecting(false)}>Done</button>
              </div>
              <div className="grid max-h-[230px] grid-cols-[repeat(auto-fit,minmax(150px,1fr))] gap-3 overflow-auto">{cases.map((item) => {
                const algorithmCase = available.find((candidate) => candidate.name === item);
                const caseStatus = algorithmCase ? statuses.get(algorithmCase.id) ?? LearningStatus.NotLearned : LearningStatus.NotLearned;
                return <div className="flex items-center gap-2 text-[13px]" key={item}><label className="flex cursor-pointer items-center gap-2"><input className="accent-cube-green" type="checkbox" disabled={!algorithmCase} checked={selected.includes(item)} onChange={() => {
                  updateSelected(selected.includes(item) ? selected.filter((c) => c !== item) : [...selected, item]);
                  setIndex(0);
                }} />{item}</label>{algorithmCase && <select aria-label={`${item} learning status`} className={`min-w-0 rounded border px-1 py-1 text-xs font-medium ${statusSelectHighlight[caseStatus]}`} value={caseStatus} onChange={(event) => void updateStatus(algorithmCase, Number(event.target.value) as LearningStatus)}><option value={LearningStatus.NotLearned}>Not learned</option><option value={LearningStatus.Learning}>Learning</option><option value={LearningStatus.Learned}>Learned</option></select>}</div>;
              })}</div>
              {preferences.focus === TrainingFocus.Slowest && !pool.length && <p className="mt-3 text-[13px] text-[#999]">No matching cases have an Ao5 yet.</p>}
            </div>}
            <div className="flex min-h-[320px] flex-1 flex-col items-center justify-center gap-6">
              <Timer disabled={selecting || loading || !!error || !current || savingAttempt || !pool.some((item) => item.id === current.caseId)} onPhaseChange={setPhase} onSolve={(time, penalty) => void saveAttempt(time, penalty)} />
            </div>
          </div>
          <aside className="border-l border-[#383838] bg-[#151515] px-[18px] py-6" aria-label="Practice attempts">
            <h2 className="text-sm font-medium">Attempts <span className="float-right text-[#999]">{attempts.length}</span></h2>
            {!attempts.length ? <p className="mt-8 text-center text-[13px] leading-relaxed text-[#888]">Your practice times will appear here.</p> : <ol className="mt-5 max-h-[510px] list-none overflow-y-auto">
              {attempts.map((attempt) => <li className="flex items-center gap-2 border-b border-cube-border py-2.5 text-[13px]" key={attempt.id}>
                <span className="flex-1 text-[#aaa]" title={attempt.scramble}>{attempt.algorithmCaseName ?? `${name} practice`}</span>
                <strong className="font-mono font-normal">{attempt.penalty === Penalty.DNF ? "DNF" : formatTime(attempt.solveTime)}</strong>
                <button className="cursor-pointer border-0 bg-transparent px-1.5 py-0.5 text-cube-text disabled:cursor-default disabled:opacity-40" aria-label="Delete training attempt" disabled={busy}
                  onClick={() => void removeAttempt(attempt.id)}>×</button>
              </li>)}
            </ol>}
          </aside>
          <aside className="border-l border-[#383838] bg-[#151515] px-[18px] py-6" aria-label="Training statistics">
            <div className="flex items-center justify-between"><h2 className="text-sm font-medium">Session</h2><button className="cursor-pointer rounded border border-[#444] bg-transparent px-3 py-2 text-cube-text disabled:cursor-default disabled:opacity-40" disabled={busy || !attempts.length} onClick={() => void clearAttempts()} aria-label="Reset practice times">↻</button></div>
            <div className="my-7 grid grid-cols-2 gap-3"><div><span className="mb-2 block text-xs text-[#999]">Current</span><strong className="font-mono text-2xl font-normal">{latest ? latest.penalty === Penalty.DNF ? "DNF" : formatTime(latest.solveTime) : "—"}</strong></div><div><span className="mb-2 block text-xs text-[#999]">Best</span><strong className="font-mono text-2xl font-normal text-cube-green">{display(best)}</strong></div></div>
            <dl className="mb-5"><div className="flex justify-between border-b border-cube-border py-2.5 text-sm"><dt className="text-[#aaa]">Solves</dt><dd className="font-mono">{attempts.length}</dd></div><div className="flex justify-between border-b border-cube-border py-2.5 text-sm"><dt className="text-[#aaa]">Mean</dt><dd className="font-mono">{display(mean)}</dd></div><div className="flex justify-between border-b border-cube-border py-2.5 text-sm"><dt className="text-[#aaa]">DNFs</dt><dd className="font-mono">{attempts.length - valid.length}</dd></div></dl>
            <p className="text-[13px] leading-relaxed text-[#999]">Mean and best exclude DNFs.</p>
            <div className="mt-10 text-xs leading-loose text-[#888]"><p>Hold space to get ready</p><p>Release to start</p><p>Space to stop · Esc for DNF</p></div>
          </aside>
        </div>
      </div>
    </section>
  );
}
