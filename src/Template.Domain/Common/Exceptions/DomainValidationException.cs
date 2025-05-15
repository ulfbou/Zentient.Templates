using System;
using System.Collections.Generic;
using System.Linq;

namespace Template.Domain.Common.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when a domain object fails validation.
    /// </summary>
    public class DomainValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainValidationException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public DomainValidationException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainValidationException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public DomainValidationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
