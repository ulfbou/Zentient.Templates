using Template.Domain.Contracts;
using Template.Domain.ValueObjects;
using Template.Domain.Contracts.Events;
using Template.Domain.Entities;

namespace Template.Domain.Events
{
    /// <summary>
    /// Represents a domain event that occurs when a tenant is soft-deleted.
    /// </summary>
    public record TenantDeletedEvent(TenantId EntityId, DateTime OccurredOn) : IDomainEvent, ISoftDeletedEvent<Tenant, TenantId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantDeletedEvent"/> record.
        /// </summary>
        /// <param name="entityId">The unique identifier of the tenant that was deleted.</param>
        public TenantDeletedEvent(TenantId entityId) : this(entityId, DateTime.UtcNow) { }
    }
}
