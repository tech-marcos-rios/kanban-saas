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

    public BoardHub(IBoardRepository boards) => _boards = boards;

    public async Task JoinBoard(Guid boardId)
    {
        var membership = await _boards.GetMembershipAsync(boardId, CurrentUserId());
        if (membership is null)
            throw new HubException("No tenés acceso a este tablero.");

        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(boardId));
    }

    public Task LeaveBoard(Guid boardId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(boardId));

    public static string GroupName(Guid boardId) => $"board-{boardId}";

    private Guid CurrentUserId()
    {
        var id = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? Context.User?.FindFirstValue("sub");
        return Guid.Parse(id ?? throw new HubException("Usuario no autenticado."));
    }
}
