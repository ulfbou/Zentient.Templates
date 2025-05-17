using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

using Template.Domain.Common.Result;

namespace Template.Application.Common.Results
{
    /// <summary>
    /// Represents the result of an operation, indicating success or failure.
    /// </summary>
    public class Result : IResult
    {
        /// <inheritdoc />
        public virtual bool IsSuccess => Errors.Count == 0 && Error is null;

        /// <inheritdoc />
        public virtual bool IsFailure => Errors.Count > 0 || Error is not null;

        /// <inheritdoc />
        public IReadOnlyList<string> Errors { get; }

        /// <inheritdoc />
        public IReadOnlyList<string> Messages { get; }

        /// <inheritdoc />
        public string? Error => Errors.FirstOrDefault();

        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        /// <param name="messages">The success messages associated with the operation.</param>
        /// <param name="errors">The error messages associated with the operation.</param>
        protected Result(IEnumerable<string>? messages = null, IEnumerable<string>? errors = null)
        {
            Messages = messages?.ToImmutableList() ?? ImmutableList<string>.Empty;
            Errors = errors?.ToImmutableList() ?? ImmutableList<string>.Empty;
        }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <param name="message">An optional success message.</param>
        /// <returns>A new instance of the <see cref="Result"/> class representing success.</returns>
        public static IResult Success(string? message = null)
        {
            return new Result(string.IsNullOrWhiteSpace(message) ? Enumerable.Empty<string>() : new List<string> { message });
        }

        /// <summary>
        /// Creates a successful result with a value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="response">The value returned by the operation.</param>
        /// <returns>A new instance of the <typeref name="IResult{T}"/> class representing success.</returns>
        public static IResult<T> Success<T>(T response) => Result<T>.Success(response);

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="error">The error message.</param>
        /// <returns>A new instance of the <see cref="Result"/> class representing failure.</returns>
        public static IResult Failure(string error)
        {
            if (string.IsNullOrWhiteSpace(error))
            {
                throw new ArgumentException("Error message cannot be null or empty.", nameof(error));
            }

            return new Result(new List<string> { error });
        }

        /// <summary>
        /// Creates a failed result with multiple error messages.
        /// </summary>
        /// <param name="errors">The error messages.</param>
        /// <returns>A new instance of the <see cref="Result"/> class representing failure.</returns>
        public static IResult Failure(IEnumerable<string> errors)
        {
            if (errors == null || !errors.Any())
            {
                throw new ArgumentException("Error messages cannot be null or empty.", nameof(errors));
            }

            return new Result(errors);
        }

        /// <summary>
        /// Creates a failed result with a value and an error message.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="error">The error message.</param>
        /// <param name="value">The value associated with the failure.</param>
        public static IResult<T> Failure<T>(T? value, string error) => Result<T>.Failure(value, error);

        /// <summary>
        /// Creates a failed result with a value and errors messages.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="errors">The error messages.</param>
        /// <param name="value">The value associated with the failure.</param>
        public static IResult<T> Failure<T>(T? value, IEnumerable<string> errors) => Result<T>.Failure(value, errors);
    }
}
