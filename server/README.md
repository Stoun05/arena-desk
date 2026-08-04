# ArenaDesk server

Stage 7 adds PostgreSQL persistence through Entity Framework Core. The model contains users, computers, tariffs, sessions, payments, and audit logs.

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

## Configuration

Configuration can be overridden with standard ASP.NET Core environment variables:

- `ASPNETCORE_URLS`
- `ArenaDesk__ClubName`
- `ArenaDesk__ComputerLimit`
- `Cors__AllowedOrigins__0`
- `ConnectionStrings__ArenaDesk`

No real secrets are committed. The database schema in `server/database/001_initial_schema.sql` creates six tables and seeds ten computers plus Standard/VIP tariffs on the first PostgreSQL startup.
