using Microsoft.EntityFrameworkCore;
using Kanban.Application.Interfaces;
using Kanban.Domain.Entities;

namespace Kanban.Infrastructure.Persistence.Repositories;

public class BoardRepository : IBoardRepository
{
    private readonly AppDbContext _context;

    public BoardRepository(AppDbContext context) => _context = context;

    public Task<Board?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _context.Boards.Include(b => b.Owner).FirstOrDefaultAsync(b => b.Id == id, ct);

    public Task<List<BoardMember>> GetMembershipsForUserAsync(Guid userId, CancellationToken ct = default) =>
        _context.BoardMembers
            .Where(m => m.UserId == userId)
            .Include(m => m.Board).ThenInclude(b => b.Owner)
            .ToListAsync(ct);

    public Task<BoardMember?> GetMembershipAsync(Guid boardId, Guid userId, CancellationToken ct = default) =>
        _context.BoardMembers.FirstOrDefaultAsync(m => m.BoardId == boardId && m.UserId == userId, ct);

    public async Task AddAsync(Board board, CancellationToken ct = default) =>
        await _context.Boards.AddAsync(board, ct);

    public async Task AddMemberAsync(BoardMember member, CancellationToken ct = default) =>
        await _context.BoardMembers.AddAsync(member, ct);

    public void Update(Board board) => _context.Boards.Update(board);

    public void Remove(Board board) => _context.Boards.Remove(board);
}
