using Template.Domain.Contracts;
using Template.Domain.ValueObjects;
using Template.Domain.Contracts.Events;
using Template.Domain.Entities;

namespace Template.Domain.Events
{
    /// <summary>
    /// Represents a domain event that occurs when a tenant user is soft-deleted.
    /// </summary>
    public record TenantUserDeletedEvent(TenantUser Entity, DateTime OccurredOn) : IDomainEvent, ISoftDeletedEvent<TenantUser, UserId>
    {
        /// <inheritdoc/>
        public UserId EntityId => Entity.Id;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantUserDeletedEvent"/> record.
        /// </summary>
        /// <param name="entity">The soft-deleted tenant user entity.</param>
        public TenantUserDeletedEvent(TenantUser entity) : this(entity, DateTime.UtcNow) { }
    }
}
