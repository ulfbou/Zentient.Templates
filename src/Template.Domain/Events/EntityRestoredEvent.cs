using Template.Domain.Contracts;
using Template.Domain.ValueObjects;
using Template.Domain.Contracts.Events;
using Template.Domain.Entities;

namespace Template.Domain.Events
{
    /// <summary>
    /// Represents a generic domain event that occurs when an entity is restored.
    /// </summary>
    /// <typeparam name="TKey">The type of the entity key.</typeparam>
    public record EntityRestoredEvent<TEntity, TKey>(TKey EntityId, DateTime OccurredOn) : IDomainEvent, IRestoredEvent<TEntity, TKey>
        where TEntity : class, ISoftDelete, IEntity<TKey>
        where TKey : struct, IIdentity<TKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityRestoredEvent{TKey}"/> record.
        /// </summary>
        /// <param name="entityId">The unique identifier of the entity.</param>
        public EntityRestoredEvent(TKey entityId) : this(entityId, DateTime.UtcNow)
        {
            EntityId = entityId;
        }
    }
}
