using Template.Domain.Contracts;
using Template.Domain.ValueObjects;

namespace Template.Domain.Events
{
    public record TenantUserChangedRolesEvent(UserId id, IReadOnlyList<string> roles, IReadOnlyList<string>? oldRoles, DateTime OccurredOn) : IDomainEvent
    {
        public TenantUserChangedRolesEvent(UserId id, IReadOnlyList<string> roles, IReadOnlyList<string>? oldRoles)
            : this(id, roles, oldRoles, DateTime.UtcNow)
        { }
    }
}
