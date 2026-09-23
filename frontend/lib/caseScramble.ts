import type { AlgorithmCase } from "./types";

export interface CaseScramble {
  caseId: number;
  caseName: string;
  scramble: string;
}

// Load the puzzle and solver only when the trainer needs them.
export async function generateCaseScramble(cases: AlgorithmCase[], selectedCase?: AlgorithmCase): Promise<CaseScramble> {
  if (!cases.length) throw new Error("Select at least one available case.");
  const item = selectedCase ?? cases[Math.floor(Math.random() * cases.length)];
  const algorithms = [...item.algorithms].sort((a, b) => a.id - b.id);
  const algorithm = algorithms.find((a) => a.isStandard) ?? algorithms[0];
  if (!algorithm?.moves.trim()) throw new Error(`${item.name} has no saved algorithm.`);

  const [{ Alg }, { cube3x3x3 }, { experimentalSolve3x3x3IgnoringCenters }] = await loadLibrary();
  const puzzle = await cube3x3x3.kpuzzle();
  const auf = ["", "U", "U2", "U'"][Math.floor(Math.random() * 4)];
  const base = puzzle.defaultPattern().applyAlg(Alg.fromString(algorithm.moves).invert());
  // Algorithms can contain x/y/z rotations. Return the centres to the canonical
  // orientation before asking the solver for ordinary face-turn scrambles.
  const solved = puzzle.defaultPattern();
  let oriented = base;
  outer: for (const tilt of ["", "x", "x2", "x'", "z", "z'"]) {
    for (const turn of ["", "y", "y2", "y'"]) {
      const candidate = base.applyAlg(`${tilt} ${turn}`);
      if (candidate.patternData.CENTERS.pieces.every((p, i) => p === solved.patternData.CENTERS.pieces[i])) {
        oriented = candidate;
        break outer;
      }
    }
  }
  const target = oriented.applyAlg(auf);
  const solution = await experimentalSolve3x3x3IgnoringCenters(target);
  // On a 3×3, clockwise and anticlockwise half-turns have the same effect.
  const scramble = solution.invert().toString().replace(/\b([URFDLB])2'(?=\s|$)/g, "$12");
  return { caseId: item.id, caseName: item.name, scramble };
}

async function loadLibrary(): Promise<[
  typeof import("cubing/alg"), typeof import("cubing/puzzles"), typeof import("cubing/search"),
]> {
  if (typeof window === "undefined") {
    return Promise.all([import("cubing/alg"), import("cubing/puzzles"), import("cubing/search")]);
  }
  // Keep the worker entry alongside its dependencies instead of rebundling it.
  const base = "/vendor/cubing";
  return Promise.all([
    import(/* webpackIgnore: true */ `${base}/alg/index.js`),
    import(/* webpackIgnore: true */ `${base}/puzzles/index.js`),
    import(/* webpackIgnore: true */ `${base}/search/index.js`),
  ]);
}
