using Microsoft.AspNetCore.Http;

using Template.Application.Common.Contracts;

namespace Template.Application.Common.Contexts
{
    public record RequestContext : IRequestContext
    {
        /// <inheritdoc />
        public string? CorrelationId { get; }

        /// <inheritdoc />
        public IDictionary<string, string> Headers { get; }

        /// <inheritdoc />
        public string? Path { get; }

        /// <inheritdoc />
        public string? Method { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestContext"/> struct.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <exception cref="InvalidOperationException">Thrown when the HTTP context is not available.</exception>
        public RequestContext(IHttpContextAccessor httpContextAccessor)
        {
            var httpContext = httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                throw new InvalidOperationException("HttpContext not available. Ensure IHttpContextAccessor is registered.");
            }

            CorrelationId = httpContext.Request.Headers.TryGetValue("X-Correlation-Id", out var cid)
                ? cid.ToString()
                : Guid.NewGuid().ToString();
            Headers = httpContext.Request.Headers.ToDictionary(kv => kv.Key, kv => kv.Value.ToString());
            Path = httpContext.Request.Path.ToString();
            Method = httpContext.Request.Method;
        }
    }
}
