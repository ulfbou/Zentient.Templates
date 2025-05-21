using Microsoft.AspNetCore.Http;

using System.Security.Claims;

using Template.Application.Common.Contracts;
using Template.Domain.ValueObjects;

namespace Template.Application.Common.Contexts
{
    /// <summary>
    /// Provides access to the current user's context, including user and tenant identifiers, roles, and claims.
    /// </summary>
    public record UserContext : IUserContext
    {
        /// <inheritdoc />
        public TenantId TenantId { get; }

        /// <inheritdoc />
        public UserId UserId { get; }

        /// <inheritdoc />
        public IReadOnlyList<string> Roles { get; }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, string> Claims { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserContext"/> struct using the specified HTTP context accessor.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor to retrieve the current user principal.</param>
        /// <exception cref="UnauthorizedAccessException">Thrown if the user is not authenticated.</exception>
        /// <exception cref="InvalidOperationException">Thrown if required claims are missing or invalid.</exception>
        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext?.User;

            if (user == null || user.Identity?.IsAuthenticated != true)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            UserId = Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, nameof(UserId));
            TenantId = Parse(user.FindFirst(nameof(TenantId))?.Value, nameof(TenantId));
            Roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            Claims = user.Claims.ToDictionary(c => c.Type, c => c.Value);
        }

        /// <summary>
        /// Parses a string value into a <see cref="Guid"/> for claim validation.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <param name="claimName">The name of the claim being parsed.</param>
        /// <returns>The parsed <see cref="Guid"/> value.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the value is null, empty, or not a valid <see cref="Guid"/>.</exception>
        private Guid Parse(string? value, string claimName)
        {
            if (string.IsNullOrEmpty(value) || !Guid.TryParse(value, out var result))
            {
                throw new InvalidOperationException($"{claimName} claim missing");
            }

            return result;
        }
    }
}
