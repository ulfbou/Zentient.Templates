namespace Zentient.Templates.MinimalAPI.API.EndpointMappings
{
    using Microsoft.OpenApi.Models;

    using Zentient.Templates.MinimalAPI.API.EndpointMappings.Extensions;
    using Zentient.Templates.MinimalAPI.Common.Constants;
    using Zentient.Templates.MinimalAPI.Routing;

    public static class WebApplicationExtensions
    {
        public static WebApplication MapEndpoints(this WebApplication app, IConfiguration configuration)
        {
            var logger = app.Services.GetRequiredService<ILogger>();

            using (var scope = logger.BeginScope("MapEndpoints"))
            {
                logger.LogInformation("Mapping endpoints...");

                // Retrieve application configuration section
                var appConfig = configuration.GetSection(ConstantNames.ConfigurationSections.Application);

                // Extract configuration values with fallbacks
                var apiRoute = appConfig[ConstantNames.Application.Route] ?? DefaultSettings.Application.Name;
                var appName = appConfig[ConstantNames.Application.Name] ?? DefaultSettings.Application.Name;
                var appDescription = appConfig[ConstantNames.Application.Description] ?? DefaultSettings.Application.Description;
                var appTags = appConfig[ConstantNames.Application.Tags]?.Split(',') ?? DefaultSettings.Application.Tags.Split(',');
                var appRequestTimeout = TimeSpan.FromSeconds(appConfig.GetValue<int?>(ConstantNames.Application.RequestTimeout) ?? DefaultSettings.Application.RequestTimeout);

                // Log the configuration values
                logger.LogInformation("API Route: {ApiRoute}", apiRoute);
                logger.LogInformation("Application Name: {AppName}", appName);
                logger.LogInformation("Application Description: {AppDescription}", appDescription);
                logger.LogInformation("Application Tags: {AppTags}", string.Join(", ", appTags));
                logger.LogInformation("Request Timeout: {RequestTimeout}", appRequestTimeout);

                // Map the health check endpoint
                app.MapHealthChecks(ApiRoutes.HealthCheck.Get)
                   .WithName(appName)
                   .WithDescription(appDescription)
                   .WithTags(appTags)
                   .WithRequestTimeout(appRequestTimeout);

                // Map the API versioning route
                var group = app.MapGroup(apiRoute)
                               .WithName(appName);

                // Configure the API versioning route
                group.WithRequestTimeout(appRequestTimeout)
                     .WithDescription(appDescription)
                     .WithTags(appTags)
                     .WithSummary(appConfig[ConstantNames.Application.Summary] ?? DefaultSettings.Application.Summary)
                     .WithDisplayName(appConfig[ConstantNames.Application.DisplayName] ?? DefaultSettings.Application.DisplayName);

                // Map the user endpoints
                group.MapUsers();

                // Map the OpenAPI documentation route
                app.MapOpenApi()
                   .WithName(appName)
                   .WithDescription(appDescription)
                   .WithTags(appTags)
                   .WithRequestTimeout(appRequestTimeout);

                // Add OpenAPI support
                group.WithOpenApi(operation =>
                {
                    operation.Summary = appConfig[ConstantNames.Application.Summary] ?? DefaultSettings.Application.Summary;
                    operation.Description = appConfig[ConstantNames.Application.Description] ?? DefaultSettings.Application.Description;

                    // Convert string tags to OpenApiTag objects
                    operation.Tags = appTags.Select(tag => new OpenApiTag { Name = tag }).ToList();

                    return operation;
                });
            }

            return app;
        }
    }
}
