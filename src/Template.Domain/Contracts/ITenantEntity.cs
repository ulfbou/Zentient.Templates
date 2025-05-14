using Template.Domain.Entities;
using Template.Domain.MultiTenancy;
using Template.Domain.ValueObjects;

namespace Template.Domain.Contracts
{
    /// <summary>
    /// A domain entity bound to a specific tenant context.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public interface ITenantEntity<TKey> : IEntity<TKey>
        where TKey : struct, IIdentity<TKey>
    {
        /// <summary>
        /// Gets or sets the tenant context for the entity.
        /// </summary>
        TenantId TenantId { get; }
    }
}
