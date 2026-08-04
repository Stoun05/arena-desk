namespace ArenaDesk.Api.Domain;

public sealed class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SessionId { get; set; }
    public Guid CashierId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TMT";
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Completed;
    public string? RefundReason { get; set; }
    public DateTimeOffset PaidAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? RefundedAtUtc { get; set; }

    public Session Session { get; set; } = null!;
    public UserAccount Cashier { get; set; } = null!;
}
