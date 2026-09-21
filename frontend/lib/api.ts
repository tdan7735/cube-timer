import type { Solve, Statistics, PostSolveRequest } from "./types";

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

export function getSolves(): Promise<Solve[]> {
  return request<Solve[]>(BASE);
}

export function getStatistics(): Promise<Statistics> {
  return request<Statistics>(`${BASE}/statistics`);
}

export function createSolve(data: PostSolveRequest): Promise<Solve> {
  return request<Solve>(BASE, {
    method: "POST",
    body: JSON.stringify(data),
  });
}

export function deleteSolve(id: number): Promise<void> {
  return request<void>(`${BASE}/${id}`, { method: "DELETE" });
}

export function deleteAllSolves(): Promise<void> {
  return request<void>(BASE, { method: "DELETE" });
}

export async function getScramble(): Promise<string> {
  const res = await request<string>(`/api/scramble/3x3`);

  console.log(res);

  return res;
}
