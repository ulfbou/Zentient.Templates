namespace Zentient.Templates.MinimalAPI.Endpoints.HealthCheck
{
    public static class Handlers
    {
        public static IResult Get() => Results.Ok("The service is healthy");
        public static IResult Post() => Results.Ok("The service is healthy");
    }

    public static class Routes
    {
        public const string Get = "/health";
    }

    public static class Tags
    {
        public const string HealthCheck = "HealthCheck";
    }
}
