import type { ReactNode } from "react";
import Link from "next/link";
import {
  Banknote,
  BarChart3,
  Clock3,
  LayoutDashboard,
  LogOut,
  Menu,
  MonitorCog,
  Settings,
  Users,
} from "lucide-react";

import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/components/ui/sheet";
import type { UserRole } from "@/types/auth";

const navigation = [
  { label: "Esasy panel", icon: LayoutDashboard, active: true },
  { label: "Kompýuterler", icon: MonitorCog, active: false },
  { label: "Sessiýalar", icon: Clock3, active: false },
  { label: "Tölegler", icon: Banknote, active: false },
  { label: "Hasabatlar", icon: BarChart3, active: false },
  { label: "Ulanyjylar", icon: Users, active: false, adminOnly: true },
  { label: "Sazlamalar", icon: Settings, active: false, adminOnly: true },
];

type DashboardShellProps = {
  children: ReactNode;
  role: UserRole;
};

function ArenaLogo() {
  return (
    <Link href="/dashboard" className="flex items-center gap-3 rounded-lg focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-indigo-400">
      <span className="grid size-10 place-items-center rounded-xl border border-indigo-400/25 bg-indigo-500/15">
        <MonitorCog aria-hidden="true" className="size-5 text-indigo-300" />
      </span>
      <div>
        <p className="font-semibold tracking-tight text-white">ArenaDesk</p>
        <p className="text-[11px] text-slate-500">Management panel</p>
      </div>
    </Link>
  );
}

function Navigation({ role }: { role: UserRole }) {
  return (
    <nav aria-label="Esasy navigasiýa" className="space-y-1">
      {navigation
        .filter((item) => !item.adminOnly || role === "admin")
        .map((item) => {
          const Icon = item.icon;
          const className = item.active
            ? "flex items-center gap-3 rounded-xl bg-indigo-500/15 px-3 py-2.5 text-sm font-medium text-indigo-200"
            : "flex cursor-not-allowed items-center gap-3 rounded-xl px-3 py-2.5 text-sm text-slate-500";

          return item.active ? (
            <Link key={item.label} href={`/dashboard?role=${role}`} className={className} aria-current="page">
              <Icon aria-hidden="true" className="size-4" />
              {item.label}
            </Link>
          ) : (
            <span key={item.label} className={className} aria-disabled="true">
              <Icon aria-hidden="true" className="size-4" />
              {item.label}
            </span>
          );
        })}
    </nav>
  );
}

function SidebarContent({ role }: { role: UserRole }) {
  const roleLabel = role === "admin" ? "Administrator" : "Kassir";

  return (
    <div className="flex h-full flex-col">
      <ArenaLogo />
      <Separator className="my-6 bg-white/10" />
      <Navigation role={role} />

      <div className="mt-auto rounded-2xl border border-white/10 bg-white/[0.03] p-3">
        <div className="flex items-center gap-3">
          <span className="grid size-9 place-items-center rounded-full bg-indigo-500/15 text-xs font-semibold text-indigo-200">
            {role === "admin" ? "AD" : "KA"}
          </span>
          <div className="min-w-0">
            <p className="truncate text-sm font-medium text-slate-200">Demo ulanyjy</p>
            <p className="truncate text-xs text-slate-500">{roleLabel}</p>
          </div>
        </div>
        <Button variant="ghost" size="sm" asChild className="mt-3 w-full justify-start text-slate-400 hover:text-white">
          <Link href="/login">
            <LogOut aria-hidden="true" data-icon="inline-start" />
            Ulgamdan çyk
          </Link>
        </Button>
      </div>
    </div>
  );
}

export function DashboardShell({ children, role }: DashboardShellProps) {
  const roleLabel = role === "admin" ? "Administrator" : "Kassir";

  return (
    <div className="min-h-screen bg-[#070a12]">
      <aside className="fixed inset-y-0 left-0 hidden w-64 border-r border-white/10 bg-[#090d17] p-5 lg:block">
        <SidebarContent role={role} />
      </aside>

      <div className="lg:pl-64">
        <header className="sticky top-0 z-30 flex h-16 items-center justify-between border-b border-white/10 bg-[#070a12]/90 px-4 backdrop-blur sm:px-6 lg:px-8">
          <div className="flex items-center gap-3">
            <Sheet>
              <SheetTrigger asChild>
                <Button variant="outline" size="icon" className="border-white/10 bg-white/[0.03] lg:hidden" aria-label="Menýuny aç">
                  <Menu aria-hidden="true" />
                </Button>
              </SheetTrigger>
              <SheetContent side="left" className="w-72 border-white/10 bg-[#090d17] p-5 text-white">
                <SheetHeader className="sr-only">
                  <SheetTitle>ArenaDesk menýusy</SheetTitle>
                </SheetHeader>
                <SidebarContent role={role} />
              </SheetContent>
            </Sheet>
            <div className="lg:hidden">
              <p className="text-sm font-semibold text-white">ArenaDesk</p>
              <p className="text-[11px] text-slate-500">Esasy panel</p>
            </div>
          </div>

          <div className="flex items-center gap-3">
            <span className="hidden rounded-full border border-emerald-400/20 bg-emerald-400/10 px-3 py-1 text-xs text-emerald-300 sm:inline-flex">
              Web-panel işleýär
            </span>
            <div className="text-right">
              <p className="text-xs font-medium text-slate-200">Demo ulanyjy</p>
              <p className="text-[11px] text-slate-500">{roleLabel}</p>
            </div>
          </div>
        </header>

        <main className="px-4 py-6 sm:px-6 lg:px-8 lg:py-8">{children}</main>
      </div>
    </div>
  );
}
