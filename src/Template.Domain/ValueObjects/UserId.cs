namespace Template.Domain.ValueObjects
{
    /// <summary>
    /// Represents a strongly-typed identifier for users.
    /// </summary>
    public readonly struct UserId : IIdentity<UserId>, IEquatable<UserId>
    {
        /// <inheritdoc />
        public Guid Value { get; }

        /// <summary>
        /// Initializes a new instance of the <typeref name="UserId"/> struct.
        /// </summary>
        /// <param name="value">The unique identifier for the user.</param>
        /// <exception cref="ArgumentException">Thrown when the provided <paramref name="value"/> is empty.</exception>
        private UserId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty.");
            }

            Value = value;
        }

        /// <summary>
        /// Creates a new instance of <typeparamref name="UserId"/> with a new <typeref name="Guid"/> value.
        /// </summary>
        /// <returns>A new instance of <typeparamref name="UserId"/>.</returns>
        public static UserId New() => new(Guid.NewGuid());

        /// <summary>
        /// Creates a new instance of <typeparamref name="UserId"/> from the provided <paramref name="id"/> value.
        /// </summary>
        /// <param name="id">The identifier value.</param>
        /// <returns>A new instance of <typeparamref name="UserId"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided <paramref name="id"/> is not a valid type.</exception>
        public static UserId From(object id)
        {
            if (id is Guid guid)
            {
                return new UserId(guid);
            }

            if (id is string strId && Guid.TryParse(strId, out var parsedGuid))
            {
                return new UserId(parsedGuid);
            }

            throw new ArgumentException($"Invalid ID type. Expected Guid or string, but got {id?.GetType().Name ?? "null"}.");
        }

        /// <inheritdoc />
        public bool Equals(UserId other) => Value.Equals(other.Value);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is UserId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() => Value.ToString("N");

        /// <summary>
        /// Converts the <see cref="UserId"/> to a string representation using the specified format.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <returns>A <typeref name="string"/> representation of the <see cref="UserId"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided <paramref name="format"/> is <see langword="null"/> or empty.</exception>
        public string ToString(string format)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                throw new ArgumentException("Format string cannot be null or empty.", nameof(format));
            }

            return Value.ToString(format);
        }

        /// <inheritdoc />
        public IIdentity<UserId> Parse(string id)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public bool TryParse(string value, out UserId? result)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public UserId? TryParse(string value)
        {
            return !Guid.TryParse(value, out var parsedGuid) ? null : new UserId(parsedGuid);
        }

        /// <summary>
        /// Implicitly converts the <typeref name="UserId"/> to a <typeref name="Guid"/> representation.
        /// </summary>
        /// <param name="id">The identity value object.</param>
        /// <returns>A <typeref name="Guid"/> representation of the <typeref name="UserId"/>.</returns>
        public static implicit operator Guid(UserId id) => id.Value;

        /// <summary>
        /// Implicitly converts a <typeref name="Guid"/> to the identity type <typeref name="UserId"/>.
        /// </summary>
        /// <param name="id">The <typeref name="Guid"/> value.</param>
        /// <returns>A <typeref name="UserId"/> representation of the <typeref name="Guid"/>.</returns>
        public static implicit operator UserId(Guid id) => new(id);

        /// <summary>
        /// Implicitly converts the <typeref name="UserId"/> to a <see cref="string"/> representation.
        /// </summary>
        /// <param name="id">The identity value object.</param>
        /// <returns>A <typeref name="string"/> representation of the <see cref="UserId"/>.</returns>
        public static implicit operator string(UserId id) => id.ToString();

        /// <summary>
        /// Determines whether two <typeref name="UserId"/> instances are equal.
        /// </summary>
        /// <param name="left">The first <typeref name="UserId"/> instance.</param>
        /// <param name="right">The second <typeref name="UserId"/> instance.</param>
        /// <returns><see langword="true"/> if the instances are equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(UserId left, UserId right) => left.Equals(right);

        /// <summary>
        /// Determines whether two <typeref name="UserId"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first <typeref name="UserId"/> instance.</param>
        /// <param name="right">The second <typeref name="UserId"/> instance.</param>
        /// <returns><see langword="true"/> if the instances are not equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(UserId left, UserId right) => !(left == right);
    }
}
