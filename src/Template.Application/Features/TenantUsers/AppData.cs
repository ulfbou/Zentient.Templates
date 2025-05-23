




using Template.Domain.ValueObjects;

using Zentient.Results;

namespace Template.Application.Common
{
    public static partial class AppData
    {
        public static class Entities
        {
            public static Zentient.Results.ErrorInfo MappingFailed(IReadOnlyList<Zentient.Results.ErrorInfo> errorInfos)
                => new Zentient.Results.ErrorInfo(ErrorCategory.General, "MappingFailed", "A mapping operation failed.", errorInfos);

            public static ErrorInfo NotFound<TKey>(TKey id)
                => new ErrorInfo(ErrorCategory.NotFound, "EntityNotFound", $"Entity with ID '{id}' not found.", id);

            public static ErrorInfo CreationFailed(IReadOnlyList<ErrorInfo> errorInfos)
                => new ErrorInfo(ErrorCategory.General, "CreationFailed", "Entity creation failed.", errorInfos ?? []);

            public static ErrorInfo PagedValidationError(string? error, int pageNumber, int pageSize)
                => new ErrorInfo(
                    ErrorCategory.Validation,
                    "PagedValidationError",
                    error ?? "Paged validation error.",
                    new { PageNumber = pageNumber, PageSize = pageSize }
                );

            public static ErrorInfo TenantUserAlreadyExists(string email)
                => new ErrorInfo(
                    ErrorCategory.Conflict,
                    "TenantUserAlreadyExists",
                    string.Format(Messages.TenantUserAlreadyExists, email),
                    new { Email = email }
                );
        }

        public static partial class TenantUsers
        {
            public const string EventDeleteAction = "DeleteAction";
            public const string EventMappingFailed = "MappingFailed";
            public const string EventMappingToDto = "MappingToDto";
        }

        public static partial class Messages
        {
            public const string TenantUserNotFound = "Tenant user with ID {0} not found.";
            public const string TenantUserAlreadyExists = "Tenant user with email '{0}' already exists for this tenant.";

            public const string TenantNameExistsFormat = "Tenant with name '{0}' already exists.";
            public const string TenantNotFound = "Tenant with name '{0}' already exists.";
        }
    }
}
