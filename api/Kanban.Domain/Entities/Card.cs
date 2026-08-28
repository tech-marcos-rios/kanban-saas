namespace Kanban.Domain.Entities;

public class Card : BaseEntity
{
    public Guid ListId { get; private set; }
    public BoardList List { get; private set; } = default!;
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public int Position { get; private set; }
    public Guid? AssignedUserId { get; private set; }
    public User? AssignedUser { get; private set; }
    public DateTime? DueDate { get; private set; }

    private Card() { }

    public static Card Create(Guid listId, string title, int position) =>
        new() { ListId = listId, Title = title, Position = position };

    public void UpdateDetails(string title, string? description, DateTime? dueDate)
    {
        Title = title;
        Description = description;
        DueDate = dueDate;
        SetUpdatedAt();
    }

    public void MoveTo(Guid listId, int position)
    {
        ListId = listId;
        Position = position;
        SetUpdatedAt();
    }

    public void AssignTo(Guid? userId)
    {
        AssignedUserId = userId;
        SetUpdatedAt();
    }
}
