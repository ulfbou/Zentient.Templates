namespace Template.Domain.Common.Exceptions
{
    /// <summary>
    /// Base class for all domain exceptions.
    /// </summary>
    public class DomainException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a <see cref="null"/> reference if no inner exception is specified.
        /// </param>
        public DomainException(string? message, Exception? innerException = null) : base(message, innerException) { }
    }
}
