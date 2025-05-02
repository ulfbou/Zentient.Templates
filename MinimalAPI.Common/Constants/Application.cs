namespace Zentient.Templates.MinimalAPI.Common.Constants
{
    public static class ConstantNames
    {
        public static class Jwt
        {
            public const string Name = "Jwt";
            public const string Scheme = "Scheme";
            public const string Issuer = "Issuer";
            public const string Audience = "Audience";
            public const string Key = "Key";
            public const string Expiration = "Expiration";
            public const string SigningKey = "SigningKey";
            public const string TokenValidationParameters = "TokenValidationParameters";
        }

        public static class ConfigurationSections
        {
            public const string Application = "App:Config:Application";
            public const string Logging = "App:Config:Logging";

            public const string Jwt = "App:Jwt";
        }
        public static class Application
        {
            public const string Name = "AppName";
            public const string DisplayName = "AppDisplayName";
            public const string Description = "AppDescription";
            public const string Tags = "AppTags";
            public const string Route = "api/v1";
            public const string RequestTimeout = "RequestTimeout";
            public const string Summary = "AppSummary";
        }
    }

    public static class DefaultSettings
    {
        public static class Jwt
        {
            public const string Scheme = "Bearer";
            public const string Issuer = "DefaultIssuer";
            public const string Audience = "DefaultAudience";

            public const string Key = "Key";
            public const int Expiration = 30;
            public const string SigningKey = "SigningKey";
            public const string TokenValidationParameters = "TokenValidationParameters";
        }

        public static class Application
        {
            public const string Name = "Zentient.Templates.MinimalAPI";
            public const string DisplayName = "Zentient Templates Minimal API";
            public const string Version = "1.0.0";
            public const string Description = "A minimal API template for .NET applications.";
            public const string Summary = "A minimal API template for .NET applications.";
            public const string Tags = "minimalapi, dotnet, templates, zentient";

            public const int RequestTimeout = 30;

        }
    }
}
