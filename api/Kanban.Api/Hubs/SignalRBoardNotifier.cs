using Microsoft.AspNetCore.SignalR;
using Kanban.Application.DTOs.Cards;
using Kanban.Application.DTOs.Lists;
using Kanban.Application.Interfaces;

namespace Kanban.Api.Hubs;

public class SignalRBoardNotifier : IBoardNotifier
{
    private readonly IHubContext<BoardHub> _hub;

    public SignalRBoardNotifier(IHubContext<BoardHub> hub) => _hub = hub;

    public Task ListCreatedAsync(Guid boardId, BoardListResponse list, CancellationToken ct = default) =>
        Group(boardId).SendAsync("ListCreated", list, ct);

    public Task ListUpdatedAsync(Guid boardId, BoardListResponse list, CancellationToken ct = default) =>
        Group(boardId).SendAsync("ListUpdated", list, ct);

    public Task ListsReorderedAsync(Guid boardId, List<BoardListResponse> lists, CancellationToken ct = default) =>
        Group(boardId).SendAsync("ListsReordered", lists, ct);

    public Task ListDeletedAsync(Guid boardId, Guid listId, CancellationToken ct = default) =>
        Group(boardId).SendAsync("ListDeleted", listId, ct);

    public Task CardCreatedAsync(Guid boardId, CardResponse card, CancellationToken ct = default) =>
        Group(boardId).SendAsync("CardCreated", card, ct);

    public Task CardUpdatedAsync(Guid boardId, CardResponse card, CancellationToken ct = default) =>
        Group(boardId).SendAsync("CardUpdated", card, ct);

    public Task CardMovedAsync(Guid boardId, CardResponse card, CancellationToken ct = default) =>
        Group(boardId).SendAsync("CardMoved", card, ct);

    public Task CardDeletedAsync(Guid boardId, Guid listId, Guid cardId, CancellationToken ct = default) =>
        Group(boardId).SendAsync("CardDeleted", new { listId, cardId }, ct);

    private IClientProxy Group(Guid boardId) => _hub.Clients.Group(BoardHub.GroupName(boardId));
}
