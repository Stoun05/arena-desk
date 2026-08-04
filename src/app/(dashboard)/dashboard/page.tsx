import { Suspense } from "react";
import {
  Banknote,
  CircleDot,
  Clock3,
  MonitorCheck,
  MonitorCog,
  Plus,
} from "lucide-react";

import { DashboardShell } from "@/components/layout/dashboard-shell";
import { Button } from "@/components/ui/button";

const statistics = [
  {
    label: "Ähli kompýuterler",
    value: "10",
    detail: "Standard we VIP",
    icon: MonitorCog,
    color: "text-indigo-300",
    background: "bg-indigo-400/10",
  },
  {
    label: "Aktiw sessiýalar",
    value: "0",
    detail: "Häzirlikçe sessiýa ýok",
    icon: Clock3,
    color: "text-amber-300",
    background: "bg-amber-400/10",
  },
  {
    label: "Boş kompýuterler",
    value: "10",
    detail: "Täze sessiýa taýýar",
    icon: MonitorCheck,
    color: "text-emerald-300",
    background: "bg-emerald-400/10",
  },
  {
    label: "Şu günki girdeji",
    value: "—",
    detail: "Backend garaşylýar",
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
              Hoş geldiňiz
            </h1>
            <p className="mt-2 text-sm text-slate-400">
              Klubuň häzirki ýagdaýy şu ýerde görkeziler.
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

        <section className="grid gap-5 xl:grid-cols-[1.55fr_0.75fr]">
          <article className="min-h-96 rounded-2xl border border-white/10 bg-white/[0.025] p-5 sm:p-6">
            <div className="flex items-center justify-between gap-4">
              <div>
                <h2 className="font-medium text-slate-100">Kompýuterleriň ýagdaýy</h2>
                <p className="mt-1 text-sm text-slate-500">10 stansiýa üçin real wagt paneli</p>
              </div>
              <span className="rounded-full border border-amber-400/20 bg-amber-400/10 px-3 py-1 text-xs text-amber-300">
                Stage 4
              </span>
            </div>

            <div className="grid min-h-72 place-items-center">
              <div className="max-w-sm text-center">
                <span className="mx-auto grid size-14 place-items-center rounded-2xl border border-white/10 bg-white/5 text-slate-400">
                  <MonitorCog aria-hidden="true" className="size-6" />
                </span>
                <h3 className="mt-5 font-medium text-slate-200">Stansiýa kartalary taýýar däl</h3>
                <p className="mt-2 text-sm leading-6 text-slate-500">
                  10 kompýuteriň Boş, Ulanylýar we Öçük ýagdaýlary 4-nji tapgyrda
                  şu ýerde peýda bolar.
                </p>
              </div>
            </div>
          </article>

          <aside className="rounded-2xl border border-white/10 bg-white/[0.025] p-5 sm:p-6">
            <h2 className="font-medium text-slate-100">Ulgam ýagdaýy</h2>
            <div className="mt-6 space-y-5">
              <div className="flex items-start gap-3">
                <CircleDot aria-hidden="true" className="mt-0.5 size-4 text-emerald-400" />
                <div>
                  <p className="text-sm text-slate-200">Web-panel</p>
                  <p className="mt-1 text-xs text-emerald-300">Işleýär</p>
                </div>
              </div>
              <div className="flex items-start gap-3">
                <CircleDot aria-hidden="true" className="mt-0.5 size-4 text-amber-400" />
                <div>
                  <p className="text-sm text-slate-200">Lokal backend</p>
                  <p className="mt-1 text-xs text-amber-300">6-njy tapgyra garaşylýar</p>
                </div>
              </div>
              <div className="flex items-start gap-3">
                <CircleDot aria-hidden="true" className="mt-0.5 size-4 text-slate-600" />
                <div>
                  <p className="text-sm text-slate-200">Windows Agent</p>
                  <p className="mt-1 text-xs text-slate-500">11-nji tapgyra garaşylýar</p>
                </div>
              </div>
            </div>
          </aside>
        </section>
      </div>
      </DashboardShell>
    </Suspense>
  );
}
