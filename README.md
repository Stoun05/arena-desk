# ArenaDesk

ArenaDesk is a planned management platform for gaming clubs and internet cafés. It is designed to manage timed computer sessions, customer access, tariffs, payments, sales, games, and operational reports from one dashboard.

## Planned architecture

- **Dashboard:** Next.js, TypeScript, Tailwind CSS, shadcn/ui
- **Backend:** ASP.NET Core and SignalR
- **Database:** PostgreSQL
- **Windows agent:** C# .NET Worker Service
- **Player screen:** WPF client application

## Backend setup

Stage 9 connects the authenticated dashboard to real computer, tariff, session, and payment APIs. Session start, extension, completion, prices, and cash/card payments now persist in PostgreSQL.

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

The Stage 9 web panel includes:

- `/login` — JWT login connected to the API, plus clearly labelled admin/cashier UI previews for the static GitHub Pages deployment;
- `/dashboard` — responsive sidebar, live-style summary cards, and 10 interactive computer station cards;
- Standard/VIP tiers and Boş, Ulanylýar, Wagt gutarýar, Gulply, and Offline visual states;
- a station detail panel with customer, remaining time, tariff, and current charge;
- a new-session flow with duration, matching tariff, cash/card payment, automatic total, and ending-time calculation;
- authenticated controls that persist session start, 30-minute extensions, completion, and payments;
- clearly labelled UI-demo navigation for reviewing the static GitHub Pages deployment without a hosted API.

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

Stages 1–9 are complete: the MVP requirements, frontend foundation, responsive navigation, interactive 10-computer dashboard, ASP.NET Core API, PostgreSQL/Entity Framework Core persistence, JWT roles, and transactional session/tariff/payment operations are ready. Real-time SignalR updates are the next milestone.
