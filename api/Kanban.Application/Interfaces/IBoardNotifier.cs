using Kanban.Application.DTOs.Cards;
using Kanban.Application.DTOs.Lists;

namespace Kanban.Application.Interfaces;

/// <summary>
/// Puerto hacia el mundo real-time. La implementación (SignalR) vive en Kanban.Api,
/// que es la única capa con dependencia a ASP.NET Core Web — Application e
/// Infrastructure no necesitan saber que existe un Hub.
/// </summary>
public interface IBoardNotifier
{
    Task ListCreatedAsync(Guid boardId, BoardListResponse list, CancellationToken ct = default);
    Task ListUpdatedAsync(Guid boardId, BoardListResponse list, CancellationToken ct = default);
    Task ListsReorderedAsync(Guid boardId, List<BoardListResponse> lists, CancellationToken ct = default);
    Task ListDeletedAsync(Guid boardId, Guid listId, CancellationToken ct = default);

    Task CardCreatedAsync(Guid boardId, CardResponse card, CancellationToken ct = default);
    Task CardUpdatedAsync(Guid boardId, CardResponse card, CancellationToken ct = default);
    Task CardMovedAsync(Guid boardId, CardResponse card, CancellationToken ct = default);
    Task CardDeletedAsync(Guid boardId, Guid listId, Guid cardId, CancellationToken ct = default);

    /// <summary>Saca a un miembro eliminado de las conexiones en vivo que tenga abiertas del tablero.</summary>
    Task MemberRemovedAsync(Guid boardId, Guid userId, CancellationToken ct = default);

    /// <summary>Avisa a todos los miembros conectados que la lista de miembros cambió (invitación o remoción).</summary>
    Task MembersChangedAsync(Guid boardId, CancellationToken ct = default);
}
