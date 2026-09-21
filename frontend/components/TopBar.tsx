"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";

export function TopBar() {
  const pathname = usePathname();

  return (
    <header className="shrink-0 border-b border-cube-border bg-cube-surface px-5">
      <nav className="flex justify-center" aria-label="Main navigation">
        {[
          { href: "/", label: "Timer" },
          { href: "/algorithms", label: "Algorithms" },
        ].map(({ href, label }) => {
          const active = href === "/"
            ? pathname === "/"
            : pathname === href || pathname.startsWith(`${href}/`);

          return (
            <Link
              key={href}
              href={href}
              className={`inline-flex min-h-[76px] w-40 min-w-0 items-center justify-center border-x border-x-[#444] border-y-0 bg-[#222] px-4 py-3 text-lg font-semibold text-cube-text no-underline transition-[background,border-color,color] duration-150 hover:border-[#666] hover:bg-[#303030] focus-visible:outline-2 focus-visible:outline-cube-green focus-visible:outline-offset-[-4px] ${active ? "border-x-cube-green bg-cube-green/12 text-cube-green" : ""}`}
              aria-current={active ? "page" : undefined}
            >
              {label}
            </Link>
          );
        })}
      </nav>
    </header>
  );
}
