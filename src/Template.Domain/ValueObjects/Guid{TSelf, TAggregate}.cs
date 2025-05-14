using Template.Domain.Common;
using Template.Domain.Common.Exceptions;
using Template.Domain.ValueObjects;

// TODO: Consider adding validation rules using FluentValidation?
// TODO: Consider adding serialization/deserialization support for JSON/XML if needed.
// TODO: Consider adding integration with EF Core for database storage if applicable.
// TODO: Consider adding source generators for generating boilerplate code if needed.
namespace Template.Domain.ValueObjects
{
    /// <summary>
    /// Represents a generic Guid-based value object.  This provides
    /// common functionality for all Guid-based identifiers.
    /// </summary>
    /// <typeparam name="T">The specific type of the value object (e.g., UserId, TenantId).</typeparam>
    public readonly struct GuidValue<T> : IEquatable<GuidValue<T>>
        where T : struct, IIdentity<T> // Constraint to IIdentity
    {
        public Guid Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GuidValue{T}"/> struct.
        /// </summary>
        /// <param name="value">The Guid value.</param>
        public GuidValue(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException($"ID of type {typeof(T).Name} cannot be empty.");
            }
            Value = value;
        }

        /// <summary>
        /// Implicitly converts a Guid to a GuidValue.
        /// </summary>
        /// <param name="value">The Guid value.</param>
        public static implicit operator GuidValue<T>(Guid value) => new(value);

        /// <summary>
        /// Implicitly converts a GuidValue to a Guid.
        /// </summary>
        /// <param name="valueObject">The GuidValue.</param>
        public static implicit operator Guid(GuidValue<T> valueObject) => valueObject.Value;

        /// <inheritdoc />
        public bool Equals(GuidValue<T> other) => Value.Equals(other.Value);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is GuidValue<T> other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() => Value.ToString("N");

        /// <summary>
        /// Converts the Guid value to a string using the specified format.
        /// </summary>
        /// <param name="format">The format string.</param>
        public string ToString(string format)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                throw new ArgumentException("Format string cannot be null or empty.", nameof(format));
            }

            return Value.ToString(format);
        }

        public static bool operator ==(GuidValue<T> left, GuidValue<T> right) => left.Equals(right);
        public static bool operator !=(GuidValue<T> left, GuidValue<T> right) => !(left == right);
    }
}
