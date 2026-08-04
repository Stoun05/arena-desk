# ArenaDesk Windows Agent

Stage 12 provides a reconnect-safe .NET Worker Service that runs as `ArenaDesk Agent` on Windows. It authenticates to `/hubs/agents`, binds its configured agent ID to one computer, sends heartbeats, receives queued session commands, and acknowledges them.

The default `CommandExecutionMode` remains `LogOnly`, so development machines are never logged out or powered down. Set it explicitly to `System` only on a club computer where logout, sleep, and shutdown are intended. The Agent stores processed command IDs in `%ProgramData%\ArenaDesk\processed-commands.json` before invoking a system action, so redelivery cannot repeat the action. `unlock` and `sync-session` are recorded but await the locked player screen planned for the next stage.

## Local start

Update `ArenaDesk.Agent/appsettings.json` so `AccessKey` matches the backend `AgentChannel:AccessKey`, then run:

```bash
dotnet run --project agent/ArenaDesk.Agent/ArenaDesk.Agent.csproj
```

Use a different unique `AgentId` and matching `ComputerCode` on each club computer.

To opt in on a prepared Windows computer:

```json
{
  "Agent": {
    "CommandExecutionMode": "System"
  }
}
```

Use `StateDirectory` to override the local command-history directory. If its JSON state is unreadable, the Agent fails closed and does not execute the incoming system command.
