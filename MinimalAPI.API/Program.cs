using Azure.Core;
using Azure.Identity;

using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Serilog;

using Zentient.Templates.MinimalAPI.API.Configuration;
using Zentient.Templates.MinimalAPI.API.Endpoints.HealthCheck;
using Zentient.Templates.MinimalAPI.API.Endpoints.UserEndpoints;
using Zentient.Templates.MinimalAPI.Common.Constants;
using Zentient.Templates.MinimalAPI.API.EndpointMappings;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;
var host = builder.Host;

configuration
    .AddConfiguration(builder.Environment, args);

host.UseSerilog((context, serviceProvider, configuration) =>
{
    // Read configuration from appsettings.json and environment variables
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(serviceProvider)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

// Configure OpenApi and Swagger services
builder.Services
       .AddEndpointsApiExplorer()
       .AddOpenApi();

builder.Services
       .MapHealthChecks(builder.Configuration);

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapEndpoints(app.Configuration);
app.Run();
