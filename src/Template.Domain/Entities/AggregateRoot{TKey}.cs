using Template.Domain.Contracts;
using Template.Domain.ValueObjects;

namespace Template.Domain.Entities
{
    /// <summary>
    /// Base class for all entities with a strongly‑typed key.
    /// </summary>
    /// typeparam name="TKey">The type of the key.</typeparam>
    public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
           where TKey : struct, IIdentity<TKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot{TKey}"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="createdBy">The user who created the entity.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="createdBy"/> is <see langword="null"/> or empty.</exception>
        protected AggregateRoot(TKey id, string createdBy) : base(id, createdBy) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot{TKey}"/> class.
        /// This constructor is used by Entity Framework Core for materialization.
        /// </summary>
        protected AggregateRoot() : base() { /* For EF Core */ }
    }
}
