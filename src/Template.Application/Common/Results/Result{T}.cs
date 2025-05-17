using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

using Template.Domain.Common.Result;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Template.Application.Common.Results
{
    /// <summary>
    /// Represents the result of an operation that returns a value.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the operation.</typeparam>
    public sealed class Result<T> : Result, IResult<T>
    {
        /// <inheritdoc />
        public T Value { get; }

        /// <inheritdoc />
        public override bool IsSuccess => base.IsSuccess && Value is not null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{T}"/> class.
        /// </summary>
        /// <param name="isSuccess">Indicates whether the operation was successful.</param>
        /// <param name="value">The value returned by the operation.</param>
        /// <param name="errors">The error messages associated with the operation.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the operation is successful, <paramref name="value"/> is <see langword="null"/> and <see cref="T"/> is a non-nullable type.
        /// </exception>
        private Result(T? value, IEnumerable<string>? messages = null, IEnumerable<string>? errors = null) : base(messages, errors)
        {
            if (Errors.Count == 0 && value is null && typeof(T).IsValueType)
            {
                throw new ArgumentNullException(nameof(value), "Value cannot be null for non-nullable types when operation is successful.");
            }

            Value = value!;
        }

        /// <inheritdoc />
        public T GetValueOrThrow()
        {
            if (IsFailure)
            {
                throw new InvalidOperationException("Cannot convert a failed result to a value.");
            }

            return Value;
        }

        /// <summary>
        /// Creates a successful result with a value.
        /// </summary>
        /// <param name="value">The value returned by the operation.</param>
        /// <param name="message">An optional success message.</param>
        /// <returns>A <see cref="Result{T}"/> representing the successful operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/> and <see cref="T"/> is a non-nullable type.</exception>
        public static IResult<T> Success(T? value, string? message = null)
        {
            if (value is null && typeof(T).IsValueType)
            {
                throw new ArgumentNullException(nameof(value), "Value cannot be null for non-nullable types.");
            }

            return new Result<T>(value, messages: string.IsNullOrWhiteSpace(message) ? Enumerable.Empty<string>() : new List<string> { message });
        }

        /// <summary>
        /// Creates a failed result with an error message.
        /// </summary>
        /// <param name="error">The error message.</param>
        /// <returns>A <see cref="Result{T}"/> representing the failed operation.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="error"/> is <see langword="null"/> or empty.</exception>
        public static IResult<T> Failure(T? value, string error)
        {
            if (string.IsNullOrWhiteSpace(error))
            {
                throw new ArgumentException("Error message cannot be null or empty.", nameof(error));
            }

            return new Result<T>(value!, errors: new List<string> { error });
        }


        /// <summary>
        /// Creates a failed result with multiple error messages.
        /// </summary>
        /// <param name="errors">The error messages.</param>
        /// <returns>A <see cref="Result{T}"/> representing the failed operation.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="errors"/> is <see langword="null"/> or empty.</exception>
        public static IResult<T> Failure(T? value, IEnumerable<string> errors)
        {
            if (errors == null || !errors.Any())
            {
                throw new ArgumentException("Error messages cannot be null or empty.", nameof(errors));
            }

            return new Result<T>(value, errors: errors);
        }

        /// <summary>
        /// Implicitly converts a value to a successful <see cref="Result{T}"/>.
        /// </summary>
        [return: NotNullIfNotNull("value")]
        public static implicit operator Result<T>(T value)
        {
            return new Result<T>(value);
        }
    }
}
