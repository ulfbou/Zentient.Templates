using MediatR;

using Microsoft.Extensions.Logging;

using System.Diagnostics;

using Template.Domain.Common.Result;

namespace Template.Application.Common.Behaviors
{
    public abstract class HandlerBase<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        protected readonly ILogger _logger;
        protected readonly ActivitySource _activitySource;

        protected HandlerBase(ILogger logger, ActivitySource? activitySource = null)
        {
            _logger = logger;
            _activitySource = activitySource ?? new ActivitySource("Application.Handlers");
        }

        protected async Task<TResponse> ExecuteWithActivityAsync(string name, Func<Task<TResponse>> handler)
        {
            using var activity = _activitySource.StartActivity(name);
            try
            {
                activity?.AddEvent(new("ExecutionStarted"));
                var result = await handler();
                if (result is IResult r && r.IsFailure)
                {
                    activity?.SetStatus(ActivityStatusCode.Error, r.Error);
                    activity?.AddEvent(new("ExecutionFailed", new ActivityTagsCollection { { "error", r.Error } }));
                }
                else
                {
                    activity?.SetStatus(ActivityStatusCode.Ok);
                    activity?.AddEvent(new("ExecutionSucceeded"));
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in handler {Name}", name);
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                throw;
            }
        }
    }
}
