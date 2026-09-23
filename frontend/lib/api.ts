import type { CaseStatistics, LearningStatus, PostSessionRequest, PostSolveRequest, PostTrainingAttemptRequest, Session, Solve, Statistics, TrainingConfiguration, TrainingPreferences, TrainingResponse } from "./types";
import type { Algorithm, AlgorithmSet } from "./types";

export function getAlgorithmSet(name: string, signal?: AbortSignal): Promise<AlgorithmSet> {
  return request<AlgorithmSet>(`/api/algorithm/${encodeURIComponent(name)}`, { signal });
}

export function setStandardAlgorithm(id: number): Promise<Algorithm> {
  return request<Algorithm>(`/api/algorithm/${id}/standard`, {
    method: "PUT",
  });
}

const BASE = "/api/solves";

async function request<T>(url: string, init?: RequestInit): Promise<T> {
  const res = await fetch(url, {
    headers: { "Content-Type": "application/json" },
    ...init,
  });
  if (!res.ok) throw new Error(`HTTP ${res.status}`);
  if (res.status === 204 || res.headers.get("content-length") === "0") {
    return undefined as T;
  }
  return res.json();
}

export function getSolves(sessionId: number): Promise<Solve[]> {
  return request<Solve[]>(`${BASE}/session/${sessionId}`);
}

export function getStatistics(sessionId: number): Promise<Statistics> {
  return request<Statistics>(`${BASE}/statistics/${sessionId}`);
}

export function createSolve(data: PostSolveRequest): Promise<Solve> {
  return request<Solve>(BASE, {
    method: "POST",
    body: JSON.stringify(data),
  });
}

export function updateSolve(id: number, data: PostSolveRequest): Promise<void> {
  return request<void>(`${BASE}/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });
}

export function deleteSolve(id: number): Promise<void> {
  return request<void>(`${BASE}/${id}`, { method: "DELETE" });
}

export function deleteAllSolves(): Promise<void> {
  return request<void>(BASE, { method: "DELETE" });
}

const SESSIONS_BASE = "/api/session";

export function getSessions(): Promise<Session[]> {
  return request<Session[]>(SESSIONS_BASE);
}

export function createSession(data: PostSessionRequest): Promise<Session> {
  return request<Session>(SESSIONS_BASE, {
    method: "POST",
    body: JSON.stringify(data),
  });
}

export function updateSession(id: number, data: PostSessionRequest): Promise<Session> {
  return request<Session>(`${SESSIONS_BASE}/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });
}

export function deleteSession(id: number): Promise<void> {
  return request<void>(`${SESSIONS_BASE}/${id}`, { method: "DELETE" });
}

export function getTraining(name: string): Promise<TrainingResponse> {
  return request<TrainingResponse>(`/api/training/${encodeURIComponent(name)}`);
}

export function getCaseStatistics(name: string, signal?: AbortSignal): Promise<CaseStatistics[]> {
  return request<CaseStatistics[]>(`/api/training/${encodeURIComponent(name)}/case-statistics`, { signal });
}

export function getTrainingConfiguration(name: string, signal?: AbortSignal): Promise<TrainingConfiguration> {
  return request<TrainingConfiguration>(`/api/training/${encodeURIComponent(name)}/configuration`, { signal });
}

export function setCaseLearningStatus(name: string, caseId: number, status: LearningStatus): Promise<{ algorithmCaseId: number; status: LearningStatus }> {
  return request(`/api/training/${encodeURIComponent(name)}/cases/${caseId}/status`, {
    method: "PUT",
    body: JSON.stringify({ status }),
  });
}

export function saveTrainingPreferences(name: string, preferences: TrainingPreferences): Promise<TrainingPreferences> {
  return request(`/api/training/${encodeURIComponent(name)}/preferences`, {
    method: "PUT",
    body: JSON.stringify(preferences),
  });
}

export function createTrainingAttempt(name: string, data: PostTrainingAttemptRequest): Promise<Solve> {
  return request<Solve>(`/api/training/${encodeURIComponent(name)}/attempts`, {
    method: "POST",
    body: JSON.stringify(data),
  });
}

export function deleteTrainingAttempt(name: string, id: number): Promise<void> {
  return request<void>(`/api/training/${encodeURIComponent(name)}/attempts/${id}`, { method: "DELETE" });
}

export function deleteTrainingAttempts(name: string): Promise<void> {
  return request<void>(`/api/training/${encodeURIComponent(name)}/attempts`, { method: "DELETE" });
}

export async function getScramble(): Promise<string> {
  const res = await request<{ scramble: string }>(`/api/scramble/3x3`, { cache: "no-store" });
  if (typeof res.scramble !== "string" || !res.scramble.trim()) {
    throw new Error("The server returned an empty scramble.");
  }
  return res.scramble;
}
