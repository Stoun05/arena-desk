import { clearSession, getStoredSession } from "./auth";
import type {
  ApiComputer,
  ComputerStation,
  PaymentMethod,
  SessionOperation,
} from "@/types/computer";

const apiUrl = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5080";

async function authorizedRequest<T>(path: string, init?: RequestInit): Promise<T> {
  const session = getStoredSession();
  if (!session) {
    throw new Error("Giriş möhleti gutardy. Täzeden ulgama giriň.");
  }

  const response = await fetch(`${apiUrl}${path}`, {
    ...init,
    headers: {
      Authorization: `Bearer ${session.accessToken}`,
      "Content-Type": "application/json",
      ...init?.headers,
    },
  });

  if (response.status === 401) {
    clearSession();
    throw new Error("Giriş möhleti gutardy. Täzeden ulgama giriň.");
  }
  if (!response.ok) {
    let message = "Amal ýerine ýetirilmedi.";
    try {
      const problem = await response.json() as { detail?: string; title?: string };
      message = problem.detail ?? problem.title ?? message;
    } catch {
      // Keep the safe fallback when no problem-details body is available.
    }
    throw new Error(message);
  }

  return response.json() as Promise<T>;
}

function formatClock(value: string) {
  return new Date(value).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
}

function toStation(computer: ApiComputer): ComputerStation {
  const session = computer.activeSession;
  return {
    id: computer.id,
    name: computer.displayName,
    tier: computer.tier,
    status: computer.status,
    hourlyRate: computer.hourlyRate,
    sessionId: session?.id,
    customer: session?.customerName ?? undefined,
    startedAt: session ? formatClock(session.startedAtUtc) : undefined,
    endsAt: session ? formatClock(session.endsAtUtc) : undefined,
    remainingSeconds: session
      ? Math.max(0, Math.floor((Date.parse(session.endsAtUtc) - Date.now()) / 1000))
      : undefined,
    currentCharge: session?.currentPrice,
    paymentMethod: session?.paymentMethod ?? undefined,
  };
}

export async function getComputers(): Promise<ComputerStation[]> {
  const computers = await authorizedRequest<ApiComputer[]>("/api/v1/computers");
  return computers.map(toStation);
}

export function startSession(input: {
  computerId: string;
  durationMinutes: number;
  customerName: string;
  paymentMethod: PaymentMethod;
}) {
  return authorizedRequest<SessionOperation>("/api/v1/sessions/", {
    method: "POST",
    body: JSON.stringify(input),
  });
}

export function extendSession(sessionId: string, durationMinutes: number, paymentMethod: PaymentMethod) {
  return authorizedRequest<SessionOperation>(`/api/v1/sessions/${sessionId}/extend`, {
    method: "POST",
    body: JSON.stringify({ durationMinutes, paymentMethod }),
  });
}

export function completeSession(sessionId: string) {
  return authorizedRequest<SessionOperation>(`/api/v1/sessions/${sessionId}/complete`, {
    method: "POST",
  });
}
