namespace Template.Domain.Common.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when an entity is in an invalid state for a requested operation.
    /// </summary>
    public class InvalidStateException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStateException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public InvalidStateException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStateException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference if no inner exception is specified.
        /// </param>
        public InvalidStateException(string message, Exception innerException) : base(message, innerException) { }
    }
}
