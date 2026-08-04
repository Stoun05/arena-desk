using ArenaDesk.Api.Domain;
using ArenaDesk.Contracts;

namespace ArenaDesk.Api.Realtime;

internal static class AgentCommandMapper
{
    public static AgentCommand ToContract(this AgentCommandRecord command) => new(
        command.Id,
        command.ComputerId,
        command.SessionId,
        command.Type,
        command.CreatedAtUtc,
        command.EndsAtUtc,
        command.EndAction);
}
