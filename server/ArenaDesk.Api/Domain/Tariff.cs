namespace ArenaDesk.Api.Domain;

public sealed class Tariff
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public ComputerTier ComputerTier { get; set; }
    public decimal HourlyRate { get; set; }
    public string Currency { get; set; } = "TMT";
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<Session> Sessions { get; } = new List<Session>();
}
