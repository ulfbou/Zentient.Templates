using Template.Domain.Contracts;
using Template.Domain.Entities;
using Template.Domain.ValueObjects;

namespace Template.Domain.SeedWork
{
    /// <summary>
    /// Base class for all entities with a strongly-typed key and a tenant context.
    /// </summary>
    /// typeparam name="TKey">The type of the key.</typeparam>
    public abstract class TenantEntityBase<TKey> : Entity<TKey>, ITenantEntity<TKey>
            where TKey : struct, IIdentity<TKey>
    {
        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        public TenantId TenantId { get; protected set; }

        /// <summary>
        /// Gets or sets the tenant for the entity.
        /// </summary>
        public virtual ITenant Tenant { get; protected set; } = default!;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantEntityBase{TKey}"/> class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="id">The entity identifier.</param>
        /// <param name="createdBy">The user who created the entity.</param>
        protected TenantEntityBase(TenantId tenantId, TKey id, string createdBy) : base(id, createdBy)
        {
            TenantId = tenantId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantEntityBase{TKey}"/> class.
        /// This constructor is for EF Core only.
        /// </summary>
        protected TenantEntityBase() { /* For EF Core */ }
    }
}
