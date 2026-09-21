import type { Metadata } from "next";

export const metadata: Metadata = {
  title: "Algorithms | Cube Timer",
};

export default function AlgorithmsPage() {
  return (
    <section className="algorithms-page">
      <h1>Algorithms</h1>
      <p>Your OLL and PLL algorithm lists will appear here.</p>
    </section>
  );
}
