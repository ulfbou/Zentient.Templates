namespace Template.Application.Common.Contracts
{
    /// <summary>
    /// Represents the context of a request, including correlation information, headers, path, and method.
    /// </summary>
    public interface IRequestContext
    {
        /// <summary>
        /// Gets the correlation identifier for the request, used for tracking and logging.
        /// </summary>
        string? CorrelationId { get; }

        /// <summary>
        /// Gets the collection of headers associated with the request.
        /// </summary>
        IDictionary<string, string> Headers { get; }

        /// <summary>
        /// Gets the path of the request, typically representing the endpoint or resource.
        /// </summary>
        string? Path { get; }

        /// <summary>
        /// Gets the HTTP method of the request (e.g., GET, POST, PUT, DELETE).
        /// </summary>
        string? Method { get; }
    }
}