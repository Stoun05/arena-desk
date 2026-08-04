import type { ComputerStation } from "@/types/computer";

const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:8080/api";
export const isDemoMode = process.env.NEXT_PUBLIC_DEMO_MODE === "true";
const DEMO_SESSION_KEY = "arena-desk-demo-user";

export type UserRole = "admin" | "cashier";
export type AuthUser = { id: string; name: string; email: string; role: UserRole };
export type Tariff = { id: string; name: string; hourlyRate: number };

type ApiComputer = {
  id: string;
  name: string;
  zone: "Standard" | "VIP";
  status: ComputerStation["status"];
  sessionId?: string | null;
  customer?: string | null;
  remainingSeconds?: number | null;
  sessionPrice?: number | null;
  agentOnline: boolean;
  lastSeenAt?: string | null;
  agentVersion?: string | null;
};

type ApiError = { message?: string };

const demoTariffs: Tariff[] = [
  { id: "demo-day", name: "Gündiz", hourlyRate: 12 },
  { id: "demo-standard", name: "Standart", hourlyRate: 15 },
  { id: "demo-vip", name: "VIP", hourlyRate: 20 },
];

const demoComputers: ComputerStation[] = [
  { id: "PC-01", databaseId: "demo-pc-01", sessionId: "demo-session-01", zone: "Standard", status: "active", customer: "Myrat", remainingSeconds: 3820, sessionPrice: 15, agentOnline: true, agentVersion: "1.0.0-demo" },
  { id: "PC-02", databaseId: "demo-pc-02", zone: "Standard", status: "available", agentOnline: true, agentVersion: "1.0.0-demo" },
  { id: "PC-03", databaseId: "demo-pc-03", sessionId: "demo-session-03", zone: "Standard", status: "warning", customer: "Aman", remainingSeconds: 245, sessionPrice: 12, agentOnline: true, agentVersion: "1.0.0-demo" },
  { id: "PC-04", databaseId: "demo-pc-04", zone: "Standard", status: "available", agentOnline: true, agentVersion: "1.0.0-demo" },
  { id: "PC-05", databaseId: "demo-pc-05", sessionId: "demo-session-05", zone: "Standard", status: "active", customer: "Selbi", remainingSeconds: 6210, sessionPrice: 30, agentOnline: true, agentVersion: "1.0.0-demo" },
  { id: "PC-06", databaseId: "demo-pc-06", zone: "Standard", status: "offline", agentOnline: false },
  { id: "PC-07", databaseId: "demo-pc-07", zone: "Standard", status: "available", agentOnline: true, agentVersion: "1.0.0-demo" },
  { id: "PC-08", databaseId: "demo-pc-08", sessionId: "demo-session-08", zone: "VIP", status: "active", customer: "Begenç", remainingSeconds: 1940, sessionPrice: 20, agentOnline: true, agentVersion: "1.0.0-demo" },
  { id: "PC-09", databaseId: "demo-pc-09", zone: "VIP", status: "available", agentOnline: true, agentVersion: "1.0.0-demo" },
  { id: "PC-10", databaseId: "demo-pc-10", zone: "VIP", status: "available", agentOnline: true, agentVersion: "1.0.0-demo" },
];

function demoUserFor(email: string): AuthUser | null {
  if (email.toLowerCase() === "admin@arena.local")
    return { id: "demo-admin", name: "Arena administrator", email: "admin@arena.local", role: "admin" };
  if (email.toLowerCase() === "cashier@arena.local")
    return { id: "demo-cashier", name: "Arena cashier", email: "cashier@arena.local", role: "cashier" };
  return null;
}

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${API_URL}${path}`, {
    ...init,
    credentials: "include",
    headers: { "Content-Type": "application/json", ...init?.headers },
  });
  if (!response.ok) {
    let message = `API error ${response.status}`;
    try {
      const body = await response.json() as ApiError;
      if (body.message) message = body.message;
    } catch {
      // The API may return an empty error response.
    }
    throw new Error(message);
  }
  if (response.status === 204) return undefined as T;
  return response.json() as Promise<T>;
}

export async function login(email: string, password: string) {
  if (isDemoMode) {
    const user = demoUserFor(email);
    if (!user || password !== "demo123") throw new Error("Demo email ýa-da parol nädogry.");
    window.sessionStorage.setItem(DEMO_SESSION_KEY, JSON.stringify(user));
    return user;
  }
  return request<AuthUser>("/auth/login", {
    method: "POST",
    body: JSON.stringify({ email, password }),
  });
}

export async function getCurrentUser() {
  if (isDemoMode) {
    const stored = window.sessionStorage.getItem(DEMO_SESSION_KEY);
    if (!stored) throw new Error("API error 401");
    return JSON.parse(stored) as AuthUser;
  }
  return request<AuthUser>("/auth/me");
}

export async function logout() {
  if (isDemoMode) {
    window.sessionStorage.removeItem(DEMO_SESSION_KEY);
    return;
  }
  return request<void>("/auth/logout", { method: "POST" });
}

export async function getComputers(): Promise<ComputerStation[]> {
  if (isDemoMode) return demoComputers.map((computer) => ({ ...computer }));
  const computers = await request<ApiComputer[]>("/computers");
  return computers.map((computer) => ({
    id: computer.name,
    databaseId: computer.id,
    sessionId: computer.sessionId ?? undefined,
    zone: computer.zone,
    status: computer.status,
    customer: computer.customer ?? undefined,
    remainingSeconds: computer.remainingSeconds ?? undefined,
    sessionPrice: computer.sessionPrice ?? undefined,
    agentOnline: computer.agentOnline,
    lastSeenAt: computer.lastSeenAt ?? undefined,
    agentVersion: computer.agentVersion ?? undefined,
  }));
}

export async function getTariffs() {
  if (isDemoMode) return demoTariffs.map((tariff) => ({ ...tariff }));
  return request<Tariff[]>("/tariffs");
}

export async function startSession(input: { computerId: string; tariffId: string; customerName: string; minutes: number }) {
  if (isDemoMode) {
    const tariff = demoTariffs.find((item) => item.id === input.tariffId) ?? demoTariffs[0];
    return { id: crypto.randomUUID(), endsAt: new Date(Date.now() + input.minutes * 60_000).toISOString(), totalPrice: Math.round(tariff.hourlyRate * input.minutes / 60 * 100) / 100 };
  }
  return request<{ id: string; endsAt: string; totalPrice: number }>("/sessions", {
    method: "POST",
    body: JSON.stringify(input),
  });
}

export async function extendSession(sessionId: string, minutes: number) {
  if (isDemoMode) return { id: sessionId, endsAt: new Date(Date.now() + minutes * 60_000).toISOString(), totalPrice: 22.5 };
  return request<{ id: string; endsAt: string; totalPrice: number }>(`/sessions/${sessionId}/extend`, {
    method: "POST",
    body: JSON.stringify({ minutes }),
  });
}

export async function finishSession(sessionId: string) {
  if (isDemoMode) return;
  return request<void>(`/sessions/${sessionId}/finish`, { method: "POST" });
}
