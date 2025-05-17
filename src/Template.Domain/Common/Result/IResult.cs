
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Template.Domain.Common.Result
{
    /// <summary>
    /// Interface for a result that does not return a value.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// Gets a value indicating whether the operation failed.
        /// </summary>
        bool IsFailure { get; }

        /// <summary>
        /// Gets the error messages associated with the operation.
        /// </summary>
        IReadOnlyList<string> Errors { get; }

        /// <summary>
        /// Gets the first error message, if any.
        /// </summary>
        string? Error { get; }
    }
}
