using Kanban.Domain.Entities;

namespace Kanban.Application.Interfaces;

public interface IBoardListRepository
{
    Task<List<BoardList>> GetByBoardIdAsync(Guid boardId, CancellationToken ct = default);
    Task<BoardList?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(BoardList list, CancellationToken ct = default);
    void Update(BoardList list);
    void Remove(BoardList list);
}
