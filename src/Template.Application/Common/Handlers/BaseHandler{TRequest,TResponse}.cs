using MediatR;

using System.Diagnostics;

using Template.Application.Common.Contracts;

namespace Template.Application.Common.Handlers
{
    public abstract class BaseHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        #region Dependencies

        protected readonly IUserContext _userContext;
        protected readonly IRequestContext _requestContext;
        protected readonly ActivitySource _activitySource;

        #endregion

        #region Constructor

        protected BaseHandler(
            IUserContext userContext,
            IRequestContext requestContext,
            ActivitySource activitySource)
        {
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _requestContext = requestContext ?? throw new ArgumentNullException(nameof(requestContext));
            _activitySource = activitySource ?? throw new ArgumentNullException(nameof(activitySource));
        }

        #endregion

        #region Handle

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            using var activity = _activitySource.StartActivity($"{typeof(TRequest).Name}{AppData.Activity.SuffixHandle}", ActivityKind.Internal);

            activity?.SetTag(AppData.Activity.TagRequestType, typeof(TRequest).FullName ?? typeof(TRequest).Name);
            activity?.SetTag(AppData.Activity.TagUserId, _userContext.UserId);
            activity?.SetTag(AppData.Activity.TagCorrelationId, _requestContext.CorrelationId);
            activity?.AddEvent(new ActivityEvent(AppData.Activity.EventExecutionStarted));

            try
            {
                TResponse response = await ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
                activity?.SetStatus(ActivityStatusCode.Ok);
                activity?.AddEvent(new ActivityEvent(AppData.Activity.EventExecutionSucceeded));
                return response;
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                activity?.AddEvent(new ActivityEvent(AppData.Activity.EventExecutionFailed, tags: new ActivityTagsCollection
                {
                    { AppData.Activity.TagExceptionType, ex.GetType().Name },
                    { AppData.Activity.TagExceptionMessage, ex.Message },
                    { AppData.Activity.TagExceptionStackTrace, ex.StackTrace }
                }));
                throw;
            }
        }

        #endregion

        #region Overridables

        protected abstract Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken);

        #endregion
    }
}
