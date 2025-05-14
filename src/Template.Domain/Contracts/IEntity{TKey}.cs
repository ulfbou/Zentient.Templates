using Template.Domain.ValueObjects;

namespace Template.Domain.Contracts
{
    /// <summary>
    /// Base interface for all entities with a strongly‑typed key.
    /// </summary>
    /// typeparam name="TKey">The type of the key.</typeparam>
    public interface IEntity<TKey> : IEntity
        where TKey : struct, IIdentity<TKey>
    {
        /// <summary>
        /// Gets the unique identifier of the entity.
        /// </summary>
        TKey Id { get; }
    }
}
