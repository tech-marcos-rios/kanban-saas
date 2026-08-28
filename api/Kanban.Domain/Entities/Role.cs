namespace Kanban.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; private set; } = default!;

    private Role() { }

    public static Role Create(string name) => new() { Name = name };

    public static class Names
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }

    /// <summary>GUIDs fijos para poder referenciarlos en seeds, servicios y tests sin consultar la DB.</summary>
    public static class WellKnownIds
    {
        public static readonly Guid AdminRoleId = new("00000000-0000-0000-0000-000000000001");
        public static readonly Guid UserRoleId = new("00000000-0000-0000-0000-000000000002");
    }
}
