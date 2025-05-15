using Template.Domain.Contracts;
using Template.Domain.ValueObjects;
using Template.Domain.Contracts.Events;
using Template.Domain.Entities;

namespace Template.Domain.Events
{
    /// <summary>
    /// Represents a domain event that occurs when a tenant user's name is changed.
    /// </summary>
    public record TenantUserChangedNameEvent(UserId EntityId, string NewName, string OldName, DateTime OccurredOn) : IDomainEvent, IEntityUpdatedEvent<TenantUser, UserId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantUserChangedNameEvent"/> record.
        /// </summary>
        /// <param name="entityId">The unique identifier of the tenant user.</param>
        /// <param name="newName">The new username.</param>
        /// <param name="oldName">The old username.</param>
        public TenantUserChangedNameEvent(UserId entityId, string newName, string oldName) : this(entityId, newName, oldName, DateTime.UtcNow) { }
    }
}
