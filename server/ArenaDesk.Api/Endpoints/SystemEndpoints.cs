using ArenaDesk.Api.Contracts;
using ArenaDesk.Api.Options;
using Microsoft.Extensions.Options;

namespace ArenaDesk.Api.Endpoints;

public static class SystemEndpoints
{
    public static IEndpointRouteBuilder MapSystemEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/system")
            .WithTags("System")
            .RequireRateLimiting("api");

        group.MapGet("/status", (
            IHostEnvironment environment,
            IOptions<ArenaDeskOptions> options) =>
        {
            var response = new SystemStatusResponse(
                Service: "ArenaDesk.Api",
                Version: "0.1.0",
                Environment: environment.EnvironmentName,
                ServerTimeUtc: DateTimeOffset.UtcNow,
                ComputerLimit: options.Value.ComputerLimit,
                Database: "not-configured",
                Stage: 6);

            return Results.Ok(response);
        })
        .WithName("GetSystemStatus")
        .Produces<SystemStatusResponse>(StatusCodes.Status200OK);

        return endpoints;
    }
}
