using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using System.Diagnostics;

using Template.Application.Common.Contracts;
using Template.Application.Common.Handlers;
using Template.Application.Common.Results;
using Template.Domain.Common.Result;
using Template.Domain.Contracts;
using Template.Domain.ValueObjects;

namespace Template.Application.Common.Handlers
{
    /// <summary>
    /// Specialized base class for queries that fetch a list of entities and apply paging.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query.</typeparam>
    /// <typeparam name="TResult">The type of the result DTO.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the entity's ID.</typeparam>
    public abstract class PagedQueryHandler<TQuery, TResult, TEntity, TKey>
        : BaseQueryHandler<TQuery, IResult<PaginatedList<TResult>>, TEntity, TKey>
        where TQuery : IRequest<IResult<PaginatedList<TResult>>>
        where TEntity : class, IEntity<TKey>
        where TKey : struct, IIdentity<TKey>
    {
        #region Constructor

        /// <summary>
        /// Initializes the paged query handler with required services.
        /// </summary>
        /// <param name="queryContext">The query context.</param>
        /// <param name="userContext">The user context.</param>
        /// <param name="requestContext">The request context.</param>
        /// <param name="activitySource">The activity source for tracing.</param>
        /// <param name="mapper">The object mapper.</param>
        protected PagedQueryHandler(
            IQueryContext<TEntity, TKey> queryContext,
            IUserContext userContext,
            IRequestContext requestContext,
            ActivitySource activitySource,
            IMapper mapper)
            : base(queryContext, userContext, requestContext, activitySource, mapper)
        {
        }

        #endregion

        #region Overridables

        /// <summary>
        /// Implements ONLY core business logic for fetching paginated entities.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected override async Task<IResult<PaginatedList<TResult>>> ExecuteQueryAsync(TQuery query, CancellationToken ct)
        {
            using var activity = _activitySource.StartActivity($"{typeof(TQuery).Name}{AppData.Activity.SuffixQuery}", ActivityKind.Internal);
            activity?.SetTag(AppData.Activity.TagQueryType, typeof(TQuery).Name);
            activity?.AddEvent(new ActivityEvent(AppData.Activity.EventQueryExecutionStarted));

            try
            {
                var validationResult = await ValidateQuery(query, ct);
                if (validationResult.IsFailure)
                {
                    return Result.Failure<PaginatedList<TResult>>(null, validationResult.Error ?? "Query Validation Failed");
                }

                var results = await FetchEntities(query, ct);

                activity?.SetStatus(ActivityStatusCode.Ok);
                activity?.AddEvent(new ActivityEvent(AppData.Activity.EventQueryExecutionSucceeded));
                return results;
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                activity?.AddEvent(new ActivityEvent(AppData.Activity.EventQueryExecutionFailed, tags: new ActivityTagsCollection
                    {
                        { AppData.Activity.TagExceptionType, ex.GetType().Name },
                        { AppData.Activity.TagExceptionMessage, ex.Message }
                    }));
                throw;
            }
        }

        /// <summary>
        /// Performs query-specific validation.
        /// </summary>
        /// <param name="query">The query to validate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the validation result.</returns>
        protected virtual Task<IResult> ValidateQuery(TQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult(Result.Success());
        }

        /// <summary>
        /// Fetches the paginated entities from the data source.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A task representing the result.</returns>
        protected abstract Task<IResult<PaginatedList<TResult>>> FetchEntities(TQuery query, CancellationToken ct);

        /// <summary>
        /// Gets the page number from the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The page number.</returns>
        protected abstract int GetPageNumber(TQuery query);

        /// <summary>
        /// Gets the page size from the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The page size.</returns>
        protected abstract int GetPageSize(TQuery query);

        #endregion
    }
}
