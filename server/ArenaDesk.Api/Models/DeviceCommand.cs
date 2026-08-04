namespace ArenaDesk.Api.Models;

public sealed class DeviceCommand
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ComputerId { get; set; }
    public Computer Computer { get; set; } = null!;
    public required string Type { get; set; }
    public string? Payload { get; set; }
    public required string Status { get; set; }
    public int DeliveryAttempts { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DeliveredAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public string? ResultMessage { get; set; }
}
