namespace ArenaDesk.Api.Contracts;

public sealed record AgentRegistrationRequest(string ComputerName, string MachineName, string? MacAddress, string AgentVersion);
public sealed record AgentRegistrationResponse(Guid ComputerId, string AgentToken, int HeartbeatIntervalSeconds);
public sealed record AgentHeartbeatRequest(string AgentVersion);
public sealed record AgentHeartbeatResponse(DateTimeOffset ServerTime, AgentCommandResponse? Command);
public sealed record AgentCommandResponse(Guid Id, string Type, string? Payload);
public sealed record AgentCommandResultRequest(string Outcome, string? Message);
