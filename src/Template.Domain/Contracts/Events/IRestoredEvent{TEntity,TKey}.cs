using Template.Domain.ValueObjects;

namespace Template.Domain.Contracts.Events
{
    /// <summary>
    /// Represents a domain event that occurs when a soft-deleted entity is restored.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity that was created.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public interface IRestoredEvent<TEntity, TKey> : IDomainEvent
        where TEntity : ISoftDelete, IEntity<TKey>
        where TKey : struct, IIdentity<TKey>
    {
        /// <summary>
        /// Gets the entity that was restored.
        /// </summary>
        TEntity Entity { get; }

        /// <summary>
        /// Gets the ID of the entity that was restored.
        /// </summary>
        TKey EntityId { get; }
    }
}
