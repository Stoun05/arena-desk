namespace ArenaDesk.Api.Models;

public sealed class Computer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string Zone { get; set; }
    public required string Status { get; set; }
    public string? MacAddress { get; set; }
    public DateTimeOffset? LastSeenAt { get; set; }
    public ICollection<GameSession> Sessions { get; set; } = [];
}
