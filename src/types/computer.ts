export type ComputerTier = "standard" | "vip";

export type PaymentMethod = "cash" | "card";

export type ComputerStatus =
  | "available"
  | "occupied"
  | "ending"
  | "locked"
  | "offline";

export type ComputerStation = {
  id: string;
  name: string;
  tier: ComputerTier;
  status: ComputerStatus;
  hourlyRate: number;
  customer?: string;
  startedAt?: string;
  endsAt?: string;
  remainingSeconds?: number;
  currentCharge?: number;
  paymentMethod?: PaymentMethod;
  sessionId?: string;
};

export type ApiActiveSession = {
  id: string;
  customerName: string | null;
  startedAtUtc: string;
  endsAtUtc: string;
  durationMinutes: number;
  currentPrice: number;
  currency: string;
  paymentMethod: PaymentMethod | null;
};

export type ApiComputer = {
  id: string;
  code: string;
  displayName: string;
  tier: ComputerTier;
  status: ComputerStatus;
  endAction: "logout" | "sleep" | "shutdown";
  hourlyRate: number;
  currency: string;
  activeSession: ApiActiveSession | null;
};

export type SessionOperation = {
  sessionId: string;
  computerId: string;
  status: "active" | "ending" | "completed" | "cancelled";
  startedAtUtc: string;
  endsAtUtc: string;
  durationMinutes: number;
  totalPrice: number;
  currency: string;
};
