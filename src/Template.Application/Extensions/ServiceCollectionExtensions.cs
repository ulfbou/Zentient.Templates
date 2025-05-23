using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

using Template.Application.Common.Contexts;
using Template.Application.Common.Contracts;

namespace Template.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddContexts(this IServiceCollection services)
        {
            services.AddScoped<IUserContext, UserContext>();
            services.AddScoped<IRequestContext, RequestContext>();

            return services;
        }
    }
}
