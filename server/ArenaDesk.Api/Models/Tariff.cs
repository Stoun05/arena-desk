namespace ArenaDesk.Api.Models;

public sealed class Tariff
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public decimal HourlyRate { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<GameSession> Sessions { get; set; } = [];
}
