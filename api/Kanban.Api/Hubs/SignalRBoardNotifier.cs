using Microsoft.AspNetCore.SignalR;
using Kanban.Application.DTOs.Cards;
using Kanban.Application.DTOs.Lists;
using Kanban.Application.Interfaces;

namespace Kanban.Api.Hubs;

public class SignalRBoardNotifier : IBoardNotifier
{
    private readonly IHubContext<BoardHub> _hub;
    private readonly IBoardConnectionTracker _tracker;

    public SignalRBoardNotifier(IHubContext<BoardHub> hub, IBoardConnectionTracker tracker)
    {
        _hub = hub;
        _tracker = tracker;
    }

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

    public async Task MemberRemovedAsync(Guid boardId, Guid userId, CancellationToken ct = default)
    {
        foreach (var connectionId in _tracker.GetConnections(boardId, userId))
        {
            await _hub.Clients.Client(connectionId).SendAsync("RemovedFromBoard", boardId, ct);
            await _hub.Groups.RemoveFromGroupAsync(connectionId, BoardHub.GroupName(boardId), ct);
        }
    }

    private IClientProxy Group(Guid boardId) => _hub.Clients.Group(BoardHub.GroupName(boardId));
}
