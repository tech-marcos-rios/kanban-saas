namespace Kanban.Domain.Entities;

public class BoardMember : BaseEntity
{
    public Guid BoardId { get; private set; }
    public Board Board { get; private set; } = default!;
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;
    public BoardRole Role { get; private set; }

    private BoardMember() { }

    public static BoardMember Create(Guid boardId, Guid userId, BoardRole role) =>
        new() { BoardId = boardId, UserId = userId, Role = role };

    public void ChangeRole(BoardRole role)
    {
        Role = role;
        SetUpdatedAt();
    }
}
