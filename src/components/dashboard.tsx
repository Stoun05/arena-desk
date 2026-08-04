"use client";

import {
  Bell,
  ChevronRight,
  CircleDollarSign,
  Clock3,
  Gamepad2,
  LayoutDashboard,
  LogOut,
  Monitor,
  MonitorCheck,
  Moon,
  Plus,
  Power,
  ReceiptText,
  RotateCcw,
  Search,
  Settings,
  ShoppingBasket,
  Users,
  X,
} from "lucide-react";
import { useEffect, useMemo, useState } from "react";
import type { ComputerStation, ComputerStatus } from "@/types/computer";
import { extendSession as extendSessionRequest, finishSession as finishSessionRequest, isDemoMode, startSession as startSessionRequest, type AuthUser, type Tariff } from "@/lib/api-client";

const statusCopy: Record<ComputerStatus, string> = {
  available: "Boş",
  active: "Ulanylýar",
  warning: "Wagt gutarýar",
  offline: "Offline",
};

const navigation = [
  { label: "Dolandyryş", icon: LayoutDashboard, active: true },
  { label: "Sessiýalar", icon: Clock3 },
  { label: "Müşderiler", icon: Users },
  { label: "Oýunlar", icon: Gamepad2 },
  { label: "Satuw", icon: ShoppingBasket },
  { label: "Hasabat", icon: ReceiptText, adminOnly: true },
];

function formatTime(totalSeconds?: number) {
  if (totalSeconds === undefined) return "Taýýar";
  const hours = Math.floor(totalSeconds / 3600);
  const minutes = Math.floor((totalSeconds % 3600) / 60);
  const seconds = totalSeconds % 60;
  return [hours, minutes, seconds].map((value) => String(value).padStart(2, "0")).join(":");
}

