import { Suspense } from "react";

import { ComputerDashboard } from "@/components/features/computers";
import { DashboardShell } from "@/components/layout/dashboard-shell";
import { computers } from "@/data/computers";

export default function DashboardPage() {
  return (
    <Suspense fallback={<div className="min-h-screen bg-[#070a12]" />}>
      <DashboardShell>
        <ComputerDashboard initialStations={computers} />
      </DashboardShell>
    </Suspense>
  );
}
