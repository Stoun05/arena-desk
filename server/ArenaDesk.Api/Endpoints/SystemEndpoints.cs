using ArenaDesk.Api.Contracts;
using ArenaDesk.Api.Options;
using ArenaDesk.Api.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ArenaDesk.Api.Endpoints;

public static class SystemEndpoints
{
    public static IEndpointRouteBuilder MapSystemEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/system")
            .WithTags("System")
            .RequireRateLimiting("api");

        group.MapGet("/status", async (
            IHostEnvironment environment,
            IOptions<ArenaDeskOptions> options,
            ArenaDeskDbContext database,
            CancellationToken cancellationToken) =>
        {
            var databaseStatus = "unavailable";
            try
            {
                databaseStatus = await database.Database.CanConnectAsync(cancellationToken)
                    ? "available"
                    : "unavailable";
            }
            catch (Exception)
            {
                // The status endpoint remains available while readiness reports the dependency failure.
            }

            var response = new SystemStatusResponse(
                Service: "ArenaDesk.Api",
                Version: "0.1.0",
                Environment: environment.EnvironmentName,
                ServerTimeUtc: DateTimeOffset.UtcNow,
                ComputerLimit: options.Value.ComputerLimit,
                Database: databaseStatus,
                Stage: 7);

            return Results.Ok(response);
        })
        .WithName("GetSystemStatus")
        .Produces<SystemStatusResponse>(StatusCodes.Status200OK);

        return endpoints;
    }
}
