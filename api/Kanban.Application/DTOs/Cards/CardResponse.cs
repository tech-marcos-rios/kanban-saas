namespace Kanban.Application.DTOs.Cards;

public record CardResponse(
    Guid Id,
    Guid ListId,
    string Title,
    string? Description,
    int Position,
    Guid? AssignedUserId,
    string? AssignedUserName,
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
