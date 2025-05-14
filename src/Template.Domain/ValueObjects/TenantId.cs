using Template.Domain.Common.Exceptions;
using Template.Domain.Entities;

namespace Template.Domain.ValueObjects
{
    /// <summary>
    /// Represents a strongly-typed identifier for tenants.
    /// </summary>
    public readonly struct TenantId : IIdentity<TenantId>, IEquatable<TenantId>
    {
        public Guid Value { get; }

        public TenantId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("TenantId cannot be empty.");
            }
            Value = value;
        }

        public static TenantId New() => new(Guid.NewGuid());

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

        public bool Equals(TenantId other) => Value.Equals(other.Value);

        public override bool Equals(object? obj) => obj is TenantId other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value.ToString("N");

        public static implicit operator Guid(TenantId id) => id.Value;
        public static implicit operator TenantId(Guid id) => new(id);

        public static bool operator ==(TenantId left, TenantId right) => left.Equals(right);
        public static bool operator !=(TenantId left, TenantId right) => !(left == right);
    }

    /// <summary>
    /// Represents a strongly-typed identifier for users.
    /// </summary>
    public readonly struct UserId : IIdentity<UserId>, IEquatable<UserId>
    {
        public Guid Value { get; }

        public UserId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty.");
            }

            Value = value;
        }

        public static UserId New() => new(Guid.NewGuid());

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

        public bool Equals(UserId other) => Value.Equals(other.Value);
        public override bool Equals(object? obj) => obj is UserId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
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

        public static implicit operator Guid(UserId id) => id.Value;
        public static implicit operator UserId(Guid id) => new(id);
        public static bool operator ==(UserId left, UserId right) => left.Equals(right);
        public static bool operator !=(UserId left, UserId right) => !(left == right);
    }
}
