using Kanban.Application.Common;
using Kanban.Application.DTOs.Cards;
using Kanban.Application.Interfaces;
using Kanban.Domain.Entities;

namespace Kanban.Application.Services;

public class CardService
{
    private const int MaxTitleLength = 200;

    private readonly ICardRepository _cards;
    private readonly IBoardListRepository _lists;
    private readonly IBoardRepository _boards;
    private readonly IUnitOfWork _uow;
    private readonly IBoardNotifier _notifier;

    public CardService(ICardRepository cards, IBoardListRepository lists, IBoardRepository boards, IUnitOfWork uow, IBoardNotifier notifier)
    {
        _cards = cards;
        _lists = lists;
        _boards = boards;
        _uow = uow;
        _notifier = notifier;
    }

    public async Task<Result<List<CardResponse>>> GetByListAsync(Guid boardId, Guid listId, Guid userId, CancellationToken ct = default)
    {
        var access = await CheckBoardAccessAsync(boardId, userId, requireEditor: false, ct);
        if (access.IsFailure)
            return Result.Failure<List<CardResponse>>(access.Error!, access.IsNotFound, access.IsForbidden);

        var list = await _lists.GetByIdAsync(listId, ct);
        if (list is null || list.BoardId != boardId)
            return Result.Failure<List<CardResponse>>("Lista no encontrada.", notFound: true);

        var cards = await _cards.GetByListIdAsync(listId, ct);
        return Result.Success(cards.Select(ToResponse).ToList());
    }

    public async Task<Result<CardResponse>> GetByIdAsync(Guid boardId, Guid cardId, Guid userId, CancellationToken ct = default)
    {
        var access = await CheckBoardAccessAsync(boardId, userId, requireEditor: false, ct);
        if (access.IsFailure)
            return Result.Failure<CardResponse>(access.Error!, access.IsNotFound, access.IsForbidden);

        var card = await _cards.GetByIdAsync(cardId, ct);
        if (card is null || card.List.BoardId != boardId)
            return Result.Failure<CardResponse>("Tarjeta no encontrada.", notFound: true);

        return Result.Success(ToResponse(card));
    }

    public async Task<Result<CardResponse>> CreateAsync(Guid boardId, Guid listId, Guid userId, CreateCardRequest request, CancellationToken ct = default)
    {
        var access = await CheckBoardAccessAsync(boardId, userId, requireEditor: true, ct);
        if (access.IsFailure)
            return Result.Failure<CardResponse>(access.Error!, access.IsNotFound, access.IsForbidden);

        var list = await _lists.GetByIdAsync(listId, ct);
        if (list is null || list.BoardId != boardId)
            return Result.Failure<CardResponse>("Lista no encontrada.", notFound: true);

        var titleResult = ValidateTitle(request.Title);
        if (titleResult.IsFailure)
            return Result.Failure<CardResponse>(titleResult.Error!);

        var existing = await _cards.GetByListIdAsync(listId, ct);
        var nextPosition = existing.Count == 0 ? 0 : existing.Max(c => c.Position) + 1;

        var card = Card.Create(listId, titleResult.Value!, nextPosition);
        card.UpdateDetails(titleResult.Value!, request.Description?.Trim(), request.DueDate);

        await _cards.AddAsync(card, ct);
        await _uow.SaveChangesAsync(ct);

        var saved = await _cards.GetByIdAsync(card.Id, ct);
        var response = ToResponse(saved!);
        await _notifier.CardCreatedAsync(boardId, response, ct);
        return Result.Success(response);
    }

    public async Task<Result<CardResponse>> UpdateAsync(Guid boardId, Guid cardId, Guid userId, UpdateCardRequest request, CancellationToken ct = default)
    {
        var access = await CheckBoardAccessAsync(boardId, userId, requireEditor: true, ct);
        if (access.IsFailure)
            return Result.Failure<CardResponse>(access.Error!, access.IsNotFound, access.IsForbidden);

        var titleResult = ValidateTitle(request.Title);
        if (titleResult.IsFailure)
            return Result.Failure<CardResponse>(titleResult.Error!);

        var card = await _cards.GetByIdAsync(cardId, ct);
        if (card is null || card.List.BoardId != boardId)
            return Result.Failure<CardResponse>("Tarjeta no encontrada.", notFound: true);

        card.UpdateDetails(titleResult.Value!, request.Description?.Trim(), request.DueDate);
        _cards.Update(card);
        await _uow.SaveChangesAsync(ct);

        var response = ToResponse(card);
        await _notifier.CardUpdatedAsync(boardId, response, ct);
        return Result.Success(response);
    }

