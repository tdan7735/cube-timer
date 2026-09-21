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
      <body>
        <TopBar />
        <main className="page-content">{children}</main>
      </body>
    </html>
  );
}
