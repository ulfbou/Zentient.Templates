using MediatR;

using Template.Application.Common.Contracts;
using Template.Domain.Common.Result;

namespace Template.Application.Common.Behaviors
{
    public sealed class IdempotencyBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommandWithRequestId<TResponse>
        where TResponse : IResult
    {
        private readonly IIdempotencyStore _store;
        public IdempotencyBehavior(IIdempotencyStore store) => _store = store;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
            var key = (typeof(TRequest).Name, request.ClientRequestId);

            if (await _store.HasResultAsync(key, ct))
                return await _store.GetResultAsync<TResponse>(key, ct);

            await _store.MarkInProgressAsync(key, ct);

            try
            {
                var response = await next();
                if (response.IsSuccess)
                    await _store.StoreResultAsync(key, response, ct);
                else
                    await _store.RemoveKeyAsync(key, ct);

                return response;
            }
            catch
            {
                await _store.RemoveKeyAsync(key, ct);
                throw;
            }
        }
    }
}
