using MediatR;

using Template.Application.Common.Contracts;
using Template.Domain.Common.Result;

using Zentient.Results;

namespace Template.Application.Common.Behaviors
{
    /// <summary>
    /// Pipeline behavior that ensures idempotency for commands implementing <see cref="ICommandWithRequestId{TResponse}"/>.
    /// Prevents duplicate processing of the same request by storing and reusing results.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request, must implement <see cref="ICommandWithRequestId{TResponse}"/>.</typeparam>
    /// <typeparam name="TResponse">The type of the response, must implement <see cref="IResult"/>.</typeparam>
    public sealed class IdempotencyBehavior<TRequest, TResponse>
        : PipelineBehaviorBase<TRequest, TResponse>
        where TRequest : ICommandWithRequestId<TResponse>
        where TResponse : IResult
    {
        private readonly IIdempotencyStore _store;

        /// <summary>Initializes a new instance of the <see cref="IdempotencyBehavior{TRequest, TResponse}"/> class.</summary>
        /// <param name="store">The idempotency store instance.</param>
        public IdempotencyBehavior(IIdempotencyStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        /// <inheritdoc />
        public override async Task<TResponse> Handle(
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
