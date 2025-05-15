using Template.Domain.Contracts;
using Template.Domain.ValueObjects;
using Template.Domain.Contracts.Events;
using Template.Domain.Entities;

namespace Template.Domain.Events
{
    /// <summary>
    /// Represents a domain event that occurs when a tenant user's name is changed.
    /// </summary>
    public record TenantUserChangedNameEvent(TenantUser Entity, string NewName, string OldName, DateTime OccurredOn) : IDomainEvent, IEntityUpdatedEvent<TenantUser, UserId>
    {
        /// <inheritdoc/>
        public UserId EntityId => Entity.Id;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantUserChangedNameEvent"/> record.
        /// </summary>
        /// <param name="entity">The tenant user entity whose name was changed.</param>
        /// <param name="newName">The new username.</param>
        /// <param name="oldName">The old username.</param>
        public TenantUserChangedNameEvent(TenantUser entity, string newName, string oldName) : this(entity, newName, oldName, DateTime.UtcNow) { }
    }
}
