using Zentient.Results;

namespace Template.Application.Common
{
    /// <summary>Application-wide constants and helpers for Tenant features.</summary>
    public static partial class AppData
    {
        /// <summary>Constants and helpers related to tenants.</summary>
        public static partial class Tenants
        {
            /// <summary>Formatting helpers for tenants (reserved for future use).</summary>
            public static partial class Format { }

            /// <summary>Event name for tenant creation.</summary>
            public const string TenantCreated = "Tenant.Created";

            /// <summary>Event name for tenant update.</summary>
            public const string TenantUpdated = "Tenant.Updated";

            /// <summary>Event name for tenant deletion.</summary>
            public const string TenantDeleted = "Tenant.Deleted";

            /// <summary>Event name for tenant restoration.</summary>
            public const string TenantRestored = "Tenant.Restored";

            /// <summary>Validation failed event name.</summary>
            public const string ValidationFailed = "Tenant.ValidationFailed";

            /// <summary>Error code for tenant not found.</summary>
            public const string ErrorTenantNotFound = "Tenant.NotFound";

            /// <summary>Error code for tenant already exists.</summary>
            public const string ErrorTenantExists = "Tenant.Exists";

            /// <summary>Event name for tenant creation started.</summary>
            public const string EventTenantCreationStarted = "TenantCreationStarted";

            /// <summary>Event name for tenant creation succeeded.</summary>
            public const string EventTenantCreationSucceeded = "TenantCreationSucceeded";

            /// <summary>Event name for tenant creation failed.</summary>
            public const string EventTenantCreationFailed = "TenantCreationFailed";

            /// <summary>Event name for tenant update started.</summary>
            public const string EventTenantUpdateStarted = "TenantUpdateStarted";

            /// <summary>Event name for tenant update succeeded.</summary>
            public const string EventTenantUpdateSucceeded = "TenantUpdateSucceeded";

            /// <summary>Event name for tenant update failed.</summary>
            public const string EventTenantUpdateFailed = "TenantUpdateFailed";

            /// <summary>Event name for tenant deletion started.</summary>
            public const string EventTenantDeletionStarted = "TenantDeletionStarted";

            /// <summary>Event name for tenant deletion succeeded.</summary>
            public const string EventTenantDeletionSucceeded = "TenantDeletionSucceeded";

            /// <summary>Event name for tenant deletion failed.</summary>
            public const string EventTenantDeletionFailed = "TenantDeletionFailed";

            /// <summary>Event name for tenant restoration started.</summary>
            public const string EventTenantRestorationStarted = "TenantRestorationStarted";

            /// <summary>Event name for tenant restoration succeeded.</summary>
            public const string EventTenantRestorationSucceeded = "TenantRestorationSucceeded";

            /// <summary>Event name for tenant restoration failed.</summary>
            public const string EventTenantRestorationFailed = "TenantRestorationFailed";

            /// <summary>Tag for exception type in telemetry.</summary>
            public const string TagExceptionType = "exception.type";

            /// <summary>Tag for exception message in telemetry.</summary>
            public const string TagExceptionMessage = "exception.message";

            /// <summary>Tag for tenant name in telemetry.</summary>
            public const string TagTenantName = "tenant.name";

            /// <summary>Success message for tenant creation.</summary>
            public const string SuccessTenantCreated = "Tenant '{0}' created successfully.";

            /// <summary>Error code for tenant mapping failure.</summary>
            public const string ErrorMappingFailed = "Tenant.MappingFailed";

            /// <summary>Error code for tenant operation failure.</summary>
            public const string ErrorOperationFailed = "Tenant.MappingFailed";

            /// <summary>Creates an <see cref="ErrorInfo"/> for when a tenant name already exists.</summary>
            /// <param name="name">The name of the tenant that already exists.</param>
            /// <returns>An <see cref="ErrorInfo"/> instance.</returns>
            public static Zentient.Results.ErrorInfo TenantNameExistsErrorInfo(string name)
                => new Zentient.Results.ErrorInfo(
                    ErrorCategory.Conflict,
                    ErrorTenantExists,
                    string.Format(AppData.Messages.TenantNameExistsFormat, name),
                    new { TenantName = name }
                );

            /// <summary>Creates an <see cref="ErrorInfo"/> for when a tenant is not found.</summary>
            /// <returns>An <see cref="ErrorInfo"/> instance.</returns>
            public static Zentient.Results.ErrorInfo TenantNotFoundErrorInfo()
                => new Zentient.Results.ErrorInfo(
                    Zentient.Results.ErrorCategory.NotFound,
                    ErrorTenantNotFound,
                    AppData.Messages.TenantNotFound
                );

