import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";

const inter = Inter({ subsets: ["latin", "cyrillic"] });

export const metadata: Metadata = {
  title: "ArenaDesk | Gaming club manager",
  description: "Gaming club session and computer management dashboard",
};

export default function RootLayout({ children }: Readonly<{ children: React.ReactNode }>) {
  return (
    <html lang="tk">
      <body className={inter.className}>{children}</body>
    </html>
  );
}
