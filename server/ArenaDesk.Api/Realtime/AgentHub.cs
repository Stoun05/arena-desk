using ArenaDesk.Api.Contracts;
using ArenaDesk.Api.Domain;
using ArenaDesk.Api.Persistence;
using ArenaDesk.Contracts;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ArenaDesk.Api.Realtime;

public sealed class AgentHub(
    ArenaDeskDbContext database,
    AgentConnectionRegistry connections,
    IHubContext<OperationsHub> operationsHub,
    ILogger<AgentHub> logger) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var agentId = Context.GetHttpContext()!.Request.Headers[AgentProtocol.AgentIdHeader].ToString();
        var computerCode = Context.GetHttpContext()!.Request.Headers[AgentProtocol.ComputerCodeHeader].ToString();
        var computer = await database.Computers.SingleOrDefaultAsync(item => item.Code == computerCode);
        if (computer is null || (computer.AgentId is not null && !string.Equals(computer.AgentId, agentId, StringComparison.Ordinal)))
        {
            Context.Abort();
            return;
        }

        var hasActiveSession = await database.Sessions.AnyAsync(session =>
            session.ComputerId == computer.Id &&
            (session.Status == SessionStatus.Active || session.Status == SessionStatus.Ending));
        var now = DateTimeOffset.UtcNow;
        computer.AgentId = agentId;
        computer.LastSeenAtUtc = now;
        computer.Status = hasActiveSession ? ComputerStatus.Occupied : ComputerStatus.Available;
        computer.UpdatedAtUtc = now;
        await database.SaveChangesAsync();

        connections.Register(computer.Id, Context.ConnectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, AgentProtocol.ComputerGroup(computer.Id));
        await Clients.Caller.SendAsync(AgentProtocol.RegisteredEvent, new AgentRegistration(
            computer.Id,
            computer.Code,
            computer.DisplayName,
            computer.EndAction.ToString().ToLowerInvariant(),
            now));
        await NotifyDashboardAsync(computer.Id, "agent-connected", now);
        logger.LogInformation("Agent {AgentId} connected to {ComputerCode}.", agentId, computer.Code);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (connections.TryUnregisterCurrent(Context.ConnectionId, out var computerId))
        {
            var computer = await database.Computers.SingleOrDefaultAsync(item => item.Id == computerId);
            if (computer is not null)
            {
                var now = DateTimeOffset.UtcNow;
                computer.Status = ComputerStatus.Offline;
                computer.UpdatedAtUtc = now;
                await database.SaveChangesAsync();
                await NotifyDashboardAsync(computer.Id, "agent-disconnected", now);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task<AgentHeartbeatResponse> Heartbeat(AgentHeartbeat heartbeat)
    {
        var computerId = GetComputerId();
        var computer = await database.Computers.SingleAsync(item => item.Id == computerId);
        computer.LastSeenAtUtc = DateTimeOffset.UtcNow;
        computer.UpdatedAtUtc = computer.LastSeenAtUtc.Value;
        await database.SaveChangesAsync();
        return new AgentHeartbeatResponse(computer.LastSeenAtUtc.Value);
    }

    public Task AcknowledgeCommand(AgentCommandAcknowledgement acknowledgement)
    {
        var computerId = GetComputerId();
        logger.LogInformation(
            "Agent for computer {ComputerId} acknowledged command {CommandId} with {Status}.",
            computerId,
            acknowledgement.CommandId,
            acknowledgement.Status);
        return NotifyDashboardAsync(computerId, $"agent-command-{acknowledgement.Status}", acknowledgement.AcknowledgedAtUtc);
    }

    private Guid GetComputerId()
    {
        if (!connections.TryGetComputer(Context.ConnectionId, out var computerId))
        {
            throw new HubException("Agent connection is not registered.");
        }

        return computerId;
    }

    private Task NotifyDashboardAsync(Guid computerId, string action, DateTimeOffset occurredAtUtc) =>
        operationsHub.Clients.All.SendAsync(
            OperationsHub.ComputerStateChangedEvent,
            new ComputerStateChangedResponse(computerId, action, occurredAtUtc));
}
