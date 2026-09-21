"use client";

import { useState } from "react";
import { AlgorithmBrowser } from "./AlgorithmBrowser";
import { AlgorithmTrainer } from "./AlgorithmTrainer";

export function AlgorithmSetTabs({ name }: { name: string }) {
  const [tab, setTab] = useState("algorithms");
  return (
    <div>
      <div className="flex justify-center border-b border-cube-border px-5" role="tablist" aria-label={`${name} pages`}>
        {["algorithms", "trainer"].map((value, index) => (
          <button key={value} role="tab" id={`${value}-tab`} aria-controls={`${value}-panel`}
            className={`min-w-32 cursor-pointer border border-cube-border bg-cube-surface px-5 py-3 text-[#aaa] ${tab === value ? "border-cube-green bg-cube-green/8 text-cube-green" : ""}`}
            aria-selected={tab === value} tabIndex={tab === value ? 0 : -1}
            onClick={() => setTab(value)} onKeyDown={(event) => {
              if (!["ArrowLeft", "ArrowRight", "Home", "End"].includes(event.key)) return;
              event.preventDefault();
              const next = event.key === "Home" ? "algorithms" : event.key === "End" ? "trainer" : index === 0 ? "trainer" : "algorithms";
              setTab(next);
              document.getElementById(`${next}-tab`)?.focus();
            }}>
            {value === "algorithms" ? "Algorithms" : "Trainer"}
          </button>
        ))}
      </div>
      <div role="tabpanel" id={`${tab}-panel`} aria-labelledby={`${tab}-tab`} tabIndex={0}>
        {tab === "algorithms" ? <AlgorithmBrowser name={name} /> : <AlgorithmTrainer name={name} />}
      </div>
    </div>
  );
}
