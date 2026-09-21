import { cp, mkdir } from "node:fs/promises";
import { fileURLToPath } from "node:url";
import { build } from "esbuild";

// Bundle bare npm imports for browsers, keeping the worker entry beside chunks.
const source = new URL("../node_modules/cubing/dist/lib/cubing/", import.meta.url);
const destination = new URL("../public/vendor/cubing/", import.meta.url);
await mkdir(destination, { recursive: true });
await build({
  entryPoints: Object.fromEntries([
    "alg/index", "puzzles/index", "search/index", "chunks/search-worker-entry",
  ].map((entry) => [entry, fileURLToPath(new URL(`${entry}.js`, source))])),
  outdir: fileURLToPath(destination),
  bundle: true,
  splitting: true,
  format: "esm",
  platform: "browser",
  target: "es2022",
  chunkNames: "chunks/[name]-[hash]",
  minify: true,
});
await cp(new URL("../node_modules/cubing/package.json", import.meta.url), new URL("package.json", destination));
