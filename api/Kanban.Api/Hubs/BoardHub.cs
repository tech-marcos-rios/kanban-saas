using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Kanban.Application.Interfaces;

namespace Kanban.Api.Hubs;

/// <summary>
/// Un grupo de SignalR por tablero ("board-{id}"). El cliente se une explícitamente
/// con JoinBoard después de conectar — así solo recibe eventos del tablero que tiene abierto.
/// </summary>
[Authorize]
public class BoardHub : Hub
{
    private readonly IBoardRepository _boards;
    private readonly IBoardConnectionTracker _tracker;

    public BoardHub(IBoardRepository boards, IBoardConnectionTracker tracker)
    {
        _boards = boards;
        _tracker = tracker;
    }

    public async Task JoinBoard(Guid boardId)
    {
        var userId = CurrentUserId();
        var membership = await _boards.GetMembershipAsync(boardId, userId);
        if (membership is null)
            throw new HubException("No tenés acceso a este tablero.");

        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(boardId));
        _tracker.Track(boardId, userId, Context.ConnectionId);
    }

    public async Task LeaveBoard(Guid boardId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(boardId));
        _tracker.Untrack(boardId, CurrentUserId(), Context.ConnectionId);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _tracker.UntrackConnection(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    public static string GroupName(Guid boardId) => $"board-{boardId}";

    private Guid CurrentUserId()
    {
        var id = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? Context.User?.FindFirstValue("sub");
        return Guid.Parse(id ?? throw new HubException("Usuario no autenticado."));
    }
}
