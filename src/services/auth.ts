import type { AuthSession } from "@/types/auth";

const sessionKey = "arena-desk-session";
const apiUrl = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5080";

export async function login(username: string, password: string): Promise<AuthSession> {
  const response = await fetch(`${apiUrl}/api/v1/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ username, password }),
  });

  if (!response.ok) {
    let message = "Ulanyjy ady ýa-da parol nädogry.";
    try {
      const problem = await response.json() as { detail?: string; title?: string };
      message = problem.detail ?? problem.title ?? message;
    } catch {
      // Keep the safe generic message when the backend returns no JSON body.
    }
    throw new Error(message);
  }

  const session = await response.json() as AuthSession;
  window.localStorage.setItem(sessionKey, JSON.stringify(session));
  return session;
}

export function getStoredSession(): AuthSession | null {
  const value = window.localStorage.getItem(sessionKey);
  if (!value) return null;

  try {
    const session = JSON.parse(value) as AuthSession;
    if (!session.accessToken || Date.parse(session.expiresAtUtc) <= Date.now()) {
      clearSession();
      return null;
    }
    return session;
  } catch {
    clearSession();
    return null;
  }
}

export function clearSession() {
  window.localStorage.removeItem(sessionKey);
}
