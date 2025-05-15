using Template.Domain.Contracts;
using Template.Domain.ValueObjects;
using Template.Domain.Contracts.Events;
using Template.Domain.Entities;

namespace Template.Domain.Events
{
    /// <summary>
    /// Represents a domain event that occurs when a tenant is updated.
    /// </summary>
    public record TenantUpdatedEvent(TenantId EntityId, DateTime OccurredOn) : IDomainEvent, IEntityUpdatedEvent<Tenant, TenantId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantUpdatedEvent"/> record.
        /// </summary>
        /// <param name="entityId">The unique identifier of the updated tenant entity.</param>
        public TenantUpdatedEvent(TenantId entityId) : this(entityId, DateTime.UtcNow) { }
    }
}