            /// <summary>Creates an <see cref="ErrorInfo"/> for a general validation failure.</summary>
            /// <param name="message">The validation error message.</param>
            /// <param name="data">Optional additional data related to the validation error.</param>
            /// <returns>An <see cref="ErrorInfo"/> instance.</returns>
            public static Zentient.Results.ErrorInfo ValidationFailedErrorInfo(string message, object? data = null)
                => new Zentient.Results.ErrorInfo(
                    Category: Zentient.Results.ErrorCategory.Validation,
                    ValidationFailed,
                    message,
                    data
                );

            /// <summary>Creates an <see cref="ErrorInfo"/> for a mapping failure.</summary>
            /// <param name="message">The mapping error message.</param>
            /// <returns>An <see cref="ErrorInfo"/> instance.</returns>
            public static Zentient.Results.ErrorInfo MappingFailedErrorInfo(string message)
                => new Zentient.Results.ErrorInfo(
                    Zentient.Results.ErrorCategory.General,
                    ErrorMappingFailed,
                    message
                );

            /// <summary>Creates an <see cref="ErrorInfo"/> for a general operation failure.</summary>
            /// <param name="message">The operation error message.</param>
            /// <returns>An <see cref="ErrorInfo"/> instance.</returns>
            public static Zentient.Results.ErrorInfo OperationFailedErrorInfo(string message)
                => new Zentient.Results.ErrorInfo(
                    Zentient.Results.ErrorCategory.General,
                    ErrorOperationFailed,
                    message
                );

            /// <summary>Creates an <see cref="ErrorInfo"/> from an exception.</summary>
            /// <param name="ex">The exception that occurred.</param>
            /// <returns>An <see cref="ErrorInfo"/> instance.</returns>
            public static Zentient.Results.ErrorInfo FromExceptionErrorInfo(Exception ex)
                => new Zentient.Results.ErrorInfo(
                    Zentient.Results.ErrorCategory.Exception,
                    ex.GetType().Name,
                    ex.Message,
                    ex
                );

            /// <summary>Creates a failure result indicating that a tenant name already exists.</summary>
            /// <param name="name">The name of the tenant that already exists.</param>
            /// <returns>An <see cref="IResult"/> indicating failure.</returns>
            public static Zentient.Results.IResult TenantNameAlreadyExistsError(string name)
                => Zentient.Results.Result.Failure(
                    new[]
                    {
                        TenantNameExistsErrorInfo(name),
                        ValidationFailedErrorInfo(string.Format(Messages.TenantNameExistsFormat, name))
                    },
                    Zentient.Results.ResultStatuses.Conflict
                );

            /// <summary>Creates a failure result indicating that a tenant name already exists.</summary>
            /// <param name="name">The name of the tenant that already exists.</param>
            /// <returns>An <see cref="IResult"/> indicating failure.</returns>
            public static IResult ErrorTenantNameAlreadyExists(string name)
                => Result.Failure(
                    new[]
                    {
                        TenantNameExistsErrorInfo(name),
                        ValidationFailedErrorInfo(string.Format(Messages.TenantNameExistsFormat, name))
                    },
                    ResultStatuses.Conflict
                );

            /// <summary>Message constants for tenants.</summary>
            public static partial class Messages
            {
                /// <summary>Format string for tenant name already exists message.</summary>
                public const string TenantNameExistsFormat = "Tenant with the name '{0}' already exists.";

                /// <summary>Message for tenant not found.</summary>
                public const string TenantNotFound = "Tenant not found.";

                /// <summary>Log message for tenant not found.</summary>
                public const string TenantNotFoundLog = "Tenant with ID {TenantId} not found.";

                /// <summary>Format string for tenant delete error.</summary>
                public const string TenantDeleteError = "Failed to delete tenant: {0}";

                /// <summary>Log message for tenant delete error.</summary>
                public const string TenantDeleteErrorLog = "Error occurred while deleting tenant with ID {TenantId}.";

                /// <summary>Format string for tenant restore error.</summary>
                public const string TenantRestoreError = "Failed to restore tenant: {0}";

                /// <summary>Log message for tenant restore error.</summary>
                public const string TenantRestoreErrorLog = "Error occurred while restoring tenant with ID {TenantId}.";
            }
        }
    }
}
