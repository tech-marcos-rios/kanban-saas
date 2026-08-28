namespace Kanban.Domain.Entities;

public class Comment : BaseEntity
{
    public Guid CardId { get; private set; }
    public Card Card { get; private set; } = default!;
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;
    public string Content { get; private set; } = default!;

    private Comment() { }

    public static Comment Create(Guid cardId, Guid userId, string content) =>
        new() { CardId = cardId, UserId = userId, Content = content };
}
