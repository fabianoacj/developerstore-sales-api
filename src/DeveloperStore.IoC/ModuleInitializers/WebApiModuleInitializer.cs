using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DeveloperStore.IoC.ModuleInitializers;

/// <summary>
/// Initializes Web API layer dependencies including controllers, health checks, and API configuration.
/// </summary>
public class WebApiModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        // Register controllers for API endpoints
        builder.Services.AddControllers();

        // Register health checks for monitoring
        builder.Services.AddHealthChecks();

        // Configure API behavior options
        builder.Services.AddEndpointsApiExplorer();

        // Register Swagger for API documentation
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "DeveloperStore Sales API",
                Version = "v1",
                Description = "API for managing sales operations in DeveloperStore",
                Contact = new Microsoft.OpenApi.Models.OpenApiContact
                {
                    Name = "DeveloperStore Team",
                    Email = "support@developerstore.com"
                }
            });
        });
    }
}
