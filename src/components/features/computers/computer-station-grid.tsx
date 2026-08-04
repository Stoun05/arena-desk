"use client";

import { useState } from "react";
import {
  Banknote,
  Clock3,
  Crown,
  LockKeyhole,
  Monitor,
  Play,
  Plus,
  Power,
  UserRound,
  WalletCards,
  WifiOff,
} from "lucide-react";

import { Button } from "@/components/ui/button";
import type { ComputerStation, ComputerStatus } from "@/types/computer";

function formatDuration(totalSeconds?: number) {
  if (totalSeconds === undefined) return undefined;

  const hours = Math.floor(totalSeconds / 3600);
  const minutes = Math.floor((totalSeconds % 3600) / 60);
  const seconds = totalSeconds % 60;

  return [hours, minutes, seconds].map((value) => String(value).padStart(2, "0")).join(":");
}

const statusStyles: Record<
  ComputerStatus,
  { label: string; dot: string; badge: string; border: string; glow: string }
> = {
  available: {
    label: "Boş",
    dot: "bg-emerald-400",
    badge: "border-emerald-400/20 bg-emerald-400/10 text-emerald-300",
    border: "hover:border-emerald-400/35",
    glow: "bg-emerald-400/10 text-emerald-300",
  },
  occupied: {
    label: "Ulanylýar",
    dot: "bg-sky-400",
    badge: "border-sky-400/20 bg-sky-400/10 text-sky-300",
    border: "hover:border-sky-400/35",
    glow: "bg-sky-400/10 text-sky-300",
  },
  ending: {
    label: "Wagt gutarýar",
    dot: "bg-amber-400",
    badge: "border-amber-400/20 bg-amber-400/10 text-amber-300",
    border: "hover:border-amber-400/40",
    glow: "bg-amber-400/10 text-amber-300",
  },
  locked: {
    label: "Gulply",
    dot: "bg-violet-400",
    badge: "border-violet-400/20 bg-violet-400/10 text-violet-300",
    border: "hover:border-violet-400/35",
    glow: "bg-violet-400/10 text-violet-300",
  },
  offline: {
    label: "Offline",
    dot: "bg-slate-500",
    badge: "border-slate-500/20 bg-slate-500/10 text-slate-400",
    border: "hover:border-slate-500/35",
    glow: "bg-slate-500/10 text-slate-400",
  },
};

function StationIcon({ status }: { status: ComputerStatus }) {
  const className = "size-5";

  if (status === "offline") return <WifiOff aria-hidden="true" className={className} />;
  if (status === "locked") return <LockKeyhole aria-hidden="true" className={className} />;

  return <Monitor aria-hidden="true" className={className} />;
}

function StationCard({
  station,
  selected,
  onSelect,
}: {
  station: ComputerStation;
  selected: boolean;
  onSelect: () => void;
}) {
  const style = statusStyles[station.status];

  return (
    <button
      type="button"
      onClick={onSelect}
      aria-pressed={selected}
      className={`group rounded-2xl border bg-[#0b101b] p-4 text-left transition-all focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-indigo-400 ${
        selected
          ? "border-indigo-400/60 shadow-lg shadow-indigo-950/30"
          : `border-white/10 ${style.border}`
      }`}
    >
      <div className="flex items-start justify-between gap-3">
        <span className={`grid size-10 place-items-center rounded-xl ${style.glow}`}>
          <StationIcon status={station.status} />
        </span>
        <span className={`inline-flex items-center gap-1.5 rounded-full border px-2.5 py-1 text-[11px] font-medium ${style.badge}`}>
          <span aria-hidden="true" className={`size-1.5 rounded-full ${style.dot}`} />
          {style.label}
        </span>
      </div>

      <div className="mt-5 flex items-end justify-between gap-3">
        <div>
          <div className="flex items-center gap-2">
            <h3 className="text-base font-semibold text-white">{station.name}</h3>
            {station.tier === "vip" ? (
              <Crown aria-label="VIP" className="size-3.5 text-amber-300" />
            ) : null}
          </div>
          <p className="mt-1 text-xs text-slate-500">
            {station.tier === "vip" ? "VIP" : "Standard"} · {station.hourlyRate} TMT/sag
          </p>
        </div>

        {station.remainingSeconds !== undefined ? (
          <div className="text-right">
            <p className="font-mono text-sm font-medium text-slate-200">{formatDuration(station.remainingSeconds)}</p>
            <p className="mt-1 text-[10px] uppercase tracking-wide text-slate-600">galan wagt</p>
          </div>
        ) : null}
      </div>
    </button>
  );
}

function DetailRow({
  icon: Icon,
  label,
  value,
}: {
  icon: typeof Clock3;
  label: string;
  value: string;
}) {
  return (
    <div className="flex items-center gap-3 rounded-xl border border-white/8 bg-white/[0.025] p-3">
      <span className="grid size-9 place-items-center rounded-lg bg-white/5 text-slate-400">
        <Icon aria-hidden="true" className="size-4" />
      </span>
      <div className="min-w-0">
        <p className="text-[11px] text-slate-500">{label}</p>
        <p className="mt-0.5 truncate text-sm font-medium text-slate-200">{value}</p>
      </div>
    </div>
  );
}

