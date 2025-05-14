namespace Template.Domain.ValueObjects
{
    /// <summary>
    /// Represents a strongly-typed identifier.
    /// </summary>
    /// <typeparam name="TSelf">The type of the identifier.</typeparam>
    /// <typeparam name="TKey">The type of the identifier's key.</typeparam>
    public interface IIdentity<TSelf, TKey> : IEquatable<TSelf>
        where TSelf : struct, IIdentity<TSelf, TKey>
        where TKey : notnull, IEquatable<TKey>
    {
        /// <summary>
        /// Gets the unique identifier value.
        /// </summary>
        TKey Value { get; }

        /// <summary>
        /// Creates a new instance of the identifier with a new <see cref="TKey"/>.
        /// </summary>
        static abstract TSelf New();

        /// <summary>
        /// Creates a new instance of the identifier from an existing <see cref="TKey"/>.
        /// </summary>
        static abstract TSelf From(object id);
    }
}
