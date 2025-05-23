namespace Template.Application.Common
{
    /// <summary>Application-wide constants and helpers.</summary>
    public static partial class AppData
    {
        /// <summary>The application name.</summary>
        public const string AppName = "Template.Application";

        /// <summary>Constants and helpers for activity tracing and telemetry.</summary>
        public static partial class Activity
        {
            /// <summary>Activity source name for behaviors.</summary>
            public const string AppBehaviors = "Application.Behaviors";

            /// <summary>Activity source name for commands.</summary>
            public const string AppCommand = "Application.Command";

            /// <summary>Activity source name for queries.</summary>
            public const string AppQuery = "Application.Query";

            /// <summary>Activity source name for events.</summary>
            public const string AppEvent = "Application.Event";

            /// <summary>Activity source name for handlers.</summary>
            public const string AppHandler = "Application.Handler";

            /// <summary>Suffix for create activities.</summary>
            public const string SuffixCreate = ".Create";

            /// <summary>Suffix for handle activities.</summary>
            public const string SuffixHandle = ".Handle";

            /// <summary>Suffix for query activities.</summary>
            public const string SuffixQuery = ".Query";

            /// <summary>Suffix for paged query activities.</summary>
            public const string SuffixPagedQuery = ".PagedQuery";

            /// <summary>Event for handling started.</summary>
            public const string EventHandlingStarted = "HandlingStarted";

            /// <summary>Event for handling completed.</summary>
            public const string EventHandlingCompleted = "HandlingCompleted";

            /// <summary>Event for handling failed.</summary>
            public const string EventHandlingFailed = "HandlingFailed";

            /// <summary>Log message for error during request handling.</summary>
            public const string LogErrorHandlingRequest = "An error occurred while handling the request: {Request}";

            /// <summary>Event for query execution started.</summary>
            public const string EventQueryExecutionStarted = "QueryExecutionStarted";

            /// <summary>Event for query execution succeeded.</summary>
            public const string EventQueryExecutionSucceeded = "QueryExecutionSucceeded";

            /// <summary>Event for query execution failed.</summary>
            public const string EventQueryExecutionFailed = "QueryExecutionFailed";

            /// <summary>Event for execution started.</summary>
            public const string EventExecutionStarted = "ExecutionStarted";

            /// <summary>Event for execution succeeded.</summary>
            public const string EventExecutionSucceeded = "ExecutionSucceeded";

            /// <summary>Event for execution failed.</summary>
            public const string EventExecutionFailed = "ExecutionFailed";

            /// <summary>Event for getting entity ID.</summary>
            public const string EventGetEntityId = "GetEntityId";

            /// <summary>Event for fetching an entity.</summary>
            public const string EventFetchEntity = "FetchEntity";

            /// <summary>Event for fetch failure.</summary>
            public const string EventFetchFailed = "FetchFailed";

            /// <summary>Event for entity not found.</summary>
            public const string EventEntityNotFound = "EntityNotFound";

            /// <summary>Event for update action.</summary>
            public const string EventUpdateAction = "UpdateAction";

            /// <summary>Event for persisting update.</summary>
            public const string EventPersistUpdate = "PersistUpdate";

            /// <summary>Event for update failure.</summary>
            public const string EventUpdateFailed = "UpdateFailed";

            /// <summary>Event for update success.</summary>
            public const string EventUpdateSucceeded = "UpdateSucceeded";

            /// <summary>Event for mapping to entity.</summary>
            public const string EventMappingToEntity = "MappingToEntity";

            /// <summary>Event for adding to context.</summary>
            public const string EventAddingToContext = "AddingToContext";

            /// <summary>Event for creation failure.</summary>
            public const string EventCreationFailed = "CreationFailed";

            /// <summary>Event for post-creation action.</summary>
            public const string EventPostCreationAction = "PostCreationAction";

            /// <summary>Event for mapping to response.</summary>
            public const string EventMappingToResponse = "MappingToResponse";

            /// <summary>Event for mapping to DTO.</summary>
            public const string EventMappingToDto = "MappingToDto";

            /// <summary>Event for mapping failure.</summary>
            public const string EventMappingFailed = "MappingFailed";

            /// <summary>Event for creation success.</summary>
            public const string EventCreationSucceeded = "CreationSucceeded";

            /// <summary>Event for paged query validation failure.</summary>
            public const string EventPagedQueryValidationFailed = "PagedQueryValidationFailed";

            /// <summary>Event for paged query execution started.</summary>
            public const string EventPagedQueryExecutionStarted = "PagedQueryExecutionStarted";

            /// <summary>Event for paged query execution succeeded.</summary>
            public const string EventPagedQueryExecutionSucceeded = "PagedQueryExecutionSucceeded";

            /// <summary>Event for paged query execution failed.</summary>
            public const string EventPagedQueryExecutionFailed = "PagedQueryExecutionFailed";

            /// <summary>Tag for error information.</summary>
            public const string TagError = "error";

            /// <summary>Tag for entity ID.</summary>
            public const string TagId = "id";

            /// <summary>Tag for request type.</summary>
            public const string TagRequestType = "request.type";

            /// <summary>Tag for user ID.</summary>
            public const string TagUserId = "user.id";

            /// <summary>Tag for user name.</summary>
            public const string TagUserName = "user.name";

            /// <summary>Tag for user email.</summary>
            public const string TagUserEmail = "user.email";

            /// <summary>Tag for user roles.</summary>
            public const string TagUserRoles = "user.roles";

            /// <summary>Tag for tenant ID.</summary>
            public const string TagTenantId = "tenant.id";

            /// <summary>Tag for tenant name.</summary>
            public const string TagTenantName = "tenant.name";

            /// <summary>Tag for correlation ID.</summary>
            public const string TagCorrelationId = "correlation.id";

            /// <summary>Tag for exception type.</summary>
            public const string TagExceptionType = "exception.type";

            /// <summary>Tag for exception message.</summary>
            public const string TagExceptionMessage = "exception.message";

            /// <summary>Tag for exception stack trace.</summary>
            public const string TagExceptionStackTrace = "exception.stacktrace";

            /// <summary>Tag for query type.</summary>
            public const string TagQueryType = "query.type";

            /// <summary>Event for query validation failure.</summary>
            public const string EventQueryValidationFailed = "QueryValidationFailed";
        }

        /// <summary>General message constants.</summary>
        public static partial class Messages
        {
            /// <summary>Format string for entity not found messages.</summary>
            public const string EntityNotFoundFormat = "{0} with ID {1} not found.";

            /// <summary>Message for query validation failure.</summary>
            public const string QueryValidationFailed = "Query validation failed.";
        }

        /// <summary>Paging-related constants.</summary>
        public static partial class Paging
        {
            /// <summary>Paging: page number key.</summary>
            public const string PageNumber = "paging.pageNumber";

            /// <summary>Paging: page size key.</summary>
            public const string PageSize = "paging.pageSize";

            /// <summary>Paging: total count key.</summary>
            public const string TotalCount = "paging.totalCount";
        }
    }
}
