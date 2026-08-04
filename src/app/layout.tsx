import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "ArenaDesk",
  description: "Gaming klub we internet-kafe dolandyryş paneli",
};

export default function RootLayout({ children }: LayoutProps<"/">) {
  return (
    <html lang="tk" className="dark">
      <body className="min-h-screen antialiased">{children}</body>
    </html>
  );
}
