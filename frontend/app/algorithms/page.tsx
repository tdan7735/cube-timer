import type { Metadata } from "next";
import Image from "next/image";
import Link from "next/link";

export const metadata: Metadata = {
  title: "Algorithms | Cube Timer",
};

export default function AlgorithmsPage() {
  return (
    <section className="mx-auto max-w-[1100px] px-6 py-10">
      <h1 className="mb-3 text-[28px] font-medium">Algorithms</h1>
      <p className="leading-relaxed text-[#aaa]">Explore last-layer algorithms.</p>
      <div className="mt-8 grid grid-cols-1 gap-6 sm:grid-cols-2">
        {[
          { name: "OLL", description: "Orientation of the Last Layer", count: "57 cases" },
          { name: "PLL", description: "Permutation of the Last Layer", count: "21 cases" },
        ].map(({ name, description, count }) => (
          <Link href={`/algorithms/${name.toLowerCase()}`} className="block overflow-hidden rounded-xl border border-cube-border bg-cube-surface text-inherit no-underline hover:border-cube-green focus-visible:border-cube-green" key={name} aria-labelledby={`card-${name}`}>
            <div className="aspect-video border-b border-cube-border">
              <Image
                src="/algorithm-placeholder.svg"
                alt={`${name} image placeholder`}
                width={480}
                height={270}
              />
            </div>
            <div className="p-6">
              <div className="mb-2 flex items-center justify-between gap-3">
                <h2 className="text-2xl font-semibold" id={`card-${name}`}>{name}</h2>
                <span className="whitespace-nowrap text-[13px] text-cube-green">{count}</span>
              </div>
              <p className="leading-relaxed text-[#aaa]">{description}</p>
            </div>
          </Link>
        ))}
      </div>
    </section>
  );
}
