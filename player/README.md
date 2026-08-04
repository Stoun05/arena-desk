# ArenaDesk Player Screen

Stage 14 packages the WPF application for the interactive Windows user session. It starts at user login, connects only to the local ArenaDesk Agent through an access-key protected named pipe, switches to a compact always-on-top countdown during an active session, and locks again when time expires or the Agent receives logout, sleep, or shutdown.

`PlayerScreen:PipeName` and `PlayerScreen:AccessKey` must match `Agent:PlayerPipeName` and `Agent:PlayerAccessKey`. Use a different random local access key on each club computer.

Build on Windows:

```powershell
dotnet publish player/ArenaDesk.Player/ArenaDesk.Player.csproj -c Release -r win-x64 --self-contained false
```

The Stage 14 installer registers the published Player Screen at user login while `ArenaDesk Agent` runs as the Windows service. The lock screen is an operational kiosk overlay, not a Windows secure-desktop security boundary; use a dedicated non-administrator player account and appropriate Windows kiosk policies.