export function Dashboard({ currentUser, initialStations, tariffs, onLogout }: { currentUser: AuthUser; initialStations: ComputerStation[]; tariffs: Tariff[]; onLogout: () => Promise<void> }) {
  const [computers, setComputers] = useState(initialStations);
  const [selectedId, setSelectedId] = useState(initialStations[0]?.id ?? "");
  const [isSessionOpen, setIsSessionOpen] = useState(false);
  const [notice, setNotice] = useState(isDemoMode ? "GitHub Pages demo režimi — üýtgeşmeler diňe şu brauzer sessiýasynda görkezilýär." : "ArenaDesk API bilen baglanyşyk işjeň.");

  useEffect(() => {
    const interval = window.setInterval(() => {
      setComputers((current) =>
        current.map((computer) => {
          if (!computer.remainingSeconds || !["active", "warning"].includes(computer.status)) return computer;
          const remainingSeconds = Math.max(0, computer.remainingSeconds - 1);
          return {
            ...computer,
            remainingSeconds,
            status: remainingSeconds <= 300 ? "warning" : computer.status,
          };
        }),
      );
    }, 1000);
    return () => window.clearInterval(interval);
  }, []);

  const selectedComputer = computers.find((computer) => computer.id === selectedId) ?? computers[0];
  const activeCount = computers.filter((computer) => ["active", "warning"].includes(computer.status)).length;
  const availableCount = computers.filter((computer) => computer.status === "available").length;

  const finishSession = async () => {
    if (!selectedComputer?.sessionId || !["active", "warning"].includes(selectedComputer.status)) return;
    try {
      await finishSessionRequest(selectedComputer.sessionId);
    setComputers((current) => current.map((computer) => computer.id === selectedComputer.id
      ? { id: computer.id, databaseId: computer.databaseId, zone: computer.zone, status: "available" }
      : computer));
    setNotice(`${selectedComputer.id} sessiýasy tamamlandy we kompýuter gulplandy.`);
    } catch (error) {
      setNotice(error instanceof Error ? error.message : "Sessiýany tamamlap bolmady.");
    }
  };

  const extendSession = async () => {
    if (!selectedComputer?.sessionId || !["active", "warning"].includes(selectedComputer.status)) return;
    try {
      const result = await extendSessionRequest(selectedComputer.sessionId, 30);
    setComputers((current) => current.map((computer) => computer.id === selectedComputer.id
      ? { ...computer, remainingSeconds: Math.max(0, Math.floor((new Date(result.endsAt).getTime() - Date.now()) / 1000)), sessionPrice: result.totalPrice, status: "active" }
      : computer));
    setNotice(`${selectedComputer.id} üçin 30 minut goşuldy.`);
    } catch (error) {
      setNotice(error instanceof Error ? error.message : "Wagt goşup bolmady.");
    }
  };

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <span className="brand-mark"><Gamepad2 size={20} /></span>
          <span><strong>ArenaDesk</strong><small>Gaming club manager</small></span>
        </div>

        <nav aria-label="Esasy menýu" className="nav-list">
          {navigation.filter((item) => !item.adminOnly || currentUser.role === "admin").map(({ label, icon: Icon, active }) => (
            <button className={active ? "nav-item active" : "nav-item"} key={label} type="button">
              <Icon size={18} /><span>{label}</span>
            </button>
          ))}
        </nav>

        <div className="sidebar-footer">
          {currentUser.role === "admin" && <button className="nav-item" type="button"><Settings size={18} /><span>Sazlamalar</span></button>}
          <div className="operator"><span className="avatar">{currentUser.role === "admin" ? "AD" : "KA"}</span><span><strong>{currentUser.name}</strong><small>{currentUser.role === "admin" ? "Administrator" : "Kassir"}</small></span><button className="logout-icon" type="button" onClick={onLogout} aria-label="Ulgamdan çyk"><LogOut size={16} /></button></div>
        </div>
      </aside>

      <main className="main-content">
        <header className="topbar">
          <div><p className="eyebrow">04.08.2026 · Sişenbe</p><h1>Dolandyryş paneli</h1></div>
          <div className="top-actions">
            <span className={`role-badge ${currentUser.role}`}>{currentUser.role === "admin" ? "Administrator" : "Kassir"}</span>
            <label className="search-box"><Search size={17} /><span className="sr-only">Kompýuter gözle</span><input placeholder="PC gözle..." /></label>
            <button className="icon-button" type="button" aria-label="Bildirişler"><Bell size={18} /><span className="notification-dot" /></button>
            <button className="primary-button" type="button" onClick={() => setIsSessionOpen(true)}><Plus size={18} />Sessiya aç</button>
          </div>
        </header>

        <div className="demo-notice"><span>{notice}</span><button type="button" onClick={() => setNotice("")} aria-label="Habary ýap"><X size={16} /></button></div>

        <section className="stats-grid" aria-label="Gysga hasabat">
          <StatCard label="Ulanylýar" value={String(activeCount)} detail="10 kompýuterden" icon={<MonitorCheck size={20} />} tone="green" />
          <StatCard label="Boş" value={String(availableCount)} detail="Häzir taýýar" icon={<Monitor size={20} />} tone="blue" />
          <StatCard label="Şu günki girdeji" value="846 TMT" detail="23 sessiýa" icon={<CircleDollarSign size={20} />} tone="violet" />
        </section>

        <section className="stations-section">
          <div className="section-title"><div><p className="eyebrow">ZALYŇ ÝAGDAÝY</p><h2>10 kompýuter</h2></div><div className="legend"><span><i className="dot active" />Ulanylýar</span><span><i className="dot warning" />Gutaryar</span><span><i className="dot available" />Boş</span><span><i className="dot offline" />Offline</span></div></div>

          <div className="station-layout">
            <div className="computer-grid">
              {computers.map((computer) => (
                <ComputerCard key={computer.id} computer={computer} selected={computer.id === selectedId} onSelect={() => setSelectedId(computer.id)} />
              ))}
            </div>

            <aside className="details-panel">
              <div className="details-head"><span className={`status-icon ${selectedComputer.status}`}><Monitor size={22} /></span><div><p className="eyebrow">SAÝLANAN ENJAM</p><h3>{selectedComputer.id}</h3></div><span className={`status-pill ${selectedComputer.status}`}>{statusCopy[selectedComputer.status]}</span></div>
              <dl className="details-list">
                <div><dt>Zona</dt><dd>{selectedComputer.zone}</dd></div>
                <div><dt>Müşderi</dt><dd>{selectedComputer.customer ?? "—"}</dd></div>
                <div><dt>Galan wagt</dt><dd className="mono">{formatTime(selectedComputer.remainingSeconds)}</dd></div>
                <div><dt>Häzirki hasap</dt><dd>{selectedComputer.sessionPrice ? `${selectedComputer.sessionPrice} TMT` : "—"}</dd></div>
              </dl>
              <div className="details-actions">
                {selectedComputer.status === "available" ? (
                  <button className="primary-button full" type="button" onClick={() => setIsSessionOpen(true)}><Plus size={17} />Sessiya aç</button>
                ) : (
                  <>
                    <button className="secondary-button full" type="button" disabled={selectedComputer.status === "offline"} onClick={extendSession}><Clock3 size={17} />+30 minut</button>
                    <button className="danger-button full" type="button" disabled={selectedComputer.status === "offline"} onClick={finishSession}><Power size={17} />Sessiýany tamamla</button>
                  </>
                )}
                <div className="device-actions"><button type="button" aria-label="Sleep" disabled={currentUser.role !== "admin"}><Moon size={17} /></button><button type="button" aria-label="Restart" disabled={currentUser.role !== "admin"}><RotateCcw size={17} /></button><button type="button" aria-label="Öçür" disabled={currentUser.role !== "admin"}><Power size={17} /></button></div>
                {currentUser.role !== "admin" && <p className="permission-note">Sleep, restart we öçürmek diňe administrator üçin.</p>}
              </div>
            </aside>
          </div>
        </section>
      </main>

      {isSessionOpen && <SessionModal computers={computers} tariffs={tariffs} preferredId={selectedComputer.status === "available" ? selectedComputer.id : undefined} onClose={() => setIsSessionOpen(false)} onStart={async (session) => {
        const computer = computers.find((item) => item.id === session.computerId);
        if (!computer) throw new Error("Kompýuter tapylmady.");
        const result = await startSessionRequest({ computerId: computer.databaseId, tariffId: session.tariffId, customerName: session.customer, minutes: session.minutes });
        setComputers((current) => current.map((item) => item.id === session.computerId ? { ...item, sessionId: result.id, status: "active", customer: session.customer, remainingSeconds: session.minutes * 60, sessionPrice: result.totalPrice } : item));
        setSelectedId(session.computerId);
        setNotice(`${session.computerId} üçin ${session.minutes} minutlyk sessiýa başlady.`);
        setIsSessionOpen(false);
      }} />}
    </div>
  );
}

