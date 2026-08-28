using System.Collections.Concurrent;

namespace Kanban.Api.Hubs;

/// <summary>
/// Registrado como singleton: vive en memoria del proceso, no en la DB. Alcanza para un
/// único server (el deploy actual en Hetzner); con más de una instancia de la API haría
/// falta un backplane de SignalR para que esto se comparta entre procesos.
/// </summary>
public class BoardConnectionTracker : IBoardConnectionTracker
{
    private readonly ConcurrentDictionary<(Guid BoardId, Guid UserId), ConcurrentDictionary<string, byte>> _byMember = new();
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<(Guid BoardId, Guid UserId), byte>> _byConnection = new();

    public void Track(Guid boardId, Guid userId, string connectionId)
    {
        var key = (boardId, userId);
        _byMember.GetOrAdd(key, _ => new()).TryAdd(connectionId, 0);
        _byConnection.GetOrAdd(connectionId, _ => new()).TryAdd(key, 0);
    }

    public void Untrack(Guid boardId, Guid userId, string connectionId)
    {
        var key = (boardId, userId);
        if (_byMember.TryGetValue(key, out var connections))
            connections.TryRemove(connectionId, out _);
        if (_byConnection.TryGetValue(connectionId, out var keys))
            keys.TryRemove(key, out _);
    }

    public void UntrackConnection(string connectionId)
    {
        if (!_byConnection.TryRemove(connectionId, out var keys)) return;

        foreach (var key in keys.Keys)
            if (_byMember.TryGetValue(key, out var connections))
                connections.TryRemove(connectionId, out _);
    }

    public IReadOnlyCollection<string> GetConnections(Guid boardId, Guid userId) =>
        _byMember.TryGetValue((boardId, userId), out var connections) ? connections.Keys.ToList() : [];
}
