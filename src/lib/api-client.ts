import type { ComputerStation } from "@/types/computer";

const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:8080/api";
export const IS_DEMO_MODE = process.env.NEXT_PUBLIC_DEMO_MODE === "true";

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

const demoUser: AuthUser = {
  id: "demo-admin",
  name: "Arena administrator",
  email: "admin@arena.demo",
  role: "admin",
};

const demoTariffs: Tariff[] = [
  { id: "demo-day", name: "Gündiz", hourlyRate: 12 },
  { id: "demo-standard", name: "Standart", hourlyRate: 15 },
  { id: "demo-vip", name: "VIP", hourlyRate: 20 },
];

const demoComputers: ComputerStation[] = [
  { id: "PC-01", databaseId: "demo-pc-01", sessionId: "demo-session-01", zone: "Standard", status: "active", customer: "Myrat", remainingSeconds: 4128, sessionPrice: 30, agentOnline: true, agentVersion: "1.0.0" },
  { id: "PC-02", databaseId: "demo-pc-02", sessionId: "demo-session-02", zone: "Standard", status: "warning", customer: "Ayna", remainingSeconds: 248, sessionPrice: 15, agentOnline: true, agentVersion: "1.0.0" },
  { id: "PC-03", databaseId: "demo-pc-03", zone: "Standard", status: "available", agentOnline: true, agentVersion: "1.0.0" },
  { id: "PC-04", databaseId: "demo-pc-04", sessionId: "demo-session-04", zone: "Standard", status: "active", customer: "Serdar", remainingSeconds: 2874, sessionPrice: 30, agentOnline: true, agentVersion: "1.0.0" },
  { id: "PC-05", databaseId: "demo-pc-05", zone: "Standard", status: "available", agentOnline: true, agentVersion: "1.0.0" },
  { id: "PC-06", databaseId: "demo-pc-06", zone: "Standard", status: "offline", agentOnline: false },
  { id: "PC-07", databaseId: "demo-pc-07", zone: "Standard", status: "available", agentOnline: true, agentVersion: "1.0.0" },
  { id: "PC-08", databaseId: "demo-pc-08", sessionId: "demo-session-08", zone: "VIP", status: "active", customer: "Selbi", remainingSeconds: 5390, sessionPrice: 40, agentOnline: true, agentVersion: "1.0.0" },
  { id: "PC-09", databaseId: "demo-pc-09", zone: "VIP", status: "available", agentOnline: true, agentVersion: "1.0.0" },
  { id: "PC-10", databaseId: "demo-pc-10", zone: "VIP", status: "available", agentOnline: true, agentVersion: "1.0.0" },
];

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

export function login(email: string, password: string) {
  if (IS_DEMO_MODE) return Promise.resolve({ ...demoUser, email });
  return request<AuthUser>("/auth/login", {
    method: "POST",
    body: JSON.stringify({ email, password }),
  });
}

export function getCurrentUser() {
  if (IS_DEMO_MODE) return Promise.resolve(demoUser);
  return request<AuthUser>("/auth/me");
}

export function logout() {
  if (IS_DEMO_MODE) return Promise.resolve();
  return request<void>("/auth/logout", { method: "POST" });
}

export async function getComputers(): Promise<ComputerStation[]> {
  if (IS_DEMO_MODE) return demoComputers.map((computer) => ({ ...computer }));
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

export function getTariffs() {
  if (IS_DEMO_MODE) return Promise.resolve(demoTariffs.map((tariff) => ({ ...tariff })));
  return request<Tariff[]>("/tariffs");
}

export function startSession(input: { computerId: string; tariffId: string; customerName: string; minutes: number }) {
  if (IS_DEMO_MODE) {
    const tariff = demoTariffs.find((item) => item.id === input.tariffId) ?? demoTariffs[1];
    return Promise.resolve({
      id: `demo-session-${Date.now()}`,
      endsAt: new Date(Date.now() + input.minutes * 60_000).toISOString(),
      totalPrice: Number((tariff.hourlyRate * input.minutes / 60).toFixed(2)),
    });
  }
  return request<{ id: string; endsAt: string; totalPrice: number }>("/sessions", {
    method: "POST",
    body: JSON.stringify(input),
  });
}

export function extendSession(sessionId: string, minutes: number) {
  if (IS_DEMO_MODE) return Promise.resolve({ id: sessionId, endsAt: new Date(Date.now() + minutes * 60_000).toISOString(), totalPrice: 22.5 });
  return request<{ id: string; endsAt: string; totalPrice: number }>(`/sessions/${sessionId}/extend`, {
    method: "POST",
    body: JSON.stringify({ minutes }),
  });
}

export function finishSession(sessionId: string) {
  if (IS_DEMO_MODE) return Promise.resolve();
  return request<void>(`/sessions/${sessionId}/finish`, { method: "POST" });
}
