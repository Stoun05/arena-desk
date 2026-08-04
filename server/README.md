# ArenaDesk server

Stage 8 adds JWT authentication and role-based authorization on top of the PostgreSQL data layer. Passwords are hashed, login events are audited, and administrator routes reject cashier tokens.

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

The database schema in `server/database/001_initial_schema.sql` creates six tables and seeds ten computers plus Standard/VIP tariffs. On API startup, missing administrator and cashier accounts are created with hashed passwords from configuration. Values in `.env.example` are local-only examples and must be replaced outside development.