function StatCard({ label, value, detail, icon, tone }: { label: string; value: string; detail: string; icon: React.ReactNode; tone: string }) {
  return <article className="stat-card"><span className={`stat-icon ${tone}`}>{icon}</span><div><p>{label}</p><strong>{value}</strong><small>{detail}</small></div><ChevronRight className="stat-arrow" size={18} /></article>;
}

function ComputerCard({ computer, selected, onSelect }: { computer: ComputerStation; selected: boolean; onSelect: () => void }) {
  return (
    <button type="button" className={`computer-card ${computer.status} ${selected ? "selected" : ""}`} onClick={onSelect} aria-pressed={selected}>
      <span className="computer-head"><span className="computer-icon"><Monitor size={19} /></span><strong>{computer.id}</strong><i className={`dot ${computer.status}`} /></span>
      <span className="computer-time mono">{computer.status === "offline" ? "Offline" : formatTime(computer.remainingSeconds)}</span>
      <span className="computer-foot"><span><small>{computer.zone}</small><b>{computer.customer ?? statusCopy[computer.status]}</b></span><span className={`status-pill ${computer.status}`}>{statusCopy[computer.status]}</span></span>
    </button>
  );
}

type NewSession = { computerId: string; tariffId: string; customer: string; minutes: number; total: number };

