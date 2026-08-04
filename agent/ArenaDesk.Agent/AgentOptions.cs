namespace ArenaDesk.Agent;

public sealed class AgentOptions
{
    public const string SectionName = "ArenaDesk";

    public string ApiBaseUrl { get; set; } = "http://localhost:8080/api";
    public string ComputerName { get; set; } = string.Empty;
    public string BootstrapKey { get; set; } = string.Empty;
    public string AgentToken { get; set; } = string.Empty;
    public int HeartbeatIntervalSeconds { get; set; } = 10;
    public bool EnableDeviceControl { get; set; }
}
