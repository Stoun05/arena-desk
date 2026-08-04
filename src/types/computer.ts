export type ComputerTier = "standard" | "vip";

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
  remainingTime?: string;
  currentCharge?: number;
};
