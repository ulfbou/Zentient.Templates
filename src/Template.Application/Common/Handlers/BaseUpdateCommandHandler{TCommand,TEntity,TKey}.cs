
using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Template.Application.Common.Contracts;
using Template.Application.Common.Results;
using Template.Domain.Common.Result;
using Template.Domain.Contracts;
using Template.Domain.ValueObjects;

namespace Template.Application.Common.Handlers
{
    public abstract class BaseUpdateCommandHandler<TCommand, TEntity, TKey>
        : BaseHandler<TCommand, IResult>
        where TCommand : IRequest<IResult>
        where TEntity : class, IEntity<TKey>
        where TKey : struct, IIdentity<TKey>
    {
        protected readonly ICommandContext<TEntity, TKey> _context;

        public virtual string CommandName => typeof(TCommand).Name;

        protected BaseUpdateCommandHandler(
            ICommandContext<TEntity, TKey> context,
            IUserContext userContext,
            IRequestContext requestContext,
            ActivitySource activitySource)
            : base(userContext, requestContext, activitySource)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected override async Task<IResult> ExecuteAsync(TCommand command, CancellationToken ct)
        {
            using var activity = _activitySource.StartActivity($"{typeof(TCommand).Name}{CommandName}", ActivityKind.Internal);
            activity?.AddEvent(new ActivityEvent(AppData.Activity.EventGetEntityId));

            var id = GetId(command);

            activity?.AddEvent(new ActivityEvent(AppData.Activity.EventFetchEntity));
            var getResult = await _context.GetByIdAsync(id, ct).ConfigureAwait(false);

            if (getResult.IsFailure)
            {
                activity?.SetStatus(ActivityStatusCode.Error, getResult.Error);
                activity?.AddEvent(new ActivityEvent(AppData.Activity.EventFetchFailed, tags: new ActivityTagsCollection { { AppData.Activity.TagError, getResult.Error } }));
                return getResult;
            }

            var entity = getResult.Value;
            if (entity is null)
            {
                var notFoundMsg = string.Format(AppData.Messages.EntityNotFoundFormat, typeof(TEntity).Name, id);
                activity?.SetStatus(ActivityStatusCode.Error, notFoundMsg);
                activity?.AddEvent(new ActivityEvent(AppData.Activity.EventEntityNotFound, tags: new ActivityTagsCollection { { AppData.Activity.TagId, id.ToString() ?? string.Empty } }));
                return Result.NotFound<TEntity, TKey>(id);
            }

            activity?.AddEvent(new ActivityEvent(AppData.Activity.EventUpdateAction));
            IResult result = await PerformUpdateActionAsync(command, entity, ct).ConfigureAwait(false);

            activity?.AddEvent(new ActivityEvent(AppData.Activity.EventPersistUpdate));
            var updateResult = await _context.UpdateAsync(entity, ct).ConfigureAwait(false);

            if (updateResult.IsFailure)
            {
                activity?.SetStatus(ActivityStatusCode.Error, updateResult.Error);
                activity?.AddEvent(new ActivityEvent(AppData.Activity.EventUpdateFailed, tags: new ActivityTagsCollection { { AppData.Activity.TagError, updateResult.Errors } }));
                return updateResult;
            }

            activity?.SetStatus(ActivityStatusCode.Ok);
            activity?.AddEvent(new ActivityEvent(AppData.Activity.EventUpdateSucceeded));
            return Result.Success();
        }

        protected abstract TKey GetId(TCommand command);

        protected abstract Task<IResult> PerformUpdateActionAsync(TCommand command, TEntity existingEntity, CancellationToken ct);
    }
}
