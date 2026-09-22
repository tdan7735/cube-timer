"use client";

import { useState } from "react";
import { type Session } from "../lib/types";

interface SessionManagerProps {
  sessions: Session[];
  activeSessionId: number | null;
  disabled: boolean;
  onSelect: (id: number) => Promise<void>;
  onCreate: (name: string) => Promise<void>;
  onUpdate: (id: number, name: string) => Promise<void>;
  onDelete: (id: number) => Promise<void>;
}

export function SessionManager({ sessions, activeSessionId, disabled, onSelect, onCreate, onUpdate, onDelete }: SessionManagerProps) {
  const [mode, setMode] = useState<"create" | "edit" | null>(null);
  const [name, setName] = useState("");
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const activeSession = sessions.find((session) => session.id === activeSessionId);

  const beginCreate = () => {
    setName("");
    setError("");
    setMode("create");
  };

  const beginEdit = () => {
    if (!activeSession) return;
    setName(activeSession.name);
    setError("");
    setMode("edit");
  };

  const save = async () => {
    setSaving(true);
    setError("");
    try {
      if (mode === "create") await onCreate(name);
      if (mode === "edit" && activeSession) await onUpdate(activeSession.id, name);
      setMode(null);
    } catch {
      setError("Could not save this session.");
    } finally {
      setSaving(false);
    }
  };

  const remove = async () => {
    if (!activeSession || !window.confirm(`Delete session “${activeSession.name}” and its solves?`)) return;
    setSaving(true);
    setError("");
    try {
      await onDelete(activeSession.id);
    } catch {
      setError("Could not delete this session.");
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="border-b border-cube-border px-5 pt-4 pb-3">
      <label className="flex flex-col gap-1 text-[11px] uppercase tracking-[1px] text-cube-dim">
        Session
        <select
          aria-label="Active session"
          className="w-full bg-cube-surface text-sm normal-case tracking-normal text-cube-text outline-none"
          value={activeSessionId ?? ""}
          disabled={disabled || saving}
          onChange={(event) => void onSelect(Number(event.target.value))}
        >
          {sessions.map((session) => <option key={session.id} value={session.id}>{session.name}</option>)}
        </select>
      </label>

      {mode ? (
        <div className="mt-3 flex flex-col gap-2">
          <input className="border border-cube-border bg-cube-bg px-2 py-1 text-sm text-cube-text outline-none focus:border-cube-green" value={name} onChange={(event) => setName(event.target.value)} placeholder="Session name" />
          <div className="flex gap-2 text-xs">
            <button className="text-cube-green hover:underline" disabled={saving} onClick={() => void save()}>Save</button>
            <button className="text-cube-dim hover:text-cube-text" disabled={saving} onClick={() => setMode(null)}>Cancel</button>
          </div>
        </div>
      ) : (
        <div className="mt-2 flex gap-3 text-xs">
          <button className="text-cube-green hover:underline" disabled={disabled} onClick={beginCreate}>New</button>
          <button className="text-cube-dim hover:text-cube-text" disabled={disabled || !activeSession} onClick={beginEdit}>Edit</button>
          <button className="text-cube-dim hover:text-cube-red" disabled={disabled || !activeSession} onClick={() => void remove()}>Delete</button>
        </div>
      )}
      {error && <p className="mt-2 text-xs text-cube-red" role="alert">{error}</p>}
    </div>
  );
}
