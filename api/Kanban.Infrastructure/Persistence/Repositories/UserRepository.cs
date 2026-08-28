using Microsoft.EntityFrameworkCore;
using Kanban.Application.Interfaces;
using Kanban.Domain.Entities;

namespace Kanban.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) => _context = context;

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default) =>
        _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, ct);

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default) =>
        _context.Users.AnyAsync(u => u.Email == email, ct);

    public async Task AddAsync(User user, CancellationToken ct = default) =>
        await _context.Users.AddAsync(user, ct);

    public void Update(User user) =>
        _context.Users.Update(user);
}
