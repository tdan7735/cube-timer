export const Penalty = {
  None: 0,
  Plus2: 1,
  DNF: 2,
} as const;

export type Penalty = (typeof Penalty)[keyof typeof Penalty];

export interface Solve {
  id: number;
  scramble: string;
  penalty: Penalty;
  solveTime: number;
  timeSolved: string;
}

export function finalTime(solve: Solve): number {
  return solve.penalty === Penalty.Plus2 ? solve.solveTime + 2000 : solve.solveTime;
}

export interface Statistics {
  totalAverage: number | null;
  ao5: number | null;
  ao12: number | null;
  ao50: number | null;
  ao100: number | null;
  personalBest: number | null;
}

export interface PostSolveRequest {
  scramble: string;
  penalty: Penalty;
  solveTime: number;
}

export interface Algorithm {
  isStandard: boolean;
  id: number;
  moves: string;
  userId: number | null;
}

export interface AlgorithmCase {
  id: number;
  name: string;
  caseNumber: number | null;
  algorithms: Algorithm[];
}

export interface AlgorithmSet {
  id: number;
  name: string;
  cases: AlgorithmCase[];
  groups: { id: number; name: string; cases: AlgorithmCase[] }[];
}
