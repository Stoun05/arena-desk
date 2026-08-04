using System.Text.Json;
using ArenaDesk.Api.Data;
using ArenaDesk.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ArenaDesk.Api.Services;

public sealed class SessionExpiryWorker(IServiceScopeFactory scopeFactory, ILogger<SessionExpiryWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await FinishExpiredSessionsAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed to finish expired ArenaDesk sessions.");
            }
        }
    }

    private async Task FinishExpiredSessionsAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var now = DateTimeOffset.UtcNow;
        var sessions = await db.Sessions
            .Include(item => item.Computer)
            .Where(item => item.Status == "active" && item.EndsAt <= now)
            .ToListAsync(cancellationToken);

        foreach (var session in sessions)
        {
            session.Status = "finished";
            session.FinishedAt = now;
            session.Computer.Status = "available";
            db.DeviceCommands.Add(new DeviceCommand
            {
                ComputerId = session.ComputerId,
                Type = "lock",
                Payload = JsonSerializer.Serialize(new { session.Id, session.FinishedAt, Reason = "time-expired" }),
                Status = "pending",
            });
        }

        if (sessions.Count > 0) await db.SaveChangesAsync(cancellationToken);
    }
}
