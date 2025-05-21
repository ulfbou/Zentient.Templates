using Template.Domain.ValueObjects;

namespace Template.Application.Common.Contracts
{
    /// <summary>
    /// Provides access to the current user's context, including identity, tenant, roles, and claims.
    /// </summary>
    public interface IUserContext
    {
        /// <summary>
        /// Gets the unique identifier of the current user.
        /// </summary>
        UserId UserId { get; }

        /// <summary>
        /// Gets the unique identifier of the tenant to which the current user belongs.
        /// </summary>
        TenantId TenantId { get; }

        /// <summary>
        /// Gets the list of roles assigned to the current user.
        /// </summary>
        IReadOnlyList<string> Roles { get; }

        /// <summary>
        /// Gets the claims associated with the current user as key-value pairs.
        /// </summary>
        IReadOnlyDictionary<string, string> Claims { get; }
    }
}
