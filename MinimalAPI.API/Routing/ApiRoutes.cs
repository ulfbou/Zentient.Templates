namespace Zentient.Templates.MinimalAPI.Routing
{
    public static class ApiRoutes
    {
        public const string Version = "v1";
        public const string BaseRoute = "/api/" + Version;

        public static class Orders
        {
            public const string Base = BaseRoute + "/orders";
            public const string GetById = Base + "/{id:guid}";
            public const string Create = Base;
        }

        public static class Users
        {
            public const string Base = BaseRoute + "/users";
            public const string GetAll = Base;
            public const string GetById = Base + "/{id:int}";
            public const string Create = Base;
            public const string Update = Base + "/{id:int}";
            public const string Delete = Base + "/{id:int}";
        }

        public static class HealthCheck
        {
            public const string Base = BaseRoute + "/health";
            public const string Get = Base;
            public const string Post = Base;
        }
    }
}
