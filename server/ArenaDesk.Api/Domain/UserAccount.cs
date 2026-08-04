namespace ArenaDesk.Api.Domain;

public sealed class UserAccount
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<Session> OpenedSessions { get; } = new List<Session>();
    public ICollection<Payment> RecordedPayments { get; } = new List<Payment>();
    public ICollection<AuditLog> AuditLogs { get; } = new List<AuditLog>();
}
