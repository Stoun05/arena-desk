# ArenaDesk

[![CI](https://github.com/Stoun05/arena-desk/actions/workflows/ci.yml/badge.svg)](https://github.com/Stoun05/arena-desk/actions/workflows/ci.yml)

ArenaDesk is a management platform for gaming clubs and internet cafés. It is designed to manage timed computer sessions, customer access, tariffs, payments, sales, games, and operational reports from one dashboard.

## Current milestone

The initial responsive dashboard is implemented with demo data. It includes:

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

Authentication and the main session workflow are connected to the ASP.NET Core API. Real Windows computer control is intentionally not included in this milestone.

## Technology

- Next.js 16
- React 19
- TypeScript
- Tailwind CSS 4
- Lucide icons
- ASP.NET Core 10
- Entity Framework Core and PostgreSQL

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
```

API health check: `http://localhost:8080/health`.

## Continuous integration

GitHub Actions validates every pull request and every push to `main` with two independent jobs:

- frontend dependency installation, linting, and production build
- backend restore/build plus an authenticated API integration test against PostgreSQL

The integration test signs in as the seeded administrator, reads computers and tariffs, starts a session, extends it, finishes it, and confirms that the computer becomes available again.

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
5. Connect real-time status updates through SignalR
6. Create the Windows agent and locked player screen
7. Test on one computer, then deploy to ten computers
