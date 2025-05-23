using MediatR;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Template.Application.Common.Contracts;

namespace Template.Application.Common.Behaviors
{
    /// <summary>
    /// Base class for pipeline behaviors that provides activity tracing and user context tagging.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public abstract class PipelineBehaviorBase<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ActivitySource _activitySource;
        private readonly IUserContext? _userContext;

        /// <summary>Initializes a new instance of the <see cref="PipelineBehaviorBase{TRequest, TResponse}"/> class.</summary>
        /// <param name="activitySource">The activity source for tracing. If null, a default is created.</param>
        /// <param name="userContext">The user context for tagging activities. Optional.</param>
        protected PipelineBehaviorBase(ActivitySource? activitySource = null, IUserContext? userContext = null)
        {
            _activitySource = activitySource ?? new ActivitySource(AppData.Activity.AppBehaviors);
            _userContext = userContext;
        }

        /// <summary>
        /// Starts a new activity for tracing and adds user/tenant tags if available.
        /// </summary>
        /// <param name="name">The name of the activity.</param>
        /// <returns>The started <see cref="Activity"/> instance, or null if tracing is not enabled.</returns>
        protected Activity? StartActivity(string name)
        {
            using var activity = _activitySource.StartActivity(name);
            if (_userContext != null)
            {
                activity?.AddTag(AppData.Activity.TagUserId, _userContext.UserId.ToString());
                activity?.AddTag(AppData.Activity.TagTenantId, _userContext.TenantId.ToString());
            }
            return activity;
        }

        /// <summary>
        /// Handles the pipeline behavior logic for the request.
        /// </summary>
        /// <param name="request">The request instance.</param>
        /// <param name="next">The next handler in the pipeline.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The response from the next handler.</returns>
        public abstract Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
    }
}
