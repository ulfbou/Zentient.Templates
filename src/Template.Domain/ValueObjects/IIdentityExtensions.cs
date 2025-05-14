using Template.Domain.Common;
using Template.Domain.Common.Exceptions;

namespace Template.Domain.ValueObjects
{
    public static class IIdentityExtensions
    {
        /// <summary>
        /// Parses a string representation of an identifier into an instance of <see cref="IIdentity{TSelf}"/>.
        /// </summary>
        /// <typeparam name="TSelf">The type of the identifier.</typeparam>
        /// <param name="id">The string representation of the identifier.</param>
        /// <returns>An instance of <see cref="TSelf"/>.</returns>
        /// <exception cref="FormatException">Thrown when the string cannot be parsed into a valid identifier.</exception>
        public static TSelf Parse<TSelf>(this string id)
            where TSelf : struct, IIdentity<TSelf>
        {
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out var guid))
            {
                throw new FormatException($"Invalid ID format for type {typeof(TSelf).Name}.");
            }
            return TSelf.From(guid);
        }

        public static bool TryParse<TSelf>(this string value, out TSelf result)
            where TSelf : struct, IIdentity<TSelf>
        {
            if (string.IsNullOrWhiteSpace(value) || !Guid.TryParse(value, out var guid))
            {
                result = default;
                return false;
            }

            result = TSelf.From(guid);
            return true;
        }

        /// <summary>
        /// Tries to parse a string representation of an identifier into an instance of <see cref="IIdentity{TSelf}"/>.
        /// </summary>
        /// <typeparam name="TSelf">The type of the identifier.</typeparam>
        /// <param name="id">The string representation of the identifier.</param>
        /// <returns>An instance of <see cref="TSelf"/>.</returns>
        public static TSelf TryParse<TSelf>(this string id)
            where TSelf : struct, IIdentity<TSelf>
        {
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out var guid))
            {
                return TSelf.New();
            }

            return TSelf.From(guid);
        }
    }
}
