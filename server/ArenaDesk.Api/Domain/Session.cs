namespace ArenaDesk.Api.Domain;

public sealed class Session
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ComputerId { get; set; }
    public Guid TariffId { get; set; }
    public Guid CashierId { get; set; }
    public string? CustomerName { get; set; }
    public DateTimeOffset StartedAtUtc { get; set; }
    public DateTimeOffset EndsAtUtc { get; set; }
    public DateTimeOffset? CompletedAtUtc { get; set; }
    public int InitialDurationMinutes { get; set; }
    public int AddedDurationMinutes { get; set; }
    public SessionStatus Status { get; set; } = SessionStatus.Active;
    public decimal HourlyRateSnapshot { get; set; }
    public decimal InitialPrice { get; set; }
    public decimal FinalPrice { get; set; }
    public string Currency { get; set; } = "TMT";
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public Computer Computer { get; set; } = null!;
    public Tariff Tariff { get; set; } = null!;
    public UserAccount Cashier { get; set; } = null!;
    public ICollection<Payment> Payments { get; } = new List<Payment>();
}
