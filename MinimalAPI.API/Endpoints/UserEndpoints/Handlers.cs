using Microsoft.AspNetCore.Mvc;

using Zentient.Templates.MinimalAPI.Application.Services;
using Zentient.Templates.MinimalAPI.Domain.Entities;
using Zentient.Templates.MinimalAPI.Routing;

namespace Zentient.Templates.MinimalAPI.API.Endpoints.UserEndpoints
{
    public static partial class UserEndpoints
    {
        public static class Handlers
        {
            public static async Task<IResult> GetAll(IUserService userService, CancellationToken ct)
            {
                IEnumerable<User> users = await userService.GetAsync(_ => true, ct);
                return Results.Ok(users);
            }

            public static async Task<IResult> GetById(
                IUserService userService,
                [FromBody] Guid id,
                CancellationToken ct)
            {
                User? user = await userService.GetAsync(id, ct);
                return user is not null ? Results.Ok(user) : Results.NotFound();
            }

            public static async Task<IResult> Create(
                IUserService userService,
                [FromBody] User user,
                CancellationToken ct)
            {
                int statusCode = await userService.CreateAsync(user, ct);
                return statusCode switch
                {
                    201 => Results.Created(ApiRoutes.Users.GetById, user),
                    400 => Results.BadRequest("Invalid user data"),
                    _ => Results.StatusCode(500)
                };
            }

            public static async Task<IResult> Update(
                IUserService userService,
                [FromBody] Guid id,
                [FromBody] User user,
                CancellationToken ct)
            {
                int statusCode = await userService.UpdateAsync(id, user, ct);
                return statusCode switch
                {
                    200 => Results.Ok(user),
                    400 => Results.BadRequest("Invalid user data"),
                    404 => Results.NotFound(),
                    _ => Results.StatusCode(500)
                };
            }

            public static async Task<IResult> Delete(
                IUserService userService,
                [FromBody] Guid id,
                CancellationToken ct)
            {
                int statusCode = await userService.DeleteAsync(id, ct);
                return statusCode switch
                {
                    200 => Results.Ok(),
                    404 => Results.NotFound(),
                    _ => Results.StatusCode(500)
                };
            }
        }
    }
}
