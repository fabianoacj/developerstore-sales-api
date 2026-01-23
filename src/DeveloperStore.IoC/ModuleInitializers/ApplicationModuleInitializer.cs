using DeveloperStore.Application;
using DeveloperStore.Application.Events;
using DeveloperStore.Common.Validation;
using DeveloperStore.Domain.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using AutoMapper;
using FluentValidation;

namespace DeveloperStore.IoC.ModuleInitializers;

/// <summary>
/// Initializes application layer dependencies including MediatR and AutoMapper.
/// </summary>
public class ApplicationModuleInitializer : IModuleInitializer
{
    /// <summary>
    /// Initializes application layer services.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    public void Initialize(WebApplicationBuilder builder)
    {
        // Get entry assembly (WebApi) for scanning controllers/profiles
        var entryAssembly = Assembly.GetEntryAssembly();
        
        // Register MediatR from Application and entry assembly
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ApplicationLayer).Assembly);
            if (entryAssembly != null)
            {
                cfg.RegisterServicesFromAssembly(entryAssembly);
            }
            
            // Register ValidationBehavior for automatic validation
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // Register all FluentValidation validators from Application assembly
        builder.Services.AddValidatorsFromAssembly(typeof(ApplicationLayer).Assembly);

        // Register AutoMapper
        var assembliesToScan = new List<Assembly> { typeof(ApplicationLayer).Assembly };
        if (entryAssembly != null)
        {
            assembliesToScan.Add(entryAssembly);
        }
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(assembliesToScan);
        });

        builder.Services.AddSingleton(mapperConfig.CreateMapper());

        // Register Event Publishers using decorator pattern
        // Inner: LoggerEventPublisher -> Outer: MongoDbEventPublisher
        builder.Services.AddScoped<LoggerEventPublisher>();
        builder.Services.AddScoped<IEventPublisher>(provider =>
        {
            var eventStore = provider.GetRequiredService<IEventStore>();
            var logger = provider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<MongoDbEventPublisher>>();
            var loggerPublisher = provider.GetRequiredService<LoggerEventPublisher>();
            
            return new MongoDbEventPublisher(eventStore, logger, loggerPublisher);
        });
    }
}


