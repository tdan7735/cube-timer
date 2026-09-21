"use client";

import { useState, useEffect, useRef } from "react";
import { formatTime } from "../lib/format";
import { Penalty } from "../lib/types";

export type Phase = "idle" | "ready" | "running";

interface TimerProps {
  onSolve: (timeMs: number, penalty: Penalty) => void;
  onPhaseChange: (phase: Phase) => void;
  disabled?: boolean;
}

export function Timer({ onSolve, onPhaseChange, disabled = false }: TimerProps) {
  const [phase, setPhase] = useState<Phase>("idle");
  const [elapsed, setElapsed] = useState(0);

  const phaseRef = useRef<Phase>("idle");
  const startTime = useRef(0);
  const raf = useRef(0);
  const disabledRef = useRef(disabled);
  disabledRef.current = disabled;
  const onSolveRef = useRef(onSolve);
  onSolveRef.current = onSolve;
  const onPhaseChangeRef = useRef(onPhaseChange);
  onPhaseChangeRef.current = onPhaseChange;

  useEffect(() => {
    function tick() {
      const now = Date.now();
      setElapsed(now - startTime.current);
      raf.current = requestAnimationFrame(tick);
    }

    function stop(penalty: Penalty) {
      cancelAnimationFrame(raf.current);
      const finalTime = Date.now() - startTime.current;
      setElapsed(finalTime);
      onSolveRef.current(finalTime, penalty);
      phaseRef.current = "idle";
      setPhase("idle");
      onPhaseChangeRef.current("idle");
    }

    function onKeyDown(e: KeyboardEvent) {
      if (e.repeat) return;
      if (phaseRef.current === "idle" && (disabledRef.current ||
        (e.target instanceof HTMLElement && e.target.closest("button, a, input, textarea, select, [contenteditable]")))) return;
      if (e.code === "Escape" && phaseRef.current === "running") {
        e.preventDefault();
        stop(Penalty.DNF);
        return;
      }

      if (e.code !== "Space") return;
      e.preventDefault();

      const p = phaseRef.current;
      if (p === "running") {
        stop(Penalty.None);
      } else if (p === "idle") {
        phaseRef.current = "ready";
        setPhase("ready");
        onPhaseChangeRef.current("ready");
      }
    }

    function onKeyUp(e: KeyboardEvent) {
      if (e.code !== "Space") return;
      if (phaseRef.current !== "ready") return;
      e.preventDefault();

      const p = phaseRef.current;
      if (p === "ready") {
        startTime.current = Date.now();
        setElapsed(0);
        raf.current = requestAnimationFrame(tick);
        phaseRef.current = "running";
        setPhase("running");
        onPhaseChangeRef.current("running");
      }
    }

    window.addEventListener("keydown", onKeyDown);
    window.addEventListener("keyup", onKeyUp);
    return () => {
      window.removeEventListener("keydown", onKeyDown);
      window.removeEventListener("keyup", onKeyUp);
      cancelAnimationFrame(raf.current);
    };
  }, []);

  const colour =
    phase === "ready"
      ? "var(--green)"
      : phase === "running"
        ? "var(--text)"
        : "var(--text-dim)";

  return (
    <div className="timer-wrap">
      <div className="timer" style={{ color: colour }}>
        {formatTime(elapsed)}
      </div>
      <div className="timer-hint">
        {phase === "idle" && (disabled ? "waiting for scramble / save" : "hold space")}
        {phase === "ready" && "release to start"}
        {phase === "running" && "press space to stop · esc for dnf"}
      </div>
    </div>
  );
}
