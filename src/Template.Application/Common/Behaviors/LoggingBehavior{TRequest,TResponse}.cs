using MediatR;

using Microsoft.Extensions.Logging;

using System.Diagnostics;

using Template.Application.Common.Contracts;

namespace Template.Application.Common.Behaviors
{
    /// <summary>Pipeline behavior that logs the start and completion of request handling, and traces exceptions.</summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public sealed class LoggingBehavior<TRequest, TResponse>
        : PipelineBehaviorBase<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        /// <summary>Initializes a new instance of the <see cref="LoggingBehavior{TRequest, TResponse}"/> class.</summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="userContext">The user context for tagging activities.</param>
        /// <param name="activitySource">The activity source for tracing. Optional.</param>
        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger, IUserContext userContext, ActivitySource? activitySource = null)
            : base(activitySource, userContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>Handles the logging logic for the request. Logs the start and completion of handling, and traces exceptions.</summary>
        /// <param name="request">The request instance.</param>
        /// <param name="next">The next handler in the pipeline.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The response from the next handler.</returns>
        public override async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            using var activity = StartActivity(typeof(TRequest).Name);
            try
            {
                activity?.AddEvent(new(AppData.Activity.EventHandlingStarted));
                var response = await next();
                activity?.AddEvent(new(AppData.Activity.EventHandlingCompleted));
                return response;
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                activity?.AddTag(AppData.Activity.TagError, ex.Message);
                activity?.AddEvent(new(AppData.Activity.EventHandlingFailed));
                _logger.LogError(ex, AppData.Activity.LogErrorHandlingRequest, request);
                throw;
            }
        }
    }
}
