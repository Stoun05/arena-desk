# ArenaDesk Windows Agent

Stage 14 runs the reconnect-safe Windows Agent as a LocalSystem service and connects it to the WPF Player Screen in the interactive user session. The named pipe grants authenticated local users transport access, while the per-computer access key still authenticates the Player handshake.

The default `CommandExecutionMode` remains `LogOnly`, so development machines are never logged out or powered down. Set it explicitly to `System` only on a club computer where logout, sleep, and shutdown are intended. The Agent stores processed command IDs in `%ProgramData%\ArenaDesk\processed-commands.json` before invoking a system action, so redelivery cannot repeat the action. `unlock` and `sync-session` now open and update the Player Screen timer.

## Local start

Update `ArenaDesk.Agent/appsettings.json` so `AccessKey` matches the backend `AgentChannel:AccessKey`, then run:

```bash
dotnet run --project agent/ArenaDesk.Agent/ArenaDesk.Agent.csproj
```

Use a different unique `AgentId` and matching `ComputerCode` on each club computer.

`PlayerPipeName` and `PlayerAccessKey` must match the Player Screen configuration. Use a different random local key of at least 32 characters on every club computer.

To opt in on a prepared Windows computer:

```json
{
  "Agent": {
    "CommandExecutionMode": "System"
  }
}
```

Use `StateDirectory` to override the local command-history directory. If its JSON state is unreadable, the Agent fails closed and does not execute the incoming system command.
