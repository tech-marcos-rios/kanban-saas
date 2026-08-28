namespace Kanban.Domain.Entities;

public class Label : BaseEntity
{
    public Guid BoardId { get; private set; }
    public Board Board { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string Color { get; private set; } = default!;

    private Label() { }

    public static Label Create(Guid boardId, string name, string color) =>
        new() { BoardId = boardId, Name = name, Color = color };
}
