namespace Kanban.Application.DTOs.Lists;

public record BoardListResponse(
    Guid Id,
    Guid BoardId,
    string Title,
    int Position,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
