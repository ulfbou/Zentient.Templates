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
    /// Base class for executing queries with structured diagnostics and tenant/user context awareness.
    /// </summary>
    /// <typeparam name="TQuery">The query request type implementing <see cref="IRequest{TResult}"/>.</typeparam>
    /// <typeparam name="TResult">The result type implementing <see cref="IResult"/>.</typeparam>
    public abstract class BaseQueryHandler<TQuery, TResult, TEntity, TKey> : IRequestHandler<TQuery, TResult>
        where TQuery : IRequest<TResult>
        where TResult : IResult
        where TEntity : class, IEntity<TKey>
        where TKey : struct, IIdentity<TKey>
    {
        #region Dependencies

        protected readonly IQueryContext<TEntity, TKey> _context;
        protected readonly IUserContext _userContext;
        protected readonly ILogger _logger;
        protected readonly IMapper _mapper;
        protected readonly ActivitySource _activitySource;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the query handler with required services.
        /// </summary>
        protected BaseQueryHandler(
            IQueryContext<TEntity, TKey> context,
            IUserContext userContext,
            ILogger<BaseQueryHandler<TQuery, TResult, TEntity, TKey>> logger,
            IMapper mapper,
            ActivitySource? activitySource = null)
        {
            _context = context;
            _userContext = userContext;
            _logger = logger;
            _mapper = mapper;
            _activitySource = activitySource ?? new ActivitySource("Template.Application");
        }

        #endregion

        #region Handler

        public async Task<TResult> Handle(TQuery query, CancellationToken ct)
        {
            using var activity = _activitySource.StartActivity(typeof(TQuery).Name);
            if (activity != null)
            {
                activity.AddTag("query.name", typeof(TQuery).Name);
                activity.AddTag("handler", GetType().Name);
                // activity.AddTag("user.id", _userContext.UserId.ToString());
                // activity.AddTag("tenant.id", _userContext.TenantId.ToString());
            }

            try
            {
                activity?.AddEvent(new ActivityEvent("QueryExecutionStarted", tags: new ActivityTagsCollection
            {
                { "query.type", typeof(TQuery).Name }
            }));

                var result = await ExecuteQuery(query, ct);

                if (result.IsFailure)
                {
                    activity?.SetStatus(ActivityStatusCode.Error, result.Error);
                    activity?.AddEvent(new ActivityEvent("QueryExecutionFailed", tags: new ActivityTagsCollection
                {
                    { "error.message", result.Error }
                }));
                }
                else
                {
                    activity?.SetStatus(ActivityStatusCode.Ok);
                    activity?.AddEvent(new ActivityEvent("QueryExecutionSucceeded"));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in query handler: {Query}", typeof(TQuery).Name);
                activity?.SetStatus(ActivityStatusCode.Error, "Unhandled exception");
                activity?.AddEvent(new ActivityEvent("QueryExecutionError", tags: new ActivityTagsCollection
            {
                { "error.message", ex.Message },
                { "error.type", ex.GetType().Name }
            }));
                return (TResult)Result.Failure($"Unexpected error: {ex.Message}");
            }
        }

        #endregion

        #region Overridables

        /// <summary>
        /// Override to implement the actual query execution logic.
        /// </summary>
        protected abstract Task<TResult> ExecuteQuery(TQuery query, CancellationToken ct);

        #endregion
    }
}
