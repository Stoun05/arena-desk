namespace ArenaDesk.Api.Contracts;

public sealed record ActiveSessionResponse(
    Guid Id,
    string? CustomerName,
    DateTimeOffset StartedAtUtc,
    DateTimeOffset EndsAtUtc,
    int DurationMinutes,
    decimal CurrentPrice,
    string Currency,
    string? PaymentMethod);

public sealed record ComputerResponse(
    Guid Id,
    string Code,
    string DisplayName,
    string Tier,
    string Status,
    string EndAction,
    decimal HourlyRate,
    string Currency,
    ActiveSessionResponse? ActiveSession);

public sealed record TariffResponse(
    Guid Id,
    string Name,
    string ComputerTier,
    decimal HourlyRate,
    string Currency);

public sealed record StartSessionRequest(
    Guid ComputerId,
    int DurationMinutes,
    string? CustomerName,
    string PaymentMethod);

public sealed record ExtendSessionRequest(int DurationMinutes, string PaymentMethod);

public sealed record SessionOperationResponse(
    Guid SessionId,
    Guid ComputerId,
    string Status,
    DateTimeOffset StartedAtUtc,
    DateTimeOffset EndsAtUtc,
    int DurationMinutes,
    decimal TotalPrice,
    string Currency);
