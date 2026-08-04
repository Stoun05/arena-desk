namespace ArenaDesk.Api.Contracts;

public sealed record SystemStatusResponse(
    string Service,
    string Version,
    string Environment,
    DateTimeOffset ServerTimeUtc,
    int ComputerLimit,
    string Database,
    int Stage);
