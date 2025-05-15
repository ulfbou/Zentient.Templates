using Template.Domain.Contracts;
using Template.Domain.ValueObjects;
using Template.Domain.Contracts.Events;
using Template.Domain.Entities;

namespace Template.Domain.Events
{
    /// <summary>
    /// Represents a domain event that occurs when a tenant is restored.
    /// </summary>
    public record TenantRestoredEvent(TenantId EntityId, DateTime OccurredOn) : IDomainEvent, IRestoredEvent<Tenant, TenantId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantRestoredEvent"/> record.
        /// </summary>
        /// <param name="entityId">The unique identifier of the restored tenant entity.</param>
        public TenantRestoredEvent(TenantId entityId) : this(entityId, DateTime.UtcNow) { }
    }
}
