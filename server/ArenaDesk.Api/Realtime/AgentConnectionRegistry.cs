using System.Collections.Concurrent;

namespace ArenaDesk.Api.Realtime;

public sealed class AgentConnectionRegistry
{
    private readonly ConcurrentDictionary<Guid, string> _connectionsByComputer = new();
    private readonly ConcurrentDictionary<string, Guid> _computersByConnection = new();

    public void Register(Guid computerId, string connectionId)
    {
        if (_connectionsByComputer.TryGetValue(computerId, out var previousConnection))
        {
            _computersByConnection.TryRemove(previousConnection, out _);
        }

        _connectionsByComputer[computerId] = connectionId;
        _computersByConnection[connectionId] = computerId;
    }

    public bool TryGetComputer(string connectionId, out Guid computerId) =>
        _computersByConnection.TryGetValue(connectionId, out computerId);

    public bool TryUnregisterCurrent(string connectionId, out Guid computerId)
    {
        if (!_computersByConnection.TryRemove(connectionId, out computerId))
        {
            return false;
        }

        return _connectionsByComputer.TryGetValue(computerId, out var currentConnection) &&
            currentConnection == connectionId &&
            _connectionsByComputer.TryRemove(computerId, out _);
    }
}
