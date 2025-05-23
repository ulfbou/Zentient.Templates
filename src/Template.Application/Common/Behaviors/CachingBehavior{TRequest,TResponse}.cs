using MediatR;

using Microsoft.Extensions.Caching.Memory;

using Template.Application.Common.Contracts;
using Template.Domain.Common.Result;

using Zentient.Results;

namespace Template.Application.Common.Behaviors
{
    /// <summary>Pipeline behavior that provides caching for queries implementing <see cref="ICacheableQuery{TResponse}"/>.</summary>
    /// <typeparam name="TRequest">The type of the request, must implement <see cref="ICacheableQuery{TResponse}"/>.</typeparam>
    /// <typeparam name="TResponse">The type of the response, must implement <see cref="IResult"/>.</typeparam>
    public sealed class CachingBehavior<TRequest, TResponse>
        : PipelineBehaviorBase<TRequest, TResponse>, IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICacheableQuery<TResponse>
        where TResponse : IResult
    {
        private readonly IMemoryCache _cache;

        /// <summary>Initializes a new instance of the <see cref="CachingBehavior{TRequest, TResponse}"/> class.</summary>
        /// <param name="cache">The memory cache instance.</param>
        public CachingBehavior(IMemoryCache cache) => _cache = cache;

        /// <summary>
        /// Handles the caching logic for the request. If a cached response exists, it is returned.
        /// Otherwise, the request is processed and the response is cached if successful.
        /// </summary>
        /// <param name="request">The request instance.</param>
        /// <param name="next">The next handler in the pipeline.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The response, either from cache or from the next handler.</returns>
        public override async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_cache.TryGetValue(request.CacheKey, out TResponse? cached))
            {
                return cached!;
            }

            var response = await next();

            if (response.IsSuccess)
            {
                _cache.Set(
                    request.CacheKey,
                    response,
                    request.CacheDuration ?? TimeSpan.FromMinutes(5));
            }

            return response;
        }
    }
}
