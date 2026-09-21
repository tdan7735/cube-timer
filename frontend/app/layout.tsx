import type { Metadata } from "next";
import "./globals.css";
import { TopBar } from "../components/TopBar";

export const metadata: Metadata = {
  title: "Cube Timer",
  icons: {
    icon: "/favicon.svg",
  },
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body className="flex flex-col">
        <TopBar />
        <main className="min-h-0 flex-1 overflow-auto">{children}</main>
      </body>
    </html>
  );
}
