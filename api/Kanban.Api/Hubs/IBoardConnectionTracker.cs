namespace Kanban.Api.Hubs;

/// <summary>
/// Mapea qué connectionId de SignalR corresponde a qué usuario en qué tablero, para poder
/// sacar por la fuerza a alguien del grupo cuando se lo elimina como miembro (si no,
/// seguiría recibiendo eventos en vivo de un tablero al que ya no tiene acceso hasta que
/// se desconecte y reconecte por su cuenta).
/// </summary>
public interface IBoardConnectionTracker
{
    void Track(Guid boardId, Guid userId, string connectionId);
    void Untrack(Guid boardId, Guid userId, string connectionId);
    void UntrackConnection(string connectionId);
    IReadOnlyCollection<string> GetConnections(Guid boardId, Guid userId);
}
