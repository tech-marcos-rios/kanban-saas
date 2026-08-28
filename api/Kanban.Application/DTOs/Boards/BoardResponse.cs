namespace Kanban.Application.DTOs.Boards;

public record BoardResponse(
    Guid Id,
    string Name,
    Guid OwnerId,
    string OwnerName,
    string Role,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
