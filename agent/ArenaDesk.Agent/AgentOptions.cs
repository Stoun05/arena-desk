using System.ComponentModel.DataAnnotations;

namespace ArenaDesk.Agent;

public sealed class AgentOptions
{
    public const string SectionName = "Agent";

    [Required, Url]
    public string HubUrl { get; init; } = string.Empty;

    [Required, MaxLength(100)]
    public string AgentId { get; init; } = string.Empty;

    [Required, MaxLength(20)]
    public string ComputerCode { get; init; } = string.Empty;

    [Required, MinLength(32)]
    public string AccessKey { get; init; } = string.Empty;

    [Range(5, 300)]
    public int HeartbeatSeconds { get; init; } = 15;

    [Required]
    public string CommandExecutionMode { get; init; } = "LogOnly";

    public string StateDirectory { get; init; } = string.Empty;
}
