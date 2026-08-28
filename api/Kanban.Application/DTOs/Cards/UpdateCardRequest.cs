namespace Kanban.Application.DTOs.Cards;

public record UpdateCardRequest(string Title, string? Description, DateTime? DueDate);
