using System.Threading.RateLimiting;
using ArenaDesk.Api.Endpoints;
using ArenaDesk.Api.Options;
using ArenaDesk.Api.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
});

builder.Services.AddProblemDetails();

var connectionString = builder.Configuration.GetConnectionString("ArenaDesk");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("ConnectionStrings:ArenaDesk must be configured.");
}

builder.Services.AddDbContextPool<ArenaDeskDbContext>(options =>
    options.UseNpgsql(connectionString, npgsql =>
        npgsql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null)));

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddCheck<DatabaseHealthCheck>("postgresql", tags: ["ready"]);

builder.Services
    .AddOptions<ArenaDeskOptions>()
    .Bind(builder.Configuration.GetSection(ArenaDeskOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .GetChildren()
    .Select(section => section.Value)
    .Where(value => !string.IsNullOrWhiteSpace(value))
    .Cast<string>()
    .ToArray();

if (allowedOrigins.Length == 0)
{
    throw new InvalidOperationException("At least one CORS origin must be configured.");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("Dashboard", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("api", limiter =>
    {
        limiter.PermitLimit = 120;
        limiter.Window = TimeSpan.FromMinutes(1);
        limiter.QueueLimit = 0;
        limiter.AutoReplenishment = true;
    });
});

var app = builder.Build();

app.UseExceptionHandler();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    await next();
});

app.UseHttpsRedirection();
app.UseCors("Dashboard");
app.UseRateLimiter();

app.MapGet("/", () => Results.Ok(new
{
    service = "ArenaDesk.Api",
    version = "0.1.0",
    statusEndpoint = "/api/v1/system/status",
    healthEndpoints = new[] { "/health/live", "/health/ready" },
}));

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("live"),
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("ready"),
});
app.MapSystemEndpoints();

app.Run();

public partial class Program { }
