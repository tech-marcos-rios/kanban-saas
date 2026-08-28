using Kanban.Application.Common;
using Kanban.Application.DTOs.Lists;
using Kanban.Application.Interfaces;
using Kanban.Domain.Entities;

namespace Kanban.Application.Services;

public class BoardListService
{
    private const int MaxTitleLength = 100;

    private readonly IBoardListRepository _lists;
    private readonly IBoardRepository _boards;
    private readonly IUnitOfWork _uow;

    public BoardListService(IBoardListRepository lists, IBoardRepository boards, IUnitOfWork uow)
    {
        _lists = lists;
        _boards = boards;
        _uow = uow;
    }

    public async Task<Result<List<BoardListResponse>>> GetForBoardAsync(Guid boardId, Guid userId, CancellationToken ct = default)
    {
        var access = await CheckAccessAsync(boardId, userId, requireEditor: false, ct);
        if (access.IsFailure)
            return Result.Failure<List<BoardListResponse>>(access.Error!, access.IsNotFound, access.IsForbidden);

        var lists = await _lists.GetByBoardIdAsync(boardId, ct);
        return Result.Success(lists.Select(ToResponse).ToList());
    }

    public async Task<Result<BoardListResponse>> CreateAsync(Guid boardId, Guid userId, CreateBoardListRequest request, CancellationToken ct = default)
    {
        var access = await CheckAccessAsync(boardId, userId, requireEditor: true, ct);
        if (access.IsFailure)
            return Result.Failure<BoardListResponse>(access.Error!, access.IsNotFound, access.IsForbidden);

        var titleResult = ValidateTitle(request.Title);
        if (titleResult.IsFailure)
            return Result.Failure<BoardListResponse>(titleResult.Error!);

        var existing = await _lists.GetByBoardIdAsync(boardId, ct);
        var nextPosition = existing.Count == 0 ? 0 : existing.Max(l => l.Position) + 1;

        var list = BoardList.Create(boardId, titleResult.Value!, nextPosition);
        await _lists.AddAsync(list, ct);
        await _uow.SaveChangesAsync(ct);

        return Result.Success(ToResponse(list));
    }

    public async Task<Result<BoardListResponse>> RenameAsync(Guid boardId, Guid listId, Guid userId, UpdateBoardListRequest request, CancellationToken ct = default)
    {
        var access = await CheckAccessAsync(boardId, userId, requireEditor: true, ct);
        if (access.IsFailure)
            return Result.Failure<BoardListResponse>(access.Error!, access.IsNotFound, access.IsForbidden);

        var titleResult = ValidateTitle(request.Title);
        if (titleResult.IsFailure)
            return Result.Failure<BoardListResponse>(titleResult.Error!);

        var list = await _lists.GetByIdAsync(listId, ct);
        if (list is null || list.BoardId != boardId)
            return Result.Failure<BoardListResponse>("Lista no encontrada.", notFound: true);

        list.Rename(titleResult.Value!);
        _lists.Update(list);
        await _uow.SaveChangesAsync(ct);

        return Result.Success(ToResponse(list));
    }

    /// <summary>Reordena renumerando todas las listas del tablero (0..n-1) — simple porque un tablero tiene pocas listas.</summary>
    public async Task<Result<List<BoardListResponse>>> MoveAsync(Guid boardId, Guid listId, Guid userId, int newPosition, CancellationToken ct = default)
    {
        var access = await CheckAccessAsync(boardId, userId, requireEditor: true, ct);
        if (access.IsFailure)
            return Result.Failure<List<BoardListResponse>>(access.Error!, access.IsNotFound, access.IsForbidden);

        var lists = await _lists.GetByBoardIdAsync(boardId, ct);
        var moved = lists.FirstOrDefault(l => l.Id == listId);
        if (moved is null)
            return Result.Failure<List<BoardListResponse>>("Lista no encontrada.", notFound: true);

        lists.Remove(moved);
        lists.Insert(Math.Clamp(newPosition, 0, lists.Count), moved);

        for (var i = 0; i < lists.Count; i++)
        {
            if (lists[i].Position == i) continue;
            lists[i].MoveTo(i);
            _lists.Update(lists[i]);
        }

        await _uow.SaveChangesAsync(ct);
        return Result.Success(lists.Select(ToResponse).ToList());
    }

    public async Task<Result> DeleteAsync(Guid boardId, Guid listId, Guid userId, CancellationToken ct = default)
    {
        var access = await CheckAccessAsync(boardId, userId, requireEditor: true, ct);
        if (access.IsFailure)
            return access;

        var list = await _lists.GetByIdAsync(listId, ct);
        if (list is null || list.BoardId != boardId)
            return Result.Failure("Lista no encontrada.", notFound: true);

        _lists.Remove(list);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }

    private async Task<Result> CheckAccessAsync(Guid boardId, Guid userId, bool requireEditor, CancellationToken ct)
    {
        var membership = await _boards.GetMembershipAsync(boardId, userId, ct);
        if (membership is null)
            return Result.Failure("Tablero no encontrado.", notFound: true);
        if (requireEditor && membership.Role == BoardRole.Viewer)
            return Result.Failure("No tenés permisos para editar este tablero.", forbidden: true);

        return Result.Success();
    }

    private static Result<string> ValidateTitle(string title)
    {
        var trimmed = title?.Trim() ?? string.Empty;
        if (trimmed.Length == 0)
            return Result.Failure<string>("El título de la lista es obligatorio.");
        if (trimmed.Length > MaxTitleLength)
            return Result.Failure<string>($"El título no puede superar los {MaxTitleLength} caracteres.");
        return Result.Success(trimmed);
    }

    private static BoardListResponse ToResponse(BoardList list) =>
        new(list.Id, list.BoardId, list.Title, list.Position, list.CreatedAt, list.UpdatedAt);
}
