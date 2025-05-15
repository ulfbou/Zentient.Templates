using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Template.Domain.Common
{
    /// <summary>
    /// Represents the result of an operation that returns a value.
    /// </summary>
    /// typeparam name="T">The type of the value returned by the operation.</typeparam>
    public sealed class Result<T> : Result
    {
        /// <summary>
        /// Gets the value returned by the operation.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{T}"/> class.
        /// </summary>
        /// <param name="isSuccess">Indicates whether the operation was successful.</param>
        /// <param name="value">The value returned by the operation.</param>
        /// <param name="errors">The error messages associated with the operation.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/> and <paramref name="isSuccess"/> is <see langword="true"/>.
        /// </exception>
        private Result(bool isSuccess, T value, IEnumerable<string> errors) : base(isSuccess, errors)
        {
            if (isSuccess && value is null)
            {
                throw new ArgumentNullException(nameof(value), "Value cannot be null when the operation is successful.");
            }

            Value = value;
        }

        /// <summary>
        /// Creates a successful result with a value.
        /// </summary>
        /// <param name="value">The value returned by the operation.</param>
        /// <param name="message">An optional success message.</param>
        /// <returns>A <see cref="Result{T}"/> representing the successful operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/> and <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        public static Result<T> Success(T value, string? message = null)
        {
            return new Result<T>(true, value, string.IsNullOrWhiteSpace(message) ? Enumerable.Empty<string>() : new List<string> { message });
        }

        /// <summary>
        /// Creates a failed result with an error message.
        /// </summary>
        /// <param name="error">The error message.</param>
        /// <returns>A <see cref="Result{T}"/> representing the failed operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/> and <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        public new static Result<T> Failure(string error)
        {
            return new Result<T>(false, default!, new List<string> { error });
        }

        /// <summary>
        /// Creates a failed result with multiple error messages.
        /// </summary>
        /// <param name="errors">The error messages.</param>
        /// <returns>A <see cref="Result{T}"/> representing the failed operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/> and <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        public new static Result<T> Failure(IEnumerable<string> errors)
        {
            return new Result<T>(false, default!, errors);
        }

        /// <summary>
        /// Implicitly converts a <see cref="Result{T}"/> to a <see cref="Result"/>.
        /// </summary>
        [return: NotNullIfNotNull("value")]
        public static implicit operator Result<T>(T value)
        {
            return value is null ? Failure("Value is null") : Success(value);
        }

        /// <summary>
        /// Gets the value of the result if it is successful.
        /// </summary>
        /// <returns>The value of the result.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the result is not successful.</exception>
        public T GetValueOrThrow()
        {
            if (IsFailure)
            {
                throw new InvalidOperationException("Cannot convert a failed result to a value.");
            }

            return Value;
        }
    }
}
