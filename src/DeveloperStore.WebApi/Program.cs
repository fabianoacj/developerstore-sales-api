using DeveloperStore.Common.HealthChecks;
using DeveloperStore.Common.Logging;
using DeveloperStore.IoC;
using DeveloperStore.WebApi.Middleware;
using Serilog;

namespace DeveloperStore.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        // Create bootstrap logger for startup
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Starting DeveloperStore Sales API");

            var builder = WebApplication.CreateBuilder(args);

            // Add Serilog logging
            builder.AddDefaultLogging();

            // Register all dependencies through IoC (includes AutoMapper and MediatR)
            builder.RegisterDependencies();

            // Configure CORS for development
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Add health checks
            builder.AddBasicHealthChecks();

            var app = builder.Build();

            // Add validation exception middleware
            app.UseMiddleware<ValidationExceptionMiddleware>();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DeveloperStore Sales API V1");
                    c.RoutePrefix = string.Empty; // Swagger at root
                });
                app.UseCors("AllowAll");
            }
            else
            {
                app.UseHttpsRedirection();
            }

            // Add health checks endpoint
            app.UseBasicHealthChecks();

            app.UseAuthorization();

            app.MapControllers();

            Log.Information("DeveloperStore Sales API started successfully");

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}

