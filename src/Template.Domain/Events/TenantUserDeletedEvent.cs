using Template.Domain.Contracts;
using Template.Domain.ValueObjects;
using Template.Domain.Contracts.Events;
using Template.Domain.Entities;

namespace Template.Domain.Events
{
    /// <summary>
    /// Represents a domain event that occurs when a tenant user is soft-deleted.
    /// </summary>
    public record TenantUserDeletedEvent(UserId EntityId, DateTime OccurredOn) : IDomainEvent, ISoftDeletedEvent<TenantUser, UserId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantUserDeletedEvent"/> record.
        /// </summary>
        /// <param name="entityId">The unique identifier of the tenant user entity.</param>
        public TenantUserDeletedEvent(UserId entityId) : this(entityId, DateTime.UtcNow) { }
    }
}
