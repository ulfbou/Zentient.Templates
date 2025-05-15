using Template.Domain.Contracts;
using Template.Domain.ValueObjects;
using Template.Domain.Contracts.Events;
using Template.Domain.Entities;

namespace Template.Domain.Events
{
    /// <summary>
    /// Represents a domain event that occurs when a tenant user is created.
    /// </summary>
    public record TenantUserCreatedEvent(UserId EntityId, DateTime OccurredOn) : IDomainEvent, IEntityCreatedEvent<TenantUser, UserId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantUserCreatedEvent"/> record.
        /// </summary>
        /// <param name="entityId">The unique identifier of the tenant user.</param>
        public TenantUserCreatedEvent(UserId entityId) : this(entityId, DateTime.UtcNow) { }
    }
}
