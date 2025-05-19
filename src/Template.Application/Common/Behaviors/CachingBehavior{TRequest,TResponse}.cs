using MediatR;

using Microsoft.Extensions.Caching.Memory;

using Template.Application.Common.Contracts;
using Template.Domain.Common.Result;

namespace Template.Application.Common.Behaviors
{
    public sealed class CachingBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICacheableQuery<TResponse>
        where TResponse : IResult
    {
        private readonly IMemoryCache _cache;
        public CachingBehavior(IMemoryCache cache) => _cache = cache;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
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
