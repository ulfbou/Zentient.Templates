using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Zentient.Templates.MinimalAPI.API.Endpoints.HealthCheck
{
    public static partial class HealthCheckEndpoints
    {
        public const string Name = "HealthCheckEndpoints";
        public const string Description = "Health Check Endpoints";
        public const string Summary = "Health Check Endpoints";
        public const string DisplayName = "Health Check Endpoints";

        public static class Tags
        {
            public const string HealthCheck = "HealthCheck";
            public const string HealthCheckDescription = "Health Check Endpoints";
        }

        public static class Routes
        {
            public const string Get = "/health";
            public const string GetDescription = "Get Health Check";
            public const string GetSummary = "Get Health Check";
            public const string GetDisplayName = "Get Health Check";
        }

        public static IServiceCollection MapHealthChecks(this IServiceCollection services, ConfigurationManager configuration)
        {
            // Configure health checks
            var healthChecks = services.AddHealthChecks();

            healthChecks.AddCheck("self", () => HealthCheckResult.Healthy("The service is healthy"))
                .AddCheck("database", () => HealthCheckResult.Degraded("The database is not reachable"))
                .AddCheck("external_service", () => HealthCheckResult.Unhealthy("The external service is not reachable"));

            return services;
        }
    }
}
