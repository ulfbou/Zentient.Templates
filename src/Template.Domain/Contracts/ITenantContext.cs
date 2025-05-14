using Template.Domain.ValueObjects;

namespace Template.Domain.Contracts
{
    /// <summary>
    /// Interface for managing tenant context.
    /// </summary>
    public interface ITenantContext
    {
        /// <summary>
        /// Gets the current tenant identifier.
        /// </summary>
        TenantId CurrentTenantId { get; }
    }
}
