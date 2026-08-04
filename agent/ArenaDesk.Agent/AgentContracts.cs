namespace ArenaDesk.Agent;

public sealed record RegistrationRequest(string ComputerName, string MachineName, string? MacAddress, string AgentVersion);
public sealed record RegistrationResponse(Guid ComputerId, string AgentToken, int HeartbeatIntervalSeconds);
public sealed record HeartbeatRequest(string AgentVersion);
public sealed record HeartbeatResponse(DateTimeOffset ServerTime, AgentCommand? Command);
public sealed record AgentCommand(Guid Id, string Type, string? Payload);
public sealed record CommandResultRequest(string Outcome, string? Message);
public sealed record CommandExecutionResult(string Outcome, string Message);
