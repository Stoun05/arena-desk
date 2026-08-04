export type ComputerStatus = "available" | "active" | "warning" | "offline";

export type ComputerStation = {
  id: string;
  zone: "Standard" | "VIP";
  status: ComputerStatus;
  customer?: string;
  remainingSeconds?: number;
  sessionPrice?: number;
};
