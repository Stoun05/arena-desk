import { Suspense } from "react";
import { Banknote, Clock3, MonitorCheck, MonitorCog, Plus } from "lucide-react";

import { ComputerStationGrid } from "@/components/features/computers";
import { DashboardShell } from "@/components/layout/dashboard-shell";
import { Button } from "@/components/ui/button";
import { computers } from "@/data/computers";

const activeSessions = computers.filter(
  (computer) => computer.status === "occupied" || computer.status === "ending",
).length;
const availableComputers = computers.filter(
  (computer) => computer.status === "available",
).length;
const currentRevenue = computers.reduce(
  (total, computer) => total + (computer.currentCharge ?? 0),
  0,
);

const statistics = [
  {
    label: "Ähli kompýuterler",
    value: String(computers.length),
    detail: "6 Standard · 4 VIP",
    icon: MonitorCog,
    color: "text-indigo-300",
    background: "bg-indigo-400/10",
  },
  {
    label: "Aktiw sessiýalar",
    value: String(activeSessions),
    detail: "2 adaty · 1 wagt gutarýar",
    icon: Clock3,
    color: "text-amber-300",
    background: "bg-amber-400/10",
  },
  {
    label: "Boş kompýuterler",
    value: String(availableComputers),
    detail: "Täze sessiýa taýýar",
    icon: MonitorCheck,
    color: "text-emerald-300",
    background: "bg-emerald-400/10",
  },
  {
    label: "Aktiw töleg",
    value: `${currentRevenue} TMT`,
    detail: "Demo sessiýalar boýunça",
    icon: Banknote,
    color: "text-sky-300",
    background: "bg-sky-400/10",
  },
];

export default function DashboardPage() {
  return (
    <Suspense fallback={<div className="min-h-screen bg-[#070a12]" />}>
      <DashboardShell>
        <div className="flex flex-col gap-7">
          <section className="flex flex-col justify-between gap-4 sm:flex-row sm:items-end">
            <div>
              <p className="text-sm font-medium text-indigo-300">Esasy panel</p>
              <h1 className="mt-1 text-3xl font-semibold tracking-[-0.035em] text-white">
                Klubuň ýagdaýy
              </h1>
              <p className="mt-2 text-sm text-slate-400">
                10 kompýuteriň ýagdaýyny bir ekrandan yzarlaň.
              </p>
            </div>
            <Button disabled className="self-start bg-indigo-500 text-white sm:self-auto">
              <Plus aria-hidden="true" data-icon="inline-start" />
              Täze sessiýa
            </Button>
          </section>

          <section aria-label="Gysga statistika" className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
            {statistics.map((statistic) => {
              const Icon = statistic.icon;

              return (
                <article
                  key={statistic.label}
                  className="rounded-2xl border border-white/10 bg-white/[0.025] p-5"
                >
                  <div className="flex items-start justify-between gap-4">
                    <div>
                      <p className="text-sm text-slate-400">{statistic.label}</p>
                      <p className="mt-3 text-3xl font-semibold tracking-tight text-white">
                        {statistic.value}
                      </p>
                    </div>
                    <span className={`grid size-10 place-items-center rounded-xl ${statistic.background} ${statistic.color}`}>
                      <Icon aria-hidden="true" className="size-4.5" />
                    </span>
                  </div>
                  <p className="mt-4 text-xs text-slate-500">{statistic.detail}</p>
                </article>
              );
            })}
          </section>

          <ComputerStationGrid stations={computers} />
        </div>
      </DashboardShell>
    </Suspense>
  );
}