    public async Task<Result<CardResponse>> AssignAsync(Guid boardId, Guid cardId, Guid userId, AssignCardRequest request, CancellationToken ct = default)
    {
        var access = await CheckBoardAccessAsync(boardId, userId, requireEditor: true, ct);
        if (access.IsFailure)
            return Result.Failure<CardResponse>(access.Error!, access.IsNotFound, access.IsForbidden);

        var card = await _cards.GetByIdAsync(cardId, ct);
        if (card is null || card.List.BoardId != boardId)
            return Result.Failure<CardResponse>("Tarjeta no encontrada.", notFound: true);

        if (request.UserId is Guid assigneeId)
        {
            var assigneeMembership = await _boards.GetMembershipAsync(boardId, assigneeId, ct);
            if (assigneeMembership is null)
                return Result.Failure<CardResponse>("El usuario no es miembro de este tablero.");
        }

        card.AssignTo(request.UserId);
        _cards.Update(card);
        await _uow.SaveChangesAsync(ct);

        var updated = await _cards.GetByIdAsync(cardId, ct);
        var response = ToResponse(updated!);
        await _notifier.CardUpdatedAsync(boardId, response, ct);
        return Result.Success(response);
    }

    /// <summary>Mueve la tarjeta (misma lista o entre listas) renumerando las columnas afectadas 0..n-1.</summary>
    public async Task<Result<CardResponse>> MoveAsync(Guid boardId, Guid cardId, Guid userId, MoveCardRequest request, CancellationToken ct = default)
    {
        var access = await CheckBoardAccessAsync(boardId, userId, requireEditor: true, ct);
        if (access.IsFailure)
            return Result.Failure<CardResponse>(access.Error!, access.IsNotFound, access.IsForbidden);

        var card = await _cards.GetByIdAsync(cardId, ct);
        if (card is null || card.List.BoardId != boardId)
            return Result.Failure<CardResponse>("Tarjeta no encontrada.", notFound: true);

        var targetList = await _lists.GetByIdAsync(request.ListId, ct);
        if (targetList is null || targetList.BoardId != boardId)
            return Result.Failure<CardResponse>("Lista no encontrada.", notFound: true);

        var sourceListId = card.ListId;
        List<Card> touched;

        if (sourceListId == request.ListId)
        {
            var cards = await _cards.GetByListIdAsync(sourceListId, ct);
            cards.RemoveAll(c => c.Id == cardId);
            cards.Insert(Math.Clamp(request.Position, 0, cards.Count), card);
            touched = Renumber(cards, sourceListId);
        }
        else
        {
            var sourceCards = await _cards.GetByListIdAsync(sourceListId, ct);
            sourceCards.RemoveAll(c => c.Id == cardId);
            var sourceTouched = Renumber(sourceCards, sourceListId);

            var targetCards = await _cards.GetByListIdAsync(request.ListId, ct);
            targetCards.Insert(Math.Clamp(request.Position, 0, targetCards.Count), card);
            var targetTouched = Renumber(targetCards, request.ListId);

            touched = sourceTouched.Concat(targetTouched).ToList();
        }

        await _uow.SaveChangesAsync(ct);

        // Todo lo que cambió de posición se notifica — no solo la tarjeta arrastrada —
        // para que los demás clientes conectados terminen con el mismo orden exacto.
        foreach (var c in touched)
            await _notifier.CardMovedAsync(boardId, ToResponse(c), ct);

        return Result.Success(ToResponse(card));
    }

    public async Task<Result> DeleteAsync(Guid boardId, Guid cardId, Guid userId, CancellationToken ct = default)
    {
        var access = await CheckBoardAccessAsync(boardId, userId, requireEditor: true, ct);
        if (access.IsFailure)
            return access;

        var card = await _cards.GetByIdAsync(cardId, ct);
        if (card is null || card.List.BoardId != boardId)
            return Result.Failure("Tarjeta no encontrada.", notFound: true);

        var listId = card.ListId;
        _cards.Remove(card);
        await _uow.SaveChangesAsync(ct);

        await _notifier.CardDeletedAsync(boardId, listId, cardId, ct);
        return Result.Success();
    }

    private List<Card> Renumber(List<Card> cards, Guid listId)
    {
        var touched = new List<Card>();
        for (var i = 0; i < cards.Count; i++)
        {
            if (cards[i].ListId == listId && cards[i].Position == i) continue;
            cards[i].MoveTo(listId, i);
            _cards.Update(cards[i]);
            touched.Add(cards[i]);
        }
        return touched;
    }

    private async Task<Result> CheckBoardAccessAsync(Guid boardId, Guid userId, bool requireEditor, CancellationToken ct)
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
            return Result.Failure<string>("El título de la tarjeta es obligatorio.");
        if (trimmed.Length > MaxTitleLength)
            return Result.Failure<string>($"El título no puede superar los {MaxTitleLength} caracteres.");
        return Result.Success(trimmed);
    }

    private static CardResponse ToResponse(Card card) => new(
        card.Id,
        card.ListId,
        card.Title,
        card.Description,
        card.Position,
        card.AssignedUserId,
        card.AssignedUser?.Name,
        card.DueDate,
        card.CreatedAt,
        card.UpdatedAt);
}
