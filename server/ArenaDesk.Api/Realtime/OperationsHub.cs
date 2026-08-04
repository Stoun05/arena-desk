using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ArenaDesk.Api.Realtime;

[Authorize]
public sealed class OperationsHub : Hub
{
    public const string ComputerStateChangedEvent = "ComputerStateChanged";
}
