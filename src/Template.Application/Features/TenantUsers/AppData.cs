using FluentValidation.Results;

using Template.Domain.ValueObjects;

using Zentient.Results;

namespace Template.Application.Common
{
    /// <summary>Application-wide constants and helpers for TenantUser features.</summary>
    public static partial class AppData
    {
        public static partial class Validation
        {
            public static ErrorInfo ValidationFailed(IEnumerable<ValidationFailure> enumerable)
                => new ErrorInfo(
                    ErrorCategory.Validation,
                    "ValidationFailed",
                    "Validation failed.",
                    enumerable.Select(e => new ErrorInfo(ErrorCategory.Validation, e.ErrorCode, e.ErrorMessage, e.AttemptedValue))
                );
        }

        /// <summary>Provides factory methods for common <see cref="ErrorInfo"/> instances related to entities.</summary>
        public static class Entities
        {
            /// <summary>Creates an <see cref="ErrorInfo"/> representing a mapping failure.</summary>
            /// <param name="errorInfos">The list of underlying error infos.</param>
            /// <returns>An <see cref="ErrorInfo"/> for a mapping failure.</returns>
            public static Zentient.Results.ErrorInfo MappingFailed(IReadOnlyList<Zentient.Results.ErrorInfo> errorInfos)
                => new Zentient.Results.ErrorInfo(ErrorCategory.General, "MappingFailed", "A mapping operation failed.", errorInfos);

            /// <summary>Creates an <see cref="ErrorInfo"/> representing an entity not found.</summary>
            /// <typeparam name="TKey">The type of the entity key.</typeparam>
            /// <param name="id">The entity identifier.</param>
            /// <returns>An <see cref="ErrorInfo"/> for not found.</returns>
            public static ErrorInfo NotFound<TKey>(TKey id)
                => new ErrorInfo(ErrorCategory.NotFound, "EntityNotFound", $"Entity with ID '{id}' not found.", id);

            /// <summary>Creates an <see cref="ErrorInfo"/> representing a creation failure.</summary>
            /// <param name="errorInfos">The list of underlying error infos.</param>
            /// <returns>An <see cref="ErrorInfo"/> for a creation failure.</returns>
            public static ErrorInfo CreationFailed(IReadOnlyList<ErrorInfo> errorInfos)
                => new ErrorInfo(ErrorCategory.General, "CreationFailed", "Entity creation failed.", errorInfos ?? []);

            /// <summary>Creates an <see cref="ErrorInfo"/> for a paged validation error.</summary>
            /// <param name="error">The error message.</param>
            /// <param name="pageNumber">The page number.</param>
            /// <param name="pageSize">The page size.</param>
            /// <returns>An <see cref="ErrorInfo"/> for paged validation error.</returns>
            public static ErrorInfo PagedValidationError(string? error, int pageNumber, int pageSize)
                => new ErrorInfo(
                    ErrorCategory.Validation,
                    "PagedValidationError",
                    error ?? "Paged validation error.",
                    new { PageNumber = pageNumber, PageSize = pageSize }
                );

            /// <summary>Creates an <see cref="ErrorInfo"/> indicating a tenant user already exists for the given email.</summary>
            /// <param name="email">The email address.</param>
            /// <returns>An <see cref="ErrorInfo"/> for a duplicate tenant user.</returns>
            public static ErrorInfo TenantUserAlreadyExists(string email)
                => new ErrorInfo(
                    ErrorCategory.Conflict,
                    "TenantUserAlreadyExists",
                    string.Format(Messages.TenantUserAlreadyExists, email),
                    new { Email = email }
                );
        }

        /// <summary>Activity/event constants for TenantUser operations.</summary>
        public static partial class TenantUsers
        {
            /// <summary>Activity event for a delete action.</summary>
            public const string EventDeleteAction = "DeleteAction";

            /// <summary>Activity event for a mapping failure.</summary>
            public const string EventMappingFailed = "MappingFailed";

            /// <summary>Activity event for mapping to a DTO.</summary>
            public const string EventMappingToDto = "MappingToDto";
        }

        /// <summary>Message constants for TenantUser operations.</summary>
        public static partial class Messages
        {
            /// <summary>Message for a tenant user not found.</summary>
            public const string TenantUserNotFound = "Tenant user with ID {0} not found.";

            /// <summary>Message for a tenant user already existing for a tenant.</summary>
            public const string TenantUserAlreadyExists = "Tenant user with email '{0}' already exists for this tenant.";

            /// <summary>Message for a tenant name already existing.</summary>
            public const string TenantNameExistsFormat = "Tenant with name '{0}' already exists.";

            /// <summary>Message for a tenant not found.</summary>
            public const string TenantNotFound = "Tenant with name '{0}' already exists.";
        }
    }
}
