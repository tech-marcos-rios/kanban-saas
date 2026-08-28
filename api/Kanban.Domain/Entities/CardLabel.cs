namespace Kanban.Domain.Entities;

/// <summary>Join puro entre Card y Label — sin BaseEntity porque su identidad es la clave compuesta (CardId, LabelId).</summary>
public class CardLabel
{
    public Guid CardId { get; private set; }
    public Card Card { get; private set; } = default!;
    public Guid LabelId { get; private set; }
    public Label Label { get; private set; } = default!;

    private CardLabel() { }

    public static CardLabel Create(Guid cardId, Guid labelId) =>
        new() { CardId = cardId, LabelId = labelId };
}
