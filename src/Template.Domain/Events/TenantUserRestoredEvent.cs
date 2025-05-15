using Template.Domain.Contracts;
using Template.Domain.ValueObjects;
using Template.Domain.Contracts.Events;
using Template.Domain.Entities;

namespace Template.Domain.Events
{
    /// <summary>
    /// Represents a domain event that occurs when a tenant user is restored.
    /// </summary>
    public record TenantUserRestoredEvent(UserId EntityId, DateTime OccurredOn) : IDomainEvent, IRestoredEvent<TenantUser, UserId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantUserRestoredEvent"/> record.
        /// </summary>
        /// <param name="entityId">The unique identifier of the tenant user entity.</param>
        public TenantUserRestoredEvent(UserId entityId) : this(entityId, DateTime.UtcNow) { }
    }
}
