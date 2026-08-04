namespace ArenaDesk.Contracts;

public static class AgentProtocol
{
    public const string AccessKeyHeader = "X-Arena-Agent-Key";
    public const string AgentIdHeader = "X-Arena-Agent-Id";
    public const string ComputerCodeHeader = "X-Arena-Computer-Code";
    public const string CommandEvent = "AgentCommand";
    public const string RegisteredEvent = "AgentRegistered";

    public static string ComputerGroup(Guid computerId) => $"computer:{computerId:N}";
}

public static class PlayerScreenProtocol
{
    public const string DefaultPipeName = "ArenaDesk.PlayerScreen";
    public const string LockedMode = "locked";
    public const string ActiveMode = "active";
}

public static class AgentCommandTypes
{
    public const string Unlock = "unlock";
    public const string SyncSession = "sync-session";
    public const string Logout = "logout";
    public const string Sleep = "sleep";
    public const string Shutdown = "shutdown";
}

public sealed record AgentCommand(
    Guid CommandId,
    Guid ComputerId,
    Guid? SessionId,
    string Type,
    DateTimeOffset IssuedAtUtc,
    DateTimeOffset? EndsAtUtc,
    string? EndAction);

public sealed record AgentRegistration(
    Guid ComputerId,
    string ComputerCode,
    string DisplayName,
    string EndAction,
    DateTimeOffset ServerTimeUtc,
    Guid? ActiveSessionId,
    DateTimeOffset? EndsAtUtc);

public sealed record AgentHeartbeat(DateTimeOffset AgentTimeUtc);

public sealed record AgentHeartbeatResponse(DateTimeOffset ServerTimeUtc);

public sealed record AgentCommandAcknowledgement(
    Guid CommandId,
    string Status,
    string? Error,
    DateTimeOffset AcknowledgedAtUtc);

public sealed record PlayerScreenHandshake(string AccessKey);

public sealed record PlayerScreenState(
    string Mode,
    string ComputerCode,
    Guid? SessionId,
    DateTimeOffset? EndsAtUtc,
    string Message,
    DateTimeOffset UpdatedAtUtc);