function SessionModal({ computers, tariffs, preferredId, onClose, onStart }: { computers: ComputerStation[]; tariffs: Tariff[]; preferredId?: string; onClose: () => void; onStart: (session: NewSession) => Promise<void> }) {
  const availableComputers = computers.filter((computer) => computer.status === "available");
  const [computerId, setComputerId] = useState(preferredId ?? availableComputers[0]?.id ?? "");
  const [customer, setCustomer] = useState("Täze müşderi");
  const [minutes, setMinutes] = useState(60);
  const [tariffId, setTariffId] = useState(tariffs[0]?.id ?? "");
  const [submitError, setSubmitError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const selectedTariff = tariffs.find((tariff) => tariff.id === tariffId);
  const hourlyRate = selectedTariff?.hourlyRate ?? 0;
  const total = useMemo(() => (minutes / 60) * hourlyRate, [minutes, hourlyRate]);

  return (
    <div className="modal-backdrop" role="presentation" onMouseDown={(event) => { if (event.target === event.currentTarget) onClose(); }}>
      <section className="modal" role="dialog" aria-modal="true" aria-labelledby="session-title">
        <div className="modal-head"><div><p className="eyebrow">TÄZE SESSIÝA</p><h2 id="session-title">Oýun wagtyny aç</h2></div><button className="icon-button" type="button" onClick={onClose} aria-label="Penjiräni ýap"><X size={18} /></button></div>
        <form onSubmit={async (event) => {
          event.preventDefault();
          if (!computerId || !tariffId || !customer.trim()) return;
          setSubmitError("");
          setIsSubmitting(true);
          try {
            await onStart({ computerId, tariffId, customer: customer.trim(), minutes, total });
          } catch (error) {
            setSubmitError(error instanceof Error ? error.message : "Sessiýany başlap bolmady.");
            setIsSubmitting(false);
          }
        }}>
          <div className="form-grid">
            <label>Kompýuter<select value={computerId} onChange={(event) => setComputerId(event.target.value)} required>{availableComputers.map((computer) => <option key={computer.id}>{computer.id}</option>)}</select></label>
            <label>Müşderi<input value={customer} onChange={(event) => setCustomer(event.target.value)} required /></label>
            <label>Wagt<select value={minutes} onChange={(event) => setMinutes(Number(event.target.value))}><option value={30}>30 minut</option><option value={60}>1 sagat</option><option value={120}>2 sagat</option><option value={180}>3 sagat</option></select></label>
            <label>Tarif<select value={tariffId} onChange={(event) => setTariffId(event.target.value)}>{tariffs.map((tariff) => <option value={tariff.id} key={tariff.id}>{tariff.name} · {tariff.hourlyRate} TMT/sag</option>)}</select></label>
          </div>
          {submitError && <p className="form-error" role="alert">{submitError}</p>}
          <div className="bill"><span><small>Kompýuter</small><strong>{computerId || "Boş PC ýok"}</strong></span><span><small>Dowamlylygy</small><strong>{minutes < 60 ? `${minutes} minut` : `${minutes / 60} sagat`}</strong></span><span><small>Jemi</small><strong className="bill-total">{total} TMT</strong></span></div>
          <div className="modal-actions"><button className="secondary-button" type="button" onClick={onClose}>Ýatyr</button><button className="primary-button" type="submit" disabled={!computerId || !tariffId || isSubmitting}><Gamepad2 size={18} />{isSubmitting ? "Başladylýar..." : "Sessiýany başlat"}</button></div>
        </form>
      </section>
    </div>
  );
}
