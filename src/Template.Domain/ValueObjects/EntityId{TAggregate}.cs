using Template.Domain.Common;
using Template.Domain.Common.Exceptions;

namespace Template.Domain.ValueObjects
{
    /// <summary>
    /// Represents a generic entity identifier.
    /// </summary>
    /// <typeparam name="TKey">The type of the identifier (e.g., int, string, Guid).</typeparam>
    public readonly struct EntityId<TKey> : IIdentity<EntityId<TKey>, TKey>, IEquatable<EntityId<TKey>>
        where TKey : struct, IIdentity<EntityId<TKey>, TKey>, IEquatable<TKey>
    {
        /// <inheritdoc />
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
        /// Creates a new instance of <typeparamref name="EntityId{TKey}"/> with a new <see cref="TKey"/> value.
        /// </summary>
        /// <returns>A new instance of <typeparamref name="TSelf"/>.</returns>
        public static EntityId<TKey> New()
        {
            if (typeof(TKey) == typeof(Guid))
            {
                return new EntityId<TKey>((TKey)(object)Guid.NewGuid());
            }

            if (typeof(TKey) == typeof(string))
            {
                return new EntityId<TKey>((TKey)(object)Guid.NewGuid().ToString());
            }

            throw new NotSupportedException($"Type {typeof(TKey).Name} is not supported for ID generation.");
        }

        /// <summary>
        /// Creates a new instance of <typeparamref name="TSelf"/> from an existing <see cref="object"/> value.
        /// </summary>
        /// <param name="id">The identifier value.</param>
        /// <returns>A new instance of <typeparamref name="TSelf"/>.</returns>
        public static EntityId<TKey> From(object id)
        {
            if (id is TKey key)
            {
                return new EntityId<TKey>(key);
            }

            throw new ArgumentException($"Invalid ID type. Expected {typeof(TKey).Name}, but got {id.GetType().Name}.");
        }

        /// <inheritdoc />
        public override string? ToString() => Value.ToString();

        /// <inheritdoc />
        public string ToString(string format) => Value.ToString(format);

        /// <summary>
        /// Implicitly converts the <see cref="EntityId{TKey}"/> to a <see cref="string"/> representation.
        /// </summary>
        /// <param name="id">The identity value object.</param>
        public static implicit operator string(EntityId<TKey> id) => id.Value.ToString() ?? DomainData.NullValues.GetNullValue<TKey>();

        /// <inheritdoc />
        public bool Equals(EntityId<TKey> other) => Value.Equals(other.Value);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is EntityId<TKey> other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// Implicitly converts the <see cref="EntityId{TKey}"/> to a <see cref="TKey"/>.
        /// </summary>
        /// <param name="id">The identity value object.</param>
        public static implicit operator TKey(EntityId<TKey> id) => id.Value;

        /// <summary>
        /// Implicitly converts a <see cref="TKey"/> to the identity type <typeparamref name="TSelf"/>.
        /// </summary>
        /// <param name="value">The <see cref="TKey"/> value.</param>
        /// <returns>A new instance of <typeparamref name="TSelf"/>.</returns>
        public static implicit operator EntityId<TKey>(TKey value) => new(value);

        /// <summary>
        /// Determines whether two <see cref="EntityId{TKey}"/> instances are equal.
        /// </summary>
        /// <param name="left">The first identity instance.</param>
        /// <param name="right">The second identity instance.</param>
        /// <returns><see langword="true" /> if the instances are equal; otherwise, <see langword="false" />.</returns>
        public static bool operator ==(EntityId<TKey> left, EntityId<TKey> right) => left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="EntityId{TKey}"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first identity instance.</param>
        /// <param name="right">The second identity instance.</param>
        /// <returns><see langword="true" /> if the instances are not equal; otherwise, <see langword="false" />.</returns>
        public static bool operator !=(EntityId<TKey> left, EntityId<TKey> right) => !(left == right);
    }
}
