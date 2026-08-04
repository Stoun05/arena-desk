"use client";

import { useEffect, useMemo, useState } from "react";
import {
  Banknote,
  CheckCircle2,
  Clock3,
  MonitorCheck,
  MonitorCog,
  Plus,
  ReceiptText,
  Server,
  UserRound,
} from "lucide-react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
} from "@/components/ui/sheet";
import type { ComputerStation, PaymentMethod } from "@/types/computer";

import { ComputerStationGrid } from "./computer-station-grid";

const durations = [
  { minutes: 30, label: "30 min" },
  { minutes: 60, label: "1 sagat" },
  { minutes: 120, label: "2 sagat" },
  { minutes: 180, label: "3 sagat" },
];

function roundMoney(value: number) {
  return Math.round(value * 100) / 100;
}

function formatClock(timestamp: number) {
  return new Date(timestamp).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
}

function SessionSheet({
  open,
  onOpenChange,
  stations,
  stationId,
  onStationChange,
  duration,
  onDurationChange,
  paymentMethod,
  onPaymentChange,
  customer,
  onCustomerChange,
  openedAt,
  onConfirm,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  stations: ComputerStation[];
  stationId: string;
  onStationChange: (stationId: string) => void;
  duration: number;
  onDurationChange: (duration: number) => void;
  paymentMethod: PaymentMethod;
  onPaymentChange: (method: PaymentMethod) => void;
  customer: string;
  onCustomerChange: (customer: string) => void;
  openedAt: number | null;
  onConfirm: () => void;
}) {
  const selectedStation = stations.find((station) => station.id === stationId);
  const total = selectedStation ? roundMoney((selectedStation.hourlyRate * duration) / 60) : 0;
  const endingAt = openedAt ? formatClock(openedAt + duration * 60_000) : "—";

  return (
    <Sheet open={open} onOpenChange={onOpenChange}>
      <SheetContent side="right" className="w-full overflow-y-auto border-white/10 bg-[#090d17] p-0 text-white sm:max-w-lg">
        <SheetHeader className="border-b border-white/10 px-6 py-5 text-left">
          <SheetTitle className="text-xl text-white">Täze sessiýa</SheetTitle>
          <SheetDescription className="text-slate-400">
            Kompýuteri, wagty we töleg görnüşini saýlaň.
          </SheetDescription>
        </SheetHeader>

        <div className="space-y-6 px-6 py-5">
          <div className="space-y-2">
            <Label htmlFor="session-computer" className="text-slate-300">Kompýuter</Label>
            <Select value={stationId} onValueChange={onStationChange}>
              <SelectTrigger id="session-computer" className="h-11 w-full border-white/10 bg-white/[0.035] text-white">
                <SelectValue placeholder="Kompýuteri saýlaň" />
              </SelectTrigger>
              <SelectContent>
                {stations.filter((station) => station.status === "available").map((station) => (
                  <SelectItem key={station.id} value={station.id}>
                    {station.name} · {station.tier === "vip" ? "VIP" : "Standard"}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2">
            <Label htmlFor="session-customer" className="text-slate-300">Müşderi</Label>
            <div className="relative">
              <UserRound aria-hidden="true" className="absolute left-3 top-3.5 size-4 text-slate-600" />
              <Input
                id="session-customer"
                value={customer}
                onChange={(event) => onCustomerChange(event.target.value)}
                placeholder="Mysal: Müşderi #1052"
                className="h-11 border-white/10 bg-white/[0.035] pl-10 text-white placeholder:text-slate-600"
              />
            </div>
          </div>

          <fieldset>
            <legend className="text-sm font-medium text-slate-300">Dowamlylygy</legend>
            <div className="mt-3 grid grid-cols-2 gap-2 sm:grid-cols-4">
              {durations.map((option) => (
                <button
                  key={option.minutes}
                  type="button"
                  onClick={() => onDurationChange(option.minutes)}
                  aria-pressed={duration === option.minutes}
                  className={`rounded-xl border px-3 py-3 text-sm transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-indigo-400 ${
                    duration === option.minutes
                      ? "border-indigo-400/50 bg-indigo-500/15 text-indigo-200"
                      : "border-white/10 bg-white/[0.025] text-slate-400 hover:border-white/20"
                  }`}
                >
                  {option.label}
                </button>
              ))}
            </div>
          </fieldset>

          <fieldset>
            <legend className="text-sm font-medium text-slate-300">Töleg görnüşi</legend>
            <div className="mt-3 grid grid-cols-2 gap-2">
              {(["cash", "card"] as PaymentMethod[]).map((method) => (
                <button
                  key={method}
                  type="button"
                  onClick={() => onPaymentChange(method)}
                  aria-pressed={paymentMethod === method}
                  className={`rounded-xl border px-4 py-3 text-sm transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-indigo-400 ${
                    paymentMethod === method
                      ? "border-emerald-400/40 bg-emerald-400/10 text-emerald-200"
                      : "border-white/10 bg-white/[0.025] text-slate-400 hover:border-white/20"
                  }`}
                >
                  {method === "cash" ? "Nagt" : "Kart"}
                </button>
              ))}
            </div>
          </fieldset>

          <div className="rounded-2xl border border-indigo-400/20 bg-indigo-400/[0.07] p-4">
            <div className="flex items-center gap-2 text-sm font-medium text-indigo-200">
              <ReceiptText aria-hidden="true" className="size-4" />
              Sessiýa jemlemesi
            </div>
            <dl className="mt-4 space-y-3 text-sm">
              <div className="flex justify-between gap-4">
                <dt className="text-slate-500">Tarif</dt>
                <dd className="text-slate-200">
                  {selectedStation ? `${selectedStation.tier === "vip" ? "VIP" : "Standard"} · ${selectedStation.hourlyRate} TMT/sag` : "—"}
                </dd>
              </div>
              <div className="flex justify-between gap-4">
                <dt className="text-slate-500">Gutarýan wagt</dt>
                <dd className="text-slate-200">{endingAt}</dd>
              </div>
              <div className="flex justify-between gap-4 border-t border-white/10 pt-3">
                <dt className="font-medium text-slate-200">Jemi töleg</dt>
                <dd className="text-lg font-semibold text-white">{total.toFixed(2)} TMT</dd>
              </div>
            </dl>
          </div>

          <Button
            type="button"
            size="lg"
            disabled={!selectedStation}
            onClick={onConfirm}
            className="h-11 w-full bg-indigo-500 text-white hover:bg-indigo-400"
          >
            <CheckCircle2 aria-hidden="true" data-icon="inline-start" />
            Tölegi tassykla we başlat
          </Button>
          <p className="text-center text-[11px] leading-5 text-slate-600">
            Demo režimi: maglumatlar backend-e ýazylmaýar.
          </p>
        </div>
      </SheetContent>
    </Sheet>
  );
}

export function ComputerDashboard({ initialStations }: { initialStations: ComputerStation[] }) {
  const [stations, setStations] = useState(initialStations);
  const [sessionOpen, setSessionOpen] = useState(false);
  const [sessionStationId, setSessionStationId] = useState("");
  const [duration, setDuration] = useState(60);
  const [paymentMethod, setPaymentMethod] = useState<PaymentMethod>("cash");
  const [customer, setCustomer] = useState("");
  const [openedAt, setOpenedAt] = useState<number | null>(null);
  const [notice, setNotice] = useState("");

  useEffect(() => {
    const timer = window.setInterval(() => {
      setStations((current) => current.map((station) => {
        if (station.remainingSeconds === undefined || station.remainingSeconds <= 0) return station;

        const remainingSeconds = station.remainingSeconds - 1;
        const status = remainingSeconds === 0 ? "locked" : remainingSeconds <= 600 ? "ending" : "occupied";
        return { ...station, remainingSeconds, status };
      }));
    }, 1000);

    return () => window.clearInterval(timer);
  }, []);

  const activeSessions = stations.filter(
    (station) => station.status === "occupied" || station.status === "ending",
  ).length;
  const availableStations = stations.filter((station) => station.status === "available");
  const currentRevenue = useMemo(
    () => stations.reduce((total, station) => total + (station.currentCharge ?? 0), 0),
    [stations],
  );

  function openSession(stationId?: string) {
    const station = stations.find((item) => item.id === stationId && item.status === "available") ?? availableStations[0];
    if (!station) return;

    setSessionStationId(station.id);
    setDuration(60);
    setPaymentMethod("cash");
    setCustomer("");
    setOpenedAt(Date.now());
    setSessionOpen(true);
  }

  function startSession() {
    const startedAt = Date.now();
    const station = stations.find((item) => item.id === sessionStationId);
    if (!station || station.status !== "available") return;

    const charge = roundMoney((station.hourlyRate * duration) / 60);
    setStations((current) => current.map((item) => item.id === station.id ? {
      ...item,
      status: duration <= 10 ? "ending" : "occupied",
      customer: customer.trim() || "Walk-in müşderi",
      startedAt: formatClock(startedAt),
      endsAt: formatClock(startedAt + duration * 60_000),
      remainingSeconds: duration * 60,
      currentCharge: charge,
      paymentMethod,
    } : item));
    setSessionOpen(false);
    setNotice(`${station.name} üçin ${duration} minutlyk sessiýa başlady.`);
  }

  function addTime(stationId: string) {
    setStations((current) => current.map((station) => station.id === stationId ? {
      ...station,
      status: "occupied",
      remainingSeconds: (station.remainingSeconds ?? 0) + 1800,
      currentCharge: roundMoney((station.currentCharge ?? 0) + station.hourlyRate / 2),
    } : station));
    setNotice("Sessiýa 30 minut uzaldyldy we töleg täzelendi.");
  }

  function finishSession(stationId: string) {
    const station = stations.find((item) => item.id === stationId);
    setStations((current) => current.map((item) => item.id === stationId ? {
      ...item,
      status: "available",
      customer: undefined,
      startedAt: undefined,
      endsAt: undefined,
      remainingSeconds: undefined,
      currentCharge: undefined,
      paymentMethod: undefined,
    } : item));
    setNotice(`${station?.name ?? "Kompýuter"} sessiýasy tamamlandy.`);
  }

  const statistics = [
    { label: "Ähli kompýuterler", value: String(stations.length), detail: "6 Standard · 4 VIP", icon: MonitorCog, color: "text-indigo-300", background: "bg-indigo-400/10" },
    { label: "Aktiw sessiýalar", value: String(activeSessions), detail: "Real wagt demo taýmeri", icon: Clock3, color: "text-amber-300", background: "bg-amber-400/10" },
    { label: "Boş kompýuterler", value: String(availableStations.length), detail: "Täze sessiýa taýýar", icon: MonitorCheck, color: "text-emerald-300", background: "bg-emerald-400/10" },
    { label: "Aktiw töleg", value: `${currentRevenue.toFixed(2)} TMT`, detail: "Demo sessiýalar boýunça", icon: Banknote, color: "text-sky-300", background: "bg-sky-400/10" },
  ];

  return (
    <div className="flex flex-col gap-7">
      <section className="flex flex-col justify-between gap-4 sm:flex-row sm:items-end">
        <div>
          <p className="text-sm font-medium text-indigo-300">Esasy panel · Stage 5</p>
          <h1 className="mt-1 text-3xl font-semibold tracking-[-0.035em] text-white">Klubuň ýagdaýy</h1>
          <p className="mt-2 text-sm text-slate-400">10 kompýuteriň sessiýalaryny bir ekrandan dolandyryň.</p>
        </div>
        <Button
          onClick={() => openSession()}
          disabled={availableStations.length === 0}
          className="self-start bg-indigo-500 text-white hover:bg-indigo-400 sm:self-auto"
        >
          <Plus aria-hidden="true" data-icon="inline-start" />
          Täze sessiýa
        </Button>
      </section>

      {notice ? (
        <div role="status" className="flex items-center gap-3 rounded-xl border border-emerald-400/20 bg-emerald-400/[0.07] px-4 py-3 text-sm text-emerald-200">
          <CheckCircle2 aria-hidden="true" className="size-4 shrink-0" />
          {notice}
        </div>
      ) : null}

      <section aria-label="Server ýagdaýy" className="grid gap-3 rounded-2xl border border-white/10 bg-white/[0.025] p-4 sm:grid-cols-2 sm:p-5">
        <div className="flex items-center gap-3">
          <span className="grid size-10 place-items-center rounded-xl bg-emerald-400/10 text-emerald-300">
            <Server aria-hidden="true" className="size-4.5" />
          </span>
          <div>
            <p className="text-sm font-medium text-slate-200">ASP.NET Core API</p>
            <p className="mt-1 text-xs text-emerald-300">Stage 6 binýady taýýar</p>
          </div>
        </div>
        <div className="flex items-center gap-3 sm:justify-end">
          <span aria-hidden="true" className="size-2 rounded-full bg-amber-400" />
          <div className="sm:text-right">
            <p className="text-sm text-slate-300">PostgreSQL</p>
            <p className="mt-1 text-xs text-amber-300">Stage 7-de birikdiriler</p>
          </div>
        </div>
      </section>

      <section aria-label="Gysga statistika" className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        {statistics.map((statistic) => {
          const Icon = statistic.icon;
          return (
            <article key={statistic.label} className="rounded-2xl border border-white/10 bg-white/[0.025] p-5">
              <div className="flex items-start justify-between gap-4">
                <div>
                  <p className="text-sm text-slate-400">{statistic.label}</p>
                  <p className="mt-3 text-3xl font-semibold tracking-tight text-white">{statistic.value}</p>
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

      <ComputerStationGrid
        stations={stations}
        onStartSession={openSession}
        onAddTime={addTime}
        onFinishSession={finishSession}
      />

      <SessionSheet
        open={sessionOpen}
        onOpenChange={setSessionOpen}
        stations={stations}
        stationId={sessionStationId}
        onStationChange={setSessionStationId}
        duration={duration}
        onDurationChange={setDuration}
        paymentMethod={paymentMethod}
        onPaymentChange={setPaymentMethod}
        customer={customer}
        onCustomerChange={setCustomer}
        openedAt={openedAt}
        onConfirm={startSession}
      />
    </div>
  );
}
