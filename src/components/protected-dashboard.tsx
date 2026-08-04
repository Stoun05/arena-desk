"use client";

import { LoaderCircle } from "lucide-react";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { Dashboard } from "@/components/dashboard";
import { clearDemoSession, loadDemoSession, type DemoUser } from "@/lib/demo-auth";

export function ProtectedDashboard() {
  const router = useRouter();
  const [user, setUser] = useState<DemoUser | null>(null);

  useEffect(() => {
    let isActive = true;
    queueMicrotask(() => {
      if (!isActive) return;
      const session = loadDemoSession();
      if (!session) {
        router.replace("/login");
        return;
      }
      setUser(session);
    });
    return () => { isActive = false; };
  }, [router]);

  if (!user) {
    return <main className="auth-loading"><LoaderCircle className="spin" size={28} /><span>Giriş barlanýar...</span></main>;
  }

  return <Dashboard currentUser={user} onLogout={() => {
    clearDemoSession();
    router.replace("/login");
  }} />;
}
