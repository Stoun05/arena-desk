# ArenaDesk

[![CI](https://github.com/Stoun05/arena-desk/actions/workflows/ci.yml/badge.svg)](https://github.com/Stoun05/arena-desk/actions/workflows/ci.yml)
[![GitHub Pages](https://github.com/Stoun05/arena-desk/actions/workflows/pages.yml/badge.svg)](https://stoun05.github.io/arena-desk/)

ArenaDesk is a management platform for gaming clubs and internet cafés. It is designed to manage timed computer sessions, customer access, tariffs, payments, sales, games, and operational reports from one dashboard.

Public static demo: [https://stoun05.github.io/arena-desk/](https://stoun05.github.io/arena-desk/). Select Administrator or Cashier and use `demo123`. The Pages demo uses browser-only sample data; the real API and PostgreSQL deployment remain separate.

## Current milestone

The responsive dashboard and persistent API workflow are implemented. The current milestone includes:

- 10 computer stations with available, active, warning, and offline states
- Live session countdowns
- Station selection and session details
- A functional new-session form with duration and tariff calculation
- Session extension and completion controls
- Responsive desktop, tablet, and mobile layouts
- ASP.NET Core login with Administrator and Cashier roles
- Role-based navigation and administrator-only device controls
- Protected dashboard navigation and logout
- PostgreSQL persistence for users, computers, tariffs, and sessions
- HttpOnly-cookie JWT authentication
- Windows Worker Service agent foundation with registration and heartbeat
- Per-computer hashed agent credentials and a persistent command queue
- Observe-only `unlock`, `session-updated`, and `lock` command handling
- Automatic session expiry that releases the station and queues a `lock` command

Authentication, sessions, and agent transport are connected to the ASP.NET Core API. Real Windows locking is intentionally disabled: the agent acknowledges commands as simulated until the locked player-screen milestone is implemented and tested on a dedicated computer.

## Technology

- Next.js 16
- React 19
- TypeScript
- Tailwind CSS 4
- Lucide icons
- ASP.NET Core 10
- Entity Framework Core and PostgreSQL
- .NET 10 Worker Service for Windows stations

## Run locally

Copy `.env.example` to `.env`, replace every placeholder secret, then start PostgreSQL and the API:

```bash
docker compose up --build
```

In another terminal, start the dashboard:

```bash
npm install
npm run dev
```

Open [http://localhost:3000](http://localhost:3000).

## Validation

```bash
npm run lint
npm run build
dotnet build server/ArenaDesk.Api/ArenaDesk.Api.csproj
dotnet build agent/ArenaDesk.Agent/ArenaDesk.Agent.csproj
```

API health check: `http://localhost:8080/health`.

## Continuous integration

GitHub Actions validates every pull request and every push to `main` with two independent jobs:

- frontend dependency installation, linting, and production build
- backend restore/build plus an authenticated API integration test against PostgreSQL

The integration test registers a station agent, signs in as the seeded administrator, reads computers and tariffs, starts a session, receives and acknowledges the observe-only commands, extends and finishes the session, and confirms that the computer becomes available again.

## Windows agent foundation

The project in `agent/ArenaDesk.Agent` follows the .NET Worker Service and Windows Service hosting model. It performs these actions:

1. registers the station with `X-Arena-Bootstrap-Key` when no individual agent token is configured
2. keeps the returned station token in memory for the service run
3. sends a heartbeat every 5–60 seconds
4. receives one queued command at a time and reports `succeeded`, `simulated`, or `failed`

The API stores only a SHA-256 hash of each individual agent token. The shared bootstrap key and any pre-issued agent token must be supplied through configuration; they are not committed to Git.

Example PowerShell configuration for a development station:

```powershell
$env:ArenaDesk__ApiBaseUrl = "http://SERVER-IP:8080/api"
$env:ArenaDesk__ComputerName = "PC-01"
$env:ArenaDesk__BootstrapKey = "replace-with-the-same-32-character-api-bootstrap-key"
dotnet run --project agent/ArenaDesk.Agent/ArenaDesk.Agent.csproj
```

Keep `ArenaDesk__EnableDeviceControl=false`. Setting it to `true` still does not lock or unlock Windows in this milestone; the worker rejects the action so that incomplete device-control code cannot run accidentally.

Microsoft references: [.NET Worker Services](https://learn.microsoft.com/en-us/dotnet/core/extensions/workers), [Windows Service with BackgroundService](https://learn.microsoft.com/en-us/dotnet/core/extensions/windows-service), and [ASP.NET Core secret storage](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-10.0).

## Planned architecture

- **Dashboard:** Next.js, TypeScript, Tailwind CSS
- **Backend:** ASP.NET Core and SignalR
- **Database:** PostgreSQL
- **Windows agent:** C# .NET Worker Service
- **Player screen:** WPF client application

## MVP roadmap

1. ✅ Build the initial administrator dashboard
2. ✅ Add the login experience and staff roles
3. ✅ Add secure server-side authentication
4. ✅ Persist users, computers, sessions, and tariffs
5. Connect real-time dashboard status updates through SignalR
6. ✅ Create the safe Windows agent transport foundation
7. Test on one computer, then deploy to ten computers
8. Add and harden the locked player screen after the one-computer test
