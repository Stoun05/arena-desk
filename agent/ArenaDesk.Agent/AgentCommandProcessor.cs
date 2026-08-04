using ArenaDesk.Contracts;
using Microsoft.Extensions.Options;

namespace ArenaDesk.Agent;

public sealed class AgentCommandProcessor(
    IOptions<AgentOptions> options,
    ILogger<AgentCommandProcessor> logger)
{
    public Task<AgentCommandAcknowledgement> ProcessAsync(
        AgentCommand command,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!string.Equals(options.Value.CommandExecutionMode, "LogOnly", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(new AgentCommandAcknowledgement(
                command.CommandId,
                "rejected",
                "Only LogOnly execution mode is enabled in Stage 11.",
                DateTimeOffset.UtcNow));
        }

        logger.LogInformation(
            "Received {CommandType} command {CommandId} for computer {ComputerId}; Stage 11 is running in LogOnly mode.",
            command.Type,
            command.CommandId,
            command.ComputerId);
        return Task.FromResult(new AgentCommandAcknowledgement(
            command.CommandId,
            "received",
            null,
            DateTimeOffset.UtcNow));
    }
}
