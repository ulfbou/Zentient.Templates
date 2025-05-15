using Template.Domain.Contracts;
using Template.Domain.ValueObjects;
using Template.Domain.Contracts.Events;
using Template.Domain.Entities;

namespace Template.Domain.Events
{
    /// <summary>
    /// Represents a domain event that occurs when a tenant is updated.
    /// </summary>
    public record TenantUpdatedEvent(Tenant Entity, DateTime OccurredOn) : IDomainEvent, IEntityUpdatedEvent<Tenant, TenantId>
    {
        /// <inheritdoc/>
        public TenantId EntityId => Entity.Id;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantUpdatedEvent"/> record.
        /// </summary>
        /// <param name="tenant">The updated tenant entity.</param>
        public TenantUpdatedEvent(Tenant tenant) : this(tenant, DateTime.UtcNow) { }
    }
}
