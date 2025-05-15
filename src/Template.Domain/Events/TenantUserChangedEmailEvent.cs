using Template.Domain.Contracts;
using Template.Domain.ValueObjects;
using Template.Domain.Contracts.Events;
using Template.Domain.Entities;

namespace Template.Domain.Events
{
    /// <summary>
    /// Represents a domain event that occurs when a tenant user's email is changed.
    /// </summary>
    public record TenantUserChangedEmailEvent(UserId EntityId, string NewEmail, string OldEmail, DateTime OccurredOn) : IDomainEvent, IEntityUpdatedEvent<TenantUser, UserId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantUserChangedEmailEvent"/> record.
        /// </summary>
        /// <param name="entityId">The unique identifier of the tenant user.</param>
        /// <param name="newEmail">The new email address.</param>
        /// <param name="oldEmail">The old email address.</param>
        public TenantUserChangedEmailEvent(UserId entityId, string newEmail, string oldEmail) : this(entityId, newEmail, oldEmail, DateTime.UtcNow) { }
    }
}
