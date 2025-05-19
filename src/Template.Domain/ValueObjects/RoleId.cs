namespace Template.Domain.ValueObjects
{
    /// <summary>
    /// Represents a strongly-typed identifier for roles.
    /// </summary>
    public readonly struct RoleId : IIdentity<RoleId>, IEquatable<RoleId>
    {
        /// <inheritdoc />
        public Guid Value { get; }

        /// <summary>
        /// Initializes a new instance of the <typeref name="RoleId"/> struct.
        /// </summary>
        /// <param name="value">The unique identifier for the role.</param>
        /// <exception cref="ArgumentException">Thrown when the provided <paramref name="value"/> is empty.</exception>
        public RoleId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("RoleId cannot be empty.");
            }
            Value = value;
        }

        /// <summary>
        /// Creates a new instance of <typeparamref name="RoleId"/> with a new <typeref name="Guid"/> value.
        /// </summary>
        /// <returns>A new instance of <typeparamref name="RoleId"/>.</returns>
        public static RoleId New() => new(Guid.NewGuid());

        /// <summary>
        /// Creates a new instance of <typeparamref name="RoleId"/> from the provided <paramref name="id"/> value.
        /// </summary>
        /// <param name="id">The identifier value.</param>
        /// <returns>A new instance of <typeparamref name="RoleId"/>.</returns>
        public static RoleId From(object id)
        {
            if (id is Guid guid)
            {
                return new RoleId(guid);
            }

            if (id is string strId && Guid.TryParse(strId, out var parsedGuid))
            {
                return new RoleId(parsedGuid);
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
        /// Implicitly converts the <see cref="RoleId"/> to a <see cref="string"/> representation.
        /// </summary>
        /// <param name="id">The identity value object.</param>
        /// <returns>A <see cref="string"/> representation of the <see cref="RoleId"/>.</returns>
        public static implicit operator string(RoleId id) => id.Value.ToString();

        /// <inheritdoc />
        public bool Equals(RoleId other) => Value.Equals(other.Value);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is RoleId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc />
        public IIdentity<RoleId> Parse(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("ID cannot be null or empty.", nameof(id));
            }

            if (!Guid.TryParse(id, out var parsedGuid))
            {
                throw new FormatException($"Invalid ID format: {id}");
            }

            return new RoleId(parsedGuid);
        }

        /// <inheritdoc />
        public bool TryParse(string value, out RoleId? result)
        {
            result = TryParse(value);
            return result is not null;
        }

        /// <inheritdoc />
        public RoleId? TryParse(string value)
        {
            return !Guid.TryParse(value, out var parsedGuid) ? null : new RoleId(parsedGuid);
        }

        /// <summary>
        /// Implicitly converts the <typeref name="RoleId"/> to a <typeref name="Guid"/> representation.
        /// </summary>
        /// <param name="id">The identity value object.</param>
        /// <returns>A <typeref name="Guid"/> representation of the <typeref name="RoleId"/>.</returns>
        public static implicit operator Guid(RoleId id) => id.Value;

        /// <summary>
        /// Implicitly converts a <typeref name="Guid"/> to the identity type <typeref name="RoleId"/>.
        /// </summary>
        /// <param name="id">The <typeref name="Guid"/> value.</param>
        /// <returns>A <typeref name="RoleId"/> representation of the <typeref name="Guid"/>.</returns>
        public static implicit operator RoleId(Guid id) => new(id);

        /// <summary>
        /// Determines whether two <typeref name="RoleId"/> instances are equal.
        /// </summary>
        /// <param name="left">The first identity instance.</param>
        /// <param name="right">The second identity instance.</param>
        /// <returns><see langword="true"/> if the instances are equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(RoleId left, RoleId right) => left.Equals(right);

        /// <summary>
        /// Determines whether two <typeref name="RoleId"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first <typeref name="RoleId"/> instance.</param>
        /// <param name="right">The second <typeref name="RoleId"/> instance.</param>
        /// <returns><see langword="true"/> if the instances are not equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(RoleId left, RoleId right) => !(left == right);
    }

}
