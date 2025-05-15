using Template.Domain.Contracts;
using Template.Domain.ValueObjects;
using Template.Domain.Contracts.Events;
using Template.Domain.Entities;

namespace Template.Domain.Events
{
    /// <summary>
    /// Represents a domain event that occurs when a tenant is restored.
    /// </summary>
    public record TenantRestoredEvent(Tenant Entity, DateTime OccurredOn) : IDomainEvent, IRestoredEvent<Tenant, TenantId>
    {
        /// <inheritdoc/>
        public TenantId EntityId => Entity.Id;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantRestoredEvent"/> record.
        /// </summary>
        /// <param name="tenant">The restored tenant entity.</param>
        public TenantRestoredEvent(Tenant tenant) : this(tenant, DateTime.UtcNow) { }
    }
}
