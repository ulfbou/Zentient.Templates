using System;
using Template.Domain.Contracts;
using Template.Domain.ValueObjects;
using Template.Domain.Contracts.Events;
using Template.Domain.Entities;

namespace Template.Domain.Events
{
    /// <summary>
    /// Represents a domain event that occurs when a tenant is created.
    /// </summary>
    public record TenantCreatedEvent(Tenant Entity, DateTime OccurredOn) : IDomainEvent, IEntityCreatedEvent<Tenant, TenantId>
    {
        /// <inheritdoc/>
        public TenantId EntityId => Entity.Id;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantCreatedEvent"/> record.
        /// </summary>
        /// <param name="tenant">The created tenant entity.</param>
        public TenantCreatedEvent(Tenant tenant) : this(tenant, DateTime.UtcNow) { }
    }
}
