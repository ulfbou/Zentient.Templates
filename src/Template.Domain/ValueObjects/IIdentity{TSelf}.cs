using Template.Domain.Common;
using Template.Domain.Common.Exceptions;

namespace Template.Domain.ValueObjects
{
    /// <summary>
    /// Represents a strongly-typed identifier.
    /// </summary>
    /// <typeparam name="TSelf">The type of the identifier.</typeparam>
    public interface IIdentity<TSelf> : IIdentity<TSelf, Guid>
        where TSelf : struct, IIdentity<TSelf>, IEquatable<TSelf>
    {

        /// <summary>
        /// Parses a string representation of an identifier into an instance of <typeref name="TSelf"/>.
        /// </summary>
        /// <param name="id">The string representation of the identifier.</param>
        /// <returns>An instance of <typeref name="TSelf"/>.</returns>
        /// <exception cref="FormatException">Thrown when the string cannot be parsed into a valid identifier.</exception>
        IIdentity<TSelf> Parse(string id);

        /// <summary>
        /// Tries to parse a string representation of an identifier into an instance of <typeref name="TSelf"/>.
        /// </summary>
        /// <param name="value">The string representation of the identifier.</param>
        /// <param name="result">The parsed instance of <typeref name="TSelf"/>.</param>
        /// <returns><see langword="true"/> if the parsing was successful; otherwise, <see langword="false"/>.</returns>
        bool TryParse(string value, out TSelf? result);

        /// <summary>
        /// Tries to parse a string representation of an identifier into an instance of <typeref name="TSelf"/>.
        /// </summary>
        /// <param name="value">The string representation of the identifier.</param>
        /// <returns>A new instance of <typeparamref name="TSelf"/> if parsing was successful; otherwise, <see langword="null"/>.</returns>
        TSelf? TryParse(string value);
    }
}
