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
};
