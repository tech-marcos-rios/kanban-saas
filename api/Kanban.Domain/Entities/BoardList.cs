namespace Kanban.Domain.Entities;

/// <summary>Una columna del tablero (ej. "To Do", "Doing", "Done"). Se llama BoardList para no chocar con List&lt;T&gt;.</summary>
public class BoardList : BaseEntity
{
    public Guid BoardId { get; private set; }
    public Board Board { get; private set; } = default!;
    public string Title { get; private set; } = default!;
    public int Position { get; private set; }

    private BoardList() { }

    public static BoardList Create(Guid boardId, string title, int position) =>
        new() { BoardId = boardId, Title = title, Position = position };

    public void Rename(string title)
    {
        Title = title;
        SetUpdatedAt();
    }

    public void MoveTo(int position)
    {
        Position = position;
        SetUpdatedAt();
    }
}
