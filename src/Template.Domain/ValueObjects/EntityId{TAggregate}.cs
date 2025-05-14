using Template.Domain.Common;
using Template.Domain.Common.Exceptions;

namespace Template.Domain.ValueObjects
{
    /// <summary>
    /// Represents a generic entity identifier.
    /// </summary>
    /// <typeparam name="TKey">The type of the identifier (e.g., int, string, Guid).</typeparam>
    public readonly struct EntityId<TKey> : IIdentity<EntityId<TKey>, TKey>, IEquatable<EntityId<TKey>>
        where TKey : notnull, IEquatable<TKey>
    {
        public TKey Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityId{TKey}"/> struct.
        /// </summary>
        /// <param name="value">The identifier value.</param>
        public EntityId(TKey value)
        {
            Value = value;
        }

        /// <summary>
        /// Creates a new instance of the identifier with a new default value.
        /// </summary>
        /// <returns>A new instance of the identifier.</returns>
        public static EntityId<TKey> New() => new(default(TKey)!);

        /// <summary>
        /// Creates a new instance of the identifier from an existing value.
        /// </summary>
        /// <param name="id">The identifier value.</param>
        /// <returns>A new instance of the identifier.</returns>
        public static EntityId<TKey> From(object id)
        {
            if (id is TKey key)
            {
                return new EntityId<TKey>(key);
            }
            throw new ArgumentException($"Invalid ID type. Expected {typeof(TKey).Name}, but got {id.GetType().Name}.");
        }

        /// <summary>
        /// Creates a new instance of the identifier from an existing TKey value.
        /// </summary>
        /// <param name="id">The identifier value.</param>
        /// <returns>A new instance of the identifier.</returns>
        public static EntityId<TKey> From(TKey id) => new(id);

        public bool Equals(EntityId<TKey> other) => Value.Equals(other.Value);

        public override bool Equals(object? obj) => obj is EntityId<TKey> other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value?.ToString() ?? string.Empty;

        public static implicit operator TKey(EntityId<TKey> id) => id.Value;

        public static implicit operator EntityId<TKey>(TKey value) => new(value);

        public static bool operator ==(EntityId<TKey> left, EntityId<TKey> right) => left.Equals(right);
        public static bool operator !=(EntityId<TKey> left, EntityId<TKey> right) => !(left == right);
    }
}
