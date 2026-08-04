# ArenaDesk server

Stage 11 adds a separately protected Agent SignalR hub. A Windows Agent binds its unique ID to one configured computer, sends heartbeats, and receives targeted commands after session transactions commit.

## Requirements

- .NET 10 SDK
- Docker with Compose

## Start locally

From the repository root, copy `.env.example` to `.env`, replace the example database password, then run:

```bash
docker compose up --build
```

The local server listens on `http://localhost:5080`.

## Verification

- `GET /` — API discovery response
- `GET /api/v1/system/status` — service, environment, computer limit, database status, and stage
- `GET /health/live` — process health check
- `GET /health/ready` — PostgreSQL readiness check
- `POST /api/v1/auth/login` — validate credentials and issue an eight-hour JWT
- `GET /api/v1/auth/me` — return the authenticated user
- `GET /api/v1/admin/users` — administrator-only user summary
- `GET /api/v1/computers` — ten computers with tariff and active-session state
- `GET /api/v1/tariffs` — active Standard/VIP tariffs
- `POST /api/v1/sessions` — atomically start a session and record its payment
- `POST /api/v1/sessions/{id}/extend` — atomically add time and another payment
- `POST /api/v1/sessions/{id}/complete` — complete the session and release the computer
- `/hubs/operations` — authenticated SignalR hub for computer-state changes
- `/hubs/agents` — shared-key protected Windows Agent registration, heartbeat, acknowledgement, and command channel

## Configuration

Configuration can be overridden with standard ASP.NET Core environment variables:

- `ASPNETCORE_URLS`
- `ArenaDesk__ClubName`
- `ArenaDesk__ComputerLimit`
- `Cors__AllowedOrigins__0`
- `ConnectionStrings__ArenaDesk`
- `Jwt__SigningKey`, `Jwt__Issuer`, `Jwt__Audience`, `Jwt__ExpirationHours`
- `BootstrapUsers__AdministratorUsername`, `BootstrapUsers__AdministratorPassword`
- `BootstrapUsers__CashierUsername`, `BootstrapUsers__CashierPassword`
- `AgentChannel__AccessKey`

The database schema in `server/database/001_initial_schema.sql` creates six tables and seeds ten computers plus Standard/VIP tariffs. On API startup, missing administrator and cashier accounts are created with hashed passwords from configuration. Values in `.env.example` are local-only examples and must be replaced outside development.
