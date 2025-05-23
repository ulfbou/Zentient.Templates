

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
            public static ErrorInfo? NotFound<TKey>(TKey id)
                => new ErrorInfo(ErrorCategory.NotFound, "EntityNotFound", $"Entity with ID '{id}' not found.", id);
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

            public static IFormatProvider? TenantNameExistsFormat { get; internal set; }
            public static string TenantNotFound { get; internal set; }
        }
    }
}
