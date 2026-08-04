"use client";

import { LoaderCircle } from "lucide-react";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { Dashboard } from "@/components/dashboard";
import { getComputers, getCurrentUser, getTariffs, logout, type AuthUser, type Tariff } from "@/lib/api-client";
import type { ComputerStation } from "@/types/computer";

export function ProtectedDashboard() {
  const router = useRouter();
  const [user, setUser] = useState<AuthUser | null>(null);
  const [computers, setComputers] = useState<ComputerStation[] | null>(null);
  const [tariffs, setTariffs] = useState<Tariff[] | null>(null);
  const [loadError, setLoadError] = useState("");

  useEffect(() => {
    let isActive = true;
    queueMicrotask(async () => {
      if (!isActive) return;
      try {
        const [session, stations, tariffList] = await Promise.all([getCurrentUser(), getComputers(), getTariffs()]);
        if (!isActive) return;
        setUser(session);
        setComputers(stations);
        setTariffs(tariffList);
      } catch (error) {
        if (!isActive) return;
        const message = error instanceof Error ? error.message : "API bilen baglanyşyk bolmady.";
        if (message.includes("401")) router.replace("/login");
        else setLoadError(message);
      }
    });
    return () => { isActive = false; };
  }, [router]);

  if (loadError) {
    return <main className="auth-loading"><strong>Servere birigip bolmady</strong><span>{loadError}</span><button className="primary-button" type="button" onClick={() => window.location.reload()}>Gaýtadan synan</button></main>;
  }

  if (!user || !computers || !tariffs) {
    return <main className="auth-loading"><LoaderCircle className="spin" size={28} /><span>Giriş barlanýar...</span></main>;
  }

  return <Dashboard currentUser={user} initialStations={computers} tariffs={tariffs} onLogout={async () => {
    await logout();
    router.replace("/login");
  }} />;
}
