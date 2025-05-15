using System.Collections.Frozen;

using Template.Domain.Events;
using Template.Domain.ValueObjects;

namespace Template.Domain.Entities
{
    /// <summary>
    /// Base class for all entities with a strongly-typed key and a tenant context.
    /// </summary>
    /// typeparam name="TKey">The type of the key.</typeparam>
    public abstract class TenantEntity<TKey> : TenantEntityBase<TKey>
        where TKey : struct, IIdentity<TKey>
    {
        /// <summary>
        /// Gets or sets the row version for concurrency checks.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="id">The entity identifier.</param>
        /// <param name="createdBy">The user who created the entity.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="createdBy"/> is <see langword="null"/> or empty.</exception>
        protected TenantEntity(TenantId tenantId, TKey id, string createdBy) : base(tenantId, id, createdBy) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantEntity{TKey}"/> class.
        /// This constructor is for EF Core only.
        /// </summary>
        protected TenantEntity() { /*For EF Core*/ }

        /// <summary>
        /// Gets or sets the row version for concurrency checks.
        /// </summary>
        /// <param name="modifiedBy">The user who modified the entity.</param>
        protected void MarkModified(string modifiedBy)
        {
            ModifiedOn = DateTime.UtcNow;
            ModifiedBy = modifiedBy;
        }
    }
}
