using System;
using System.Diagnostics;
using System.Threading.Tasks;

using MediatR;

using Microsoft.Extensions.Logging;

using Template.Application.Common;
using Template.Domain.Common.Result;

namespace Template.Application.Common.Behaviors
{
    public abstract class HandlerBase<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        protected readonly ActivitySource _activitySource;

        protected HandlerBase(ActivitySource? activitySource = null)
        {
            _activitySource = activitySource ?? new ActivitySource(AppData.Activity.AppHandler);
        }

        protected async Task<TResponse> ExecuteWithActivityAsync(string name, Func<Task<TResponse>> handler)
        {
            using var activity = _activitySource.StartActivity(name);
            try
            {
                activity?.AddEvent(new ActivityEvent(AppData.Activity.EventExecutionStarted));
                var result = await handler();

                if (result is IResult r && r.IsFailure)
                {
                    activity?.SetStatus(ActivityStatusCode.Error, r.Error);
                    activity?.AddEvent(new ActivityEvent(AppData.Activity.EventExecutionFailed, tags: new ActivityTagsCollection { { AppData.Activity.TagError, r.Error } }));
                }
                else
                {
                    activity?.SetStatus(ActivityStatusCode.Ok);
                    activity?.AddEvent(new ActivityEvent(AppData.Activity.EventExecutionSucceeded));
                }
                return result;
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                throw;
            }
        }
    }
}
