namespace Kanban.Domain.Entities;

public class Board : BaseEntity
{
    public string Name { get; private set; } = default!;
    public Guid OwnerId { get; private set; }
    public User Owner { get; private set; } = default!;

    private readonly List<BoardMember> _members = new();
    public IReadOnlyCollection<BoardMember> Members => _members;

    private Board() { }

    public static Board Create(string name, Guid ownerId) =>
        new() { Name = name, OwnerId = ownerId };

    public void Rename(string name)
    {
        Name = name;
        SetUpdatedAt();
    }
}
