namespace Zentient.Templates.MinimalAPI.Configuration
{
    public static class ConfigurationManagerExtensions
    {
        public static ConfigurationManager AddConfiguration(
            this ConfigurationManager configurationManager,
            IWebHostEnvironment environment,
            params string[] args)
        {
            var configuration = configurationManager
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (environment.IsDevelopment())
            {
                configuration.AddUserSecrets<Program>(optional: true, reloadOnChange: true);
            }

            if (args.Length > 0)
            {
                configuration.AddCommandLine(args);
            }

            return configurationManager;
        }
    }
}
