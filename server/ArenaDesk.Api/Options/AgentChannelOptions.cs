using System.ComponentModel.DataAnnotations;

namespace ArenaDesk.Api.Options;

public sealed class AgentChannelOptions
{
    public const string SectionName = "AgentChannel";

    [Required, MinLength(32)]
    public string AccessKey { get; init; } = string.Empty;
}
