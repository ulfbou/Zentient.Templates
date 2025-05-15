using Template.Domain.Contracts;
using Template.Domain.ValueObjects;
using Template.Domain.Contracts.Events;
using Template.Domain.Entities;

namespace Template.Domain.Events
{
    /// <summary>
    /// Represents a generic domain event that occurs when an entity is soft-deleted.
    /// </summary>
    /// <typeparam name="TKey">The type of the entity key.</typeparam>
    public record EntitySoftDeletedEvent<TKey>(Entity<TKey> Entity, DateTime OccurredOn) : IDomainEvent, ISoftDeletedEvent<Entity<TKey>, TKey>
        where TKey : struct, IIdentity<TKey>
    {
        /// <inheritdoc/>
        public TKey EntityId => Entity.Id;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySoftDeletedEvent{TKey}"/> record.
        /// </summary>
        /// <param name="entity">The soft-deleted entity.</param>
        public EntitySoftDeletedEvent(Entity<TKey> entity) : this(entity, DateTime.UtcNow) { }
    }
}
