using Kanban.Application.Common;
using Kanban.Application.DTOs.Boards;
using Kanban.Application.Interfaces;
using Kanban.Domain.Entities;

namespace Kanban.Application.Services;

public class BoardService
{
    private const int MaxNameLength = 150;

    private readonly IBoardRepository _boards;
    private readonly IUnitOfWork _uow;

    public BoardService(IBoardRepository boards, IUnitOfWork uow)
    {
        _boards = boards;
        _uow = uow;
    }

    public async Task<Result<BoardResponse>> CreateAsync(Guid ownerId, CreateBoardRequest request, CancellationToken ct = default)
    {
        var nameResult = ValidateName(request.Name);
        if (nameResult.IsFailure)
            return Result.Failure<BoardResponse>(nameResult.Error!);

        var board = Board.Create(nameResult.Value!, ownerId);
        await _boards.AddAsync(board, ct);
        await _boards.AddMemberAsync(BoardMember.Create(board.Id, ownerId, BoardRole.Owner), ct);
        await _uow.SaveChangesAsync(ct);

        var saved = await _boards.GetByIdAsync(board.Id, ct);
        return Result.Success(ToResponse(saved!, BoardRole.Owner));
    }

    public async Task<Result<List<BoardResponse>>> GetMyBoardsAsync(Guid userId, CancellationToken ct = default)
    {
        var memberships = await _boards.GetMembershipsForUserAsync(userId, ct);
        var boards = memberships
            .OrderByDescending(m => m.Board.CreatedAt)
            .Select(m => ToResponse(m.Board, m.Role))
            .ToList();

        return Result.Success(boards);
    }

    public async Task<Result<BoardResponse>> GetByIdAsync(Guid boardId, Guid userId, CancellationToken ct = default)
    {
        var membership = await _boards.GetMembershipAsync(boardId, userId, ct);
        if (membership is null)
            return Result.Failure<BoardResponse>("Tablero no encontrado.", notFound: true);

        var board = await _boards.GetByIdAsync(boardId, ct);
        return Result.Success(ToResponse(board!, membership.Role));
    }

    public async Task<Result<BoardResponse>> RenameAsync(Guid boardId, Guid userId, UpdateBoardRequest request, CancellationToken ct = default)
    {
        var membership = await _boards.GetMembershipAsync(boardId, userId, ct);
        if (membership is null)
            return Result.Failure<BoardResponse>("Tablero no encontrado.", notFound: true);
        if (membership.Role == BoardRole.Viewer)
            return Result.Failure<BoardResponse>("No tenés permisos para editar este tablero.", forbidden: true);

        var nameResult = ValidateName(request.Name);
        if (nameResult.IsFailure)
            return Result.Failure<BoardResponse>(nameResult.Error!);

        var board = await _boards.GetByIdAsync(boardId, ct);
        board!.Rename(nameResult.Value!);
        _boards.Update(board);
        await _uow.SaveChangesAsync(ct);

        return Result.Success(ToResponse(board, membership.Role));
    }

    public async Task<Result> DeleteAsync(Guid boardId, Guid userId, CancellationToken ct = default)
    {
        var membership = await _boards.GetMembershipAsync(boardId, userId, ct);
        if (membership is null)
            return Result.Failure("Tablero no encontrado.", notFound: true);
        if (membership.Role != BoardRole.Owner)
            return Result.Failure("Solo el dueño puede eliminar el tablero.", forbidden: true);

        var board = await _boards.GetByIdAsync(boardId, ct);
        _boards.Remove(board!);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }

    private static Result<string> ValidateName(string name)
    {
        var trimmed = name?.Trim() ?? string.Empty;
        if (trimmed.Length == 0)
            return Result.Failure<string>("El nombre del tablero es obligatorio.");
        if (trimmed.Length > MaxNameLength)
            return Result.Failure<string>($"El nombre no puede superar los {MaxNameLength} caracteres.");
        return Result.Success(trimmed);
    }

    private static BoardResponse ToResponse(Board board, BoardRole role) =>
        new(board.Id, board.Name, board.OwnerId, board.Owner.Name, role.ToString(), board.CreatedAt, board.UpdatedAt);
}
