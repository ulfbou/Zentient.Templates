using MediatR;

using Microsoft.Extensions.DependencyInjection;

using Template.Application.Common.Behaviors;

namespace Template.Application.Common.Contracts
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationBehavior<TRequest, TResponse>(this IServiceCollection services)
            where TRequest : notnull
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(IdempotencyBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
            return services;
        }
    }
}
