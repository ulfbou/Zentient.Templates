using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using Template.Application.Common.Contracts;
using Template.Domain.Common.Contracts;

namespace Template.Application.Common.Handlers;

/// <summary>
/// Base command handler with support for entity resolution, unit of work, structured diagnostics, and tenant/user context.
/// </summary>
/// <typeparam name="TCommand">The command type implementing <see cref="IRequest{IResult}"/>.</typeparam>
/// <typeparam name="TEntity">The aggregate root or entity type.</typeparam>
/// <typeparam name="TKey">The type of the entity identity, constrained to <see cref="IIdentity{TKey}"/>.</typeparam>
public abstract class BaseCommandHandler<TCommand, TEntity, TKey> : IRequestHandler<TCommand, IResult>
    where TCommand : IRequest<IResult>
    where TEntity : class, IEntity<TKey>
    where TKey : struct, IIdentity<TKey>
{
    #region Dependencies

    protected readonly ICommandContext<TEntity, TKey> _context;
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly IUserContext _userContext;
    protected readonly ILogger _logger;
    protected readonly ActivitySource _activitySource;

    #endregion

    #region Constructor

    /// <summary>
    /// Constructs a new instance of <see cref="BaseCommandHandler{TCommand, TEntity, TKey}"/>.
    /// </summary>
    protected BaseCommandHandler(
        ICommandContext<TEntity, TKey> context,
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        ILogger<BaseCommandHandler<TCommand, TEntity, TKey>> logger,
        ActivitySource? activitySource = null)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _logger = logger;
        _activitySource = activitySource ?? new ActivitySource("Template.Application");
    }

    #endregion

    #region Handler

    public async Task<IResult> Handle(TCommand command, CancellationToken ct)
    {
        using var activity = _activitySource.StartActivity(typeof(TCommand).Name);
        if (activity != null)
        {
            activity.AddTag("command.name", typeof(TCommand).Name);
            activity.AddTag("entity.type", typeof(TEntity).Name);
            activity.AddTag("handler", GetType().Name);

            if (RequiresEntity(command))
            {
                activity.AddTag("entity.id", GetIdSafe(command)?.ToString());
            }

            activity.AddEvent(new ActivityEvent("CommandHandlingStarted", tags: new ActivityTagsCollection
            {
                { "command.type", typeof(TCommand).FullName! }
            }));
        }

        try
        {
            var validation = await ValidateCommand(command, ct);
            if (validation.IsFailure)
            {
                activity?.SetStatus(ActivityStatusCode.Error, validation.Error);
                activity?.AddEvent(new ActivityEvent("CommandValidationFailed", tags: new ActivityTagsCollection
                {
                    { "error.message", validation.Error }
                }));
                return validation;
            }

            TEntity? entity = RequiresEntity(command)
                ? await _context.GetByIdAsync(GetId(command), ct)
                : null;

            if (entity is null && RequiresEntity(command))
            {
                const string notFoundError = "Entity not found.";
                activity?.SetStatus(ActivityStatusCode.Error, notFoundError);
                activity?.AddEvent(new ActivityEvent("EntityResolutionFailed", tags: new ActivityTagsCollection
                {
                    { "error.message", notFoundError }
                }));
                return Result.Failure($"{typeof(TEntity).Name} not found.");
            }

            if (entity is not null)
                await PerformAction(command, entity, ct);
            else
                await PerformAction(command, ct);

            await _unitOfWork.BeginTransactionAsync(ct);

            try
            {
                await _unitOfWork.SaveChangesAsync(ct);
                await _unitOfWork.CommitTransactionAsync(ct);

                activity?.SetStatus(ActivityStatusCode.Ok);
                activity?.AddEvent(new ActivityEvent("CommandHandledSuccessfully"));

                return Result.Success();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(ct);
                _logger.LogError(ex, "Commit failed: {Command} [{EntityId}]", typeof(TCommand).Name, GetIdSafe(command));

                activity?.SetStatus(ActivityStatusCode.Error, "Transaction failed");
                activity?.AddEvent(new ActivityEvent("TransactionFailed", tags: new ActivityTagsCollection
                {
                    { "error.message", ex.Message }
                }));

                return Result.Failure($"Transaction failed: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in handler: {Command} [{EntityId}]", typeof(TCommand).Name, GetIdSafe(command));

            activity?.SetStatus(ActivityStatusCode.Error, "Unhandled exception");
            activity?.AddEvent(new ActivityEvent("UnhandledCommandError", tags: new ActivityTagsCollection
            {
                { "error.message", ex.Message },
                { "error.type", ex.GetType().Name }
            }));

            return Result.Failure($"Unexpected error: {ex.Message}");
        }
    }

    #endregion

    #region Overridables

    /// <summary>
    /// Defines if the command requires loading an entity before execution.
    /// </summary>
    protected virtual bool RequiresEntity(TCommand command) => true;

    /// <summary>
    /// Extracts the entity identifier from the command.
    /// </summary>
    protected abstract TKey GetId(TCommand command);

    /// <summary>
    /// Safely attempts to get the identifier for diagnostics purposes.
    /// </summary>
    protected virtual TKey? GetIdSafe(TCommand command)
    {
        try { return GetId(command); }
        catch { return default; }
    }

    /// <summary>
    /// Performs command logic that targets an existing entity.
    /// </summary>
    protected virtual Task PerformAction(TCommand command, TEntity entity, CancellationToken ct) =>
        Task.CompletedTask;

    /// <summary>
    /// Performs command logic that does not target a specific entity.
    /// </summary>
    protected virtual Task PerformAction(TCommand command, CancellationToken ct) =>
        Task.CompletedTask;

    /// <summary>
    /// Validates the incoming command.
    /// </summary>
    protected virtual Task<IResult> ValidateCommand(TCommand command, CancellationToken ct) =>
        Task.FromResult(Result.Success());

    #endregion
}