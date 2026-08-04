using ArenaDesk.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ArenaDesk.Api.Data;

public sealed class DatabaseSeeder(AppDbContext db, IConfiguration configuration, IHostEnvironment environment)
{
    public async Task SeedAsync()
    {
        if (!await db.Users.AnyAsync())
        {
            var adminPassword = configuration["Seed:AdminPassword"];
            var cashierPassword = configuration["Seed:CashierPassword"];
            if (string.IsNullOrWhiteSpace(adminPassword) || string.IsNullOrWhiteSpace(cashierPassword))
            {
                if (environment.IsProduction())
                {
                    throw new InvalidOperationException("Seed passwords are required on first production startup.");
                }
                throw new InvalidOperationException("Set Seed:AdminPassword and Seed:CashierPassword before first startup.");
            }

            var hasher = new PasswordHasher<User>();
            var admin = new User
            {
                Name = "Arena administrator",
                Email = configuration["Seed:AdminEmail"] ?? "admin@arena.local",
                PasswordHash = string.Empty,
                Role = UserRole.Admin,
            };
            admin.PasswordHash = hasher.HashPassword(admin, adminPassword);

            var cashier = new User
            {
                Name = "Arena cashier",
                Email = configuration["Seed:CashierEmail"] ?? "cashier@arena.local",
                PasswordHash = string.Empty,
                Role = UserRole.Cashier,
            };
            cashier.PasswordHash = hasher.HashPassword(cashier, cashierPassword);
            db.Users.AddRange(admin, cashier);
        }

        if (!await db.Tariffs.AnyAsync())
        {
            db.Tariffs.AddRange(
                new Tariff { Name = "Gündiz", HourlyRate = 12m },
                new Tariff { Name = "Standart", HourlyRate = 15m },
                new Tariff { Name = "VIP", HourlyRate = 20m });
        }

        if (!await db.Computers.AnyAsync())
        {
            db.Computers.AddRange(Enumerable.Range(1, 10).Select(index => new Computer
            {
                Name = $"PC-{index:00}",
                Zone = index >= 8 ? "VIP" : "Standard",
                Status = index == 6 ? "offline" : "available",
            }));
        }

        await db.SaveChangesAsync();
    }
}
