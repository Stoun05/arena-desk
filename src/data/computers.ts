import type { ComputerStation } from "@/types/computer";

export const initialComputers: ComputerStation[] = [
  { id: "PC-01", zone: "Standard", status: "active", customer: "Merdan", remainingSeconds: 6138, sessionPrice: 26 },
  { id: "PC-02", zone: "Standard", status: "available" },
  { id: "PC-03", zone: "Standard", status: "warning", customer: "Aýna", remainingSeconds: 292, sessionPrice: 15 },
  { id: "PC-04", zone: "Standard", status: "active", customer: "Döwlet", remainingSeconds: 2229, sessionPrice: 18 },
  { id: "PC-05", zone: "Standard", status: "available" },
  { id: "PC-06", zone: "Standard", status: "offline" },
  { id: "PC-07", zone: "Standard", status: "available" },
  { id: "PC-08", zone: "VIP", status: "active", customer: "Serdar", remainingSeconds: 7900, sessionPrice: 44 },
  { id: "PC-09", zone: "VIP", status: "available" },
  { id: "PC-10", zone: "VIP", status: "available" },
];
