import {
  HubConnectionBuilder,
  LogLevel,
} from "@microsoft/signalr";

import { getStoredSession } from "./auth";

const signalRUrl = process.env.NEXT_PUBLIC_SIGNALR_URL
  ?? "http://localhost:5080/hubs/operations";

export type RealtimeConnectionState =
  | "connecting"
  | "connected"
  | "reconnecting"
  | "offline";

export type ComputerStateChanged = {
  computerId: string;
  action: "session-started" | "session-extended" | "session-completed";
  occurredAtUtc: string;
};

export function connectOperationsRealtime({
  onChange,
  onStateChange,
}: {
  onChange: (event: ComputerStateChanged) => void;
  onStateChange: (state: RealtimeConnectionState) => void;
}) {
  const connection = new HubConnectionBuilder()
    .withUrl(signalRUrl, {
      accessTokenFactory: () => getStoredSession()?.accessToken ?? "",
    })
    .withAutomaticReconnect([0, 2_000, 5_000, 10_000])
    .configureLogging(LogLevel.Warning)
    .build();
  let disposed = false;

  connection.on("ComputerStateChanged", onChange);
  connection.onreconnecting(() => onStateChange("reconnecting"));
  connection.onreconnected(() => onStateChange("connected"));
  connection.onclose(() => onStateChange("offline"));

  onStateChange("connecting");
  void connection.start()
    .then(() => {
      if (disposed) {
        return connection.stop();
      }

      onStateChange("connected");
    })
    .catch(() => {
      if (!disposed) onStateChange("offline");
    });

  return () => {
    disposed = true;
    connection.off("ComputerStateChanged", onChange);
    void connection.stop();
  };
}
