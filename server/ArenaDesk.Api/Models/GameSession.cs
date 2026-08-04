namespace ArenaDesk.Api.Models;

public sealed class GameSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ComputerId { get; set; }
    public Computer Computer { get; set; } = null!;
    public Guid TariffId { get; set; }
    public Tariff Tariff { get; set; } = null!;
    public Guid CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = null!;
    public required string CustomerName { get; set; }
    public int PurchasedMinutes { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }
    public decimal TotalPrice { get; set; }
    public required string Status { get; set; }
}
