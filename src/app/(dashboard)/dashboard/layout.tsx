import type { Metadata } from "next";

export const metadata: Metadata = {
  title: "Dashboard | ArenaDesk",
  description: "ArenaDesk gaming klub dolandyryş paneli",
};

export default function DashboardLayout({ children }: LayoutProps<"/dashboard">) {
  return <div className="min-h-screen bg-[#070a12] text-white">{children}</div>;
}
