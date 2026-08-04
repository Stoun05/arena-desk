# ArenaDesk

ArenaDesk is a planned management platform for gaming clubs and internet cafés. It is designed to manage timed computer sessions, customer access, tariffs, payments, sales, games, and operational reports from one dashboard.

## Planned architecture

- **Dashboard:** Next.js, TypeScript, Tailwind CSS, shadcn/ui
- **Backend:** ASP.NET Core and SignalR
- **Database:** PostgreSQL
- **Windows agent:** C# .NET Worker Service
- **Player screen:** WPF client application

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

The Stage 5 frontend demo includes:

- `/login` — administrator/cashier role selection and validated demo login form;
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

Stages 1–5 are complete: the MVP requirements, frontend foundation, demo login, responsive navigation, interactive 10-computer dashboard, and frontend session-management flow are ready. Persisting sessions in the backend and database is the next milestone.
