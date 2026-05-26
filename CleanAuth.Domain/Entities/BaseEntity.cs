namespace CleanAuth.Domain.Entities;

/// <summary>
/// Represents the base entity with a unique identifier for all entities in the authentication system.
/// </summary>
public class BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public Guid Id { get; set; }
}
