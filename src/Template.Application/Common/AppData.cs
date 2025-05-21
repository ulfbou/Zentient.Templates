namespace Template.Application.Common;

public static partial class AppData
{
    public static partial class Activity
    {
        public const string SuffixCreate = ".Create";
        public const string SuffixHandle = ".Handle";
        public const string SuffixQuery = ".Query";
        public const string SuffixPagedQuery = ".PagedQuery";

        public const string EventQueryExecutionStarted = "QueryExecutionStarted";
        public const string EventQueryExecutionSucceeded = "QueryExecutionSucceeded";
        public const string EventQueryExecutionFailed = "QueryExecutionFailed";

        public const string EventExecutionStarted = "ExecutionStarted";
        public const string EventExecutionSucceeded = "ExecutionSucceeded";
        public const string EventExecutionFailed = "ExecutionFailed";

        public const string EventGetEntityId = "GetEntityId";
        public const string EventFetchEntity = "FetchEntity";
        public const string EventFetchFailed = "FetchFailed";
        public const string EventEntityNotFound = "EntityNotFound";
        public const string EventUpdateAction = "UpdateAction";
        public const string EventPersistUpdate = "PersistUpdate";
        public const string EventUpdateFailed = "UpdateFailed";
        public const string EventUpdateSucceeded = "UpdateSucceeded";
        public const string EventMappingToEntity = "MappingToEntity";
        public const string EventAddingToContext = "AddingToContext";
        public const string EventCreationFailed = "CreationFailed";
        public const string EventPostCreationAction = "PostCreationAction";
        public const string EventMappingToResponse = "MappingToResponse";
        public const string EventCreationSucceeded = "CreationSucceeded";

        public const string EventPagedQueryValidationFailed = "PagedQueryValidationFailed";
        public const string EventPagedQueryExecutionStarted = "PagedQueryExecutionStarted";
        public const string EventPagedQueryExecutionSucceeded = "PagedQueryExecutionSucceeded";
        public const string EventPagedQueryExecutionFailed = "PagedQueryExecutionFailed";

        public const string TagError = "error";
        public const string TagId = "id";
        public const string TagRequestType = "request.type";
        public const string TagUserId = "user.id";
        public const string TagCorrelationId = "correlation.id";
        public const string TagExceptionType = "exception.type";
        public const string TagExceptionMessage = "exception.message";
        public const string TagExceptionStackTrace = "exception.stacktrace";
        public const string TagQueryType = "query.type";
    }

    public static partial class Messages
    {
        public const string EntityNotFoundFormat = "{0} with ID {1} not found.";
    }

    public static partial class Paging
    {
        public const string PageNumber = "paging.pageNumber";
        public const string PageSize = "paging.pageSize";
        public const string TotalCount = "paging.totalCount";
    }
}
