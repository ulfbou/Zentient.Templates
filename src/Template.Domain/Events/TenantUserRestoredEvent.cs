using Template.Domain.Contracts;
using Template.Domain.ValueObjects;
using Template.Domain.Contracts.Events;
using Template.Domain.Entities;

namespace Template.Domain.Events
{
    /// <summary>
    /// Represents a domain event that occurs when a tenant user is restored.
    /// </summary>
    public record TenantUserRestoredEvent(TenantUser Entity, DateTime OccurredOn) : IDomainEvent, IRestoredEvent<TenantUser, UserId>
    {
        /// <inheritdoc/>
        public UserId EntityId => Entity.Id;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantUserRestoredEvent"/> record.
        /// </summary>
        /// <param name="entity">The restored tenant user entity.</param>
        public TenantUserRestoredEvent(TenantUser entity) : this(entity, DateTime.UtcNow) { }
    }
}
