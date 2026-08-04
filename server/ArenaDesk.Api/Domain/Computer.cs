namespace ArenaDesk.Api.Domain;

public sealed class Computer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Code { get; set; }
    public required string DisplayName { get; set; }
    public ComputerTier Tier { get; set; }
    public ComputerStatus Status { get; set; } = ComputerStatus.Offline;
    public SessionEndAction EndAction { get; set; } = SessionEndAction.Logout;
    public string? AgentId { get; set; }
    public DateTimeOffset? LastSeenAtUtc { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<Session> Sessions { get; } = new List<Session>();
}
