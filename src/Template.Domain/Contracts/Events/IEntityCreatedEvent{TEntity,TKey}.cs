using Template.Domain.ValueObjects;

namespace Template.Domain.Contracts.Events
{
    /// <summary>
    /// Represents a domain event that occurs when an entity is created.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity that was created.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public interface IEntityCreatedEvent<TEntity, TKey> : IDomainEvent
        where TEntity : IEntity<TKey>
        where TKey : struct, IIdentity<TKey>
    {
        /// <summary>
        /// Gets the entity that was created.
        /// </summary>
        TEntity Entity { get; }

        /// <summary>
        /// Gets the ID of the entity that was created.
        /// </summary>
        TKey EntityId { get; }
    }
}
