using Template.Domain.Contracts;
using Template.Domain.ValueObjects;
using Template.Domain.Contracts.Events;
using Template.Domain.Entities;

namespace Template.Domain.Events
{
    /// <summary>
    /// Represents a domain event that occurs when a tenant user is created.
    /// </summary>
    public record TenantUserCreatedEvent(TenantUser Entity, DateTime OccurredOn) : IDomainEvent, IEntityCreatedEvent<TenantUser, UserId>
    {
        /// <inheritdoc/>
        public UserId EntityId => Entity.Id;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantUserCreatedEvent"/> record.
        /// </summary>
        /// <param name="entity">The created tenant user entity.</param>
        public TenantUserCreatedEvent(TenantUser entity) : this(entity, DateTime.UtcNow) { }
    }
}
