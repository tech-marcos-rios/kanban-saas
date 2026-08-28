using Microsoft.EntityFrameworkCore;
using Kanban.Domain.Entities;

namespace Kanban.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    private static readonly Guid AdminRoleId = new("00000000-0000-0000-0000-000000000001");

    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Users.AnyAsync(u => u.Email == "admin@kanban.com")) return;

        var hash = BCrypt.Net.BCrypt.HashPassword("Admin123!");
        var admin = User.Create("Admin", "admin@kanban.com", hash, AdminRoleId);
        db.Users.Add(admin);
        await db.SaveChangesAsync();
    }
}
