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
    public abstract class PipelineBehaviorBase<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ActivitySource _activitySource;
        private readonly IUserContext? _userContext;

        protected PipelineBehaviorBase(ActivitySource? activitySource = null, IUserContext? userContext = null)
        {
            _activitySource = activitySource ?? new ActivitySource(AppData.Activity.AppBehaviors);
            _userContext = userContext;
        }

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

        public abstract Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
    }
}
