using Template.Domain.Common.Result;
using Template.Domain.ValueObjects;

namespace Template.Application.Common.Contracts
{
    public interface IIdempotencyStore
    {
        Task<TResponse> GetResultAsync<TResponse>((string Name, RequestId ClientRequestId) key, CancellationToken ct) where TResponse : IResult;
        Task<bool> HasResultAsync((string Name, RequestId ClientRequestId) key, CancellationToken ct);
        Task MarkInProgressAsync((string Name, RequestId ClientRequestId) key, CancellationToken ct);
        Task RemoveKeyAsync((string Name, RequestId ClientRequestId) key, CancellationToken ct);
        Task StoreResultAsync<TResponse>((string Name, RequestId ClientRequestId) key, TResponse response, CancellationToken ct) where TResponse : IResult;
    }
}