using MediatR;

using Microsoft.Extensions.Logging;

using Template.Application.Common.Contracts;
using Template.Application.Common.Results;
using Template.Domain.Common.Result;
using Template.Domain.Contracts;
using Template.Domain.ValueObjects;

namespace Template.Application.Common.Handlers
{
    public abstract class BaseCommandHandler<TCommand, TEntity, TKey> : IRequestHandler<TCommand, IResult>
        where TCommand : IRequest<IResult>
        where TEntity : class, IEntity<TKey>
        where TKey : struct, IIdentity<TKey>
    {
        protected readonly ICommandContext<TEntity, TKey> _context;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IUserContext _userContext;
        protected readonly ILogger _logger;

        protected BaseCommandHandler(
            ICommandContext<TEntity, TKey> context,
            IUnitOfWork unitOfWork,
            IUserContext userContext,
            ILogger<BaseCommandHandler<TCommand, TEntity, TKey>> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IResult> Handle(TCommand command, CancellationToken ct)
        {
            try
            {
                var validation = await ValidateCommand(command, ct);
                if (validation.IsFailure) return validation;

                TEntity? entity = null;

                if (RequiresEntity(command))
                {
                    entity = await _context.GetByIdAsync(GetId(command), ct);

                    if (entity is null)
                    {
                        return Result.Failure($"{typeof(TEntity).Name} not found.");
                    }
                }

                if (RequiresEntity(command))
                {
                    await PerformAction(command, entity!, ct);
                }
                else
                {
                    await PerformAction(command, ct);
                }

                await _unitOfWork.BeginTransactionAsync(ct);

                try
                {
                    await _unitOfWork.SaveChangesAsync(ct);
                    await _unitOfWork.CommitTransactionAsync(ct);
                    return Result.Success();
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync(ct);
                    _logger.LogError(ex, "Commit failed in {Handler}.Command: {Command}.Entity: {EntityId}",
                        GetType().Name, typeof(TCommand).Name, RequiresEntity(command) ? GetId(command).Value : null);
                    return Result.Failure($"Transaction failed: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in {Handler}.Command: {Command}.Entity: {EntityId}",
                    GetType().Name, typeof(TCommand).Name, RequiresEntity(command) ? GetId(command).Value : null);
                return Result.Failure($"Unexpected error: {ex.Message}");
            }
        }

        protected virtual Task<IResult> ValidateCommand(TCommand command, CancellationToken ct) =>
            Task.FromResult(Result.Success());

        protected abstract bool RequiresEntity(TCommand command);
        protected abstract TKey GetId(TCommand command);

        protected virtual Task PerformAction(TCommand command, TEntity entity, CancellationToken ct) =>
            throw new NotImplementedException($"{GetType().Name} must override PerformAction(command, entity, ct)");

        protected virtual Task PerformAction(TCommand command, CancellationToken ct) =>
            throw new NotImplementedException($"{GetType().Name} must override PerformAction(command, ct)");

        protected virtual bool ShouldUpdateEntity(TCommand command) => true;
        protected virtual bool ShouldAddEntity(TCommand command) => false;

        protected virtual TEntity CreateEntity(TCommand command) =>
            throw new NotImplementedException($"{GetType().Name} must override CreateEntity(command)");
    }
}
