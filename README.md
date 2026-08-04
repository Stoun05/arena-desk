# ArenaDesk

ArenaDesk is a management platform for gaming clubs and internet cafés. It is designed to manage timed computer sessions, customer access, tariffs, payments, sales, games, and operational reports from one dashboard.

## Current milestone

The initial responsive dashboard is implemented with demo data. It includes:

- 10 computer stations with available, active, warning, and offline states
- Live session countdowns
- Station selection and session details
- A functional new-session form with duration and tariff calculation
- Session extension and completion controls
- Responsive desktop, tablet, and mobile layouts
- Demo login with Administrator and Cashier roles
- Role-based navigation and administrator-only device controls
- Protected dashboard navigation and logout

Authentication is currently a clearly labeled frontend demo stored in the browser. Backend persistence, secure server-side sessions, and real computer control are intentionally not included in this milestone.

## Technology

- Next.js 16
- React 19
- TypeScript
- Tailwind CSS 4
- Lucide icons

## Run locally

```bash
npm install
npm run dev
```

Open [http://localhost:3000](http://localhost:3000).

## Validation

```bash
npm run lint
npm run build
```

## Planned architecture

- **Dashboard:** Next.js, TypeScript, Tailwind CSS
- **Backend:** ASP.NET Core and SignalR
- **Database:** PostgreSQL
- **Windows agent:** C# .NET Worker Service
- **Player screen:** WPF client application

## MVP roadmap

1. ✅ Build the initial administrator dashboard
2. ✅ Add the demo login experience and staff roles
3. Replace demo login with secure server-side authentication
4. Persist computers, sessions, tariffs, and payments
5. Connect real-time status updates through SignalR
6. Create the Windows agent and locked player screen
7. Test on one computer, then deploy to ten computers
