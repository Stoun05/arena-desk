import {
  CheckCircle2,
  Circle,
  Layers3,
  MonitorCog,
  Server,
  ShieldCheck,
} from "lucide-react";

import { Button } from "@/components/ui/button";

const technologies = [
  {
    name: "Next.js",
    version: "16.3.0",
    description: "App Router we server komponentleri",
    icon: Layers3,
  },
  {
    name: "TypeScript",
    version: "Strict",
    description: "Ygtybarly type barlagy",
    icon: ShieldCheck,
  },
  {
    name: "Tailwind CSS",
    version: "4.x",
    description: "Responsive dizaýn gurluşy",
    icon: MonitorCog,
  },
  {
    name: "shadcn/ui",
    version: "Radix",
    description: "Elýeterli dashboard komponentleri",
    icon: Server,
  },
];

export default function Home() {
  return (
    <main className="relative min-h-screen overflow-hidden bg-[#070a12] px-5 py-8 text-white sm:px-8 lg:px-12">
      <div
        aria-hidden="true"
        className="pointer-events-none absolute inset-x-0 top-0 h-96 bg-[radial-gradient(circle_at_top_right,rgba(99,102,241,0.18),transparent_48%),radial-gradient(circle_at_top_left,rgba(34,197,94,0.10),transparent_40%)]"
      />

      <div className="relative mx-auto flex w-full max-w-6xl flex-col gap-12">
        <header className="flex items-center justify-between border-b border-white/10 pb-5">
          <div className="flex items-center gap-3">
            <span className="grid size-10 place-items-center rounded-xl border border-indigo-400/25 bg-indigo-500/15">
              <MonitorCog aria-hidden="true" className="size-5 text-indigo-300" />
            </span>
            <div>
              <p className="text-lg font-semibold tracking-tight">ArenaDesk</p>
              <p className="text-xs text-slate-400">Gaming club management</p>
            </div>
          </div>

          <span className="rounded-full border border-emerald-400/20 bg-emerald-400/10 px-3 py-1 text-xs font-medium text-emerald-300">
            Stage 2 ready
          </span>
        </header>

        <section className="grid items-end gap-10 lg:grid-cols-[1.25fr_0.75fr]">
          <div className="max-w-3xl">
            <p className="mb-4 text-sm font-medium uppercase tracking-[0.22em] text-indigo-300">
              Web-panel foundation
            </p>
            <h1 className="text-balance text-4xl font-semibold tracking-[-0.04em] text-white sm:text-6xl">
              Tehniki binýat taýýar.
            </h1>
            <p className="mt-6 max-w-2xl text-pretty text-base leading-7 text-slate-400 sm:text-lg">
              ArenaDesk indi TypeScript, Tailwind CSS we shadcn/ui bilen işleýän
              Next.js gurluşyna eýe. Indiki tapgyrda login we esasy panel şu
              binýadyň üstünde gurlar.
            </p>

            <div className="mt-8 flex flex-wrap gap-3">
              <Button disabled size="lg" className="bg-indigo-500 text-white">
                Dashboard — indiki tapgyr
              </Button>
              <Button variant="outline" size="lg" asChild>
                <a href="https://github.com/Stoun05/arena-desk/blob/main/docs/requirements.md">
                  MVP talaplary
                </a>
              </Button>
            </div>
          </div>

          <aside className="rounded-2xl border border-white/10 bg-white/[0.035] p-6 shadow-2xl shadow-black/20 backdrop-blur">
            <p className="text-sm font-medium text-slate-200">Tapgyr ýagdaýy</p>
            <ol className="mt-5 space-y-4 text-sm">
              <li className="flex items-center gap-3 text-emerald-300">
                <CheckCircle2 aria-hidden="true" className="size-5" />
                <span>1. MVP talaplary</span>
              </li>
              <li className="flex items-center gap-3 text-emerald-300">
                <CheckCircle2 aria-hidden="true" className="size-5" />
                <span>2. Tehniki taýýarlyk</span>
              </li>
              <li className="flex items-center gap-3 text-slate-500">
                <Circle aria-hidden="true" className="size-5" />
                <span>3. Login we esasy panel</span>
              </li>
            </ol>
          </aside>
        </section>

        <section aria-labelledby="stack-title">
          <div className="mb-5 flex items-center justify-between">
            <h2 id="stack-title" className="text-lg font-medium text-slate-100">
              Taýýar tehnologiýalar
            </h2>
            <p className="hidden text-sm text-slate-500 sm:block">4/4 configured</p>
          </div>

          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
            {technologies.map((technology) => {
              const Icon = technology.icon;

              return (
                <article
                  key={technology.name}
                  className="rounded-2xl border border-white/10 bg-white/[0.025] p-5 transition-colors hover:border-indigo-400/25 hover:bg-indigo-400/[0.04]"
                >
                  <div className="flex items-start justify-between gap-4">
                    <span className="grid size-9 place-items-center rounded-lg bg-white/5 text-slate-300">
                      <Icon aria-hidden="true" className="size-4" />
                    </span>
                    <span className="rounded-md bg-white/5 px-2 py-1 text-[11px] text-slate-400">
                      {technology.version}
                    </span>
                  </div>
                  <h3 className="mt-5 font-medium text-white">{technology.name}</h3>
                  <p className="mt-2 text-sm leading-6 text-slate-500">
                    {technology.description}
                  </p>
                </article>
              );
            })}
          </div>
        </section>
      </div>
    </main>
  );
}
