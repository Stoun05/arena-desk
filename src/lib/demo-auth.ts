export type UserRole = "admin" | "cashier";

export type DemoUser = {
  id: string;
  name: string;
  email: string;
  role: UserRole;
};

type DemoAccount = DemoUser & { password: string };

export const AUTH_STORAGE_KEY = "arena-desk-demo-session";

export const demoAccounts: DemoAccount[] = [
  {
    id: "user-admin",
    name: "Admin Kassir",
    email: "admin@arena.demo",
    password: "admin123",
    role: "admin",
  },
  {
    id: "user-cashier",
    name: "Gündizki kassir",
    email: "cashier@arena.demo",
    password: "cashier123",
    role: "cashier",
  },
];

export function authenticateDemoUser(email: string, password: string): DemoUser | null {
  const account = demoAccounts.find(
    (candidate) => candidate.email.toLowerCase() === email.trim().toLowerCase() && candidate.password === password,
  );

  if (!account) return null;
  const { password: _password, ...user } = account;
  void _password;
  return user;
}

export function saveDemoSession(user: DemoUser) {
  window.localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(user));
}

export function loadDemoSession(): DemoUser | null {
  const value = window.localStorage.getItem(AUTH_STORAGE_KEY);
  if (!value) return null;

  try {
    const session = JSON.parse(value) as Partial<DemoUser>;
    if (!session.id || !session.name || !session.email || !["admin", "cashier"].includes(session.role ?? "")) {
      window.localStorage.removeItem(AUTH_STORAGE_KEY);
      return null;
    }
    return session as DemoUser;
  } catch {
    window.localStorage.removeItem(AUTH_STORAGE_KEY);
    return null;
  }
}

export function clearDemoSession() {
  window.localStorage.removeItem(AUTH_STORAGE_KEY);
}
