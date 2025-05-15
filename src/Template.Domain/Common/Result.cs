using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Template.Domain.Common
{
    /// <summary>
    /// Represents the result of an operation, indicating success or failure.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets a value indicating whether the operation failed.
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Gets the error messages associated with the operation.
        /// </summary>
        public IReadOnlyList<string> Errors { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        protected Result(bool isSuccess, IEnumerable<string> errors)
        {
            IsSuccess = isSuccess;
            Errors = errors?.ToImmutableList() ?? ImmutableList<string>.Empty;
        }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <param name="message">An optional success message.</param>
        /// <returns>A new instance of the <see cref="Result"/> class representing success.</returns>
        public static Result Success(string? message = null)
        {
            return new Result(true, string.IsNullOrWhiteSpace(message) ? Enumerable.Empty<string>() : new List<string> { message });
        }

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="error">The error message.</param>
        /// <returns>A new instance of the <see cref="Result"/> class representing failure.</returns>
        public static Result Failure(string error)
        {
            if (string.IsNullOrWhiteSpace(error))
            {
                throw new ArgumentException("Error message cannot be null or empty.", nameof(error));
            }

            return new Result(false, new List<string> { error });
        }

        /// <summary>
        /// Creates a failed result with multiple error messages.
        /// </summary>
        /// <param name="errors">The error messages.</param>
        /// <returns>A new instance of the <see cref="Result"/> class representing failure.</returns>
        public static Result Failure(IEnumerable<string> errors)
        {
            return new Result(false, errors);
        }
    }
}
