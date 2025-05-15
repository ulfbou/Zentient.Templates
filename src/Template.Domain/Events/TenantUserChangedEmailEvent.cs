using Template.Domain.Contracts;
using Template.Domain.ValueObjects;
using Template.Domain.Contracts.Events;
using Template.Domain.Entities;

namespace Template.Domain.Events
{
    /// <summary>
    /// Represents a domain event that occurs when a tenant user's email is changed.
    /// </summary>
    public record TenantUserChangedEmailEvent(TenantUser Entity, string NewEmail, string OldEmail, DateTime OccurredOn) : IDomainEvent, IEntityUpdatedEvent<TenantUser, UserId>
    {
        /// <inheritdoc/>
        public UserId EntityId => Entity.Id;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantUserChangedEmailEvent"/> record.
        /// </summary>
        /// <param name="entity">The tenant user entity whose email was changed.</param>
        /// <param name="newEmail">The new email address.</param>
        /// <param name="oldEmail">The old email address.</param>
        public TenantUserChangedEmailEvent(TenantUser entity, string newEmail, string oldEmail) : this(entity, newEmail, oldEmail, DateTime.UtcNow) { }
    }
}