type ComputerStationGridProps = {
  stations: ComputerStation[];
  onStartSession: (stationId: string) => void;
  onAddTime: (stationId: string) => void;
  onFinishSession: (stationId: string) => void;
};

export function ComputerStationGrid({
  stations,
  onStartSession,
  onAddTime,
  onFinishSession,
}: ComputerStationGridProps) {
  const [selectedId, setSelectedId] = useState(stations[1]?.id ?? stations[0]?.id);
  const selectedStation = stations.find((station) => station.id === selectedId) ?? stations[0];
  const style = statusStyles[selectedStation.status];
  const isActive = selectedStation.status === "occupied" || selectedStation.status === "ending";

  return (
    <section className="grid gap-5 xl:grid-cols-[1.55fr_0.75fr]">
      <article className="rounded-2xl border border-white/10 bg-white/[0.025] p-5 sm:p-6">
        <div className="flex flex-col justify-between gap-3 sm:flex-row sm:items-center">
          <div>
            <h2 className="font-medium text-slate-100">Kompýuterleriň ýagdaýy</h2>
            <p className="mt-1 text-sm text-slate-500">Kartany saýlap, jikme-jik maglumatyny görüň</p>
          </div>
          <span className="self-start rounded-full border border-indigo-400/20 bg-indigo-400/10 px-3 py-1 text-xs text-indigo-300 sm:self-auto">
            Demo maglumat
          </span>
        </div>

        <div className="mt-6 grid gap-3 sm:grid-cols-2 2xl:grid-cols-3">
          {stations.map((station) => (
            <StationCard
              key={station.id}
              station={station}
              selected={station.id === selectedStation.id}
              onSelect={() => setSelectedId(station.id)}
            />
          ))}
        </div>
      </article>

      <aside aria-live="polite" className="self-start rounded-2xl border border-white/10 bg-white/[0.025] p-5 sm:p-6 xl:sticky xl:top-24">
        <div className="flex items-start justify-between gap-4">
          <div>
            <p className="text-xs font-medium uppercase tracking-[0.18em] text-slate-500">Saýlanan stansiýa</p>
            <div className="mt-2 flex items-center gap-2">
              <h2 className="text-2xl font-semibold text-white">{selectedStation.name}</h2>
              {selectedStation.tier === "vip" ? <Crown aria-hidden="true" className="size-4 text-amber-300" /> : null}
            </div>
          </div>
          <span className={`inline-flex items-center gap-1.5 rounded-full border px-2.5 py-1 text-[11px] font-medium ${style.badge}`}>
            <span aria-hidden="true" className={`size-1.5 rounded-full ${style.dot}`} />
            {style.label}
          </span>
        </div>

        <div className="mt-6 space-y-3">
          <DetailRow
            icon={Crown}
            label="Kompýuter görnüşi"
            value={`${selectedStation.tier === "vip" ? "VIP" : "Standard"} · ${selectedStation.hourlyRate} TMT/sag`}
          />
          <DetailRow icon={UserRound} label="Müşderi" value={selectedStation.customer ?? "Müşderi ýok"} />
          <DetailRow
            icon={Clock3}
            label={isActive ? "Galan wagt" : "Sessiýa wagty"}
            value={formatDuration(selectedStation.remainingSeconds) ?? "Sessiýa açylmady"}
          />
          <DetailRow
            icon={Banknote}
            label="Häzirki töleg"
            value={selectedStation.currentCharge ? `${selectedStation.currentCharge} TMT` : "0 TMT"}
          />
          <DetailRow
            icon={WalletCards}
            label="Töleg görnüşi"
            value={selectedStation.paymentMethod === "card" ? "Kart" : selectedStation.paymentMethod === "cash" ? "Nagt" : "Bellige alynmady"}
          />
        </div>

        <div className="mt-6 grid gap-2 sm:grid-cols-2 xl:grid-cols-1 2xl:grid-cols-2">
          <Button
            disabled={!isActive && selectedStation.status !== "available"}
            onClick={() => isActive ? onAddTime(selectedStation.id) : onStartSession(selectedStation.id)}
            className="bg-indigo-500 text-white hover:bg-indigo-400"
          >
            {isActive ? <Plus aria-hidden="true" data-icon="inline-start" /> : <Play aria-hidden="true" data-icon="inline-start" />}
            {isActive ? "+30 minut" : "Sessiýa aç"}
          </Button>
          <Button
            disabled={!isActive}
            onClick={() => onFinishSession(selectedStation.id)}
            variant="outline"
            className="border-white/10 bg-white/[0.03] text-slate-300"
          >
            <Power aria-hidden="true" data-icon="inline-start" />
            Tamamla
          </Button>
        </div>
        <p className="mt-3 text-center text-[11px] leading-5 text-slate-600">
          Demo amallary diňe şu brauzer sessiýasynda saklanýar.
        </p>
      </aside>
    </section>
  );
}
