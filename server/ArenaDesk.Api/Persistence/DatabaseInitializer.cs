using ArenaDesk.Api.Domain;
using ArenaDesk.Api.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ArenaDesk.Api.Persistence;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<ArenaDeskDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<UserAccount>>();
        var users = scope.ServiceProvider.GetRequiredService<IOptions<BootstrapUsersOptions>>().Value;

        await AddUserIfMissingAsync(
            database,
            passwordHasher,
            Guid.Parse("20000000-0000-0000-0000-000000000001"),
            users.AdministratorUsername,
            users.AdministratorPassword,
            UserRole.Administrator,
            cancellationToken);
        await AddUserIfMissingAsync(
            database,
            passwordHasher,
            Guid.Parse("20000000-0000-0000-0000-000000000002"),
            users.CashierUsername,
            users.CashierPassword,
            UserRole.Cashier,
            cancellationToken);

        await database.SaveChangesAsync(cancellationToken);
    }

    private static async Task AddUserIfMissingAsync(
        ArenaDeskDbContext database,
        IPasswordHasher<UserAccount> passwordHasher,
        Guid id,
        string username,
        string password,
        UserRole role,
        CancellationToken cancellationToken)
    {
        var normalizedUsername = username.Trim().ToLowerInvariant();
        if (await database.Users.AnyAsync(user => user.Username == normalizedUsername, cancellationToken))
        {
            return;
        }

        var user = new UserAccount
        {
            Id = id,
            Username = normalizedUsername,
            PasswordHash = string.Empty,
            Role = role,
        };
        user.PasswordHash = passwordHasher.HashPassword(user, password);
        database.Users.Add(user);
    }
}
