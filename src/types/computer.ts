export type ComputerStatus = "available" | "active" | "warning" | "offline";

export type ComputerStation = {
  id: string;
  databaseId: string;
  sessionId?: string;
  zone: "Standard" | "VIP";
  status: ComputerStatus;
  customer?: string;
  remainingSeconds?: number;
  sessionPrice?: number;
  agentOnline?: boolean;
  lastSeenAt?: string;
  agentVersion?: string;
};
