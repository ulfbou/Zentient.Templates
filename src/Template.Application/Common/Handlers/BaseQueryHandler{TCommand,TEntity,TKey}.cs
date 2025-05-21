using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.Extensions.Logging;

using Template.Application.Common.Contracts;
using Template.Application.Common.Results;
using Template.Domain.Common.Result;
using Template.Domain.Contracts;
using Template.Domain.ValueObjects;
using System.Diagnostics;
using AutoMapper;

namespace Template.Application.Common.Handlers
{
    /// <summary>
    /// Base class for executing queries with only business logic. 
    /// All cross-cutting concerns are handled by the pipeline.
    /// </summary>
    public abstract class BaseQueryHandler<TQuery, TResult, TEntity, TKey>
        : BaseHandler<TQuery, TResult>
        where TQuery : IRequest<TResult>
        where TResult : IResult
        where TEntity : class, IEntity<TKey>
        where TKey : struct, IIdentity<TKey>
    {
        #region Dependencies

        protected readonly IQueryContext<TEntity, TKey> _queryContext;
        protected readonly IMapper _mapper;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the query handler with required services.
        /// </summary>
        /// <param name="queryContext">The query context.</param>
        /// <param name="userContext">The user context.</param>
        protected BaseQueryHandler(
            IQueryContext<TEntity, TKey> queryContext,
            IUserContext userContext,
            IRequestContext requestContext,
            ActivitySource activitySource,
            IMapper mapper)
            : base(userContext, requestContext, activitySource)
        {
            _queryContext = queryContext ?? throw new ArgumentNullException(nameof(queryContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #endregion

        #region Overridables

        /// <summary>
        /// Implement ONLY core business logic for the query. 
        /// All validation, logging, error handling, etc. are handled by the pipeline.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A <typeref name="Task{TResult}"/> representing the asynchronous operation.</returns>
        protected override async Task<TResult> ExecuteAsync(TQuery query, CancellationToken ct)
        {
            using var activity = _activitySource.StartActivity($"{typeof(TQuery).Name}{AppData.Activity.SuffixQuery}", ActivityKind.Internal);
            activity?.SetTag(AppData.Activity.TagQueryType, typeof(TQuery).Name);
            activity?.AddEvent(new ActivityEvent(AppData.Activity.EventQueryExecutionStarted));

            try
            {
                var result = await ExecuteQueryAsync(query, ct).ConfigureAwait(false);
                activity?.SetStatus(ActivityStatusCode.Ok);
                activity?.AddEvent(new ActivityEvent(AppData.Activity.EventQueryExecutionSucceeded));
                return result;
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
        /// Implement the actual query execution logic here.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A <typeref name="Task{TResult}"/> representing the asynchronous operation.</returns>
        protected abstract Task<TResult> ExecuteQueryAsync(TQuery query, CancellationToken ct);

        /// <summary>
        /// Override to implement the actual query execution logic.
        /// </summary>
        protected abstract Task<TResult> ExecuteQuery(TQuery query, CancellationToken ct);

        #endregion
    }
}
