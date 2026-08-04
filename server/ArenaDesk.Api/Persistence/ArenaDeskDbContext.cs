using ArenaDesk.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace ArenaDesk.Api.Persistence;

public sealed class ArenaDeskDbContext(DbContextOptions<ArenaDeskDbContext> options)
    : DbContext(options)
{
    public DbSet<UserAccount> Users => Set<UserAccount>();
    public DbSet<Computer> Computers => Set<Computer>();
    public DbSet<Tariff> Tariffs => Set<Tariff>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ArenaDeskDbContext).Assembly);
    }
}
