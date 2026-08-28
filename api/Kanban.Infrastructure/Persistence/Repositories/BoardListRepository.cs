using Microsoft.EntityFrameworkCore;
using Kanban.Application.Interfaces;
using Kanban.Domain.Entities;

namespace Kanban.Infrastructure.Persistence.Repositories;

public class BoardListRepository : IBoardListRepository
{
    private readonly AppDbContext _context;

    public BoardListRepository(AppDbContext context) => _context = context;

    public Task<List<BoardList>> GetByBoardIdAsync(Guid boardId, CancellationToken ct = default) =>
        _context.BoardLists
            .Where(l => l.BoardId == boardId)
            .OrderBy(l => l.Position)
            .ToListAsync(ct);

    public Task<BoardList?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _context.BoardLists.FirstOrDefaultAsync(l => l.Id == id, ct);

    public async Task AddAsync(BoardList list, CancellationToken ct = default) =>
        await _context.BoardLists.AddAsync(list, ct);

    public void Update(BoardList list) => _context.BoardLists.Update(list);

    public void Remove(BoardList list) => _context.BoardLists.Remove(list);
}
