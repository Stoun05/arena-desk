using ArenaDesk.Api.Contracts;
using ArenaDesk.Api.Domain;
using ArenaDesk.Api.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ArenaDesk.Api.Endpoints;

public static class AdminEndpoints
{
    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/admin")
            .WithTags("Administration")
            .RequireAuthorization("AdministratorOnly")
            .RequireRateLimiting("api");

        group.MapGet("/users", async (ArenaDeskDbContext database, CancellationToken cancellationToken) =>
        {
            var users = await database.Users
                .AsNoTracking()
                .OrderBy(user => user.Username)
                .Select(user => new UserSummaryResponse(
                    user.Id,
                    user.Username,
                    user.Role == UserRole.Administrator ? "admin" : "cashier",
                    user.IsActive))
                .ToListAsync(cancellationToken);

            return Results.Ok(users);
        })
        .Produces<List<UserSummaryResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden);

        return endpoints;
    }
}
