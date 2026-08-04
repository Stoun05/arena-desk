using ArenaDesk.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArenaDesk.Api.Persistence;

public sealed class UserAccountConfiguration : IEntityTypeConfiguration<UserAccount>
{
    public void Configure(EntityTypeBuilder<UserAccount> builder)
    {
        builder.ToTable("users");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Id).HasColumnName("id");
        builder.Property(entity => entity.Username).HasColumnName("username").HasMaxLength(80).IsRequired();
        builder.Property(entity => entity.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
        builder.Property(entity => entity.Role).HasColumnName("role").HasConversion<string>().HasMaxLength(24);
        builder.Property(entity => entity.IsActive).HasColumnName("is_active");
        builder.Property(entity => entity.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(entity => entity.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.HasIndex(entity => entity.Username).IsUnique().HasDatabaseName("ux_users_username");
    }
}

public sealed class ComputerConfiguration : IEntityTypeConfiguration<Computer>
{
    public void Configure(EntityTypeBuilder<Computer> builder)
    {
        builder.ToTable("computers");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Id).HasColumnName("id");
        builder.Property(entity => entity.Code).HasColumnName("code").HasMaxLength(20).IsRequired();
        builder.Property(entity => entity.DisplayName).HasColumnName("display_name").HasMaxLength(80).IsRequired();
        builder.Property(entity => entity.Tier).HasColumnName("tier").HasConversion<string>().HasMaxLength(16);
        builder.Property(entity => entity.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(24);
        builder.Property(entity => entity.EndAction).HasColumnName("end_action").HasConversion<string>().HasMaxLength(16);
        builder.Property(entity => entity.AgentId).HasColumnName("agent_id").HasMaxLength(100);
        builder.Property(entity => entity.LastSeenAtUtc).HasColumnName("last_seen_at_utc");
        builder.Property(entity => entity.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(entity => entity.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.HasIndex(entity => entity.Code).IsUnique().HasDatabaseName("ux_computers_code");
        builder.HasIndex(entity => entity.AgentId).IsUnique().HasDatabaseName("ux_computers_agent_id");
        builder.HasIndex(entity => entity.Status).HasDatabaseName("ix_computers_status");
    }
}

public sealed class TariffConfiguration : IEntityTypeConfiguration<Tariff>
{
    public void Configure(EntityTypeBuilder<Tariff> builder)
    {
        builder.ToTable("tariffs", table => table.HasCheckConstraint("ck_tariffs_hourly_rate", "hourly_rate > 0"));
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Id).HasColumnName("id");
        builder.Property(entity => entity.Name).HasColumnName("name").HasMaxLength(80).IsRequired();
        builder.Property(entity => entity.ComputerTier).HasColumnName("computer_tier").HasConversion<string>().HasMaxLength(16);
        builder.Property(entity => entity.HourlyRate).HasColumnName("hourly_rate").HasPrecision(12, 2);
        builder.Property(entity => entity.Currency).HasColumnName("currency").HasMaxLength(3).IsFixedLength();
        builder.Property(entity => entity.IsActive).HasColumnName("is_active");
        builder.Property(entity => entity.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(entity => entity.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.HasIndex(entity => new { entity.Name, entity.ComputerTier })
            .IsUnique()
            .HasDatabaseName("ux_tariffs_name_tier");
    }
}

public sealed class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("sessions", table =>
        {
            table.HasCheckConstraint("ck_sessions_initial_duration", "initial_duration_minutes > 0");
            table.HasCheckConstraint("ck_sessions_added_duration", "added_duration_minutes >= 0");
            table.HasCheckConstraint("ck_sessions_end_after_start", "ends_at_utc > started_at_utc");
        });
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Id).HasColumnName("id");
        builder.Property(entity => entity.ComputerId).HasColumnName("computer_id");
        builder.Property(entity => entity.TariffId).HasColumnName("tariff_id");
        builder.Property(entity => entity.CashierId).HasColumnName("cashier_id");
        builder.Property(entity => entity.CustomerName).HasColumnName("customer_name").HasMaxLength(120);
        builder.Property(entity => entity.StartedAtUtc).HasColumnName("started_at_utc");
        builder.Property(entity => entity.EndsAtUtc).HasColumnName("ends_at_utc");
        builder.Property(entity => entity.CompletedAtUtc).HasColumnName("completed_at_utc");
        builder.Property(entity => entity.InitialDurationMinutes).HasColumnName("initial_duration_minutes");
        builder.Property(entity => entity.AddedDurationMinutes).HasColumnName("added_duration_minutes");
        builder.Property(entity => entity.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(24);
        builder.Property(entity => entity.HourlyRateSnapshot).HasColumnName("hourly_rate_snapshot").HasPrecision(12, 2);
        builder.Property(entity => entity.InitialPrice).HasColumnName("initial_price").HasPrecision(12, 2);
        builder.Property(entity => entity.FinalPrice).HasColumnName("final_price").HasPrecision(12, 2);
        builder.Property(entity => entity.Currency).HasColumnName("currency").HasMaxLength(3).IsFixedLength();
        builder.Property(entity => entity.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(entity => entity.UpdatedAtUtc).HasColumnName("updated_at_utc");

        builder.HasOne(entity => entity.Computer).WithMany(entity => entity.Sessions)
            .HasForeignKey(entity => entity.ComputerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(entity => entity.Tariff).WithMany(entity => entity.Sessions)
            .HasForeignKey(entity => entity.TariffId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(entity => entity.Cashier).WithMany(entity => entity.OpenedSessions)
            .HasForeignKey(entity => entity.CashierId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(entity => entity.ComputerId)
            .IsUnique()
            .HasDatabaseName("ux_sessions_active_computer")
            .HasFilter("status IN ('Active', 'Ending')");
        builder.HasIndex(entity => entity.StartedAtUtc).HasDatabaseName("ix_sessions_started_at_utc");
        builder.HasIndex(entity => entity.CashierId).HasDatabaseName("ix_sessions_cashier_id");
    }
}

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments", table => table.HasCheckConstraint("ck_payments_amount", "amount > 0"));
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Id).HasColumnName("id");
        builder.Property(entity => entity.SessionId).HasColumnName("session_id");
        builder.Property(entity => entity.CashierId).HasColumnName("cashier_id");
        builder.Property(entity => entity.Amount).HasColumnName("amount").HasPrecision(12, 2);
        builder.Property(entity => entity.Currency).HasColumnName("currency").HasMaxLength(3).IsFixedLength();
        builder.Property(entity => entity.Method).HasColumnName("method").HasConversion<string>().HasMaxLength(16);
        builder.Property(entity => entity.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(16);
        builder.Property(entity => entity.RefundReason).HasColumnName("refund_reason").HasMaxLength(300);
        builder.Property(entity => entity.PaidAtUtc).HasColumnName("paid_at_utc");
        builder.Property(entity => entity.RefundedAtUtc).HasColumnName("refunded_at_utc");

        builder.HasOne(entity => entity.Session).WithMany(entity => entity.Payments)
            .HasForeignKey(entity => entity.SessionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(entity => entity.Cashier).WithMany(entity => entity.RecordedPayments)
            .HasForeignKey(entity => entity.CashierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(entity => entity.SessionId).HasDatabaseName("ix_payments_session_id");
        builder.HasIndex(entity => entity.PaidAtUtc).HasDatabaseName("ix_payments_paid_at_utc");
    }
}

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Id).HasColumnName("id");
        builder.Property(entity => entity.UserId).HasColumnName("user_id");
        builder.Property(entity => entity.Action).HasColumnName("action").HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.EntityType).HasColumnName("entity_type").HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.EntityId).HasColumnName("entity_id");
        builder.Property(entity => entity.DetailsJson).HasColumnName("details_json").HasColumnType("jsonb");
        builder.Property(entity => entity.IpAddress).HasColumnName("ip_address").HasMaxLength(64);
        builder.Property(entity => entity.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.HasOne(entity => entity.User).WithMany(entity => entity.AuditLogs)
            .HasForeignKey(entity => entity.UserId).OnDelete(DeleteBehavior.SetNull);
        builder.HasIndex(entity => entity.CreatedAtUtc).HasDatabaseName("ix_audit_logs_created_at_utc");
        builder.HasIndex(entity => new { entity.EntityType, entity.EntityId }).HasDatabaseName("ix_audit_logs_entity");
    }
}
