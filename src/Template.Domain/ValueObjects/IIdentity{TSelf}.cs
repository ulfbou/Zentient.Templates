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
    { }
}
