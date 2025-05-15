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
    public record TenantCreatedEvent(TenantId EntityId, DateTime OccurredOn) : IDomainEvent, IEntityCreatedEvent<Tenant, TenantId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantCreatedEvent"/> record.
        /// </summary>
        /// <param name="entityId">The unique identifier of the tenant.</param>
        public TenantCreatedEvent(TenantId entityId) : this(entityId, DateTime.UtcNow) { }
    }
}
