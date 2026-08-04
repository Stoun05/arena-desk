using ArenaDesk.Contracts;
using Microsoft.Extensions.Options;

namespace ArenaDesk.Agent;

public sealed class AgentCommandProcessor(
    IOptions<AgentOptions> options,
    ProcessedCommandStore processedCommands,
    WindowsCommandExecutor executor,
    ILogger<AgentCommandProcessor> logger)
{
    public async Task<AgentCommandExecutionPlan> PrepareAsync(
        AgentCommand command,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (await processedCommands.ContainsAsync(command.CommandId, cancellationToken))
        {
            logger.LogInformation("Skipping duplicate command {CommandId}.", command.CommandId);
            return new AgentCommandExecutionPlan(Acknowledge(command, "completed"), null);
        }

        var systemMode = string.Equals(
            options.Value.CommandExecutionMode,
            "System",
            StringComparison.OrdinalIgnoreCase);
        if (systemMode && !OperatingSystem.IsWindows())
        {
            return new AgentCommandExecutionPlan(
                Acknowledge(command, "rejected", "System mode is available only on Windows."),
                null);
        }

        await processedCommands.MarkProcessedAsync(command.CommandId, cancellationToken);
        if (systemMode && executor.CanExecute(command.Type))
        {
            logger.LogWarning(
                "Accepted guarded system command {CommandType} ({CommandId}).",
                command.Type,
                command.CommandId);
            return new AgentCommandExecutionPlan(
                Acknowledge(command, "accepted"),
                token => executor.ExecuteAsync(command, token));
        }

        logger.LogInformation(
            "Recorded {CommandType} command {CommandId} for computer {ComputerId} in {Mode} mode.",
            command.Type,
            command.CommandId,
            command.ComputerId,
            options.Value.CommandExecutionMode);
        return new AgentCommandExecutionPlan(Acknowledge(command, "received"), null);
    }

    public static AgentCommandAcknowledgement Acknowledge(
        AgentCommand command,
        string status,
        string? error = null) => new(
            command.CommandId,
            status,
            error,
            DateTimeOffset.UtcNow);
}

public sealed record AgentCommandExecutionPlan(
    AgentCommandAcknowledgement InitialAcknowledgement,
    Func<CancellationToken, Task>? ExecuteAsync);
