using Kanban.Domain.Entities;

namespace Kanban.Application.Interfaces;

public interface IBoardRepository
{
    Task<Board?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<BoardMember>> GetMembershipsForUserAsync(Guid userId, CancellationToken ct = default);
    Task<BoardMember?> GetMembershipAsync(Guid boardId, Guid userId, CancellationToken ct = default);
    Task<List<BoardMember>> GetMembersAsync(Guid boardId, CancellationToken ct = default);
    Task AddAsync(Board board, CancellationToken ct = default);
    Task AddMemberAsync(BoardMember member, CancellationToken ct = default);
    void Update(Board board);
    void Remove(Board board);
    void RemoveMember(BoardMember member);
}
