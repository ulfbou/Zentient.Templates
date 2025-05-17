using MediatR;

using Microsoft.Extensions.Logging;

using Template.Application.Common.Contracts;
using Template.Application.Common.Results;
using Template.Domain.Common.Result;
using Template.Domain.Contracts;
using Template.Domain.ValueObjects;

namespace Template.Application.Common.Handlers
{
    /// <summary>
    ///     Specialized base class for queries that fetch a list of entities and apply paging.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query.</typeparam>
    /// <typeparam name="TResult">The type of the result DTO.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the entity's ID.</typeparam>
    public abstract class PagedQueryHandler<TQuery, TResult, TEntity, TKey>
        : IRequestHandler<TQuery, IResult<PaginatedList<TResult>>>
        where TQuery : IRequest<IResult<PaginatedList<TResult>>>
        where TEntity : class, IEntity<TKey>
        where TKey : struct, IIdentity<TKey>
    {
        protected readonly IQueryContext<TEntity, TKey> _context;
        protected readonly ILogger<PagedQueryHandler<TQuery, TResult, TEntity, TKey>> _logger;

        protected PagedQueryHandler(
            IQueryContext<TEntity, TKey> context,
            ILogger<PagedQueryHandler<TQuery, TResult, TEntity, TKey>> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IResult<PaginatedList<TResult>>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await ValidateQuery(query, cancellationToken);
                if (validationResult.IsFailure)
                {
                    return (IResult<PaginatedList<TResult>>)(
                          validationResult.Error is not null
                        ? validationResult
                        : Result.Failure<PaginatedList<TResult>>(null, validationResult.Error ?? "Query Validation Failed"));
                }

                IResult<PaginatedList<TResult>> results = await FetchEntities(query, cancellationToken);

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling paged query {QueryName} for entity {EntityName}",
                    typeof(TQuery).Name, typeof(TEntity).Name);
                return Result.Failure<PaginatedList<TResult>>(null, $"Error processing paged query: {ex.Message}");
            }
        }

        /// <summary>
        ///     Performs query-specific validation.
        /// </summary>
        /// <param name="query">The query to validate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a Result indicating success or failure.</returns>
        protected virtual Task<IResult> ValidateQuery(TQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult(Result.Success());
        }

        /// <summary>
        ///     Fetches the entities from the data source with paging.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The fetched entities and the total count.</returns>
        protected abstract Task<IResult<PaginatedList<TResult>>> FetchEntities(TQuery query, CancellationToken cancellationToken);

        /// <summary>
        ///     Maps the entity to the result DTO.
        /// </summary>
        /// <param name="entity">The entity to map.</param>
        /// <returns>The result DTO.</returns>
        protected abstract TResult MapToResult(TEntity entity);

        /// <summary>
        /// Gets the page number from the query
        /// </summary>
        /// <param name="query">The query</param>
        /// <returns>The page number</returns>
        protected abstract int GetPageNumber(TQuery query);

        /// <summary>
        /// Gets the page size from the query.
        /// </summary>
        /// <param name="query">The query</param>
        /// <returns>The page size.</returns>
        protected abstract int GetPageSize(TQuery query);
    }
}
