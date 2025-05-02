using Zentient.Templates.MinimalAPI.API.Endpoints.HealthCheck;
using Zentient.Templates.MinimalAPI.Routing;

namespace Zentient.Templates.MinimalAPI.API.EndpointMappings.Extensions
{
    internal static class MapHealthCheckExtensions
    {
        public static RouteGroupBuilder MapHealthChecks(this RouteGroupBuilder group)
        {
            group.MapGet(HealthCheckEndpoints.Routes.Get, HealthCheckEndpoints.Handlers.Get)
                 .WithName(HealthCheckEndpoints.Routes.Get)
                 .WithTags(HealthCheckEndpoints.Tags.HealthCheck)
                 .Produces(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status500InternalServerError);

            group.MapPost(ApiRoutes.HealthCheck.Post, HealthCheckEndpoints.Handlers.Post)
                 .WithName(ApiRoutes.HealthCheck.Post)
                 .WithTags(HealthCheckEndpoints.Tags.HealthCheck)
                 .Produces(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status500InternalServerError);

            return group;
        }
    }
}
