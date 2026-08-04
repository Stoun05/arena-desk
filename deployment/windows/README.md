# ArenaDesk Windows deployment

The GitHub Actions artifact `ArenaDesk-Windows-x64` contains self-contained Agent and Player builds plus the deployment scripts. The target computer needs 64-bit Windows 10/11, but does not need a separate .NET installation.

## Install one test computer

1. Download and extract `ArenaDesk-Windows-x64.zip` on the target computer.
2. Open Windows PowerShell 5.1 or newer as Administrator in the extracted folder.
3. Run:

```powershell
.\install.ps1 `
  -ApiBaseUrl 'https://api.example.com' `
  -ComputerCode 'PC-01' `
  -AgentId 'arena-pc-01' `
  -AgentAccessKey 'replace-with-the-backend-agent-key-32-chars-minimum' `
  -PlayerAccessKey 'generate-a-different-local-key-32-chars-minimum' `
  -SkipPlayerStart
```

The default command mode is `LogOnly`, so logout, sleep, and shutdown commands are recorded without changing Windows. `-SkipPlayerStart` registers the login task without immediately opening the full-screen lock overlay, making the first installation recoverable while the service connection is verified. Start the task manually only after `verify.ps1` passes. Add `-EnableSystemCommands` only after the one-computer test passes.

The installer creates:

- the automatic `ArenaDeskAgent` Windows service under LocalSystem;
- the `ArenaDesk Player Screen` scheduled task for the current interactive user;
- protected Agent configuration under `C:\Program Files\ArenaDesk\Agent`;
- reconnect state under `C:\ProgramData\ArenaDesk`.

## Verify and remove

```powershell
.\verify.ps1
.\uninstall.ps1
.\uninstall.ps1 -RemoveData
```

The Player Screen is an operational kiosk overlay, not a Windows secure desktop. A local administrator can stop it or inspect local configuration. Use a dedicated non-administrator Windows account for players, Windows kiosk policies where appropriate, HTTPS for the API, and a unique local Player key on every computer.
