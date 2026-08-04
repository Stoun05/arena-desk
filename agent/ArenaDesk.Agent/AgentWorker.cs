using ArenaDesk.Contracts;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace ArenaDesk.Agent;

public sealed class AgentWorker(
    IOptions<AgentOptions> options,
    AgentCommandProcessor commandProcessor,
    PlayerScreenChannel playerScreen,
    ILogger<AgentWorker> logger) : BackgroundService
{
    private HubConnection? _connection;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _connection = BuildConnection();
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_connection.State == HubConnectionState.Disconnected)
                {
                    await _connection.StartAsync(stoppingToken);
                    logger.LogInformation("ArenaDesk Agent connected as {AgentId}.", options.Value.AgentId);
                }

                await _connection.InvokeAsync<AgentHeartbeatResponse>(
                    "Heartbeat",
                    new AgentHeartbeat(DateTimeOffset.UtcNow),
                    stoppingToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                logger.LogWarning(exception, "Agent channel is unavailable; retrying after the heartbeat interval.");
            }

            await Task.Delay(TimeSpan.FromSeconds(options.Value.HeartbeatSeconds), stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }

        await base.StopAsync(cancellationToken);
    }

    private HubConnection BuildConnection()
    {
        var connection = new HubConnectionBuilder()
            .WithUrl(options.Value.HubUrl, connectionOptions =>
            {
                connectionOptions.Headers.Add(AgentProtocol.AccessKeyHeader, options.Value.AccessKey);
                connectionOptions.Headers.Add(AgentProtocol.AgentIdHeader, options.Value.AgentId);
                connectionOptions.Headers.Add(AgentProtocol.ComputerCodeHeader, options.Value.ComputerCode);
            })
            .WithAutomaticReconnect([TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10)])
            .Build();

        connection.On<AgentRegistration>(AgentProtocol.RegisteredEvent, async registration =>
        {
            logger.LogInformation(
                "Registered for {ComputerCode}; end action is {EndAction}.",
                registration.ComputerCode,
                registration.EndAction);
            await playerScreen.PublishRegistrationAsync(registration);
        });
        connection.On<AgentCommand>(AgentProtocol.CommandEvent, async command =>
        {
            try
            {
                var plan = await commandProcessor.PrepareAsync(command, CancellationToken.None);
                await playerScreen.PublishCommandAsync(command);
                await SendAcknowledgementAsync(connection, plan.InitialAcknowledgement);
                if (plan.ExecuteAsync is not null)
                {
                    try
                    {
                        await plan.ExecuteAsync(CancellationToken.None);
                        await SendAcknowledgementAsync(
                            connection,
                            AgentCommandProcessor.Acknowledge(command, "completed"));
                    }
                    catch (Exception exception)
                    {
                        logger.LogError(exception, "System command {CommandId} failed.", command.CommandId);
                        await SendAcknowledgementAsync(
                            connection,
                            AgentCommandProcessor.Acknowledge(command, "failed", exception.Message));
                    }
                }
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Command {CommandId} could not be prepared safely.", command.CommandId);
                await SendAcknowledgementAsync(
                    connection,
                    AgentCommandProcessor.Acknowledge(command, "failed", exception.Message));
            }
        });
        connection.Reconnecting += exception =>
        {
            logger.LogWarning(exception, "Agent channel reconnecting.");
            return Task.CompletedTask;
        };
        connection.Reconnected += connectionId =>
        {
            logger.LogInformation("Agent channel reconnected with connection {ConnectionId}.", connectionId);
            return Task.CompletedTask;
        };

        return connection;
    }

    private async Task SendAcknowledgementAsync(
        HubConnection connection,
        AgentCommandAcknowledgement acknowledgement)
    {
        if (connection.State != HubConnectionState.Connected)
        {
            return;
        }

        try
        {
            await connection.SendAsync("AcknowledgeCommand", acknowledgement);
        }
        catch (Exception exception)
        {
            logger.LogWarning(
                exception,
                "Acknowledgement for command {CommandId} will be recovered after reconnect.",
                acknowledgement.CommandId);
        }
    }
}
