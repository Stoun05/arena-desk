namespace ArenaDesk.Api.Domain;

public sealed class AgentCommandRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ComputerId { get; set; }
    public Guid? SessionId { get; set; }
    public required string Type { get; set; }
    public DateTimeOffset? EndsAtUtc { get; set; }
    public string? EndAction { get; set; }
    public AgentCommandStatus Status { get; set; } = AgentCommandStatus.Pending;
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DeliveredAtUtc { get; set; }
    public DateTimeOffset? AcknowledgedAtUtc { get; set; }
    public string? Error { get; set; }

    public Computer Computer { get; set; } = null!;
    public Session? Session { get; set; }
}
