
using Template.Domain.Common.Result;

namespace Template.Application.Common.Contracts
{
    public interface ICacheableQuery<TResponse> where TResponse : IResult
    {
        string CacheKey { get; }
        TimeSpan? CacheDuration { get; }
    }
}