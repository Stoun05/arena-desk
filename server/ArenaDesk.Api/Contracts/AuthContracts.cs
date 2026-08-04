namespace ArenaDesk.Api.Contracts;

public sealed record LoginRequest(string Email, string Password);
public sealed record UserResponse(Guid Id, string Name, string Email, string Role);
