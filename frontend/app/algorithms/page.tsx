import type { Metadata } from "next";
import Image from "next/image";
import Link from "next/link";

export const metadata: Metadata = {
  title: "Algorithms | Cube Timer",
};

export default function AlgorithmsPage() {
  return (
    <section className="algorithms-page">
      <h1>Algorithms</h1>
      <p>Explore last-layer algorithms.</p>
      <div className="algorithm-cards">
        {[
          { name: "OLL", description: "Orientation of the Last Layer", count: "57 cases" },
          { name: "PLL", description: "Permutation of the Last Layer", count: "21 cases" },
        ].map(({ name, description, count }) => (
          <Link href={`/algorithms/${name.toLowerCase()}`} className="algorithm-card" key={name} aria-labelledby={`card-${name}`}>
            <div className="algorithm-card-image">
              <Image
                src="/algorithm-placeholder.svg"
                alt={`${name} image placeholder`}
                width={480}
                height={270}
              />
            </div>
            <div className="algorithm-card-content">
              <div className="algorithm-card-heading">
                <h2 id={`card-${name}`}>{name}</h2>
                <span className="algorithm-card-count">{count}</span>
              </div>
              <p>{description}</p>
            </div>
          </Link>
        ))}
      </div>
    </section>
  );
}
