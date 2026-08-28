using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Kanban.Domain.Entities;

namespace Kanban.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    private static readonly Guid AdminRoleId = new("00000000-0000-0000-0000-000000000001");

    /// <summary>
    /// Crea un admin solo si Seed:AdminEmail/Seed:AdminPassword están configurados
    /// (ver appsettings.Development.json, gitignoreado). Sin esos valores no hace nada —
    /// nunca hay una cuenta con credenciales conocidas de fábrica en el código.
    /// </summary>
    public static async Task SeedAsync(AppDbContext db, IConfiguration config)
    {
        var email = config["Seed:AdminEmail"];
        var password = config["Seed:AdminPassword"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return;

        if (await db.Users.AnyAsync(u => u.Email == email)) return;

        var hash = BCrypt.Net.BCrypt.HashPassword(password);
        var admin = User.Create("Admin", email, hash, AdminRoleId);
        db.Users.Add(admin);
        await db.SaveChangesAsync();
    }
}
