using Template.Domain.Common.Exceptions;
using Template.Domain.Entities;

namespace Template.Domain.ValueObjects
{
    /// <summary>
    /// Represents a strongly-typed identifier for tenants.
    /// </summary>
    public readonly struct TenantId : IIdentity<TenantId>, IEquatable<TenantId>
    {
        /// <inheritdoc />
        public Guid Value { get; }

        /// <summary>
        /// Initializes a new instance of the <typeref name="TenantId"/> struct.
        /// </summary>
        /// <param name="value">The unique identifier for the tenant.</param>
        /// <exception cref="ArgumentException">Thrown when the provided <paramref name="value"/> is empty.</exception>
        public TenantId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("TenantId cannot be empty.");
            }

            Value = value;
        }

        /// <summary>
        /// Creates a new instance of <typeparamref name="TenantId"/> with a new <typeref name="Guid"/> value.
        /// </summary>
        /// <returns>A new instance of <typeparamref name="TenantId"/>.</returns>
        public static TenantId New() => new(Guid.NewGuid());

        /// <summary>
        /// Creates a new instance of <typeparamref name="TenantId"/> from the provided <paramref name="id"/> value.
        /// </summary>
        /// <param name="id">The identifier value.</param>
        /// <returns>A new instance of <typeparamref name="TenantId"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided <paramref name="id"/> is not a valid type.</exception>
        public static TenantId From(object id)
        {
            if (id is Guid guid)
            {
                return new TenantId(guid);
            }

            if (id is string strId && Guid.TryParse(strId, out var parsedGuid))
            {
                return new TenantId(parsedGuid);
            }

            throw new ArgumentException($"Invalid ID type. Expected Guid or string, but got {id?.GetType().Name ?? "null"}.");
        }

        /// <inheritdoc />
        public override string ToString() => Value.ToString("N");

        /// <inheritdoc />
        public string ToString(string format)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                throw new ArgumentException("Format string cannot be null or empty.", nameof(format));
            }

            return Value.ToString(format);
        }

        /// <summary>
        /// Implicitly converts the <see cref="TenantId"/> to a <see cref="string"/> representation.
        /// </summary>
        /// <param name="id">The identity value object.</param>
        /// <returns>A <see cref="string"/> representation of the <see cref="TenantId"/>.</returns>
        public static implicit operator string(TenantId id) => id.ToString();

        /// <inheritdoc />
        public bool Equals(TenantId other) => Value.Equals(other.Value);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is TenantId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// Implicitly converts the <typeref name="TenantId"/> to a <typeref name="Guid"/> representation.
        /// </summary>
        /// <param name="id">The identity value object.</param>
        public static implicit operator Guid(TenantId id) => id.Value;

        /// <summary>
        /// Implicitly converts a <typeref name="Guid"/> to the identity type <typeref name="TenantId"/>.
        /// </summary>
        /// <param name="id">The <typeref name="Guid"/> value.</param>
        /// <returns>A <typeref name="TenantId"/> representation of the <typeref name="Guid"/>.</returns>
        public static implicit operator TenantId(Guid id) => new(id);

        /// <summary>
        /// Determines whether two <typeref name="TenantId"/> instances are equal.
        /// </summary>
        /// <param name="left">The first <typeref name="TenantId"/> instance.</param>
        /// <param name="right">The second <typeref name="TenantId"/> instance.</param>
        /// <returns><see langword="true"/> if the instances are equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(TenantId left, TenantId right) => left.Equals(right);

        /// <summary>
        /// Determines whether two <typeref name="TenantId"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first <typeref name="TenantId"/> instance.</param>
        /// <param name="right">The second <typeref name="TenantId"/> instance.</param>
        /// <returns><see langword="true"/> if the instances are not equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(TenantId left, TenantId right) => !(left == right);

        /// <inheritdoc />
        public IIdentity<TenantId> Parse(string id) => TryParse(id) ?? throw new FormatException($"Invalid TenantId format: {id}.");

        /// <inheritdoc />
        public TenantId? TryParse(string value) => !Guid.TryParse(value, out var parsedGuid) ? null : new TenantId(parsedGuid);

        /// <inheritdoc />
        public bool TryParse(string value, out TenantId? result)
        {
            result = TryParse(value);
            return result is not null;
        }
    }
}
