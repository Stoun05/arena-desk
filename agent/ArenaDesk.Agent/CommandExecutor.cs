using Microsoft.Extensions.Options;

namespace ArenaDesk.Agent;

public sealed class CommandExecutor(IOptions<AgentOptions> options, ILogger<CommandExecutor> logger)
{
    private static readonly HashSet<string> KnownCommands = new(StringComparer.OrdinalIgnoreCase)
    {
        "unlock",
        "session-updated",
        "lock",
    };

    public Task<CommandExecutionResult> ExecuteAsync(AgentCommand command, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!KnownCommands.Contains(command.Type))
            return Task.FromResult(new CommandExecutionResult("failed", $"Unknown command: {command.Type}"));

        if (!options.Value.EnableDeviceControl)
        {
            logger.LogInformation("Observe-only mode: simulated {CommandType} command {CommandId}.", command.Type, command.Id);
            return Task.FromResult(new CommandExecutionResult("simulated", "Observe-only mode; no Windows action was executed."));
        }

        logger.LogWarning("Device control was enabled, but destructive Windows actions are not implemented in this milestone.");
        return Task.FromResult(new CommandExecutionResult("failed", "Windows device control is not implemented yet."));
    }
}
