using MediatR;

using Microsoft.Extensions.Logging;

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Template.Application.Common.Contracts;
using Template.Application.Common.Results;
using Template.Domain.Common.Result;
using Template.Domain.Contracts;
using Template.Domain.ValueObjects;

using Zentient.Results;

namespace Template.Application.Common.Handlers
{
    /// <summary>
    /// Base class for handling create commands.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the entity's ID.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public abstract class BaseCreateCommandHandler<TCommand, TEntity, TKey, TResponse>
        : BaseHandler<TCommand, IResult<TResponse>>
        where TCommand : IRequest<IResult<TResponse>>
        where TEntity : class, IEntity<TKey>
        where TKey : struct, IIdentity<TKey>
        where TResponse : class
    {
        protected readonly ICommandContext<TEntity, TKey> _commandContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCreateCommandHandler{TCommand, TEntity, TKey, TResponse}"/> class.
        /// </summary>
        /// <param name="commandContext">The command context.</param>
        /// <param name="userContext">The user context.</param>
        /// <param name="requestContext">The request context.</param>
        protected BaseCreateCommandHandler(
            ICommandContext<TEntity, TKey> commandContext,
            IUserContext userContext,
            IRequestContext requestContext,
            ActivitySource activitySource)
            : base(userContext, requestContext, activitySource)
        {
            _commandContext = commandContext ?? throw new ArgumentNullException(nameof(commandContext));
        }

        /// <summary>
        /// Executes the command asynchronously.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A <typeref name="Task{IResult{TResponse}}"/> representing the result of the command execution.</returns>
        protected override async Task<IResult<TResponse>> ExecuteAsync(TCommand command, CancellationToken ct)
        {
            using var activity = _activitySource.StartActivity($"{typeof(TCommand).Name}{AppData.Activity.SuffixCreate}", ActivityKind.Internal);
            activity?.AddEvent(new ActivityEvent(AppData.Activity.EventMappingToEntity));

            var entityToCreateResult = await MapToEntityAsync(command, ct).ConfigureAwait(false);

            if (entityToCreateResult.IsFailure)
            {
                var errorString = entityToCreateResult.Error
                    ?? (entityToCreateResult.Errors is { Count: > 0 }
                        ? string.Join(", ", entityToCreateResult.Errors.Select(e => e.ToString()))
                        : string.Empty);

                activity?.SetStatus(ActivityStatusCode.Error, errorString);
                activity?.AddEvent(new ActivityEvent(AppData.Activity.EventMappingFailed, tags: new ActivityTagsCollection
                {
                    { AppData.Activity.TagError, errorString }
                }));
                return Result.Failure<TResponse>(AppData.Entities.MappingFailed(entityToCreateResult.Errors ?? []));
            }

            activity?.AddEvent(new ActivityEvent(AppData.Activity.EventAddingToContext));
            var entityToCreate = entityToCreateResult.Value;
            var addResult = await _commandContext.AddAsync(entityToCreate!, ct).ConfigureAwait(false);

            if (addResult.IsFailure)
            {
                activity?.SetStatus(ActivityStatusCode.Error, addResult.Error);
                activity?.AddEvent(new ActivityEvent(AppData.Activity.EventCreationFailed, tags: new ActivityTagsCollection
                {
                    { AppData.Activity.TagError, addResult.Error ?? string.Join(", ", addResult.Errors ?? new string[0]) }
                }));
                return Result<TResponse>.Failure(null, addResult.Errors ?? []);
            }

            activity?.AddEvent(new ActivityEvent(AppData.Activity.EventPostCreationAction));
            await PerformPostCreationActionAsync(command, addResult.Value, ct).ConfigureAwait(false);

            activity?.AddEvent(new ActivityEvent(AppData.Activity.EventMappingToResponse));
            var response = await MapToResponseAsync(addResult.Value, ct).ConfigureAwait(false);

            activity?.SetStatus(ActivityStatusCode.Ok);
            activity?.AddEvent(new ActivityEvent(AppData.Activity.EventCreationSucceeded));
            return response;
        }

        /// <summary>
        /// Maps the command to an entity.
        /// </summary>
        /// <param name="command">The command to map.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A <typeref name="Task{TEntity}"/> representing the mapped entity.</returns>
        protected abstract Task<IResult<TEntity>> MapToEntityAsync(TCommand command, CancellationToken ct);

        /// <summary>
        /// Maps the entity to a response asynchronously.
        /// </summary>
        /// <param name="entity">The entity to map.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A <typeref name="Task{TResponse}"/> representing the mapped response.</returns>
        protected abstract Task<IResult<TResponse>> MapToResponseAsync(TEntity entity, CancellationToken ct);

        /// <summary>
        /// Performs post-creation actions asynchronously.
        /// Optional: override to perform actions after creation, but before returning.
        /// </summary>
        /// <param name="command">The command that was executed.</param>
        /// <param name="createdEntity">The entity that was created.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A <typeref name="Task"/> representing the asynchronous operation.</returns>
        protected virtual Task PerformPostCreationActionAsync(TCommand command, TEntity createdEntity, CancellationToken ct)
            => Task.CompletedTask;
    }
}
