namespace ArenaDesk.Api.Contracts;

public sealed record ComputerStateChangedResponse(
    Guid ComputerId,
    string Action,
    DateTimeOffset OccurredAtUtc);
