using ArenaDesk.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ArenaDesk.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Computer> Computers => Set<Computer>();
    public DbSet<Tariff> Tariffs => Set<Tariff>();
    public DbSet<GameSession> Sessions => Set<GameSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Id).HasColumnName("id");
            entity.Property(item => item.Name).HasColumnName("name").HasMaxLength(120);
            entity.Property(item => item.Email).HasColumnName("email").HasMaxLength(180);
            entity.Property(item => item.PasswordHash).HasColumnName("password_hash");
            entity.Property(item => item.Role).HasColumnName("role").HasConversion<string>().HasMaxLength(30);
            entity.Property(item => item.CreatedAt).HasColumnName("created_at");
            entity.HasIndex(item => item.Email).IsUnique();
        });

        modelBuilder.Entity<Computer>(entity =>
        {
            entity.ToTable("computers");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Id).HasColumnName("id");
            entity.Property(item => item.Name).HasColumnName("name").HasMaxLength(30);
            entity.Property(item => item.Zone).HasColumnName("zone").HasMaxLength(40);
            entity.Property(item => item.Status).HasColumnName("status").HasMaxLength(30);
            entity.Property(item => item.MacAddress).HasColumnName("mac_address").HasMaxLength(40);
            entity.Property(item => item.LastSeenAt).HasColumnName("last_seen_at");
            entity.HasIndex(item => item.Name).IsUnique();
        });

        modelBuilder.Entity<Tariff>(entity =>
        {
            entity.ToTable("tariffs");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Id).HasColumnName("id");
            entity.Property(item => item.Name).HasColumnName("name").HasMaxLength(80);
            entity.Property(item => item.HourlyRate).HasColumnName("hourly_rate").HasPrecision(10, 2);
            entity.Property(item => item.IsActive).HasColumnName("is_active");
            entity.HasIndex(item => item.Name).IsUnique();
        });

        modelBuilder.Entity<GameSession>(entity =>
        {
            entity.ToTable("sessions");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Id).HasColumnName("id");
            entity.Property(item => item.ComputerId).HasColumnName("computer_id");
            entity.Property(item => item.TariffId).HasColumnName("tariff_id");
            entity.Property(item => item.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(item => item.CustomerName).HasColumnName("customer_name").HasMaxLength(120);
            entity.Property(item => item.PurchasedMinutes).HasColumnName("purchased_minutes");
            entity.Property(item => item.StartedAt).HasColumnName("started_at");
            entity.Property(item => item.EndsAt).HasColumnName("ends_at");
            entity.Property(item => item.FinishedAt).HasColumnName("finished_at");
            entity.Property(item => item.TotalPrice).HasColumnName("total_price").HasPrecision(10, 2);
            entity.Property(item => item.Status).HasColumnName("status").HasMaxLength(30);
            entity.HasOne(item => item.Computer).WithMany(item => item.Sessions).HasForeignKey(item => item.ComputerId);
            entity.HasOne(item => item.Tariff).WithMany(item => item.Sessions).HasForeignKey(item => item.TariffId);
            entity.HasOne(item => item.CreatedByUser).WithMany().HasForeignKey(item => item.CreatedByUserId);
        });
    }
}
