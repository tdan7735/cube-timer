export const Penalty = {
  None: 0,
  Plus2: 1,
  DNF: 2,
} as const;

export type Penalty = (typeof Penalty)[keyof typeof Penalty];

export const SessionType = {
  Solves: 0,
  AlgorithmTraining: 1,
} as const;

export type SessionType = (typeof SessionType)[keyof typeof SessionType];

export interface Session {
  id: number;
  name: string;
  type: SessionType;
  whenMade: string;
}

export interface Solve {
  id: number;
  scramble: string;
  penalty: Penalty;
  solveTime: number;
  timeSolved: string;
  algorithmCaseId?: number | null;
  algorithmCaseName?: string | null;
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
  sessionId: number;
}

export interface PostSessionRequest {
  name?: string;
  type: SessionType;
}

export interface TrainingResponse {
  session: Session;
  attempts: Solve[];
  statistics: Statistics;
}

export interface PostTrainingAttemptRequest {
  scramble: string;
  penalty: Penalty;
  solveTime: number;
  algorithmCaseId: number;
}

export interface CaseStatistics {
  algorithmCaseId: number;
  algorithmCaseName: string;
  attemptCount: number;
  bestTime: number | null;
  averageTime: number | null;
  ao5: number | null;
}

export const LearningStatus = {
  NotLearned: 0,
  Learning: 1,
  Learned: 2,
} as const;
export type LearningStatus = (typeof LearningStatus)[keyof typeof LearningStatus];

export const TrainingFocus = { All: 0, Slowest: 1 } as const;
export type TrainingFocus = (typeof TrainingFocus)[keyof typeof TrainingFocus];
export const TrainingOrder = { Balanced: 0, Random: 1 } as const;
export type TrainingOrder = (typeof TrainingOrder)[keyof typeof TrainingOrder];

export interface TrainingPreferences {
  includeNotLearned: boolean;
  includeLearning: boolean;
  includeLearned: boolean;
  focus: TrainingFocus;
  slowestCount: number;
  order: TrainingOrder;
  selectedCaseIds: number[] | null;
}

export interface TrainingConfiguration {
  statuses: { algorithmCaseId: number; status: LearningStatus }[];
  preferences: TrainingPreferences;
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
