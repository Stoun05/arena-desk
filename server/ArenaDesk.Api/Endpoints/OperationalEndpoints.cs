using System.Data;
using System.Security.Claims;
using System.Text.Json;
using ArenaDesk.Api.Contracts;
using ArenaDesk.Api.Domain;
using ArenaDesk.Api.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ArenaDesk.Api.Endpoints;

public static class OperationalEndpoints
{
    public static IEndpointRouteBuilder MapOperationalEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/v1/computers", GetComputersAsync)
            .WithTags("Computers")
            .RequireAuthorization()
            .RequireRateLimiting("api")
            .Produces<List<ComputerResponse>>(StatusCodes.Status200OK);

        endpoints.MapGet("/api/v1/tariffs", GetTariffsAsync)
            .WithTags("Tariffs")
            .RequireAuthorization()
            .RequireRateLimiting("api")
            .Produces<List<TariffResponse>>(StatusCodes.Status200OK);

        var sessions = endpoints.MapGroup("/api/v1/sessions")
            .WithTags("Sessions")
            .RequireAuthorization()
            .RequireRateLimiting("api");

        sessions.MapPost("/", StartSessionAsync)
            .Produces<SessionOperationResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);
        sessions.MapPost("/{sessionId:guid}/extend", ExtendSessionAsync)
            .Produces<SessionOperationResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        sessions.MapPost("/{sessionId:guid}/complete", CompleteSessionAsync)
            .Produces<SessionOperationResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return endpoints;
    }

    private static async Task<IResult> GetComputersAsync(
        ArenaDeskDbContext database,
        CancellationToken cancellationToken)
    {
        var computers = await database.Computers
            .AsNoTracking()
            .OrderBy(computer => computer.Code)
            .ToListAsync(cancellationToken);
        var tariffs = (await database.Tariffs
            .AsNoTracking()
            .Where(tariff => tariff.IsActive)
            .OrderBy(tariff => tariff.HourlyRate)
            .ToListAsync(cancellationToken))
            .GroupBy(tariff => tariff.ComputerTier)
            .ToDictionary(group => group.Key, group => group.First());
        var activeSessions = await database.Sessions
            .AsNoTracking()
            .Where(session => session.Status == SessionStatus.Active || session.Status == SessionStatus.Ending)
            .Include(session => session.Payments)
            .ToDictionaryAsync(session => session.ComputerId, cancellationToken);

        var response = computers.Select(computer =>
        {
            activeSessions.TryGetValue(computer.Id, out var session);
            tariffs.TryGetValue(computer.Tier, out var tariff);
            var displayedStatus = GetDisplayedStatus(computer.Status, session);
            var latestPayment = session?.Payments.OrderByDescending(payment => payment.PaidAtUtc).FirstOrDefault();
            var activeSession = session is null
                ? null
                : new ActiveSessionResponse(
                    session.Id,
                    session.CustomerName,
                    session.StartedAtUtc,
                    session.EndsAtUtc,
                    session.InitialDurationMinutes + session.AddedDurationMinutes,
                    session.FinalPrice,
                    session.Currency,
                    latestPayment is null ? null : ToApiValue(latestPayment.Method));

            return new ComputerResponse(
                computer.Id,
                computer.Code,
                computer.DisplayName,
                ToApiValue(computer.Tier),
                displayedStatus,
                ToApiValue(computer.EndAction),
                tariff?.HourlyRate ?? 0,
                tariff?.Currency ?? "TMT",
                activeSession);
        }).ToList();

        return Results.Ok(response);
    }

    private static async Task<IResult> GetTariffsAsync(
        ArenaDeskDbContext database,
        CancellationToken cancellationToken)
    {
        var tariffs = await database.Tariffs
            .AsNoTracking()
            .Where(tariff => tariff.IsActive)
            .OrderBy(tariff => tariff.HourlyRate)
            .Select(tariff => new TariffResponse(
                tariff.Id,
                tariff.Name,
                tariff.ComputerTier == ComputerTier.Vip ? "vip" : "standard",
                tariff.HourlyRate,
                tariff.Currency))
            .ToListAsync(cancellationToken);
        return Results.Ok(tariffs);
    }

    private static async Task<IResult> StartSessionAsync(
        StartSessionRequest request,
        ClaimsPrincipal principal,
        HttpContext context,
        ArenaDeskDbContext database,
        CancellationToken cancellationToken)
    {
        if (!IsValidDuration(request.DurationMinutes, 15, 720) || !TryParsePaymentMethod(request.PaymentMethod, out var paymentMethod))
        {
            return InvalidOperation("Dowamlylyk 15–720 minut bolmaly we töleg görnüşi cash ýa-da card bolmaly.");
        }

        var cashierId = GetUserId(principal);
        try
        {
            return await ExecuteInTransactionAsync(database, cancellationToken, async () =>
            {
                var computer = await database.Computers.SingleOrDefaultAsync(item => item.Id == request.ComputerId, cancellationToken);
                if (computer is null)
                {
                    return Results.NotFound();
                }

                var alreadyActive = await database.Sessions.AnyAsync(
                    session => session.ComputerId == computer.Id &&
                        (session.Status == SessionStatus.Active || session.Status == SessionStatus.Ending),
                    cancellationToken);
                if (alreadyActive || computer.Status is ComputerStatus.Offline or ComputerStatus.Locking)
                {
                    return Conflict("Bu kompýuter täze sessiýa üçin taýýar däl.");
                }

                var tariff = await database.Tariffs.SingleOrDefaultAsync(
                    item => item.ComputerTier == computer.Tier && item.IsActive,
                    cancellationToken);
                if (tariff is null)
                {
                    return Conflict("Bu kompýuter görnüşi üçin aktiw tarif tapylmady.");
                }

                var now = DateTimeOffset.UtcNow;
                var price = CalculatePrice(tariff.HourlyRate, request.DurationMinutes);
                var session = new Session
                {
                    ComputerId = computer.Id,
                    TariffId = tariff.Id,
                    CashierId = cashierId,
                    CustomerName = NormalizeCustomerName(request.CustomerName),
                    StartedAtUtc = now,
                    EndsAtUtc = now.AddMinutes(request.DurationMinutes),
                    InitialDurationMinutes = request.DurationMinutes,
                    Status = SessionStatus.Active,
                    HourlyRateSnapshot = tariff.HourlyRate,
                    InitialPrice = price,
                    FinalPrice = price,
                    Currency = tariff.Currency,
                };
                database.Sessions.Add(session);
                database.Payments.Add(new Payment
                {
                    Session = session,
                    CashierId = cashierId,
                    Amount = price,
                    Currency = tariff.Currency,
                    Method = paymentMethod,
                    PaidAtUtc = now,
                });
                computer.Status = ComputerStatus.Occupied;
                computer.UpdatedAtUtc = now;
                database.AuditLogs.Add(CreateAuditLog(cashierId, "Session.Started", "Session", session.Id, context,
                    new { computerId = computer.Id, request.DurationMinutes, price, paymentMethod = request.PaymentMethod }));

                await database.SaveChangesAsync(cancellationToken);
                var response = ToResponse(session);
                return Results.Created($"/api/v1/sessions/{session.Id}", response);
            });
        }
        catch (DbUpdateException)
        {
            return Conflict("Bu kompýuterde eýýäm aktiw sessiýa bar.");
        }
    }

    private static async Task<IResult> ExtendSessionAsync(
        Guid sessionId,
        ExtendSessionRequest request,
        ClaimsPrincipal principal,
        HttpContext context,
        ArenaDeskDbContext database,
        CancellationToken cancellationToken)
    {
        if (!IsValidDuration(request.DurationMinutes, 15, 360) || !TryParsePaymentMethod(request.PaymentMethod, out var paymentMethod))
        {
            return InvalidOperation("Goşulýan wagt 15–360 minut bolmaly we töleg görnüşi cash ýa-da card bolmaly.");
        }

        var cashierId = GetUserId(principal);
        return await ExecuteInTransactionAsync(database, cancellationToken, async () =>
        {
            var session = await database.Sessions
                .Include(item => item.Computer)
                .SingleOrDefaultAsync(item => item.Id == sessionId, cancellationToken);
            if (session is null)
            {
                return Results.NotFound();
            }
            if (session.Status is not (SessionStatus.Active or SessionStatus.Ending))
            {
                return Conflict("Diňe aktiw sessiýany uzaldyp bolýar.");
            }

            var now = DateTimeOffset.UtcNow;
            var extraPrice = CalculatePrice(session.HourlyRateSnapshot, request.DurationMinutes);
            session.EndsAtUtc = (session.EndsAtUtc > now ? session.EndsAtUtc : now).AddMinutes(request.DurationMinutes);
            session.AddedDurationMinutes += request.DurationMinutes;
            session.FinalPrice += extraPrice;
            session.Status = SessionStatus.Active;
            session.UpdatedAtUtc = now;
            session.Computer.Status = ComputerStatus.Occupied;
            session.Computer.UpdatedAtUtc = now;
            database.Payments.Add(new Payment
            {
                SessionId = session.Id,
                CashierId = cashierId,
                Amount = extraPrice,
                Currency = session.Currency,
                Method = paymentMethod,
                PaidAtUtc = now,
            });
            database.AuditLogs.Add(CreateAuditLog(cashierId, "Session.Extended", "Session", session.Id, context,
                new { request.DurationMinutes, extraPrice, paymentMethod = request.PaymentMethod }));

            await database.SaveChangesAsync(cancellationToken);
            return Results.Ok(ToResponse(session));
        });
    }

    private static async Task<IResult> CompleteSessionAsync(
        Guid sessionId,
        ClaimsPrincipal principal,
        HttpContext context,
        ArenaDeskDbContext database,
        CancellationToken cancellationToken)
    {
        var cashierId = GetUserId(principal);
        return await ExecuteInTransactionAsync(database, cancellationToken, async () =>
        {
            var session = await database.Sessions
                .Include(item => item.Computer)
                .SingleOrDefaultAsync(item => item.Id == sessionId, cancellationToken);
            if (session is null)
            {
                return Results.NotFound();
            }
            if (session.Status is not (SessionStatus.Active or SessionStatus.Ending))
            {
                return Conflict("Bu sessiýa eýýäm tamamlandy.");
            }

            var now = DateTimeOffset.UtcNow;
            session.Status = SessionStatus.Completed;
            session.CompletedAtUtc = now;
            session.UpdatedAtUtc = now;
            session.Computer.Status = ComputerStatus.Available;
            session.Computer.UpdatedAtUtc = now;
            database.AuditLogs.Add(CreateAuditLog(cashierId, "Session.Completed", "Session", session.Id, context));
            await database.SaveChangesAsync(cancellationToken);
            return Results.Ok(ToResponse(session));
        });
    }

    private static Task<IResult> ExecuteInTransactionAsync(
        ArenaDeskDbContext database,
        CancellationToken cancellationToken,
        Func<Task<IResult>> operation)
    {
        var executionStrategy = database.Database.CreateExecutionStrategy();
        return executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await database.Database.BeginTransactionAsync(
                IsolationLevel.Serializable,
                cancellationToken);
            var result = await operation();
            await transaction.CommitAsync(cancellationToken);
            return result;
        });
    }

    private static SessionOperationResponse ToResponse(Session session) => new(
        session.Id,
        session.ComputerId,
        ToApiValue(session.Status),
        session.StartedAtUtc,
        session.EndsAtUtc,
        session.InitialDurationMinutes + session.AddedDurationMinutes,
        session.FinalPrice,
        session.Currency);

    private static decimal CalculatePrice(decimal hourlyRate, int durationMinutes) =>
        decimal.Round(hourlyRate * durationMinutes / 60m, 2, MidpointRounding.AwayFromZero);

    private static bool IsValidDuration(int value, int minimum, int maximum) => value >= minimum && value <= maximum;

    private static bool TryParsePaymentMethod(string? value, out PaymentMethod method)
    {
        var normalized = value?.Trim().ToLowerInvariant();
        method = normalized switch
        {
            "cash" => PaymentMethod.Cash,
            "card" => PaymentMethod.Card,
            _ => default,
        };
        return normalized is "cash" or "card";
    }

    private static string? NormalizeCustomerName(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized[..Math.Min(normalized.Length, 120)];
    }

    private static Guid GetUserId(ClaimsPrincipal principal) =>
        Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private static AuditLog CreateAuditLog(
        Guid userId,
        string action,
        string entityType,
        Guid entityId,
        HttpContext context,
        object? details = null) => new()
    {
        UserId = userId,
        Action = action,
        EntityType = entityType,
        EntityId = entityId,
        DetailsJson = details is null ? null : JsonSerializer.Serialize(details),
        IpAddress = context.Connection.RemoteIpAddress?.ToString(),
    };

    private static string GetDisplayedStatus(ComputerStatus status, Session? session)
    {
        if (session is null)
        {
            return status == ComputerStatus.Locking ? "locked" : ToApiValue(status);
        }

        return session.EndsAtUtc <= DateTimeOffset.UtcNow.AddMinutes(10) ? "ending" : "occupied";
    }

    private static string ToApiValue<TEnum>(TEnum value) where TEnum : struct, Enum =>
        value.ToString().ToLowerInvariant();

    private static IResult InvalidOperation(string detail) => Results.Problem(
        statusCode: StatusCodes.Status400BadRequest,
        title: "Nädogry maglumat",
        detail: detail);

    private static IResult Conflict(string detail) => Results.Problem(
        statusCode: StatusCodes.Status409Conflict,
        title: "Amal ýerine ýetirilmedi",
        detail: detail);
}
