using Zentient.Results;

namespace Template.Application.Common
{
    public static partial class AppData
    {
        public static partial class Tenants
        {
            public static partial class Format
            {

            }
            public const string TenantCreated = "Tenant.Created";
            public const string TenantUpdated = "Tenant.Updated";
            public const string TenantDeleted = "Tenant.Deleted";
            public const string TenantRestored = "Tenant.Restored";

            public const string ValidationFailed = "Tenant.ValidationFailed";

            public const string ErrorTenantNotFound = "Tenant.NotFound";
            public const string ErrorTenantExists = "Tenant.Exists";

            public const string EventTenantCreationStarted = "TenantCreationStarted";
            public const string EventTenantCreationSucceeded = "TenantCreationSucceeded";
            public const string EventTenantCreationFailed = "TenantCreationFailed";

            public const string EventTenantUpdateStarted = "TenantUpdateStarted";
            public const string EventTenantUpdateSucceeded = "TenantUpdateSucceeded";
            public const string EventTenantUpdateFailed = "TenantUpdateFailed";

            public const string EventTenantDeletionStarted = "TenantDeletionStarted";
            public const string EventTenantDeletionSucceeded = "TenantDeletionSucceeded";
            public const string EventTenantDeletionFailed = "TenantDeletionFailed";

            public const string EventTenantRestorationStarted = "TenantRestorationStarted";
            public const string EventTenantRestorationSucceeded = "TenantRestorationSucceeded";
            public const string EventTenantRestorationFailed = "TenantRestorationFailed";

            public const string TagExceptionType = "exception.type";
            public const string TagExceptionMessage = "exception.message";

            public const string TagTenantName = "tenant.name";

            public const string SuccessTenantCreated = "Tenant '{0}' created successfully.";

            public const string ErrorMappingFailed = "Tenant.MappingFailed";
            public const string ErrorOperationFailed = "Tenant.MappingFailed";

            /// <summary>
            /// Creates an ErrorInfo for when a tenant name already exists.
            /// </summary>
            /// <param name="name">The name of the tenant that already exists.</param>
            /// <returns>An ErrorInfo instance.</returns>
            public static Zentient.Results.ErrorInfo TenantNameExistsErrorInfo(string name)
                => new Zentient.Results.ErrorInfo(
                    ErrorCategory.Conflict,
                    ErrorTenantExists,
                    string.Format(AppData.Messages.TenantNameExistsFormat, name),
                    new { TenantName = name }
                );

            /// <summary>
            /// Creates an ErrorInfo for when a tenant is not found.
            /// </summary>
            /// <returns>An ErrorInfo instance.</returns>
            public static Zentient.Results.ErrorInfo TenantNotFoundErrorInfo()
                => new Zentient.Results.ErrorInfo(
                    Zentient.Results.ErrorCategory.NotFound,
                    ErrorTenantNotFound,
                    AppData.Messages.TenantNotFound
                );

            /// <summary>
            /// Creates an ErrorInfo for a general validation failure.
            /// </summary>
            /// <param name="message">The validation error message.</param>
            /// <param name="data">Optional additional data related to the validation error.</param>
            /// <returns>An ErrorInfo instance.</returns>
            public static Zentient.Results.ErrorInfo ValidationFailedErrorInfo(string message, object? data = null)
                => new Zentient.Results.ErrorInfo(
                    Category: Zentient.Results.ErrorCategory.Validation,
                    ValidationFailed,
                    message,
                    data
                );

            /// <summary>
            /// Creates an ErrorInfo for a mapping failure.
            /// </summary>
            /// <param name="message">The mapping error message.</param>
            /// <returns>An ErrorInfo instance.</returns>
            public static Zentient.Results.ErrorInfo MappingFailedErrorInfo(string message)
                => new Zentient.Results.ErrorInfo(
                    Zentient.Results.ErrorCategory.General, // Or create a new ErrorCategory.Mapping
                    ErrorMappingFailed,
                    message
                );

            /// <summary>
            /// Creates an ErrorInfo for a general operation failure.
            /// </summary>
            /// <param name="message">The operation error message.</param>
            /// <returns>An ErrorInfo instance.</returns>
            public static Zentient.Results.ErrorInfo OperationFailedErrorInfo(string message)
                => new Zentient.Results.ErrorInfo(
                    Zentient.Results.ErrorCategory.General,
                    ErrorOperationFailed,
                    message
                );

            /// <summary>
            /// Creates an ErrorInfo from an exception.
            /// </summary>
            /// <param name="ex">The exception that occurred.</param>
            /// <returns>An ErrorInfo instance.</returns>
            public static Zentient.Results.ErrorInfo FromExceptionErrorInfo(Exception ex)
                => new Zentient.Results.ErrorInfo(
                    Zentient.Results.ErrorCategory.Exception,
                    ex.GetType().Name,
                    ex.Message,
                    ex
                );

            // --- Existing IResult Factory Method (updated to use new ErrorInfo factories) ---

            /// <summary>
            /// Creates a failure result indicating that a tenant name already exists.
            /// </summary>
            /// <param name="name">The name of the tenant that already exists.</param>
            /// <returns>An IResult indicating failure.</returns>
            public static Zentient.Results.IResult TenantNameAlreadyExistsError(string name)
                => Zentient.Results.Result.Failure(
                    new[]
                    {
                        TenantNameExistsErrorInfo(name),
                        ValidationFailedErrorInfo(string.Format(Messages.TenantNameExistsFormat, name))
                    },
                    Zentient.Results.ResultStatuses.Conflict
                );

            internal static IResult ErrorTenantNameAlreadyExists(string name) => throw new NotImplementedException();

            public static partial class Messages
            {
                public const string TenantNameExistsFormat = "Tenant with the name '{0}' already exists.";
                public const string TenantNotFound = "Tenant not found.";
                public const string TenantNotFoundLog = "Tenant with ID {TenantId} not found.";
                public const string TenantDeleteError = "Failed to delete tenant: {0}";
                public const string TenantDeleteErrorLog = "Error occurred while deleting tenant with ID {TenantId}.";
                public const string TenantRestoreError = "Failed to restore tenant: {0}";
                public const string TenantRestoreErrorLog = "Error occurred while restoring tenant with ID {TenantId}.";
            }
        }
    }
}
