namespace Kanban.Domain.Entities;

/// <summary>
/// Clase base para todas las entidades del dominio.
/// </summary>
/// <remarks>
/// Guid como Id: permite generarlo en memoria sin ir a la BD, es seguro en
/// escenarios multi-tenant y no expone volumen de registros en la URL.
/// Todas las fechas son UTC.
/// </remarks>
public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }

    protected void SetUpdatedAt() => UpdatedAt = DateTime.UtcNow;
}
