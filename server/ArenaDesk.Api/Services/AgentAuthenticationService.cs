using System.Security.Cryptography;
using System.Text;
using ArenaDesk.Api.Data;
using ArenaDesk.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ArenaDesk.Api.Services;

public sealed class AgentAuthenticationService(AppDbContext db, IConfiguration configuration)
{
    public bool IsBootstrapKeyValid(string? candidate)
    {
        var configured = configuration["Agent:BootstrapKey"];
        if (string.IsNullOrWhiteSpace(candidate) || string.IsNullOrWhiteSpace(configured)) return false;
        return FixedTimeEquals(candidate, configured);
    }

    public async Task<Computer?> AuthenticateAsync(string? authorizationHeader)
    {
        const string prefix = "Bearer ";
        if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return null;

        var token = authorizationHeader[prefix.Length..].Trim();
        if (token.Length < 32) return null;
        var tokenHash = HashToken(token);
        return await db.Computers.SingleOrDefaultAsync(item => item.AgentTokenHash == tokenHash);
    }

    public static string CreateToken() => Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();

    public static string HashToken(string token) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token))).ToLowerInvariant();

    private static bool FixedTimeEquals(string left, string right)
    {
        var leftHash = SHA256.HashData(Encoding.UTF8.GetBytes(left));
        var rightHash = SHA256.HashData(Encoding.UTF8.GetBytes(right));
        return CryptographicOperations.FixedTimeEquals(leftHash, rightHash);
    }
}
