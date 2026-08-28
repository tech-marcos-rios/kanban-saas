using Kanban.Domain.Entities;

namespace Kanban.Application.Interfaces;

public interface ICardRepository
{
    Task<List<Card>> GetByListIdAsync(Guid listId, CancellationToken ct = default);
    Task<Card?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Card card, CancellationToken ct = default);
    void Update(Card card);
    void Remove(Card card);
}
