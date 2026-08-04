import type { ComputerStation } from "@/types/computer";

const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:8080/api";

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
};

type ApiError = { message?: string };

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
  return request<AuthUser>("/auth/login", {
    method: "POST",
    body: JSON.stringify({ email, password }),
  });
}

export function getCurrentUser() {
  return request<AuthUser>("/auth/me");
}

export function logout() {
  return request<void>("/auth/logout", { method: "POST" });
}

export async function getComputers(): Promise<ComputerStation[]> {
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
  }));
}

export function getTariffs() {
  return request<Tariff[]>("/tariffs");
}

export function startSession(input: { computerId: string; tariffId: string; customerName: string; minutes: number }) {
  return request<{ id: string; endsAt: string; totalPrice: number }>("/sessions", {
    method: "POST",
    body: JSON.stringify(input),
  });
}

export function extendSession(sessionId: string, minutes: number) {
  return request<{ id: string; endsAt: string; totalPrice: number }>(`/sessions/${sessionId}/extend`, {
    method: "POST",
    body: JSON.stringify({ minutes }),
  });
}

export function finishSession(sessionId: string) {
  return request<void>(`/sessions/${sessionId}/finish`, { method: "POST" });
}
