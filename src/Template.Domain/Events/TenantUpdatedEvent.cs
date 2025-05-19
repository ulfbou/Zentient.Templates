using Template.Domain.Contracts;
using Template.Domain.ValueObjects;
using Template.Domain.Contracts.Events;
using Template.Domain.Entities;
using Template.Domain.Common.Result;

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

    /// <summary>
    /// Represents a domain event that occurs when the metadata of a tenant is updated.
    /// </summary>
    public record TenantMetadataUpdatedEvent(TenantId EntityId, IReadOnlyDictionary<string, string> Metadata, DateTime OccurredOn) : IDomainEvent, IEntityUpdatedEvent<Tenant, TenantId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantMetadataUpdatedEvent"/> record.
        /// </summary>
        /// <param name="entityId">The unique identifier of the updated tenant entity.</param>
        /// <param name="metadata">The updated metadata associated with the tenant.</param>
        public TenantMetadataUpdatedEvent(TenantId entityId, IReadOnlyDictionary<string, string> metadata) : this(entityId, metadata, DateTime.UtcNow) { }
    }

    /// <summary>
    /// Represents a domain event that occurs when the status of a tenant is updated.
    /// </summary>
    public record TenantStatusUpdatedEvent(TenantId EntityId, TenantStatus Status, DateTime OccurredOn) : IDomainEvent, IEntityUpdatedEvent<Tenant, TenantId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantStatusUpdatedEvent"/> record.
        /// </summary>
        /// <param name="entityId">The unique identifier of the updated tenant entity.</param>
        /// <param name="status">The updated status of the tenant.</param>
        public TenantStatusUpdatedEvent(TenantId entityId, TenantStatus status) : this(entityId, status, DateTime.UtcNow) { }
    }
}
