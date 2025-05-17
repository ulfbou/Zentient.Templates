using System.Diagnostics.CodeAnalysis;

using Template.Domain.ValueObjects;

namespace Template.Domain.Common.Result
{
    /// <summary>
    /// Represents the result of an operation that returns a value.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the operation.</typeparam>
    public interface IResult<T> : IResult
    {
        /// <summary>
        /// Gets the value returned by the operation.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        IReadOnlyList<string> Messages { get; }

        /// <summary>
        /// Gets the value of the result if it is successful.
        /// </summary>
        /// <returns>The value of the result.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the result is not successful.</exception>
        T GetValueOrThrow();
    }
}
