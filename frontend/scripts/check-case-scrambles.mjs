import assert from "node:assert/strict";
import { readFile } from "node:fs/promises";
import { cube3x3x3 } from "cubing/puzzles";
import { setSearchDebug } from "cubing/search";
import { generateCaseScramble } from "../lib/caseScramble.ts";

setSearchDebug({ logPerf: false });
const puzzle = await cube3x3x3.kpuzzle();
const sql = await readFile(new URL("../../backend/SeedOllAlgorithmsStandard.sql", import.meta.url), "utf8");
const fixtures = [...sql.matchAll(/\((\d+), '((?:''|[^'])*)'\)/g)].map((m) => ({
  name: `OLL ${m[1]}`, moves: m[2].replaceAll("''", "'"),
}));
assert.equal(fixtures.length, 57);
fixtures.push({ name: "Aa", moves: "x R' U R' D2 R U' R' D2 R2 x'" });
for (const [id, fixture] of fixtures.entries()) {
  const item = { id, name: fixture.name, caseNumber: null, algorithms: [{ id, moves: fixture.moves, isStandard: true, userId: null }] };
  const generated = await generateCaseScramble([item]);
  assert.equal(generated.caseId, id);
  assert.match(generated.scramble, /^[URFDLB2' ]+$/);
  assert.doesNotMatch(generated.scramble, /2'/);
  const actual = puzzle.defaultPattern().applyAlg(generated.scramble);
  const target = puzzle.defaultPattern().applyAlg((await import("cubing/alg")).Alg.fromString(fixture.moves).invert());
  let matches = false;
  for (const tilt of ["", "x", "x2", "x'", "z", "z'"]) {
    for (const y of ["", "y", "y2", "y'"]) {
      for (const u of ["", "U", "U2", "U'"]) {
        const candidate = target.applyAlg(`${tilt} ${y} ${u}`);
        if (["CORNERS", "EDGES"].every((orbit) => JSON.stringify(candidate.patternData[orbit]) === JSON.stringify(actual.patternData[orbit]))) matches = true;
      }
    }
  }
  assert.ok(matches, `Wrong target for ${fixture.name}`);
}
await assert.rejects(() => generateCaseScramble([]));
console.log(`Verified ${fixtures.length} case scrambles, including all 57 OLL standards and a rotated PLL.`);
// Exercise the same packaged module/worker tree that the browser loads.
const bundledPuzzle = await (await import("../public/vendor/cubing/puzzles/index.js")).cube3x3x3.kpuzzle();
const bundledSearch = await import("../public/vendor/cubing/search/index.js");
bundledSearch.setSearchDebug({ logPerf: false });
const bundledTarget = bundledPuzzle.defaultPattern().applyAlg("R U R' U R U2 R'");
const bundledSolution = await bundledSearch.experimentalSolve3x3x3IgnoringCenters(bundledTarget);
const solvedBundle = bundledTarget.applyAlg(bundledSolution);
for (const orbit of ["CORNERS", "EDGES"]) {
  assert.deepEqual(solvedBundle.patternData[orbit], bundledPuzzle.defaultPattern().patternData[orbit]);
}
console.log("Verified the packaged solver worker and its module dependencies.");
process.exit(0);
