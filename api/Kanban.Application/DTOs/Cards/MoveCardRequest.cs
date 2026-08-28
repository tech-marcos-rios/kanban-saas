namespace Kanban.Application.DTOs.Cards;

public record MoveCardRequest(Guid ListId, int Position);
