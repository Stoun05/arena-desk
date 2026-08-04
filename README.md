# ArenaDesk

ArenaDesk is a planned management platform for gaming clubs and internet cafés. It is designed to manage timed computer sessions, customer access, tariffs, payments, sales, games, and operational reports from one dashboard.

## Planned architecture

- **Dashboard:** Next.js, TypeScript, Tailwind CSS, shadcn/ui
- **Backend:** ASP.NET Core and SignalR
- **Database:** PostgreSQL
- **Windows agent:** C# .NET Worker Service
- **Player screen:** WPF client application

## Backend setup

Stage 8 adds real JWT login and administrator/cashier authorization under `server/ArenaDesk.Api`. PostgreSQL stores hashed user credentials and audit events, while the web panel keeps the active token until it expires or the user signs out.

```bash
cp .env.example .env
# Replace the example ARENA_DB_PASSWORD value in .env
docker compose up --build
```

Then verify `http://localhost:5080/api/v1/system/status`, `http://localhost:5080/health/live`, and `http://localhost:5080/health/ready`.

Local development accounts from `.env.example` are `admin / Admin123!` and `cashier / Cashier123!`. Replace these examples and the JWT signing key before using ArenaDesk outside local development.

## MVP roadmap

1. Build the administrator dashboard and authentication
2. Display and manage 10 computer stations
3. Add session timing, tariffs, and payments
4. Connect the dashboard to the backend and database
5. Build real-time computer status updates
6. Create the Windows agent and locked player screen
7. Test on one computer, then deploy to ten computers

## Documentation

- [MVP requirements](docs/requirements.md) — scope, roles, session rules, payments, offline operation, security, and acceptance criteria

## Web dashboard setup

Requirements:

- Node.js 20.9 or newer
- npm

Start the development server:

```bash
npm install
npm run dev
```

Then open `http://localhost:3000`.

The Stage 8 web panel includes:

- `/login` — JWT login connected to the API, plus clearly labelled admin/cashier UI previews for the static GitHub Pages deployment;
- `/dashboard` — responsive sidebar, live-style summary cards, and 10 interactive computer station cards;
- Standard/VIP tiers and Boş, Ulanylýar, Wagt gutarýar, Gulply, and Offline visual states;
- a station detail panel with demo customer, remaining time, tariff, and current charge;
- a new-session flow with duration, matching tariff, cash/card payment, automatic total, and ending-time calculation;
- demo controls to add 30 minutes, finish a session, and update statistics in real time;
- demo-only navigation while the backend authentication service is not yet available.

### Frontend structure

```text
src/
├── app/                 # Next.js routes and layouts
├── components/
│   ├── ui/              # shadcn/ui components
│   ├── layout/          # reusable page shells
│   └── features/        # business feature components
├── data/                # static and mock data
├── hooks/               # reusable React hooks
├── lib/                 # shared utilities
├── services/            # API and SignalR clients
└── types/               # shared TypeScript types
```

## Core session flow

1. A cashier selects an available computer.
2. The cashier chooses a duration and tariff.
3. The selected computer is unlocked and the timer starts.
4. The player receives warnings before time expires.
5. At the end of the session, the player is signed out and the computer returns to the locked screen.
6. The session and payment are recorded in the report.

## Status

Stages 1–8 are complete: the MVP requirements, frontend foundation, responsive navigation, interactive 10-computer dashboard, frontend session-management flow, ASP.NET Core API, PostgreSQL/Entity Framework Core persistence, and JWT authentication with cashier/administrator roles are ready. Real session and payment business operations are the next milestone.
