namespace Kanban.Application.DTOs.Cards;

public record CreateCardRequest(string Title, string? Description, DateTime? DueDate);
