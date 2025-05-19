namespace Template.Domain.ValueObjects
{
    /// <summary>
    /// Represents a strongly-typed identity value object.
    /// </summary>
    /// <typeparam name="TSelf">The type that implements this interface.</typeparam>
    /// <typeparam name="TKey">The underlying value type of the identity.</typeparam>
    public interface IIdentity<TSelf, TKey> : IEquatable<TSelf>
        where TSelf : struct, IIdentity<TSelf, TKey>
        where TKey : notnull, IEquatable<TKey>
    {
        /// <summary>
        /// Gets the underlying value of the identity.
        /// </summary>
        TKey Value { get; }

        /// <summary>
        /// Converts the <see cref="IIdentity{TSelf, TKey}"/> to a <see cref="string"/> representation.
        /// </summary>
        /// <param name="format">The format <see cref="string"/>.</param>
        string ToString(string format);
    }
}
