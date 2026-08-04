# ArenaDesk Windows Agent

Stage 11 provides a .NET Worker Service that can run as `ArenaDesk Agent` on Windows. It authenticates to `/hubs/agents`, binds its configured agent ID to one computer, sends a heartbeat, receives targeted session commands, and acknowledges them.

The default `CommandExecutionMode` is `LogOnly`. Stage 11 deliberately does not invoke Windows logout, sleep, shutdown, or player-screen unlock APIs yet; those actions require the guarded executor and local recovery behavior planned for the next stage.

## Local start

Update `ArenaDesk.Agent/appsettings.json` so `AccessKey` matches the backend `AgentChannel:AccessKey`, then run:

```bash
dotnet run --project agent/ArenaDesk.Agent/ArenaDesk.Agent.csproj
```

Use a different unique `AgentId` and matching `ComputerCode` on each club computer.
