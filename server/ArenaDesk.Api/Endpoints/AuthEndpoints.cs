using System.Security.Claims;
using System.Text.Json;
using ArenaDesk.Api.Authentication;
using ArenaDesk.Api.Contracts;
using ArenaDesk.Api.Domain;
using ArenaDesk.Api.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ArenaDesk.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/auth")
            .WithTags("Authentication")
            .RequireRateLimiting("api");

        group.MapPost("/login", LoginAsync)
            .RequireRateLimiting("login")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapGet("/me", (ClaimsPrincipal principal) =>
        {
            var id = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var username = principal.Identity!.Name!;
            var role = principal.FindFirstValue(ClaimTypes.Role)!;
            return Results.Ok(ToUserResponse(id, username, role));
        })
        .RequireAuthorization()
        .Produces<AuthUserResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        return endpoints;
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        ArenaDeskDbContext database,
        IPasswordHasher<UserAccount> passwordHasher,
        ITokenService tokenService,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var username = request.Username.Trim().ToLowerInvariant();
        if (username.Length < 2 || string.IsNullOrWhiteSpace(request.Password))
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["credentials"] = ["Ulanyjy adyny we paroly doly giriziň."],
            });
        }

        var user = await database.Users.SingleOrDefaultAsync(
            candidate => candidate.Username == username,
            cancellationToken);
        var verification = user is null
            ? PasswordVerificationResult.Failed
            : passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (user is null || !user.IsActive || verification == PasswordVerificationResult.Failed)
        {
            database.AuditLogs.Add(CreateAuditLog(
                user?.Id,
                "Auth.LoginFailed",
                context,
                JsonSerializer.Serialize(new { username })));
            await database.SaveChangesAsync(cancellationToken);
            return Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Giriş şowsuz",
                detail: "Ulanyjy ady ýa-da parol nädogry.");
        }

        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
            user.UpdatedAtUtc = DateTimeOffset.UtcNow;
        }

        var (token, expiresAtUtc) = tokenService.CreateAccessToken(user);
        database.AuditLogs.Add(CreateAuditLog(user.Id, "Auth.LoginSucceeded", context));
        await database.SaveChangesAsync(cancellationToken);

        return Results.Ok(new LoginResponse(
            token,
            expiresAtUtc,
            ToUserResponse(user.Id, user.Username, user.Role.ToString())));
    }

    private static AuditLog CreateAuditLog(
        Guid? userId,
        string action,
        HttpContext context,
        string? detailsJson = null) => new()
    {
        UserId = userId,
        Action = action,
        EntityType = "UserAccount",
        EntityId = userId,
        DetailsJson = detailsJson,
        IpAddress = context.Connection.RemoteIpAddress?.ToString(),
    };

    private static AuthUserResponse ToUserResponse(Guid id, string username, string role) =>
        new(id, username, role.Equals(nameof(UserRole.Administrator), StringComparison.OrdinalIgnoreCase) ? "admin" : "cashier");
}
