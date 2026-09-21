"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";

export function TopBar() {
  const pathname = usePathname();

  return (
    <header className="top-bar">
      <nav aria-label="Main navigation">
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
              className="top-bar-link"
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
