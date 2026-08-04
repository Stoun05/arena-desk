# ArenaDesk server

Stage 6 adds the ASP.NET Core API foundation. PostgreSQL and Entity Framework Core are intentionally reserved for Stage 7.

## Requirements

- .NET 10 SDK

## Start locally

```bash
dotnet restore ArenaDesk.Api/ArenaDesk.Api.csproj
dotnet run --project ArenaDesk.Api/ArenaDesk.Api.csproj --launch-profile http
```

The local server listens on `http://localhost:5080`.

## Verification

- `GET /` — API discovery response
- `GET /api/v1/system/status` — service, environment, computer limit, database status, and stage
- `GET /health/live` — process health check

## Configuration

Configuration can be overridden with standard ASP.NET Core environment variables:

- `ASPNETCORE_URLS`
- `ArenaDesk__ClubName`
- `ArenaDesk__ComputerLimit`
- `Cors__AllowedOrigins__0`

No secrets are committed. The PostgreSQL connection string will be added through local configuration in Stage 7.
