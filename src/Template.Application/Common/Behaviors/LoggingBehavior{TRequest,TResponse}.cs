using MediatR;

using Microsoft.Extensions.Logging;

using System.Diagnostics;

using Template.Application.Common.Contracts;

namespace Template.Application.Common.Behaviors
{
    public sealed class LoggingBehavior<TRequest, TResponse> : PipelineBehaviorBase<TRequest, TResponse>
        where TRequest : notnull
    {
        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger, IUserContext userContext, ActivitySource? activitySource = null)
            : base(activitySource, userContext) { }

        public override async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            using var activity = StartActivity(typeof(TRequest).Name);
            try
            {
                activity?.AddEvent(new("HandlingStarted"));
                var response = await next();
                activity?.AddEvent(new("HandlingCompleted"));
                return response;
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                throw;
            }
        }
    }
}
