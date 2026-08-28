using Microsoft.EntityFrameworkCore;
using Kanban.Application.Interfaces;
using Kanban.Domain.Entities;

namespace Kanban.Infrastructure.Persistence.Repositories;

public class CardRepository : ICardRepository
{
    private readonly AppDbContext _context;

    public CardRepository(AppDbContext context) => _context = context;

    public Task<List<Card>> GetByListIdAsync(Guid listId, CancellationToken ct = default) =>
        _context.Cards
            .Where(c => c.ListId == listId)
            .Include(c => c.AssignedUser)
            .OrderBy(c => c.Position)
            .ToListAsync(ct);

    public Task<Card?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _context.Cards
            .Include(c => c.AssignedUser)
            .Include(c => c.List)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task AddAsync(Card card, CancellationToken ct = default) =>
        await _context.Cards.AddAsync(card, ct);

    public void Update(Card card) => _context.Cards.Update(card);

    public void Remove(Card card) => _context.Cards.Remove(card);
}
