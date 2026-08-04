import type { Metadata } from "next";
import { Clock3, MonitorCog, ShieldCheck, WifiOff } from "lucide-react";

import { LoginForm } from "@/components/features/auth/login-form";

export const metadata: Metadata = {
  title: "Giriş | ArenaDesk",
  description: "ArenaDesk kassir we administrator giriş sahypasy",
};

const benefits = [
  {
    icon: MonitorCog,
    title: "10 kompýuter",
    description: "Ähli stansiýalary bir panelden dolandyr.",
  },
  {
    icon: Clock3,
    title: "Takyk wagt",
    description: "Sessiýalary we galan wagty real wagtda yzarla.",
  },
  {
    icon: WifiOff,
    title: "Offline iş",
    description: "Internet bolmasa-da lokal ulgamda işlemegi dowam etdir.",
  },
];

export default function LoginPage() {
  return (
    <main className="relative min-h-screen overflow-hidden bg-[#070a12] text-white">
      <div
        aria-hidden="true"
        className="pointer-events-none absolute inset-0 bg-[radial-gradient(circle_at_15%_15%,rgba(99,102,241,0.18),transparent_35%),radial-gradient(circle_at_85%_85%,rgba(34,197,94,0.08),transparent_32%)]"
      />

      <div className="relative mx-auto grid min-h-screen w-full max-w-7xl lg:grid-cols-[1.05fr_0.95fr]">
        <section className="hidden border-r border-white/10 px-12 py-10 lg:flex lg:flex-col lg:justify-between xl:px-20">
          <div className="flex items-center gap-3">
            <span className="grid size-11 place-items-center rounded-xl border border-indigo-400/25 bg-indigo-500/15">
              <MonitorCog aria-hidden="true" className="size-5 text-indigo-300" />
            </span>
            <div>
              <p className="text-xl font-semibold tracking-tight">ArenaDesk</p>
              <p className="text-xs text-slate-400">Gaming club management</p>
            </div>
          </div>

          <div className="max-w-xl py-16">
            <span className="inline-flex items-center gap-2 rounded-full border border-emerald-400/20 bg-emerald-400/10 px-3 py-1.5 text-xs font-medium text-emerald-300">
              <ShieldCheck aria-hidden="true" className="size-3.5" />
              Lokal we ygtybarly dolandyryş
            </span>
            <h1 className="mt-7 text-balance text-5xl font-semibold leading-[1.06] tracking-[-0.045em] xl:text-6xl">
              Klubuň ähli işi bir ekranda.
            </h1>
            <p className="mt-6 max-w-lg text-lg leading-8 text-slate-400">
              Kompýuterler, wagt, tarifler we tölegler üçin düşnükli dolandyryş
              paneli.
            </p>

            <div className="mt-10 grid gap-4">
              {benefits.map((benefit) => {
                const Icon = benefit.icon;

                return (
                  <article
                    key={benefit.title}
                    className="flex gap-4 rounded-2xl border border-white/8 bg-white/[0.025] p-4"
                  >
                    <span className="grid size-10 shrink-0 place-items-center rounded-xl bg-white/5 text-indigo-300">
                      <Icon aria-hidden="true" className="size-4.5" />
                    </span>
                    <div>
                      <h2 className="font-medium text-slate-100">{benefit.title}</h2>
                      <p className="mt-1 text-sm leading-6 text-slate-500">
                        {benefit.description}
                      </p>
                    </div>
                  </article>
                );
              })}
            </div>
          </div>

          <p className="text-xs text-slate-600">ArenaDesk MVP · Stage 14</p>
        </section>

        <section className="flex min-h-screen items-center justify-center px-5 py-10 sm:px-10 lg:px-14">
          <div className="w-full max-w-md">
            <div className="mb-8 flex items-center gap-3 lg:hidden">
              <span className="grid size-10 place-items-center rounded-xl border border-indigo-400/25 bg-indigo-500/15">
                <MonitorCog aria-hidden="true" className="size-5 text-indigo-300" />
              </span>
              <div>
                <p className="text-lg font-semibold">ArenaDesk</p>
                <p className="text-xs text-slate-400">Gaming club management</p>
              </div>
            </div>

            <div className="mb-8">
              <p className="text-sm font-medium text-indigo-300">Hoş geldiňiz</p>
              <h2 className="mt-2 text-3xl font-semibold tracking-[-0.035em]">
                Ulgama giriş
              </h2>
              <p className="mt-3 text-sm leading-6 text-slate-400">
                Iş nobatyňyzy başlamak üçin maglumatlaryňyzy giriziň.
              </p>
            </div>

            <LoginForm />
          </div>
        </section>
      </div>
    </main>
  );
}
