using Template.Domain.Contracts;
using Template.Domain.ValueObjects;
using Template.Domain.Contracts.Events;
using Template.Domain.Entities;

namespace Template.Domain.Events
{
    /// <summary>
    /// Represents a domain event that occurs when a tenant is soft-deleted.
    /// </summary>
    public record TenantDeletedEvent(Tenant Entity, DateTime OccurredOn) : IDomainEvent, ISoftDeletedEvent<Tenant, TenantId>
    {
        /// <inheritdoc/>
        public TenantId EntityId => Entity.Id;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantDeletedEvent"/> record.
        /// </summary>
        /// <param name="tenant">The deleted tenant entity.</param>
        public TenantDeletedEvent(Tenant tenant) : this(tenant, DateTime.UtcNow) { }
    }
}
