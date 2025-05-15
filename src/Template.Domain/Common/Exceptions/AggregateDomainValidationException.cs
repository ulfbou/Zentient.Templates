namespace Template.Domain.Common.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when a domain object fails validation and has multiple validation errors.
    /// </summary>
    public class AggregateDomainValidationException : DomainValidationException
    {
        /// <summary>
        /// Gets the collection of validation errors.
        /// </summary>
        public IReadOnlyCollection<string> Errors { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateDomainValidationException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="errors">The collection of validation error messages.</param>
        public AggregateDomainValidationException(string message, IEnumerable<string> errors)
            : base(message)
        {
            Errors = errors?.ToList().AsReadOnly() ?? throw new ArgumentNullException(nameof(errors));
            if (!Errors.Any())
            {
                throw new ArgumentException("Errors collection cannot be empty.", nameof(errors));
            }
        }

        ///  <inheritdoc />
        public AggregateDomainValidationException(string message, IEnumerable<string> errors, Exception innerException) : base(message, innerException)
        {
            Errors = errors?.ToList().AsReadOnly() ?? throw new ArgumentNullException(nameof(errors));
            if (!Errors.Any())
            {
                throw new ArgumentException("Errors collection cannot be empty.", nameof(errors));
            }
        }
    }
}
