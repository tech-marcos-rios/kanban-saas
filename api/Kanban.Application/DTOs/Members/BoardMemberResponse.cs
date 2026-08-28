namespace Kanban.Application.DTOs.Members;

public record BoardMemberResponse(Guid UserId, string Name, string Email, string Role, DateTime JoinedAt);
