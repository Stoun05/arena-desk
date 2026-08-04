using Microsoft.Extensions.Options;

namespace ArenaDesk.Agent;

public sealed class AgentWorker(
    ArenaApiClient api,
    CommandExecutor executor,
    IOptions<AgentOptions> options,
    ILogger<AgentWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var delay = TimeSpan.FromSeconds(Math.Clamp(options.Value.HeartbeatIntervalSeconds, 5, 60));
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await api.EnsureRegisteredAsync(stoppingToken);
                var heartbeat = await api.SendHeartbeatAsync(stoppingToken);
                if (heartbeat.Command is not null)
                {
                    var result = await executor.ExecuteAsync(heartbeat.Command, stoppingToken);
                    await api.ReportCommandResultAsync(heartbeat.Command.Id, result, stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "ArenaDesk heartbeat failed; retrying in {DelaySeconds} seconds.", delay.TotalSeconds);
            }

            await Task.Delay(delay, stoppingToken);
        }
    }
}
