
namespace Zentient.Templates.MinimalAPI.API.EndpointMappings.Extensions
{
    using Zentient.Templates.MinimalAPI.Domain.Entities;
    using Zentient.Templates.MinimalAPI.Routing;
    internal static class MapUsersExtensions
    {
        public static RouteGroupBuilder MapUsers(this RouteGroupBuilder group)
        {
            group.MapGet(ApiRoutes.Users.GetAll, Endpoints.UserEndpoints.UserEndpoints.Handlers.GetAll)
                 .WithName(ApiRoutes.Users.GetAll)
                 .WithTags(Endpoints.UserEndpoints.UserEndpoints.Tags.Users)
                 .Produces<IEnumerable<User>>(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status500InternalServerError);

            group.MapGet(ApiRoutes.Users.GetById, Endpoints.UserEndpoints.UserEndpoints.Handlers.GetById)
                 .WithName(ApiRoutes.Users.GetById)
                 .WithTags(Endpoints.UserEndpoints.UserEndpoints.Tags.Users)
                 .Produces<User>(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status404NotFound)
                 .Produces(StatusCodes.Status500InternalServerError);

            group.MapPost(ApiRoutes.Users.Create, Endpoints.UserEndpoints.UserEndpoints.Handlers.Create)
                 .WithName(ApiRoutes.Users.Create)
                 .WithTags(Endpoints.UserEndpoints.UserEndpoints.Tags.Users)
                 .Produces<User>(StatusCodes.Status201Created)
                 .Produces(StatusCodes.Status400BadRequest)
                 .Produces(StatusCodes.Status500InternalServerError);

            group.MapPut(ApiRoutes.Users.Update, Endpoints.UserEndpoints.UserEndpoints.Handlers.Update)
                 .WithName(ApiRoutes.Users.Update)
                 .WithTags(Endpoints.UserEndpoints.UserEndpoints.Tags.Users)
                 .Produces<User>(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status400BadRequest)
                 .Produces(StatusCodes.Status404NotFound)
                 .Produces(StatusCodes.Status500InternalServerError);

            group.MapDelete(ApiRoutes.Users.Delete, Endpoints.UserEndpoints.UserEndpoints.Handlers.Delete)
                 .WithName(ApiRoutes.Users.Delete)
                 .WithTags(Endpoints.UserEndpoints.UserEndpoints.Tags.Users)
                 .Produces(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status404NotFound)
                 .Produces(StatusCodes.Status500InternalServerError);

            return group;
        }
    }
}
