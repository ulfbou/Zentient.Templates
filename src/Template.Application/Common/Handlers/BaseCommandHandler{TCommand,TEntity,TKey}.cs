using MediatR;

using Microsoft.Extensions.Logging;

using Template.Application.Common.Contracts;
using Template.Application.Common.Results;
using Template.Domain.Common.Result;
using Template.Domain.Contracts;
using Template.Domain.ValueObjects;

namespace Template.Application.Common.Handlers
{
    public abstract class BaseUpdateCommandHandler<TCommand, TEntity, TKey> : IRequestHandler<TCommand, IResult>
        where TCommand : IRequest<IResult>
        where TEntity : class, IEntity<TKey>
        where TKey : struct, IIdentity<TKey>
    {
        protected readonly ICommandContext<TEntity, TKey> _context;
        protected readonly ILogger _logger;

        protected BaseUpdateCommandHandler(
            ICommandContext<TEntity, TKey> context,
            ILogger logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IResult> Handle(TCommand command, CancellationToken ct)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }


            return await PerformAction(command, entity, ct);
        }

        protected abstract bool RequiresEntity(TCommand command);
        protected abstract TKey GetId(TCommand command);

        protected virtual Task<IResult> PerformAction(TCommand command, TEntity entity, CancellationToken ct) =>
            throw new NotImplementedException($"{GetType().Name} must override PerformAction(command, entity, ct)");
    }

    public abstract class BaseCreateCommandHandler<TCommand, TResponse, TEntity, TKey> : IRequestHandler<TCommand, IResult<TResponse>>
        where TCommand : IRequest<IResult<TResponse>>
        where TResponse : class
        where TEntity : class, IEntity<TKey>
        where TKey : struct, IIdentity<TKey>
    {
        protected readonly ICommandContext<TEntity, TKey> _context;
        protected readonly ILogger _logger;

        protected BaseCreateCommandHandler(
            ICommandContext<TEntity, TKey> context,
            ILogger logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected virtual bool RequiresEntity(TCommand command) => false;
        protected virtual Task<IResult<TResponse>> PerformAction(TCommand command, CancellationToken ct) =>
            throw new NotImplementedException($"{GetType().Name} must override PerformAction(command, ct)");
        protected virtual Task<IResult> ValidateCommand(TCommand command, CancellationToken ct) =>
            Task.FromResult(Result.Success());

        protected abstract TKey GetId(TCommand command);
        public virtual async Task<IResult<TResponse>> Handle(TCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var entity = await _context.GetByIdAsync(entityId, cancellationToken);

            if (entity is not null)
            {
                return Result.Failure<TResponse>(default, $"{typeof(TEntity).Name} not found.");
            }

            return await PerformAction(request, cancellationToken);
        }
    }
}
