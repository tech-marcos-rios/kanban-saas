using Microsoft.EntityFrameworkCore;
using Kanban.Application.Interfaces;
using Kanban.Domain.Entities;

namespace Kanban.Infrastructure.Persistence;

/// <summary>
/// Implementa <see cref="IUnitOfWork"/> directamente: el DbContext ya ES una unidad de
/// trabajo (change tracker acumula cambios, SaveChangesAsync los persiste juntos).
/// </summary>
public class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Board> Boards => Set<Board>();
    public DbSet<BoardMember> BoardMembers => Set<BoardMember>();
    public DbSet<BoardList> BoardLists => Set<BoardList>();
    public DbSet<Card> Cards => Set<Card>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Label> Labels => Set<Label>();
    public DbSet<CardLabel> CardLabels => Set<CardLabel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
