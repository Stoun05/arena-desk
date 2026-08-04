namespace ArenaDesk.Api.Contracts;

public sealed record LoginRequest(string Username, string Password);

public sealed record AuthUserResponse(Guid Id, string Username, string Role);

public sealed record LoginResponse(string AccessToken, DateTimeOffset ExpiresAtUtc, AuthUserResponse User);

public sealed record UserSummaryResponse(Guid Id, string Username, string Role, bool IsActive);
